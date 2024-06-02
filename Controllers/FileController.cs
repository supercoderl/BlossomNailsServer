using BlossomServer.Datas.File;
using BlossomServer.Entities;
using BlossomServer.Response;
using BlossomServer.Services.BookingServices;
using BlossomServer.Services.CategoryServices;
using BlossomServer.Services.FileServices;
using BlossomServer.Services.ProductServices;
using BlossomServer.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FileController : ControllerBase
	{
		private readonly IFileService _fileService;
		private readonly IProductService _productService;
		private readonly IBookingService _bookingService;
		private readonly ICategoryService _categoryService;
		private readonly IUserService _userService;

		public FileController(IFileService fileService, IProductService productService, IBookingService bookingService, ICategoryService categoryService, IUserService userService)
        {
			_fileService = fileService;
			_productService = productService;
			_bookingService = bookingService;
			_categoryService = categoryService;
			_userService = userService;
		}

		[HttpPost("export-excel")]
		public async Task<IActionResult> ExportExcel(string type)
		{
			switch (type)
			{
				case "service":
					var services = await _productService.GetServices(null);
					if(services.Data != null)
					{
						var file = _fileService.CreateFile(services.Data);
						var excel64 = File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{type}s.xlsx");
						return StatusCode(200, new ApiResponse<ExcelProfile> {
							Success = true,
							Message = "Export sucessfully",
							Data = new ExcelProfile { Filename = type + ".xlsx", Data = excel64.FileContents },
							Status = (int)HttpStatusCode.OK
						});
					}
					break;
				case "booking":
					var bookings = await _bookingService.GetBookings(null);
					if (bookings.Data != null)
					{
						var file = _fileService.CreateFile(bookings.Data);
						return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{type}s.xlsx");
					}
					break;
				case "category":
					var categories = await _categoryService.GetCategories(null);
					if (categories.Data != null)
					{
						var file = _fileService.CreateFile(categories.Data);
						return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"categories.xlsx");
					}
					break;
				case "user":
					var users = await _userService.GetUsers(null);
					if (users.Data != null)
					{
						var file = _fileService.CreateFile(users.Data);
						return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{type}s.xlsx");
					}
					break;
			}

			return StatusCode(400);
		}
    }
}
