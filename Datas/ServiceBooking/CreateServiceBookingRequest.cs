namespace BlossomServer.Datas.ServiceBooking
{
	public class CreateServiceBookingRequest
	{
		public int BookingID { get; set; }
		public int ServiceID { get; set; }
		public double ServiceCost { get; set; }
	}

	public class CreateServiceBookingListRequest
	{
		public List<CreateServiceBookingRequest> Requests { get; set; }
	}
}
