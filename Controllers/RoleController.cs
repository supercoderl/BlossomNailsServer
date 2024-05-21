using BlossomServer.Datas.Role;
using BlossomServer.Datas.UserRole;
using BlossomServer.Services.RoleServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + "Firebase")]
	public class RoleController : ControllerBase
	{
		private readonly IRoleService _roleService;

		public RoleController(IRoleService roleService)
		{
			_roleService = roleService;
		}

		[HttpGet("roles")]
		public async Task<IActionResult> GetRoles()
		{
			var result = await _roleService.GetRoles();
			return StatusCode(result.Status, result);
		}

		[HttpGet("roles-map-user")]
		public async Task<IActionResult> GetRolesMapUser()
		{
			var result = await _roleService.GetRolesMapUser();
			return StatusCode(result.Status, result);
		}

		[HttpGet("role-by-id")]
		public async Task<IActionResult> GetRoleByID(int roleID)
		{
			var result = await _roleService.GetRoleByID(roleID);
			return StatusCode(result.Status, result);
		}

		[HttpPost("create-role")]
		public async Task<IActionResult> CreateRole(CreateRoleRequest newRole)
		{
			var result = await _roleService.CreateRole(newRole);
			return StatusCode(result.Status, result);
		}

		[HttpPost("create-mapping")]
		public async Task<IActionResult> CreateRoleMapUser(CreateUserRoleRequest newUserRole)
		{
			var result = await _roleService.CreateRolesMapUser(newUserRole);
			return StatusCode(result.Status, result);
		}

		[HttpPut("update-role")]
		public async Task<IActionResult> UpdateRole(int roleID, RoleProfile role)
		{
			var result = await _roleService.UpdateRole(roleID, role);
			return StatusCode(result.Status, result);
		}

		[HttpDelete("delete-role/{roleID}")]
		public async Task<IActionResult> DeleteRole(int roleID)
		{
			var result = await _roleService.DeleteRole(roleID);
			return StatusCode(result.Status, result);
		}

		[HttpDelete("delete-mapping/{userRoleID}")]
		public async Task<IActionResult> DeleteRoleMapUser(int userRoleID)
		{
			var result = await _roleService.DeleteRoleMapUser(userRoleID);
			return StatusCode(result.Status, result);
		}
	}
}
