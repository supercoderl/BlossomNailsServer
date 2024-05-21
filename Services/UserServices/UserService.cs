using AutoMapper;
using BlossomServer.Datas.Authentication;
using BlossomServer.Datas.User;
using BlossomServer.Entities;
using BlossomServer.Response;
using Microsoft.EntityFrameworkCore;
using static BlossomServer.Datas.Authentication.Login;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using BlossomServer.Services.AuthenticationServices;
using BlossomServer.Datas.Service;
using System.Reflection;

namespace BlossomServer.Services.UserServices
{
	public class UserService : IUserService
	{
		private readonly BlossomNailsContext _context;
		private readonly IAuthenticationService _jwtService;
		private readonly IMapper _mapper;

		public UserService(BlossomNailsContext context, IAuthenticationService jwtService, IMapper mapper)
		{
			_context = context;
			_jwtService = jwtService;
			_mapper = mapper;
		}

		public async Task<ApiResponse<LoginResult>> Login(LoginRequest request)
		{
			try
			{
				await Task.CompletedTask;
				var user = _context.Users.FirstOrDefault(x => x.Username == request.Username);

				if (user == null)
				{
					return new ApiResponse<LoginResult>
					{
						Success = false,
						Message = "User does not exists.",
						Status = (int)HttpStatusCode.OK
					};
				}

				else if (!user.IsActive)
				{
					return new ApiResponse<LoginResult>
					{
						Success = false,
						Message = "Your account was been blocked, please contact to administrator.",
						Status = (int)HttpStatusCode.OK
					};
				}

				if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
				{
					return new ApiResponse<LoginResult>
					{
						Success = false,
						Message = "Wrong password!",
						Status = (int)HttpStatusCode.OK
					};
				}

				var roles = (from u in _context.Users
							 join ur in _context.UserRoles
							 on u.UserID equals ur.UserID
							 join r in _context.Roles
							 on ur.RoleID equals r.RoleID
							 where u.UserID == user.UserID
							 select r.Name).ToArray();

				var claims = new List<Claim>
				{
					new Claim("UserID", user.UserID.ToString()),
					new Claim("Fullname", user.Firstname + " " + user.Lastname),
				};

				foreach (var role in roles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}

				var result = _jwtService.GenerateToken(user, claims, DateTime.Now);
				result.UserResult = new UserResult
				{
					UserID = user.UserID,
					Username = user.Username,
					Roles = roles,
					Fullname = user.Lastname + " " + user.Firstname,
					Phone = "0" + user.Phone.ToString(),
				};

				return new ApiResponse<LoginResult>
				{
					Success = true,
					Message = "Login successfully!",
					Data = result,
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<LoginResult>
				{
					Success = false,
					Message = "User service - Login: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<CreateUserRequest>> Register(CreateUserRequest newUser)
		{
			try
			{
				if (newUser.Username == null || newUser.Password == null)
				{
					return new ApiResponse<CreateUserRequest>
					{
						Success = false,
						Message = "Invalid username or password!",
						Status = (int)HttpStatusCode.OK
					};
				}

				if (await IsExist(newUser.Username))
				{
					return new ApiResponse<CreateUserRequest>
					{
						Success = false,
						Message = "This username or email exists.",
						Status = (int)HttpStatusCode.OK
					};
				}

				var userEntity = _mapper.Map<User>(newUser);

				await _context.Users.AddAsync(userEntity);
				bool checkAnyUserExists = await IsAnyUserInSystem();

				var roleCode = newUser.RoleCode != null ? newUser.RoleCode : 300;
				var role = await _context.Roles.FirstOrDefaultAsync(checkAnyUserExists ? x => x.Code == roleCode : x => x.Code == 100);
				if (role == null)
				{
					return new ApiResponse<CreateUserRequest>
					{
						Success = false,
						Message = "There aren't any role to create new account.",
						Status = (int)HttpStatusCode.OK
					};
				}

				await _context.SaveChangesAsync();

				await RelateRole(role.RoleID, userEntity.UserID);

				return new ApiResponse<CreateUserRequest>
				{
					Success = true,
					Message = "Register successfully.",
					Data = newUser,
					Status = (int)HttpStatusCode.Created
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<CreateUserRequest>
				{
					Success = false,
					Message = "User service - Register: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		private async Task<bool> IsAnyUserInSystem()
		{
			var users = await _context.Users.ToListAsync();
			return users.Any();
		}

		private async Task RelateRole(int roleID, Guid userID)
		{
			var userRole = new UserRole
			{
				RoleID = roleID,
				UserID = userID,
			};

			await _context.UserRoles.AddAsync(userRole);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> IsExist(string userName)
		{
			try
			{
				await Task.CompletedTask;
				bool result = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName) is not null;
				return result;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<ApiResponse<string>> ChangePassword(Guid UserID, ChangePasswordRequest request)
		{
			try
			{
				await Task.CompletedTask;
				var user = await _context.Users.FindAsync(UserID);
				if (user == null)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "User does not exists.",
						Status = (int)HttpStatusCode.OK
					};
				}
				if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Old password is not correct.",
						Status = (int)HttpStatusCode.OK
					};
				}

				user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
				_context.Entry(user).State = EntityState.Modified;
				await _context.SaveChangesAsync();
				return new ApiResponse<string>
				{
					Success = true,
					Message = "Change password successfully, please login again.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<string>
				{
					Success = false,
					Message = "User service - ChangePassword: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<Object>> Logout(string token)
		{
			try
			{
				await Task.CompletedTask;
				bool result = await _jwtService.RevokeToken(token);
				if (!result)
				{
					return new ApiResponse<Object>
					{
						Success = false,
						Message = "Invalid token.",
						Status = (int)HttpStatusCode.OK
					};
				}

				else
					return new ApiResponse<Object>
					{
						Success = true,
						Message = "Logout successfully.",
						Status = (int)HttpStatusCode.OK
					};
			}
			catch (Exception ex)
			{
				return new ApiResponse<Object>
				{
					Success = false,
					Message = "User service - Logout: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<UserProfile>> GetProfile(Guid UserID)
		{
			try
			{
				await Task.CompletedTask;
				var user = await _context.Users.FindAsync(UserID);
				if (user == null)
				{
					return new ApiResponse<UserProfile>
					{
						Success = false,
						Message = "User does not exists.",
						Status = (int)HttpStatusCode.OK
					};
				}

				return new ApiResponse<UserProfile>
				{
					Success = true,
					Message = $"Got profile of user {user.Username}.",
					Data = _mapper.Map<UserProfile>(user),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<UserProfile>
				{
					Success = false,
					Message = "UserService - GetProfile: " + ex,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<UserProfile>>> GetUsers(FilterUser? filter)
		{
			try
			{
				await Task.CompletedTask;
				var users = await _context.Users.ToListAsync();

				if (!users.Any())
				{
					return new ApiResponse<List<UserProfile>>
					{
						Success = false,
						Message = "There aren't any users.",
						Status = (int)HttpStatusCode.OK
					};
				}

				var usersProfile = users.Select(x => _mapper.Map<UserProfile>(x)).ToList();
				foreach (var user in usersProfile)
				{
					user.Roles = (from u in _context.Users
								  join ur in _context.UserRoles
								  on u.UserID equals ur.UserID
								  join r in _context.Roles
								  on ur.RoleID equals r.RoleID
								  where u.UserID == user.UserID
								  select r.Name).ToList();
				};

				if (filter != null) usersProfile = FilterUser(usersProfile, filter);

				return new ApiResponse<List<UserProfile>>
				{
					Success = true,
					Message = $"Got users. Found {usersProfile.Count()} users!",
					Data = usersProfile,
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<UserProfile>>
				{
					Success = false,
					Message = "UserService - GetUsers: " + ex,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<UpdateUserRequest>> UpdateUser(Guid UserID, UpdateUserRequest user)
		{
			try
			{
				await Task.CompletedTask;

				if (UserID != user.UserID)
				{
					return new ApiResponse<UpdateUserRequest>
					{
						Success = false,
						Message = $"User with ID {UserID} not match.",
						Status = (int)HttpStatusCode.OK
					};
				}

				var userInData = await GetByID(UserID);
				if (userInData is null)
				{
					return new ApiResponse<UpdateUserRequest>
					{
						Success = false,
						Message = "User does not exist.",
						Status = (int)HttpStatusCode.OK
					};
				}

				_mapper.Map(user, userInData);


				_context.Users.Update(userInData);
				await _context.SaveChangesAsync();

				return new ApiResponse<UpdateUserRequest>
				{
					Success = true,
					Message = $"Updated user {user.Username}.",
					Data = user,
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<UpdateUserRequest>
				{
					Success = false,
					Message = "UserService - UpdateUser: " + ex,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		private async Task<User?> GetByID(Guid UserID)
		{
			try
			{
				await Task.CompletedTask;
				return await _context.Users.FindAsync(UserID);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<ApiResponse<object>> DeleteUser(Guid UserID)
		{
			try
			{
				await Task.CompletedTask;
				await _context.Database.ExecuteSqlInterpolatedAsync($"sp_delete_user {UserID}");
				await _context.SaveChangesAsync();
				return new ApiResponse<object>
				{
					Success = true,
					Message = $"Deleted.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<Object>
				{
					Success = false,
					Message = "UserService - DeleteUser: " + ex,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public ApiResponse<Object> SendBackupEmail(string recipientEmail, string token)
		{
			string senderEmail = "minh.quang1720@gmail.com";
			string senderPassword = "acanhcamlay2";

			SmtpClient smtpClient = new SmtpClient("smtp.test.com");
			smtpClient.Port = 587;
			smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
			smtpClient.EnableSsl = true;

			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress(senderEmail);
			mailMessage.To.Add(recipientEmail);
			mailMessage.Subject = "Xác thực yêu cầu sao lưu tài khoản";
			mailMessage.Body = $"Vui lòng nhấp vào liên kết sau để xác thực yêu cầu sao lưu tài khoản: {token}";
			mailMessage.IsBodyHtml = false;

			try
			{
				smtpClient.Send(mailMessage);
				return new ApiResponse<Object>
				{
					Success = true,
					Message = "Gửi mail thành công",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<Object>
				{
					Success = false,
					Message = "UserService - SendBackupRequest: " + ex,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		private List<UserProfile> FilterUser(List<UserProfile> users, FilterUser filter)
		{
			if (filter.Status is not null)
				users = users.Where(x => filter.Status == "activated" ? x.IsActive : !x.IsActive).ToList();
			if(filter.Role is not null)
				users = users.Where(u => u.Roles is not null && u.Roles.Contains(filter.Role)).ToList();
			if (filter.SortType is not null)
			{
				PropertyInfo propertyInfo = typeof(UserProfile).GetProperty(char.ToUpper(filter.SortType[0]) + filter.SortType.Substring(1))!;
				switch (filter.SortFrom)
				{
					case "ascending":
						users = users.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
						break;
					default:
						users = users.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
						break;
				}
			}
			if (filter.SearchText is not null)
				users = users.Where(x => 
					x.Username.ToLower().Contains(filter.SearchText.ToLower()) ||
					string.Concat(x.Firstname, x.Lastname).Trim().ToLower().Contains(filter.SearchText.ToLower())
				).ToList();
			return users;
		}
	}
}
