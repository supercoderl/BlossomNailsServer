using BlossomServer.Datas.Payment;
using BlossomServer.Response;

namespace BlossomServer.Services.PaymentServices
{
    public interface IPaymentService
    {
        Task<ApiResponse<VnPayProfile>> VnPayClick(VnPayRequest request, IHttpContextAccessor httpContextAccessor);
    }
}
