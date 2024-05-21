using AutoMapper;
using BlossomServer.Datas.Service;
using BlossomServer.Entities;
using BlossomServer.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using System.Reflection;

namespace BlossomServer.Services.ProductServices
{
	public class ProductService : IProductService
	{
		private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;

		public ProductService(BlossomNailsContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<ApiResponse<ServiceProfile>> CreateService(CreateServiceRequest service)
		{
			try
			{
				await Task.CompletedTask;
				var serviceEntity = _mapper.Map<Service>(service);
				await _context.Services.AddAsync(serviceEntity);
				await _context.SaveChangesAsync();


				return new ApiResponse<ServiceProfile>
				{
					Success = true,
					Message = $"Create service {serviceEntity.Name} successfully.",
					Data = _mapper.Map<ServiceProfile>(serviceEntity),
					Status = (int)HttpStatusCode.Created
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<ServiceProfile>
				{
					Success = false,
					Message = "ProductService - CreateService: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<ServiceProfile>> DeleteService(int serviceID)
		{
			try
			{
				await Task.CompletedTask;
				var serviceEntity = await GetServiceByID(serviceID);
				if(serviceEntity.Data != null)
				{
					serviceEntity.Data.DeletedAt = DateTime.Now;
					_context.Services.Update(_mapper.Map<Service>(serviceEntity.Data));
				}

				await _context.SaveChangesAsync();
				return new ApiResponse<ServiceProfile>
				{
					Success = true,
					Message = $"Deleted service with id ${serviceID}.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<ServiceProfile>
				{
					Success = false,
					Message = "ProductService - DeleteProduct: " + ex,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<ServiceProfile>> GetServiceByID(int? serviceID)
		{
			try
			{
				await Task.CompletedTask;
				var service = await _context.Services.Where(service => service.DeletedAt == null).FirstOrDefaultAsync(service => service.ServiceID == serviceID);
				if (service == null)
				{
					return new ApiResponse<ServiceProfile>
					{
						Success = false,
						Message = "Service not found.",
						Status = (int)HttpStatusCode.OK
					};
				}

				return new ApiResponse<ServiceProfile>
				{
					Success = true,
					Message = $"Found service {service.Name}",
					Data = _mapper.Map<ServiceProfile>(service),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<ServiceProfile>
				{
					Success = false,
					Message = "ProductService - GetServiceByID: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<ServiceProfile>>> GetServices(FilterService? filter)
		{
			try
			{
				await Task.CompletedTask;
				var services = await _context.Services.Where(service => service.DeletedAt == null).ToListAsync();
				if (!services.Any())
				{
					return new ApiResponse<List<ServiceProfile>>
					{
						Success = false,
						Message = "There is not any services.",
						Status = (int)HttpStatusCode.OK
					};
				}

				var servicesProfile = services.Select(x => _mapper.Map<ServiceProfile>(x)).ToList();
				if(filter != null) servicesProfile = FilterService(servicesProfile, filter);

				return new ApiResponse<List<ServiceProfile>>
				{
					Success = true,
					Message = $"Get services list successfully. Found {servicesProfile.Count()} services!",
					Data = servicesProfile,
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<ServiceProfile>>
				{
					Success = false,
					Message = "ProductService - GetServices: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<ServiceProfile>> UpdateService(int serviceID, UpdateServiceRequest service)
		{
			try
			{
				await Task.CompletedTask;

				if (serviceID != service.ServiceID)
				{
					return new ApiResponse<ServiceProfile>
					{
						Success = false,
						Message = "Service with this ID not match.",
						Status = (int)HttpStatusCode.OK
					};
				};

				var serviceEntity = await _context.Services.FindAsync(serviceID);

				if (serviceEntity == null)
				{
					return new ApiResponse<ServiceProfile>
					{
						Success = false,
						Message = "Couldn't update because this service does not exists.",
						Status = (int)HttpStatusCode.OK
					};
				};

				_mapper.Map(service, serviceEntity);
				_context.Services.Update(serviceEntity);
				await _context.SaveChangesAsync();

				return new ApiResponse<ServiceProfile>
				{
					Success = true,
					Message = $"Updated service with ID {serviceID}.",
					Data = _mapper.Map<ServiceProfile>(serviceEntity),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<ServiceProfile>
				{
					Success = false,
					Message = "ProductService - UpdateService: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		private List<ServiceProfile> FilterService(List<ServiceProfile> services, FilterService filter)
		{
			if(filter.WorkingTimeFrom is not null && filter.WorkingTimeTo is not null)
				services = services.Where(x => Convert.ToInt32(x.WorkingTime) >= Convert.ToInt32(filter.WorkingTimeFrom) && Convert.ToInt32(x.WorkingTime) <= Convert.ToInt32(filter.WorkingTimeTo)).ToList();
			if(filter.PriceMin is not null && filter.PriceMax is not null)
				services = services.Where(x => x.Price >= filter.PriceMin && x.Price <= filter.PriceMax).ToList();
			if(filter.CategoryID is not null)
				services = services.Where(x => x.CategoryID == filter.CategoryID).ToList();
			if(filter.SortType is not null)
			{
				PropertyInfo propertyInfo = typeof(ServiceProfile).GetProperty(char.ToUpper(filter.SortType[0]) + filter.SortType.Substring(1))!;
				switch(filter.SortFrom)
				{
					case "ascending":
						services = services.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
						break;
					default:
						services = services.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
						break;
				}
			}
			if(filter.SearchText is not null)
				services = services.Where(x => x.Name.ToLower().Contains(filter.SearchText.ToLower())).ToList();
			return services;
		}
	}
}
