using BlossomServer.Datas.Email;
using BlossomServer.Response;

namespace BlossomServer.Services.EmailService
{
	public interface IEmailService
	{
		Task<ApiResponse<string>> SendEmail(Message message);
	}
}
