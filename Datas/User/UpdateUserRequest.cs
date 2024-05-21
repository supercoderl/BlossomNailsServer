namespace BlossomServer.Datas.User
{
	public class UpdateUserRequest
	{
		public Guid UserID { get; set; }
		public string Username { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public int? Phone { get; set; }

		public bool? IsActive { get; set; } = true;
		public DateTime? UpdatedAt { get; set; } = DateTime.Now;
	}
}
