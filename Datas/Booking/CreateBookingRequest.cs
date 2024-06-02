namespace BlossomServer.Datas.Booking
{
	public class CreateBookingRequest
	{
		public Guid? CustomerID { get; set; }
		public string? CustomerName { get; set; }
		public string? CustomerPhone { get; set; }
		public string? CustomerEmail { get; set; }
		public Guid? NailTechnicianID { get; set; }
		public string? BookingDate { get; set; }
		public string? StartTime { get; set; }
		public string? EndTime { get; set; }
		public string? Status { get; set; } = "Created";
		public double? TotalCost { get; set; } = 0;
		public string? Notes { get; set; }
		public DateTime? CreatedAt { get; set; } = DateTime.Now;
	}
}
