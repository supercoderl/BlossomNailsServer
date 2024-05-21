using BlossomServer.Entities;
using BlossomServer.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static BlossomServer.Datas.Authentication.Login;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlossomServer.Datas.Authentication;
using System.Net;
using System;
using BlossomServer.Services.EmailService;
using BlossomServer.Datas.Email;
using AutoMapper;

namespace BlossomServer.Services.AuthenticationServices
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IConfiguration _configuration;
		private readonly BlossomNailsContext _context;
		private readonly IEmailService _emailService;
		private readonly IMapper _mapper;
		private static Random random = new Random();

		public AuthenticationService(IConfiguration configuration, BlossomNailsContext context, IEmailService emailService, IMapper mapper)
		{
			_configuration = configuration;
			_context = context;
			_emailService = emailService;
			_mapper = mapper;
		}

		public LoginResult GenerateToken(User user, List<Claim> claims, DateTime now)
		{
			var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(
				claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value
			);

			var jwtToken = new JwtSecurityToken(
				_configuration["JWT_Configuration:Issuer"],
				shouldAddAudienceClaim ? _configuration["JWT_Configuration:Audience"] : String.Empty,
				claims,
				expires: now.AddMinutes(Convert.ToDouble(_configuration["JWT_Configuration:TokenExpirationMinutes"])),
				signingCredentials: new SigningCredentials
				(
					new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_Configuration:SecretKey"]!)),
					SecurityAlgorithms.HmacSha256Signature
				)
			);


			var token = new TokenResult
			{
				AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
				ExpirationMinutes = Convert.ToInt32(_configuration["JWT_Configuration:TokenExpirationMinutes"])
			};

			var refreshToken = new RefreshTokenResult
			{
				RefreshToken = GenerateRefreshToken(),
				ExpirationDays = Convert.ToInt32(_configuration["JWT_Configuration:RefreshTokenExpirationDays"])
			};

			Token newToken = new Token
			{
				RefreshToken = refreshToken.RefreshToken,
				UserID = user.UserID,
				IsActive = true,
			};

			_context.Tokens.Add(newToken);
			_context.SaveChanges();

			return new LoginResult
			{
				Token = token,
				RefreshToken = refreshToken
			};
		}

		public async Task<ApiResponse<LoginResult>> Refresh(string refreshToken, DateTime now)
		{
			var refreshTokenObject = await _context.Tokens.SingleAsync(x => x.RefreshToken == refreshToken);
			var user = await _context.Users.FindAsync(refreshTokenObject.UserID);

			if (refreshTokenObject.RevokeAt != null || !refreshTokenObject.IsActive)
			{
				return new ApiResponse<LoginResult>
				{
					Success = false,
					Message = "Invalid token.",
					Status = 200
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
					new Claim(ClaimTypes.NameIdentifier, user.Firstname + " " + user.Lastname),
				};

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var result = GenerateToken(user, claims, now);
			return new ApiResponse<LoginResult>
			{
				Success = true,
				Message = "Refresh Token thành công.",
				Data = result,
				Status = 200
			};
		}

		public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true, //you might want to validate the audience and issuer depending on your use case
				ValidateIssuer = true,
				ValidAudience = _configuration["JWT_Configuration:Audience"],
				ValidIssuer = _configuration["JWT_Configuration:Issuer"],
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_Configuration:SecretKey"]!)),
				ValidateLifetime = true //here we are saying that we don't care about the token's expiration date
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			SecurityToken securityToken;
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
				throw new SecurityTokenException("Invalid token");
			return principal;
		}

		public Task RemoveRefreshToken(string username)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> RevokeToken(string token)
		{
			var item = await _context.Tokens.FirstOrDefaultAsync(x => x.RefreshToken.Equals(token));
			if (item == null || !item.IsActive || item.RevokeAt != null)
			{
				return false;
			}

			item.IsActive = false;
			item.RevokeAt = DateTime.Now;

			_context.Entry(item).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return true;
		}

		private string GenerateRefreshToken()
		{
			var randomNumber = new Byte[32];
			using var randomNumberGenerator = RandomNumberGenerator.Create();
			randomNumberGenerator.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		public async Task<ApiResponse<string>> SendResetCode(SendResetCodeRequest request)
		{
			try
			{
				await Task.CompletedTask;
				var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Email);
				if (user == null)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "User doesn't exist.",
						Status = (int)HttpStatusCode.OK
					};
				}
				var code = GenerateCode();
				var message = new Message(
					new string[] { request.Email },
					"Reset Password",
					$"<html><body>" +
					$"<p>Hi {string.Concat(user.Firstname, " ", user.Lastname)},</p>" +
					$"<p>There was a request to change your password!</p>" +
					$"<p>If you did not make this request then please ignore this email.</p>" +
					$"<p>Otherwise, this is a code to access your reset password: <b>{code}</b></p>" +
					$"</body></html>"
				);

				var resetPassword = new ResetPasswordProfile
				{
					Username = request.Email,
					Code = code,
					ExpMinutes = DateTime.Now.AddMinutes(5)
				};
				var resetPasswordEntity = _mapper.Map<ResetPassword>(resetPassword);
				await _context.ResetPasswords.AddAsync(resetPasswordEntity);
				await _context.SaveChangesAsync();

				return await _emailService.SendEmail(message);
			}
			catch (Exception ex)
			{
				return new ApiResponse<string>
				{
					Success = false,
					Message = "Authentication Service - Send Reset Code: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		private static string GenerateCode()
		{
			int randomNumber = random.Next(100000, 999999);
			string code = randomNumber.ToString();
			return code;
		}

		public async Task<ApiResponse<string>> VerifyResetCode(VerifyCodeRequest request)
		{
			try
			{
				await Task.CompletedTask;
				var resetPasswords = await _context.ResetPasswords.Where(rp => rp.Username == request.Email && rp.IsActive).ToListAsync();
				if (resetPasswords.Count() <= 0 && resetPasswords.Count() >= 2)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Server error!",
						Status = (int)HttpStatusCode.OK
					};
				}
				else if (DateTime.Now > resetPasswords[0].ExpMinutes)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Code expired!",
						Status = (int)HttpStatusCode.OK
					};
				}
				else if (request.Code != resetPasswords[0].Code)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Your code isn't correct!",
						Status = (int)HttpStatusCode.OK
					};
				}
				resetPasswords[0].IsActive = false;
				_context.ResetPasswords.Update(resetPasswords[0]);
				await _context.SaveChangesAsync();
				return new ApiResponse<string>
				{
					Success = true,
					Message = "Verify code successfully!",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<string>
				{
					Success = false,
					Message = "Authentication Service - Verify Reset Code: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest request)
		{
			try
			{
				await Task.CompletedTask;
				var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Email);

				if (user is null) 
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "User doesn't exists.",
						Status = (int)HttpStatusCode.OK
					};
				}

				user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
				_context.Users.Update(user);
				await _context.SaveChangesAsync();
				return new ApiResponse<string>
				{
					Success = true,
					Message = "Reset password successfully.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<string>
				{
					Success = false,
					Message = "Authentication Service - Reset Password: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}
	}
}
