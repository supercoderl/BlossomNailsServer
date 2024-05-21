using AutoMapper;
using BlossomServer.Datas.Category;
using BlossomServer.Datas.Service;
using BlossomServer.Entities;
using BlossomServer.Response;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;

namespace BlossomServer.Services.CategoryServices
{
	public class CategoryService : ICategoryService
	{
		private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;

		public CategoryService(BlossomNailsContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ApiResponse<CategoryProfile>> CreateCategory(CreateCategoryRequest request)
		{
			try
			{
				await Task.CompletedTask;
				var categoryEntity = _mapper.Map<Category>(request);
				await _context.Categories.AddAsync(categoryEntity);
				await _context.SaveChangesAsync();
				return new ApiResponse<CategoryProfile>
				{
					Success = true,
					Message = $"Create category {categoryEntity.Name} successfully!",
					Data = _mapper.Map<CategoryProfile>(categoryEntity),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<CategoryProfile>
				{
					Success = false,
					Message = "CategoryService - CreateCategory: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<CategoryProfile>>> GetCategories(FilterCategory? filter)
		{
			try
			{
				await Task.CompletedTask;
				var categories = await _context.Categories.ToListAsync();
				if (!categories.Any())
				{
					return new ApiResponse<List<CategoryProfile>>
					{
						Success = true,
						Status = (int)HttpStatusCode.OK,
						Message = "There is not any categories."
					};
				}

				var categoriesProfile = categories.Select(x => _mapper.Map<CategoryProfile>(x)).ToList();
				if (filter != null) categoriesProfile = FilterCategory(categoriesProfile, filter);

				return new ApiResponse<List<CategoryProfile>>
				{
					Success = true,
					Status = (int)HttpStatusCode.OK,
					Data = categoriesProfile,
					Message = $"Get categories successfully. Found {categoriesProfile.Count()} categories!"
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<CategoryProfile>>
				{
					Success = false,
					Status = (int)HttpStatusCode.InternalServerError,
					Message = "CategoryService - GetCategories: " + ex.Message
				};
			}
		}

		private List<CategoryProfile> FilterCategory(List<CategoryProfile> categories, FilterCategory filter)
		{
			if (filter.Status is not null)
				categories = categories.Where(x => filter.Status == "activated" ? x.IsActive : !x.IsActive).ToList();
			if (filter.SortType is not null)
			{
				PropertyInfo propertyInfo = typeof(ServiceProfile).GetProperty(char.ToUpper(filter.SortType[0]) + filter.SortType.Substring(1))!;
				switch (filter.SortFrom)
				{
					case "ascending":
						categories = categories.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
						break;
					default:
						categories = categories.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
						break;
				}
			}
			if (filter.SearchText is not null)
				categories = categories.Where(x => x.Name.ToLower().Contains(filter.SearchText.ToLower())).ToList();
			return categories;
		}
	}
}
