using BlossomServer.Datas.ServiceImage;
using BlossomServer.Response;

namespace BlossomServer.Services.ProductImageServices
{
	public interface IProductImageService
	{
		Task<ApiResponse<List<ServiceImageProfile>>> GetImagesByService(int serviceID);
		Task<ApiResponse<List<ServiceImageProfile>>> GetAllImages();
		Task<ApiResponse<ServiceImageProfile>> UploadImage(IFormFile file, CreateServiceImageRequest request);
		Task<ApiResponse<ServiceImageProfile>> DeleteImage(int ImageID);
	}
}
