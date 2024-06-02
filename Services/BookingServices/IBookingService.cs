using BlossomServer.Datas.Booking;
using BlossomServer.Datas.ServiceBooking;
using BlossomServer.Response;

namespace BlossomServer.Services.BookingServices
{
	public interface IBookingService
	{
		Task<ApiResponse<BookingProfile>> CreateBooking(CreateBookingRequest booking);
		Task<ApiResponse<string>> AddServiceToBooking(CreateServiceBookingListRequest requests);
		Task<ApiResponse<List<BookingProfile>>> GetBookings(FilterBooking? filter);
		Task<ApiResponse<List<ServiceBookingProfile>>> GetServiceByBookingID(int bookingID);
		Task<ApiResponse<BookingProfile>> GetBookingByID(int bookingID);
		Task<ApiResponse<BookingProfile>> GetBookingByCustomer(Guid? customerID);
		Task<ApiResponse<BookingProfile>> UpdateBooking(int bookingID, UpdateBookingRequest booking);
		Task<ApiResponse<BookingProfile>> DeleteBooking(int bookingID);
		Task<ApiResponse<object>> RemoveServiceFromBooking(int serviceBookingID);
		Task<ApiResponse<List<EmployeeAvailabilityResponse>>> GetEmployeesAvailability(EmployeeAvailabilityRequest request);
	}
}
