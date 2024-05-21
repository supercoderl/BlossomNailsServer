using BlossomServer.Datas.Service;

namespace BlossomServer.Datas.ServiceBooking
{
	public class ServiceBookingProfile
	{
		public int ServiceBookingID { get; set; }
		public int BookingID { get; set; }
		public int ServiceID { get; set; }
		public double ServiceCost { get; set; }
		public ServiceProfile? Service {  get; set; }
	}
}
