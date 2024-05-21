using AutoMapper;
using BlossomServer.Datas.Contact;
using BlossomServer.Entities;
using BlossomServer.Response;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlossomServer.Services.ContactServices
{
	public class ContactService : IContactService
	{
		private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;

		public ContactService(BlossomNailsContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<ApiResponse<ContactProfile>> CreateContact(CreateContactRequest request)
		{
			try
			{
				await Task.CompletedTask;
				var contactEntity = _mapper.Map<Contact>(request);
				await _context.Contacts.AddAsync(contactEntity);
				await _context.SaveChangesAsync();
				return new ApiResponse<ContactProfile>
				{
					Success = true,
					Message = "Message sent!",
					Data = _mapper.Map<ContactProfile>(contactEntity),
					Status = (int)HttpStatusCode.Created
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<ContactProfile>
				{
					Success = false,
					Message = "ContactService - CreateContact: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<List<ContactProfile>>> GetContacts(string? text)
		{
			try
			{
				await Task.CompletedTask;
				var contacts = await _context.Contacts.ToListAsync();
				if (contacts.Any() && text != null)
				{
					contacts = contacts.Where(x => x.Fullname.ToLower().Contains(text.ToLower())).ToList();
				}
				return new ApiResponse<List<ContactProfile>>
				{
					Success = true,
					Message = $"Get messages successfully. Found {contacts.Count()} contacts!",
					Data = contacts.Select(x => _mapper.Map<ContactProfile>(x)).ToList(),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<ContactProfile>>
				{
					Success = false,
					Message = "ContactService - GetContacts: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}
	}
}
