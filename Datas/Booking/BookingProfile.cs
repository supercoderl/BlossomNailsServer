using BlossomServer.Datas.User;

namespace BlossomServer.Datas.Booking
{
	public class BookingProfile
	{
		public int BookingID { get; set; }
		public Guid? CustomerID { get; set; }
		public string? CustomerName { get; set; }
		public string? CustomerPhone { get; set; }
		public Guid? NailTechnicianID { get; set; }
		public DateTime? BookingDate { get; set; }
		public string? StartTime { get; set; }
		public string? EndTime { get; set; }
		public string Status { get; set; }
		public double TotalCost { get; set; }
		public string? Notes { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }
	}
}
