using AutoMapper;
using BlossomServer.Datas.ServiceImage;
using BlossomServer.Services.ProductImageServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServiceImageController : ControllerBase
	{
		private readonly IProductImageService _productImageService;
		private readonly IMapper _mapper;

		public ServiceImageController(IProductImageService productImageService, IMapper mapper)
		{
			_productImageService = productImageService;
			_mapper = mapper;
		}

		[HttpGet("images/{serviceID}")]
		public async Task<IActionResult> GetImagesByProduct(int serviceID)
		{
			var result = await _productImageService.GetImagesByService(serviceID);
			return StatusCode(result.Status, result);
		}

		[HttpGet("images")]
		public async Task<IActionResult> GetAllImages()
		{
			var result = await _productImageService.GetAllImages();
			return StatusCode(result.Status, result);
		}

		[HttpPost("upload-image")]
		public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] CreateServiceImageRequest request)
		{
			var result = await _productImageService.UploadImage(file, request);
			return StatusCode(result.Status, result);
		}

		[HttpDelete("image/{ImageID}")]
		public async Task<IActionResult> DeleteImage(int ImageID)
		{
			var result = await _productImageService.DeleteImage(ImageID);
			return StatusCode(result.Status, result);
		}
	}
}
