namespace BlossomServer.Datas.Service
{
	public class ServiceProfile
	{
		public int ServiceID { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public int? CategoryID { get; set; }
		public double Price { get; set; }
		public string WorkingTime { get; set; }
		public string? RepresentativeImage { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }
	}
}
