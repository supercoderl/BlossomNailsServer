using BlossomServer.Datas.Whatsapp;
using BlossomServer.Response;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BlossomServer.Services.WhatsappServices
{
    public class WhatsappService : IWhatsappService
    {
        private readonly IConfiguration _configuration;

        public WhatsappService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse<MessageResource>> SendNotification(string phoneNumber)
        {
            try
            {
                TwilioClient.Init(_configuration["TwillioConfiguration:SID"], _configuration["TwillioConfiguration:AuthToken"]);
                var messageOptions = new CreateMessageOptions(
                    new PhoneNumber($"whatsapp:+84{phoneNumber}"));
                messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
                messageOptions.Body = "Your appointment is coming up on July 21 at 3PM. To get more bookings, go to https://blossom-nails-client.vercel.app";

                var message = await MessageResource.CreateAsync(messageOptions);

                return new ApiResponse<MessageResource>
                {
                    Success = true,
                    Message = "Notification sent successfully!",
                    Data = message,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MessageResource>
                {
                    Success = false,
                    Message = "Whastapp Service - Send Notification: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
