namespace BlossomServer.Datas.Notification
{
	public class UpdateNotificationRequest
	{
		public int NotificationID { get; set; }
		public DateTime? ReadAt { get; set; } = DateTime.Now;
	}
}
