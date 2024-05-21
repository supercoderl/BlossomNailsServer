using BlossomServer.Response;

namespace BlossomServer.Services.FileServices
{
	public interface IFileService
	{
		Task<ApiResponse<object>> ImportFile(IFormFile file);
		byte[] CreateFile<T>(List<T> source);
	}
}
