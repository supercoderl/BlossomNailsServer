using BlossomServer.Datas.Contact;
using BlossomServer.Response;

namespace BlossomServer.Services.ContactServices
{
	public interface IContactService
	{
		Task<ApiResponse<ContactProfile>> CreateContact(CreateContactRequest request);
		Task<ApiResponse<List<ContactProfile>>> GetContacts(string? text);
	}
}
