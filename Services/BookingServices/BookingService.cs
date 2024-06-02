using AutoMapper;
using BlossomServer.Datas.Booking;
using BlossomServer.Datas.Email;
using BlossomServer.Datas.Service;
using BlossomServer.Datas.ServiceBooking;
using BlossomServer.Datas.User;
using BlossomServer.Entities;
using BlossomServer.Response;
using BlossomServer.Services.EmailService;
using BlossomServer.Services.ProductServices;
using BlossomServer.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Reflection;
using static ICSharpCode.SharpZipLib.Zip.ZipEntryFactory;

namespace BlossomServer.Services.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly BlossomNailsContext _context;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public BookingService(BlossomNailsContext context, IMapper mapper, IProductService productService, IUserService userService, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
            _userService = userService;
            _emailService = emailService;
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
                    CustomerID = customerID,
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

        public async Task<ApiResponse<List<BookingProfile>>> GetBookings(FilterBooking? filter)
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
                var bookingsProfile = bookings.Select(booking => _mapper.Map<BookingProfile>(booking)).ToList();
                if (filter != null) bookingsProfile = FilterBooking(bookingsProfile, filter);

                return new ApiResponse<List<BookingProfile>>
                {
                    Success = true,
                    Message = $"Get bookings successfully. Found {bookingsProfile.Count()} bookings!",
                    Data = bookingsProfile,
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
                            CustomerEmail = booking.CustomerEmail,
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
                        var mailSent = await SendThanksMail(booking.CustomerEmail, booking.CustomerName, bookingID, booking.BookingDate, booking.StartTime);
                        if (!mailSent)
                        {
                            return new ApiResponse<BookingProfile>
                            {
                                Success = false,
                                Message = "Send mail failed",
                                Status = (int)HttpStatusCode.OK,
                            };
                        }
                    }

                    else
                    {
                        _mapper.Map(booking, emptyBooking);

                        _context.Bookings.Update(emptyBooking);
                        await _context.SaveChangesAsync();
                        if (booking.Status == "Booked")
                        {
                            var mailSent = await SendThanksMail(booking.CustomerEmail, booking.CustomerName, bookingID, booking.BookingDate, booking.StartTime);
                            if (!mailSent)
                            {
                                return new ApiResponse<BookingProfile>
                                {
                                    Success = false,
                                    Message = "Send mail failed",
                                    Status = (int)HttpStatusCode.OK,
                                };
                            }
                        }
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

        private async Task<bool> SendThanksMail(string? mail, string? customerName, int bookingID, string? bookingDate, string? startTime)
        {
            if (mail == null || customerName == null || bookingDate == null || startTime == null)
                return false;
            DateTime time = DateTime.ParseExact(startTime, "HH:mm:ss", null);
            string formattedTime = time.ToString("hh:mm tt");

            var message = new Message(
            new string[] { mail },
            "Your appointment has been approved",
                $"<div style=\"margin:0;padding:0;\">\r\n" +
                $"<div>\r\n" +
                $"<table cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: collapse; table-layout: fixed; border-spacing: 0px; vertical-align: top; min-width: 320px; margin: 0px auto; width: 100%;\">\r\n" +
                $"<tbody>\r\n" +
                $"<tr style=\"vertical-align:top\">\r\n" +
                $"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td align=\"center\">\r\n" +
                $"<div style=\"background-color:transparent\">\r\n" +
                $"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:transparent;word-break:break-word\">\r\n" +
                $"<div style=\"display:table;border-collapse:collapse;width:100%;background-color:transparent\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
                $"<tbody><tr style=\"background-color:transparent\">\r\n" +
                $"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:5px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
                $"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
                $"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;min-width:100%\" width=\"100%\">\r\n" +
                $"<tbody>\r\n" +
                $"<tr style=\"vertical-align:top\">\r\n" +
                $"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;padding-right:10px;padding-left:10px;padding-top:10px;padding-bottom:10px;min-width:100%\">\r\n" +
                $"<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"0px\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;border-top:1px solid transparent\" width=\"100%\">\r\n" +
                $"<tbody>\r\n" +
                $"<tr style=\"vertical-align:top\">\r\n" +
                $"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;font-size:0px;line-height:0px\">\r\n\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody>\r\n" +
                $"</table>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody>\r\n" +
                $"</table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"<div style=\"background-color:transparent\">\r\n" +
                $"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:transparent;word-break:break-word\">\r\n" +
                $"<div style=\"display:table;border-collapse:collapse;width:100%;background-color:transparent\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
                $"<tbody>" +
                $"<tr style=\"background-color:transparent\">\r\n" +
                $"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:0px;padding-bottom:0px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
                $"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
                $"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
                $"<div style=\"padding-top:0px;padding-right:0px;padding-left:0px;padding-bottom:0px\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;min-width:100%\" width=\"100%\">\r\n" +
                $"<tbody>\r\n" +
                $"<tr style=\"vertical-align:top\">\r\n" +
                $"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;padding-right:0px;padding-left:0px;padding-top:0px;padding-bottom:0px;min-width:100%\">\r\n" +
                $"<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"0px\" style=\"border-collapse:collapse;table-layout:fixed;border-spacing:0;vertical-align:top;border-top:3px solid #a2acb7\" width=\"100%\">\r\n" +
                $"<tbody>\r\n" +
                $"<tr style=\"vertical-align:top\">\r\n" +
                $"<td style=\"word-break:break-word;border-collapse:collapse!important;vertical-align:top;font-size:0px;line-height:0px\">\r\n" +
                $"\r\n\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody>\r\n" +
                $"</table>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody>\r\n" +
                $"</table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n " +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"<div style=\"background-color:transparent\">\r\n" +
                $"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:#ffffff;word-break:break-word\">\r\n" +
                $"<div style=\"display: table; border-collapse: collapse; width: 100%;\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:5px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
                $"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
                $"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
                $"<div align=\"center\" style=\"padding-left:0px;padding-right:0px; padding-top: 20px;\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr style=\"line-height:0px;line-height:0px\">\r\n" +
                $"<td align=\"center\" style=\"padding-right:0px;padding-left:0px\">\r\n" +
                $"<div style=\"font-size:1px;line-height:10px\">\r\n" +
                $"</div>\r\n" +
                $"<img align=\"center\" alt=\"Image\" border=\"0\" src=\"https://blossom-nails-client.vercel.app/logo.png\" style=\"outline: none; clear: both; border: 0px; height: auto; float: none; width: 100%; max-width: 80px; display: block !important;\" title=\"Image\" width=\"80\">\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"<div>\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td style=\"padding-right:10px;padding-left:10px;padding-top:10px;padding-bottom:10px\">\r\n" +
                $"<div style=\"color:#555555;line-height:150%;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n" +
                $"<div style=\"font-size: 12px; line-height: 18px;\">\r\n" +
                $"<p style=\"margin:0;font-size:20px;line-height:21px;text-align:center\">\r\n" +
                $"Blossom Nails\r\n" +
                $"</p>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"<div style=\"background-color:transparent\">\r\n" +
                $"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:#ffffff;word-break:break-word\">\r\n" +
                $"<div style=\"display: table; border-collapse: collapse; width: 100%;\">\r\n               " +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n                " +
                $"<tbody><tr>\r\n" +
                $"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n                   " +
                $"<tbody><tr>\r\n                    " +
                $"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:0px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
                $"<div style=\"min-width:320px;display:table-cell;max-width:600px;vertical-align:top\">\r\n" +
                $"<div style=\"width:100%!important;background-color:transparent\">\r\n" +
                $"<div style=\"padding-top:0px;padding-right:0px;padding-left:0px;padding-bottom:5px\">\r\n" +
                $"<div>\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td style=\"padding-right:10px;padding-left:10px;padding-top:10px;padding-bottom:20px\">\r\n" +
                $"<div style=\"color:#0d0d0d;line-height:120%;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n" +
                $"<div style=\"font-size: 12px; line-height: 14px;\">\r\n<p style=\"margin:0;font-size:14px;line-height:17px;text-align:center\">\r\n" +
                $"<span style=\"font-size:36px;line-height:55px\">\r\n" +
                $"<span style=\"color:#a2acb7\">\r\n" +
                $"<strong>\r\n" +
                $"BookingID #{bookingID}\r\n" +
                $"</strong>\r\n" +
                $"</span>\r\n" +
                $"</span>\r\n" +
                $"</p>\r\n" +
                $"<p style=\"margin:0;font-size:14px;line-height:17px;text-align:center\">\r\n" +
                $"<span style=\"font-size:28px;line-height:33px\">\r\n" +
                $"Thank you for your booking!\r\n" +
                $"<br>\r\n" +
                $"</span>\r\n" +
                $"</p>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"<div align=\"center\" style=\"padding-left:0px;padding-right:0px\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr style=\"line-height:0px;line-height:0px\">\r\n" +
                $"<td align=\"center\" style=\"padding-right:0px;padding-left:0px\">\r\n" +
                $"<img align=\"center\" alt=\"Image\" border=\"0\" src=\"https://ci6.googleusercontent.com/proxy/C84yaqYHHeoHcq2KCRRU3NsMGJwpx8hNhn6BHto3yo04Xpo1IOJIw4y1rIjEhyHX53emw6aqeh-PNk6Kb4SXvsl_Dqrgn2vimQIHkj1On4kTnPa8s-7eTbM=s0-d-e1-ft#https://d1oco4z2z1fhwp.cloudfront.net/templates/default/90/divider.png\" style=\"outline: none; clear: both; border: 0px; height: auto; float: none; width: 100%; max-width: 316px; display: block !important;\" title=\"Image\" width=\"316\">\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"<div>\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td style=\"padding-right:20px;padding-left:20px;padding-top:10px;padding-bottom:10px\">\r\n" +
                $"<div style=\"color:#555555;line-height:150%;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n" +
                $"<div style=\"font-size: 12px; line-height: 18px;\">\r\n" +
                $"<p style=\"margin:0;font-size:14px;line-height:21px;text-align:center\">\r\n" +
                $"Dear {customerName},\r\n" +
                $"</p>\r\n" +
                $"<p style=\"margin:0;font-size:14px;line-height:21px;text-align:center\">\r\n" +
                $"Your appointment is set for {bookingDate} {formattedTime}. Our team is eager to give you a fantastic experience, whether you're after a classic manicure, trendy nail art, or a relaxing pedicure. We're here to make sure you leave feeling pampered and pleased with your nails.\r\n" +
                $"</p>\r\n" +
                $"<p style=\"margin:0;font-size:14px;line-height:21px;text-align:center\">\r\n" +
                $"Thank you once again for choosing Blossom Nails. We can’t wait to welcome you and provide you with an exceptional nail care experience.\r\n" +
                $"</p>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"<div align=\"center\" style=\"padding-left:0px;padding-right:0px; padding-bottom: 20px;\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr style=\"line-height:0px;line-height:0px\">\r\n" +
                $"<td align=\"center\" style=\"padding-right:0px;padding-left:0px\">\r\n" +
                $"<img align=\"center\" alt=\"Image\" border=\"0\" src=\"https://ci6.googleusercontent.com/proxy/C84yaqYHHeoHcq2KCRRU3NsMGJwpx8hNhn6BHto3yo04Xpo1IOJIw4y1rIjEhyHX53emw6aqeh-PNk6Kb4SXvsl_Dqrgn2vimQIHkj1On4kTnPa8s-7eTbM=s0-d-e1-ft#https://d1oco4z2z1fhwp.cloudfront.net/templates/default/90/divider.png\" style=\"outline: none; clear: both; border: 0px; height: auto; float: none; width: 100%; max-width: 316px; display: block !important;\" title=\"Image\" width=\"316\">\r\n                           " +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"<div style=\"background-color:transparent\">\r\n" +
                $"<div style=\"Margin:0 auto;max-width:600px;word-wrap:break-word;min-width:320px;background-color:#a2acb7;word-break:break-word\">\r\n" +
                $"<div style=\"display: table; border-collapse: collapse; width: 100%;\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td align=\"center\" style=\"background-color:transparent\">\r\n" +
                $"<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:600px\">\r\n" +
                $"<tbody><tr>\r\n" +
                $"<td align=\"center\" style=\"width:600px;padding-right:0px;padding-left:0px;padding-top:5px;padding-bottom:5px;border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent\" valign=\"top\" width=\"600\">\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody></table>\r\n" +
                $"</td>\r\n" +
                $"</tr>\r\n" +
                $"</tbody>\r\n" +
                $"</table>\r\n" +
                $"</div>\r\n" +
                $"</div>"
            );
            var result = await _emailService.SendEmail(message);
            return result.Success;
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
                        var timeCheck = CombineStringDateAndTime(request.BookingDate, request.StartTime);
                        bool isBusy = bookings.Data.Any(b =>
                        {
                            var start = CombineDateAndTime(b.BookingDate, b.StartTime);
                            var end = CombineDateAndTime(b.BookingDate, b.EndTime);
                            return timeCheck >= start && timeCheck <= end;
                        });
                        var employeeAvailability = new EmployeeAvailabilityResponse
                        {
                            EmployeeID = user.UserID,
                            Firstname = user.Firstname,
                            Lastname = user.Lastname,
                            IsBusy = isBusy,
                        };
                        result.Add(employeeAvailability);
                    }
                    else
                    {
                        var employeeAvailability = new EmployeeAvailabilityResponse
                        {
                            EmployeeID = user.UserID,
                            Firstname = user.Firstname,
                            Lastname = user.Lastname,
                            IsBusy = false,
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

        private static DateTime? CombineDateAndTime(string? date, string? time)
        {
            if (date is null || time is null) return null;
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]);
            int minutes = int.Parse(timeParts[1]);
            int seconds = int.Parse(timeParts[2]);

            DateTime dateTime;
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                DateTime finalDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, seconds);
                return finalDateTime;
            }

            return null;
        }

        private static DateTime CombineStringDateAndTime(string? date, string? time)
        {
            if (date is null || time is null) return DateTime.Now;
            string dateTimeString = $"{date} {time}";
            string format = "yyyy-MM-dd HH:mm:ss";

            DateTime dateTime = DateTime.ParseExact(dateTimeString, format, CultureInfo.InvariantCulture);

            return dateTime;
        }

        private List<BookingProfile> FilterBooking(List<BookingProfile> bookings, FilterBooking filter)
        {
            if (filter.BookingDateFrom is not null && filter.BookingDateTo is not null)
            {
                DateTime bookingDateFrom = DateTime.ParseExact(filter.BookingDateFrom, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime bookingDateTo = DateTime.ParseExact(filter.BookingDateTo, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                bookings = bookings.Where(x => x.BookingDate != null &&
                    DateTime.ParseExact(x.BookingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= bookingDateFrom
                    &&
                    DateTime.ParseExact(x.BookingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= bookingDateTo
               ).ToList();
            }
            if (filter.PriceMin is not null && filter.PriceMax is not null)
                bookings = bookings.Where(x => x.TotalCost >= filter.PriceMin && x.TotalCost <= filter.PriceMax).ToList();
            if (filter.SortType is not null)
            {
                PropertyInfo propertyInfo = typeof(BookingProfile).GetProperty(char.ToUpper(filter.SortType[0]) + filter.SortType.Substring(1))!;
                switch (filter.SortFrom)
                {
                    case "ascending":
                        bookings = bookings.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                        break;
                    default:
                        bookings = bookings.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(filter.SearchText))
                bookings = bookings.Where(x =>
                    x.CustomerName != null && x.CustomerName.ToLower().Contains(filter.SearchText.ToLower()) ||
                    x.CustomerPhone != null && x.CustomerPhone.ToLower().Contains(filter.SearchText.ToLower()) ||
                    x.CustomerEmail != null && x.CustomerEmail.ToLower().Contains(filter.SearchText.ToLower())
                ).ToList();
            return bookings;
        }
    }
}
