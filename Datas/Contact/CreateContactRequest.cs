namespace BlossomServer.Datas.Contact
{
	public class CreateContactRequest
	{
		public string Fullname { get; set; }
		public int Phone { get; set; }
		public string Email { get; set; }
		public string Notes { get; set; }

		public string Status { get; set; }

		public DateTime? CreatedAt { get; set; } = DateTime.Now;
	}
}
