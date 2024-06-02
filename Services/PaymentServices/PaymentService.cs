using BlossomServer.Datas.Payment;
using BlossomServer.Response;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X9;
using System.Net;

namespace BlossomServer.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;

        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse<VnPayProfile>> VnPayClick(VnPayRequest request, IHttpContextAccessor httpContextAccessor)
        {
            //Get Config Info
            string? vnp_Returnurl = _configuration["VNPayConfiguration:vnp_Returnurl"]; //URL get return value
            string? vnp_Url = _configuration["VNPayConfiguration:vnp_Url"]; //URL VNPAY's payment
            string? vnp_TmnCode = _configuration["VNPayConfiguration:vnp_TmnCode"]; //ID merchant connection (Terminal Id)
            string? vnp_HashSecret = _configuration["VNPayConfiguration:vnp_HashSecret"]; //Secret Key

            try
            {
                await Task.CompletedTask;
                if(vnp_Returnurl == null || vnp_Url == null || vnp_TmnCode == null || vnp_HashSecret == null)
                {
                    return new ApiResponse<VnPayProfile>
                    {
                        Success = false,
                        Message = "Any null value",
                        Status = (int)HttpStatusCode.OK,
                    };
                }

                //Get payment input
                VnPayOrderInfo order = new VnPayOrderInfo();
                order.OrderId = DateTime.Now.Ticks; // ID merchant
                order.Amount = request.Money; // Money
                order.Status = "0"; //0: Payment status
                order.CreatedDate = DateTime.Now;

                //Build URL for VNPAY
                VnPayLibrary vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Payment cost.
                if (request.Bankcode_Vnpayqr)
                {
                    vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
                }
                else if (request.Bankcode_Vnbank)
                {
                    vnpay.AddRequestData("vnp_BankCode", "VNBANK");
                }
                else if (request.Bankcode_Intcard)
                {
                    vnpay.AddRequestData("vnp_BankCode", "INTCARD");
                }

                vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContextAccessor));

                if (request.Locale_Vn)
                {
                    vnpay.AddRequestData("vnp_Locale", "vn");
                }
                else if (request.Locale_En)
                {
                    vnpay.AddRequestData("vnp_Locale", "en");
                }
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
                vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Unique id merchant

                //Add Params of 2.1.0 Version
                //Billing
                var result = new VnPayProfile
                {
                    PaymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret)
                };

                return new ApiResponse<VnPayProfile>
                {
                    Success = true,
                    Message = "Created url",
                    Data = result,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<VnPayProfile>
                {
                    Success = false,
                    Message = "Payment Service - VnPay Click: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
