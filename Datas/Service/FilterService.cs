namespace BlossomServer.Datas.Service
{
	public class FilterService
	{
		public string? WorkingTimeFrom {  get; set; }
		public string? WorkingTimeTo { get; set; }
		public double? PriceMin { get; set; }
		public double? PriceMax { get; set; }
		public int? CategoryID { get; set; }
		public string? SortType { get; set; }
		public string? SortFrom { get; set; }
		public string? SearchText { get; set; }
	}
}
