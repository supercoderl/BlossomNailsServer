using AutoMapper;
using BlossomServer.Datas.Role;
using BlossomServer.Datas.UserRole;
using BlossomServer.Entities;
using BlossomServer.Response;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlossomServer.Services.RoleServices
{
	public class RoleService : IRoleService
	{
		private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;

		public RoleService(BlossomNailsContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ApiResponse<RoleProfile>> CreateRole(CreateRoleRequest newRole)
		{
			try
			{
				await Task.CompletedTask;
				var roleEntity = _mapper.Map<Role>(newRole);
				await _context.Roles.AddAsync(roleEntity);
				await _context.SaveChangesAsync();
				return new ApiResponse<RoleProfile>
				{
					Success = true,
					Message = $"Create role {roleEntity.Name} successfully.",
					Data = _mapper.Map<RoleProfile>(roleEntity),
					Status = (int)HttpStatusCode.Created
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<RoleProfile>
				{
					Success = false,
					Message = "RoleService - CreateRole: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<object>> DeleteRole(int roleID)
		{
			try
			{
				await Task.CompletedTask;
				var role = await _context.Roles.FindAsync(roleID);
				if (role == null)
					return new ApiResponse<object>
					{
						Success = false,
						Message = $"Role with ID {roleID} not found.",
						Status = (int)HttpStatusCode.OK
					};

				_context.Roles.Remove(role);

				await _context.SaveChangesAsync();
				return new ApiResponse<object>
				{
					Success = true,
					Message = $"Deleted role with ID {roleID}.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<Object>
				{
					Success = false,
					Message = "RoleService - DeleteRole: " + ex,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<RoleProfile>> GetRoleByID(int roleID)
		{
			try
			{
				await Task.CompletedTask;
				var role = await _context.Roles.FindAsync(roleID);
				if (role == null)
				{
					return new ApiResponse<RoleProfile>
					{
						Success = false,
						Message = "This role is not exists.",
						Status = (int)HttpStatusCode.OK
					};
				}

				return new ApiResponse<RoleProfile>
				{
					Success = true,
					Message = $"Get role {role.Name} successfully.",
					Data = _mapper.Map<RoleProfile>(role),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<RoleProfile>
				{
					Success = false,
					Message = "RoleService - GetRoleByID: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<RoleProfile>>> GetRoles()
		{
			try
			{
				await Task.CompletedTask;
				var roles = await _context.Roles.ToListAsync();
				if (!roles.Any())
				{
					return new ApiResponse<List<RoleProfile>>
					{
						Success = false,
						Message = "There aren't any roles here.",
						Status = (int)HttpStatusCode.OK
					};
				}

				return new ApiResponse<List<RoleProfile>>
				{
					Success = true,
					Message = $"Get roles successfully. Found {roles.Count()} roles!",
					Data = roles.Select(x => _mapper.Map<RoleProfile>(x)).ToList(),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<RoleProfile>>
				{
					Success = false,
					Message = "RoleService - GetRoles: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<object>>> GetRolesMapUser()
		{
			try
			{
				await Task.CompletedTask;
				var rolesUser = await _context.UserRoles.ToListAsync();
				if (!rolesUser.Any())
				{
					return new ApiResponse<List<object>>
					{
						Success = false,
						Message = "There aren't any users with permissions.",
						Status = (int)HttpStatusCode.OK
					};
				}

				return new ApiResponse<List<object>>
				{
					Success = true,
					Message = $"Get linked roles successfully. Found {rolesUser.Count()} roles!",
					Data = rolesUser.Select(x => _mapper.Map<object>(x)).ToList(),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<object>>
				{
					Success = false,
					Message = "RoleService - GetRolesMapUser: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<RoleProfile>> UpdateRole(int roleID, RoleProfile role)
		{
			try
			{
				await Task.CompletedTask;

				if (roleID != role.RoleID)
				{
					return new ApiResponse<RoleProfile>
					{
						Success = false,
						Message = "This role with ID not match.",
						Status = (int)HttpStatusCode.OK
					};
				};

				var roleEntity = await _context.Roles.FindAsync(roleID);

				if (roleEntity == null)
				{
					return new ApiResponse<RoleProfile>
					{
						Success = false,
						Message = "Couldn't update because this role does not exists.",
						Status = (int)HttpStatusCode.OK
					};
				};

				_mapper.Map(role, roleEntity);
				_context.Roles.Update(roleEntity);
				await _context.SaveChangesAsync();

				return new ApiResponse<RoleProfile>
				{
					Success = true,
					Message = $"Updated {roleEntity.Name} role.",
					Data = _mapper.Map<RoleProfile>(roleEntity),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<RoleProfile>
				{
					Success = false,
					Message = "RoleService - UpdateRole: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<object>> CreateRolesMapUser(CreateUserRoleRequest rolesMapUser)
		{
			try
			{
				await Task.CompletedTask;

				var userRoleEntity = _mapper.Map<UserRole>(rolesMapUser);
				await _context.UserRoles.AddAsync(userRoleEntity);
				await _context.SaveChangesAsync();
				return new ApiResponse<object>
				{
					Success = true,
					Message = "Map role with user successfully.",
					Data = _mapper.Map<object>(userRoleEntity),
					Status = (int)HttpStatusCode.Created
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<object>
				{
					Success = false,
					Message = "UserRoleService - UpdateRolesMapUser: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<object>> DeleteRoleMapUser(int userRoleID)
		{
			try
			{
				await Task.CompletedTask;
				var userRole = await _context.UserRoles.FindAsync(userRoleID);
				if (userRole == null)
					return new ApiResponse<object>
					{
						Success = false,
						Message = "User role with this ID not match.",
						Status = (int)HttpStatusCode.OK
					};

				_context.UserRoles.Remove(userRole);

				await _context.SaveChangesAsync();
				return new ApiResponse<object>
				{
					Success = true,
					Message = "Deleted role linking.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<object>
				{
					Success = false,
					Message = "RoleService - DeleteRoleMapUser: " + ex,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}
	}
}
