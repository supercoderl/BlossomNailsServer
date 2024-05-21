using AutoMapper;
using BlossomServer.Response;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.Net;
using BlossomServer.Entities;
using BlossomServer.Datas.ServiceImage;
using Microsoft.EntityFrameworkCore;

namespace BlossomServer.Services.ProductImageServices
{
	public class ProductImageService : IProductImageService
	{
		private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _web;
		private Cloudinary _cloudinary = new Cloudinary(new Account("do02vtlo0", "249919974662472", "_bvgpqXmViNhRrmocCnhhougqJc"));

		public ProductImageService(BlossomNailsContext context, IMapper mapper, IWebHostEnvironment web)
		{
			_context = context;
			_mapper = mapper;
			_web = web;
		}

		public async Task<ApiResponse<ServiceImageProfile>> DeleteImage(int ImageID)
		{
			try
			{
				await Task.CompletedTask;
				var serviceImage = await _context.ServiceImages.FindAsync(ImageID);
				if (serviceImage == null)
				{
					return new ApiResponse<ServiceImageProfile>
					{
						Success = true,
						Message = "This image doesn't exists.",
						Status = (int)HttpStatusCode.OK
					};
				}
				if (!(serviceImage.ImageURL.Contains("https://") || serviceImage.ImageURL.Contains("http://"))) System.IO.File.Delete(Path.Combine(_web.WebRootPath, serviceImage.ImageURL));
				_context.ServiceImages.Remove(serviceImage);
				await _context.SaveChangesAsync();
				return new ApiResponse<ServiceImageProfile>
				{
					Success = true,
					Message = "Đã xóa hình ảnh",
					Data = _mapper.Map<ServiceImageProfile>(serviceImage),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<ServiceImageProfile>
				{
					Success = false,
					Message = "ServiceImageService - DeleteImage: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<ServiceImageProfile>>> GetAllImages()
		{
			try
			{
				await Task.CompletedTask;
				var images = await _context.ServiceImages.ToListAsync();
				if(!images.Any())
				{
					return new ApiResponse<List<ServiceImageProfile>>
					{
						Success = false,
						Message = "Gallery is empty!",
						Status = (int)HttpStatusCode.OK
					};
				}
				return new ApiResponse<List<ServiceImageProfile>>
				{
					Success = true,
					Message = $"Get images successfully. Found {images.Count()} images!",
					Data = images.Select(i => _mapper.Map<ServiceImageProfile>(i)).ToList(),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<ServiceImageProfile>>
				{
					Success = false,
					Message = "ProductImageService - Get All Images: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<ServiceImageProfile>>> GetImagesByService(int serviceID)
		{
			try
			{
				await Task.CompletedTask;
				var images = await _context.ServiceImages.Where(x => x.ServiceID == serviceID).ToListAsync();
				if (!images.Any())
				{
					return new ApiResponse<List<ServiceImageProfile>>
					{
						Success = false,
						Message = "This service has not any images.",
						Status = (int)HttpStatusCode.OK
					};
				}

				return new ApiResponse<List<ServiceImageProfile>>
				{
					Success = true,
					Message = $"Get images successfully. Found {images.Count()} images!",
					Data = images.Select(x => _mapper.Map<ServiceImageProfile>(x)).ToList(),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<ServiceImageProfile>>
				{
					Success = false,
					Message = "ProductImageService - Get Images By Service: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<ServiceImageProfile>> UploadImage(IFormFile file, CreateServiceImageRequest request)
		{
			try
			{
				await Task.CompletedTask;
				request.ImageURL = await UploadFile(file, request.ImageName);
				var serviceImageEntity = _mapper.Map<ServiceImage>(request);
				await _context.ServiceImages.AddAsync(serviceImageEntity);
				await _context.SaveChangesAsync();
				return new ApiResponse<ServiceImageProfile>
				{
					Success = true,
					Data = _mapper.Map<ServiceImageProfile>(serviceImageEntity),
					Message = $"Uploaded image {serviceImageEntity.ImageName}.",
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<ServiceImageProfile>
				{
					Success = false,
					Message = "ProductImageService - Upload Image: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		private async Task<string> UploadFile(IFormFile file, string fileName)
		{
			try
			{
				using (var stream = new MemoryStream())
				{
					await file.CopyToAsync(stream);
					stream.Seek(0, SeekOrigin.Begin);
					var uploadParams = new ImageUploadParams()
					{
						File = new FileDescription(file.FileName, stream)
					};
					var uploadResult = await _cloudinary.UploadAsync(uploadParams);
					return uploadResult.Url.ToString();
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}
}
