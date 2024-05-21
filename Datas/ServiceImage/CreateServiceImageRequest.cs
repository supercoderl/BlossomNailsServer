namespace BlossomServer.Datas.ServiceImage
{
	public class CreateServiceImageRequest
	{
		public string ImageName { get; set; }
		public string? ImageURL { get; set; }
		public int? ServiceID { get; set; }
		public string? Description { get; set; }
		public DateTime? CreatedAt { get; set; } = DateTime.Now;
	}
}
