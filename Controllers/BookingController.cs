using BlossomServer.Datas.Booking;
using BlossomServer.Datas.ServiceBooking;
using BlossomServer.Services.BookingServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingController : ControllerBase
	{
		private readonly IBookingService _bookingService;

		public BookingController(IBookingService bookingService)
		{
			_bookingService = bookingService;
		}

		[HttpGet("bookings")]
		public async Task<IActionResult> GetBookings()
		{
			var result = await _bookingService.GetBookings();
			return StatusCode(result.Status, result);
		}

		[HttpGet("booking-empty")]
		public async Task<IActionResult> GetBookingByCustomer()
		{
			var userID = User.FindFirstValue("UserID");
			var result = await _bookingService.GetBookingByCustomer(userID != null ? Guid.Parse(userID) : null);
			return StatusCode(result.Status, result);
		}

		[HttpGet("booking-by-id/{bookingID}")]
		public async Task<IActionResult> GetBookingByID(int bookingID)
		{
			var result = await _bookingService.GetBookingByID(bookingID);
			return StatusCode(result.Status, result);
		}

		[HttpGet("services-in-booking/{bookingID}")]
		public async Task<IActionResult> GetServiceInBooking(int bookingID)
		{
			var result = await _bookingService.GetServiceByBookingID(bookingID);
			return StatusCode(result.Status, result);
		}

		[HttpGet("employees-availability")]
		public async Task<IActionResult> GetEmployeesAvailability([FromQuery] EmployeeAvailabilityRequest request)
		{
			var result = await _bookingService.GetEmployeesAvailability(request);
			return StatusCode(result.Status, result);
		}


		[HttpPost("add-service-to-booking")]
		public async Task<IActionResult> AddServiceToBooking(CreateServiceBookingListRequest requests)
		{
			var result = await _bookingService.AddServiceToBooking(requests);
			return StatusCode(result.Status, result);
		}

		[HttpPut("update-booking/{bookingID}")]
		public async Task<IActionResult> UpdateBooking(int bookingID, UpdateBookingRequest request)
		{
			var result = await _bookingService.UpdateBooking(bookingID, request);
			return StatusCode(result.Status, result);
		}

		[HttpDelete("remove-service-from-booking/{serviceBookingID}")]
		public async Task<IActionResult> RemoveServiceFromBooking(int serviceBookingID)
		{
			var result = await _bookingService.RemoveServiceFromBooking(serviceBookingID);
			return StatusCode(result.Status, result);
		}
	}
}
