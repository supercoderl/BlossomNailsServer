namespace BlossomServer.Datas.Category
{
	public class CategoryProfile
	{
		public int CategoryID { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; } = true;
		public int Priority { get; set; } = 0;
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }
	}
}
