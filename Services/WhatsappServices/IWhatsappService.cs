using BlossomServer.Response;
using Twilio.Rest.Api.V2010.Account;

namespace BlossomServer.Services.WhatsappServices
{
    public interface IWhatsappService
    {
        Task<ApiResponse<MessageResource>> SendNotification(string phoneNumber);
    }
}
