using BlossomServer.Datas.Contact;
using BlossomServer.Services.ContactServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContactController : ControllerBase
	{
		private readonly IContactService _contactService;

		public ContactController(IContactService contactService)
		{
			_contactService = contactService;
		}

		[HttpGet("contacts")]
		public async Task<IActionResult> GetContacts(string? text)
		{
			var result = await _contactService.GetContacts(text);
			return StatusCode(result.Status, result);
		}

		[HttpPost("create-contact")]
		public async Task<IActionResult> CreateContact(CreateContactRequest request)
		{
			var result = await _contactService.CreateContact(request);
			return StatusCode(result.Status, result);
		}
	}
}
