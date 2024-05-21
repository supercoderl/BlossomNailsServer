using AutoMapper;
using BlossomServer.Datas.Authentication;
using BlossomServer.Datas.Booking;
using BlossomServer.Datas.Category;
using BlossomServer.Datas.Contact;
using BlossomServer.Datas.Notification;
using BlossomServer.Datas.Role;
using BlossomServer.Datas.Service;
using BlossomServer.Datas.ServiceBooking;
using BlossomServer.Datas.ServiceImage;
using BlossomServer.Datas.User;
using BlossomServer.Datas.UserRole;
using BlossomServer.Entities;

namespace BlossomServer.Datas.Mapper
{
	public class MapperProfile : Profile
	{
        public MapperProfile()
        {
			CreateMap<Entities.User, CreateUserRequest>();
			CreateMap<CreateUserRequest, Entities.User>();
			CreateMap<UserProfile, Entities.User>();
			CreateMap<Entities.User, UserProfile>();
			CreateMap<Entities.User, UpdateUserRequest>();
			CreateMap<UpdateUserRequest, Entities.User>();

			CreateMap<Entities.Role, RoleProfile>();
			CreateMap<RoleProfile, Entities.Role>();
			CreateMap<Entities.Role, CreateRoleRequest>();
			CreateMap<CreateRoleRequest, Entities.Role>();

			CreateMap<Entities.UserRole, CreateUserRoleRequest>();
			CreateMap<CreateUserRoleRequest, Entities.UserRole>();

			CreateMap<Entities.Service, CreateServiceRequest>();
			CreateMap<CreateServiceRequest, Entities.Service>();
			CreateMap<ServiceProfile, Entities.Service>();
			CreateMap<Entities.Service, ServiceProfile>();
			CreateMap<Entities.Service, UpdateServiceRequest>();
			CreateMap<UpdateServiceRequest, Entities.Service>();

			CreateMap<Entities.ServiceImage, ServiceImageProfile>();
			CreateMap<ServiceImageProfile, Entities.ServiceImage>();
			CreateMap<Entities.ServiceImage, CreateServiceImageRequest>();
			CreateMap<CreateServiceImageRequest, Entities.ServiceImage>();

			CreateMap<Entities.Contact, CreateContactRequest>();
			CreateMap<CreateContactRequest, Entities.Contact>();
			CreateMap<Entities.Contact, ContactProfile>();
			CreateMap<ContactProfile, Entities.Contact>();

			CreateMap<Entities.Category, CategoryProfile>();
			CreateMap<CategoryProfile, Entities.Category>();
			CreateMap<Entities.Category, CreateCategoryRequest>();
			CreateMap<CreateCategoryRequest, Entities.Category>();

			CreateMap<Entities.Notification, NotificationProfile>();
			CreateMap<NotificationProfile, Entities.Notification>();
			CreateMap<Entities.Notification, UpdateNotificationRequest>();
			CreateMap<UpdateNotificationRequest, Entities.Notification>();

			CreateMap<Entities.Booking, CreateBookingRequest>();
			CreateMap<CreateBookingRequest, Entities.Booking>();
			CreateMap<Entities.Booking, UpdateBookingRequest>();
			CreateMap<UpdateBookingRequest, Entities.Booking>();
			CreateMap<Entities.Booking, BookingProfile>();
			CreateMap<BookingProfile, Entities.Booking>();
			
			CreateMap<Entities.ServiceBooking, CreateServiceBookingRequest>();
			CreateMap<CreateServiceBookingRequest, Entities.ServiceBooking>();
			CreateMap<Entities.ServiceBooking, ServiceBookingProfile>();
			CreateMap<ServiceBookingProfile, Entities.ServiceBooking>();

			CreateMap<Entities.ResetPassword, ResetPasswordProfile>();
			CreateMap<ResetPasswordProfile,  Entities.ResetPassword>();
		}
    }
}
