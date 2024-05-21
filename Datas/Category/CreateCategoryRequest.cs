namespace BlossomServer.Datas.Category
{
	public class CreateCategoryRequest
	{
		public string Name { get; set; }
		public bool? IsActive { get; set; } = true;
		public int? Priority { get; set; } = 0;
		public DateTime? CreatedAt { get; set; } = DateTime.Now;
	}
}
