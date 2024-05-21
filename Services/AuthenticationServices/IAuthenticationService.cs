using BlossomServer.Entities;
using BlossomServer.Response;
using static BlossomServer.Datas.Authentication.Login;
using System.Security.Claims;
using BlossomServer.Datas.Authentication;

namespace BlossomServer.Services.AuthenticationServices
{
	public interface IAuthenticationService
	{
		LoginResult GenerateToken(User user, List<Claim> claims, DateTime now);
		Task RemoveRefreshToken(string username);
		Task<ApiResponse<LoginResult>> Refresh(string refreshToken, DateTime now);
		Task<bool> RevokeToken(string token);
		Task<ApiResponse<string>> SendResetCode(SendResetCodeRequest request);
		Task<ApiResponse<string>> VerifyResetCode(VerifyCodeRequest request);
		Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest request);
	}
}
