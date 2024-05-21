using BlossomServer.Datas.Category;
using BlossomServer.Response;

namespace BlossomServer.Services.CategoryServices
{
	public interface ICategoryService
	{
		Task<ApiResponse<List<CategoryProfile>>> GetCategories(FilterCategory? filter);
		Task<ApiResponse<CategoryProfile>> CreateCategory(CreateCategoryRequest request);
	}
}
