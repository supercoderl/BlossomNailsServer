using BlossomServer.Datas.Service;
using BlossomServer.Response;

namespace BlossomServer.Services.ProductServices
{
	public interface IProductService
	{
		Task<ApiResponse<List<ServiceProfile>>> GetServices(FilterService? filter);
		Task<ApiResponse<ServiceProfile>> CreateService(CreateServiceRequest service);
		Task<ApiResponse<ServiceProfile>> UpdateService(int serviceID, UpdateServiceRequest service);
		Task<ApiResponse<ServiceProfile>> DeleteService(int serviceID);
		Task<ApiResponse<ServiceProfile>> GetServiceByID(int? serviceID);
	}
}
