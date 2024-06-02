namespace BlossomServer.Datas.Booking
{
	public class UpdateBookingRequest
	{
		public int BookingID { get; set; }
		public string? CustomerName { get; set; }
		public string? CustomerPhone { get; set; }
		public string? CustomerEmail { get; set; }
		public Guid? NailTechnicianID { get; set; }
		public string? BookingDate { get; set; }
		public string? StartTime { get; set; }
		public string? EndTime { get; set; }
		public string Status { get; set; }
		public double TotalCost { get; set; }
		public string? Notes { get; set; }
		public DateTime? UpdatedAt { get; set; } = DateTime.Now;
	}
}
