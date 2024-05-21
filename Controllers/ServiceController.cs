using AutoMapper;
using BlossomServer.Datas.Service;
using BlossomServer.Services.ProductServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + "Firebase")]
	public class ServiceController : ControllerBase
	{
		private readonly IProductService _productService;
		private readonly IMapper _mapper;

		public ServiceController(IProductService productService, IMapper mapper)
		{
			_productService = productService;
			_mapper = mapper;
		}

		[AllowAnonymous]
		[HttpGet("services")]
		public async Task<IActionResult> GetServices([FromQuery] FilterService? filterObject)
		{
			var result = await _productService.GetServices(filterObject);
			return StatusCode(result.Status, result);
		}

		[HttpPut("update-service/{serviceID}")]
		public async Task<IActionResult> UpdateProduct(int serviceID, UpdateServiceRequest request)
		{
			var result = await _productService.UpdateService(serviceID, request);
			return StatusCode(result.Status, result);
		}

		[HttpDelete("delete-service/{serviceID}")]
		public async Task<IActionResult> DeleteProduct(int serviceID)
		{
			var result = await _productService.DeleteService(serviceID);
			return StatusCode(result.Status, result);
		}

		[AllowAnonymous]
		[HttpGet("service-by-id/{serviceID}")]
		public async Task<IActionResult> GetProductByID(int serviceID)
		{
			var result = await _productService.GetServiceByID(serviceID);
			return StatusCode(result.Status, result);
		}

		[HttpPost("create-service")]
		public async Task<IActionResult> CreateProduct(CreateServiceRequest request)
		{
			var result = await _productService.CreateService(request);
			return StatusCode(result.Status, result);
		}
	}
}
