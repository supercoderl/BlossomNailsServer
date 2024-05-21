using BlossomServer.Datas.Category;
using BlossomServer.Datas.Service;
using BlossomServer.Services.CategoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		[HttpGet("categories")]
		public async Task<IActionResult> GetCategories([FromQuery] FilterCategory? filterObject)
		{
			var result = await _categoryService.GetCategories(filterObject);
			return StatusCode(result.Status, result);
		}

		[HttpPost("create-category")]
		public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
		{
			var result = await _categoryService.CreateCategory(request);
			return StatusCode(result.Status, result);
		}
	}
}
