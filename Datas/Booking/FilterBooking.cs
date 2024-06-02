namespace BlossomServer.Datas.Booking
{
    public class FilterBooking
    {
        public string? BookingDateFrom { get; set; }
        public string? BookingDateTo { get; set; }
        public double? PriceMin { get; set; }
        public double? PriceMax { get; set; }
        public string? SortType { get; set; }
        public string? SortFrom { get; set; }
        public string? SearchText { get; set; }
    }
}
