using AutoMapper;
using BlossomServer.Datas.Booking;
using BlossomServer.Datas.Service;
using BlossomServer.Datas.ServiceBooking;
using BlossomServer.Datas.User;
using BlossomServer.Entities;
using BlossomServer.Response;
using BlossomServer.Services.ProductServices;
using BlossomServer.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;

namespace BlossomServer.Services.BookingServices
{
	public class BookingService : IBookingService
	{
		private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;
		private readonly IProductService _productService;
		private readonly IUserService _userService;

		public BookingService(BlossomNailsContext context, IMapper mapper, IProductService productService, IUserService userService)
		{
			_context = context;
			_mapper = mapper;
			_productService = productService;
			_userService = userService;
		}

		public async Task<ApiResponse<string>> AddServiceToBooking(CreateServiceBookingListRequest requests)
		{
			try
			{
				if (requests.Requests.Any())
				{
					requests.Requests.ForEach(r =>
					{
						var serviceBookingEntity = _mapper.Map<ServiceBooking>(r);
						_context.ServiceBookings.Add(serviceBookingEntity);
					});
					await _context.SaveChangesAsync();
				}
				return new ApiResponse<string>
				{
					Success = true,
					Message = $"Services were selected.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<string>
				{
					Success = false,
					Message = "BookingService - AddServiceToBooking: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<BookingProfile>> CreateBooking(CreateBookingRequest booking)
		{
			try
			{
				await Task.CompletedTask;
				var bookingEntity = _mapper.Map<Booking>(booking);
				await _context.AddAsync(bookingEntity);
				await _context.SaveChangesAsync();
				return new ApiResponse<BookingProfile>
				{
					Success = true,
					Message = $"Created booking {bookingEntity.BookingID}.",
					Data = _mapper.Map<BookingProfile>(bookingEntity),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<BookingProfile>
				{
					Success = false,
					Message = "BookingService - CreateBooking: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<BookingProfile>> DeleteBooking(int bookingID)
		{
			try
			{
				var result = await GetBookingByID(bookingID);
				if (result.Data != null)
				{
					result.Data.DeletedAt = DateTime.Now;
					_context.Bookings.Update(_mapper.Map<Booking>(result.Data));
				}
				await _context.SaveChangesAsync();
				return new ApiResponse<BookingProfile>
				{
					Success = true,
					Message = $"Deleted booking with ID {bookingID}.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<BookingProfile>
				{
					Success = false,
					Message = "BookingService - DeleteBooking: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<BookingProfile>> GetBookingByCustomer(Guid? customerID)
		{
			try
			{
				await Task.CompletedTask;
				var bookingsEntity = await _context.Bookings.Where(booking => booking.Status == "Created").ToListAsync();
				if (bookingsEntity.Any())
				{
					if (customerID != null)
					{
						var bookingEntity = bookingsEntity.FirstOrDefault(booking => booking.CustomerID == customerID);

						if (bookingEntity is null)
						{
							return await CreateBooking(new CreateBookingRequest
							{
								CustomerID = customerID!.Value,
							});
						}

						return new ApiResponse<BookingProfile>
						{
							Success = true,
							Message = "Get booking successfully.",
							Data = _mapper.Map<BookingProfile>(bookingEntity),
							Status = (int)HttpStatusCode.OK
						};
					}
					else
					{
						var bookingEntity = bookingsEntity.FirstOrDefault(booking => booking.CustomerID == null);

						if (bookingEntity is null)
						{
							return await CreateBooking(new CreateBookingRequest());
						}

						return new ApiResponse<BookingProfile>
						{
							Success = true,
							Message = "Get booking successfully.",
							Data = _mapper.Map<BookingProfile>(bookingEntity),
							Status = (int)HttpStatusCode.OK
						};
					}
				}
				return await CreateBooking(new CreateBookingRequest
				{
					CustomerID = customerID!.Value,
				});
			}
			catch (Exception ex)
			{
				return new ApiResponse<BookingProfile>
				{
					Success = false,
					Message = "BookingService - GetBookingByCustomer: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<BookingProfile>>> GetBookingsByEmployee(Guid? employeeID)
		{
			try
			{
				await Task.CompletedTask;
				var bookingsEntity = await _context.Bookings.Where(booking => booking.NailTechnicianID == employeeID).ToListAsync();
				if (!bookingsEntity.Any())
				{
					return new ApiResponse<List<BookingProfile>>
					{
						Success = false,
						Message = "This employee is not scheduled",
						Status = (int)HttpStatusCode.OK
					};
				}
				return new ApiResponse<List<BookingProfile>>
				{
					Success = true,
					Message = "Get bookings successfully.",
					Data = bookingsEntity.Select(booking => _mapper.Map<BookingProfile>(booking)).ToList(),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<BookingProfile>>
				{
					Success = false,
					Message = "Booking Service - Get Bookings By Employee: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<BookingProfile>> GetBookingByID(int bookingID)
		{
			try
			{
				var booking = await _context.Bookings.FindAsync(bookingID);
				if (booking == null)
				{
					return new ApiResponse<BookingProfile>
					{
						Success = false,
						Message = $"Booking with ID {bookingID} does not exists.",
						Status = (int)HttpStatusCode.OK
					};
				}
				return new ApiResponse<BookingProfile>
				{
					Success = true,
					Message = $"Get booking with ID {bookingID} successfully.",
					Data = _mapper.Map<BookingProfile>(booking),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<BookingProfile>
				{
					Success = false,
					Message = "BookingService - GetBookingByID: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<BookingProfile>>> GetBookings()
		{
			try
			{
				await Task.CompletedTask;
				var bookings = await _context.Bookings.Where(booking => booking.DeletedAt == null).ToListAsync();
				if (!bookings.Any())
				{
					return new ApiResponse<List<BookingProfile>>
					{
						Success = false,
						Message = "There aren't any bookings.",
						Status = (int)HttpStatusCode.OK
					};
				}
				var bookingProfile = bookings.Select(booking => _mapper.Map<BookingProfile>(booking)).ToList();
				return new ApiResponse<List<BookingProfile>>
				{
					Success = true,
					Message = $"Get bookings successfully. Found {bookings.Count()} bookings!",
					Data = bookings.Select(booking => _mapper.Map<BookingProfile>(booking)).ToList(),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<BookingProfile>>
				{
					Success = false,
					Message = "BookingService - GetBookings: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<ServiceBookingProfile>>> GetServiceByBookingID(int bookingID)
		{
			try
			{
				var services = await _context.ServiceBookings.Where(service => service.BookingID == bookingID).ToListAsync();
				ServiceProfile service = new ServiceProfile();
				List<ServiceBookingProfile> serviceBookingProfiles = new List<ServiceBookingProfile>();
				if (services.Any())
				{
					serviceBookingProfiles = services.Select(serviceBooking => _mapper.Map<ServiceBookingProfile>(serviceBooking)).ToList();
					foreach (var serviceBooking in serviceBookingProfiles)
					{
						var result = await _productService.GetServiceByID(serviceBooking.ServiceID);
						if (result.Data != null)
						{
							serviceBooking.Service = result.Data;
						}
					}
				}

				return new ApiResponse<List<ServiceBookingProfile>>
				{
					Success = true,
					Message = "",
					Data = serviceBookingProfiles,
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<ServiceBookingProfile>>
				{
					Success = false,
					Message = "BookingService - GetServiceByBookingID: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<object>> RemoveServiceFromBooking(int serviceBookingID)
		{
			try
			{
				var serviceBooking = await _context.ServiceBookings.FindAsync(serviceBookingID);
				if (serviceBooking == null)
				{
					return new ApiResponse<object>
					{
						Success = false,
						Message = "Booking has not this service.",
						Status = (int)HttpStatusCode.OK
					};
				}
				_context.ServiceBookings.Remove(serviceBooking);
				await _context.SaveChangesAsync();
				return new ApiResponse<object>
				{
					Success = true,
					Message = "Removed this service from booking.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<object>
				{
					Success = false,
					Message = "BookingService - RemoveServiceFromBooking: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<BookingProfile>> UpdateBooking(int bookingID, UpdateBookingRequest booking)
		{
			try
			{
				if (bookingID != booking.BookingID)
				{
					return new ApiResponse<BookingProfile>
					{
						Success = false,
						Message = "Booking with ID not match.",
						Status = (int)HttpStatusCode.OK
					};
				}

				using (var transaction = await _context.Database.BeginTransactionAsync())
				{
					var emptyBooking = await _context.Bookings.FirstOrDefaultAsync(booking => booking.BookingID == bookingID && booking.Status == "Created");

					if (emptyBooking == null)
					{
						emptyBooking = new Booking
						{
							BookingID = 0,
							CustomerName = booking.CustomerName,
							CustomerPhone = booking.CustomerPhone,
							NailTechnicianID = booking.NailTechnicianID,
							BookingDate = booking.BookingDate,
							StartTime = booking.StartTime,
							EndTime = booking.EndTime,
							Status = string.IsNullOrEmpty(booking.Status) ? "Booked" : booking.Status,
							TotalCost = Convert.ToDecimal(booking.TotalCost),
							Notes = booking.Notes,
							CreatedAt = DateTime.Now,
						};
						await _context.Bookings.AddAsync(emptyBooking);
						await _context.SaveChangesAsync();
					}

					else
					{
						_mapper.Map(booking, emptyBooking);

						_context.Bookings.Update(emptyBooking);
						await _context.SaveChangesAsync();
					}
					await transaction.CommitAsync();
					return new ApiResponse<BookingProfile>
					{
						Success = true,
						Message = $"Updated booking with ID {bookingID}.",
						Data = _mapper.Map<BookingProfile>(emptyBooking),
						Status = (int)HttpStatusCode.OK
					};
				}
			}
			catch (Exception ex)
			{
				return new ApiResponse<BookingProfile>
				{
					Success = false,
					Message = "BookingService - UpdateBooking: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<EmployeeAvailabilityResponse>>> GetEmployeesAvailability(EmployeeAvailabilityRequest request)
		{
			List<EmployeeAvailabilityResponse> result = new List<EmployeeAvailabilityResponse>();
			try
			{
				await Task.CompletedTask;
				var users = await _userService.GetUsers(new FilterUser { Role = "Employee" });
				if (users.Data is null)
				{
					return new ApiResponse<List<EmployeeAvailabilityResponse>>
					{
						Success = false,
						Message = "There aren't any employees.",
						Status = (int)HttpStatusCode.OK
					};
				}
				foreach (var user in users.Data)
				{
					var bookings = await GetBookingsByEmployee(user.UserID);
					if (bookings.Data is not null && bookings.Data.Any())
					{
						bool isBusy = bookings.Data.Any(b =>
							CombineStringDateAndTime(request.BookingDate, request.StartTime) > CombineDateAndTime(b.BookingDate, b.StartTime)
							&&
							CombineStringDateAndTime(request.BookingDate, request.StartTime) < CombineDateAndTime(b.BookingDate, b.EndTime));
						var employeeAvailability = new EmployeeAvailabilityResponse
						{
							EmployeeID = user.UserID,
							Firstname = user.Firstname,
							Lastname = user.Lastname,
							IsBusy = isBusy,
						};
						result.Add(employeeAvailability);
					}
				}

				return new ApiResponse<List<EmployeeAvailabilityResponse>>
				{
					Success = true,
					Message = $"Get employees availability, found {result.Count()} employee.",
					Data = result,
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<EmployeeAvailabilityResponse>>
				{
					Success = false,
					Message = "Booking Service - Get Employees Availability: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		private static DateTime? CombineDateAndTime(DateTime? date, string? time)
		{
			if (date is null || time is null) return null;
			string[] timeParts = time.Split(':');
			int hours = int.Parse(timeParts[0]);
			int minutes = int.Parse(timeParts[1]);
			int seconds = int.Parse(timeParts[2]);

			DateTime dateTime = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, hours, minutes, seconds);

			return dateTime;
		}

		private static DateTime CombineStringDateAndTime(string? date, string? time)
		{
			if (date is null || time is null) return DateTime.Now;
			string dateTimeString = $"{date} {time}";
			string format = "dd-MM-yyyy HH:mm:ss";

			DateTime dateTime = DateTime.ParseExact(dateTimeString, format, CultureInfo.InvariantCulture);

			return dateTime;
		}
	}
}
