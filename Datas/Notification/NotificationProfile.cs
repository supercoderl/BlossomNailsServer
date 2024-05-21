namespace BlossomServer.Datas.Notification
{
	public class NotificationProfile
	{
		public int NotificationID { get; set; }
		public Guid? Receiver { get; set; }
		public string Type { get; set; }
		public int? ObjectID { get; set; }
		public string? Message { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ReadAt { get; set; }
	}
}
