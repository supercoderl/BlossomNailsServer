using System.ComponentModel.DataAnnotations;

namespace BlossomServer.Datas.Authentication
{
	public class SendResetCodeRequest
	{
		[EmailAddress]
		public string Email { get; set; }
	}

	public class VerifyCodeRequest
	{
		[EmailAddress]
		public string Email { get; set; }

		public string Code { get; set; }
	}

	public class ResetPasswordProfile
	{
		public string Username { get; set; }
		public string Code { get; set; }
		public DateTime ExpMinutes {  get; set; }
		public bool? IsActive { get; set; } = true;
	}

	public class ResetPasswordRequest
	{
		public string Email { get; set; }
		public string NewPassword { get; set; }
	}
}
