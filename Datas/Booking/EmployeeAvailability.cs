namespace BlossomServer.Datas.Booking
{
	public class EmployeeAvailabilityRequest
	{
		public string BookingDate { get; set; }
		public string StartTime { get; set; }
	}

	public class EmployeeAvailabilityResponse
	{
		public Guid EmployeeID { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public bool IsBusy { get; set; }
	}
}
