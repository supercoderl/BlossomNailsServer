using BlossomServer.Datas.Role;
using BlossomServer.Datas.UserRole;
using BlossomServer.Response;

namespace BlossomServer.Services.RoleServices
{
	public interface IRoleService
	{
		Task<ApiResponse<List<RoleProfile>>> GetRoles();
		Task<ApiResponse<RoleProfile>> CreateRole(CreateRoleRequest newRole);
		Task<ApiResponse<RoleProfile>> UpdateRole(int roleID, RoleProfile role);
		Task<ApiResponse<Object>> DeleteRole(int roleID);
		Task<ApiResponse<RoleProfile>> GetRoleByID(int roleID);
		Task<ApiResponse<List<object>>> GetRolesMapUser();
		Task<ApiResponse<object>> CreateRolesMapUser(CreateUserRoleRequest rolesMapUser);
		Task<ApiResponse<object>> DeleteRoleMapUser(int userRoleID);
	}
}
