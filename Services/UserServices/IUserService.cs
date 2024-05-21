using BlossomServer.Datas.Authentication;
using BlossomServer.Datas.User;
using BlossomServer.Response;
using static BlossomServer.Datas.Authentication.Login;

namespace BlossomServer.Services.UserServices
{
	public interface IUserService
	{
		Task<ApiResponse<LoginResult>> Login(LoginRequest request);
		Task<ApiResponse<CreateUserRequest>> Register(CreateUserRequest newUser);
		Task<ApiResponse<string>> ChangePassword(Guid UserID, ChangePasswordRequest request);
		Task<bool> IsExist(string userName);
		Task<ApiResponse<Object>> Logout(string token);
		Task<ApiResponse<UserProfile>> GetProfile(Guid UserID);
		Task<ApiResponse<List<UserProfile>>> GetUsers(FilterUser? filter);
		Task<ApiResponse<UpdateUserRequest>> UpdateUser(Guid UserID, UpdateUserRequest user);
		Task<ApiResponse<Object>> DeleteUser(Guid UserID);
		ApiResponse<Object> SendBackupEmail(string recipientEmail, string token);
	}
}
