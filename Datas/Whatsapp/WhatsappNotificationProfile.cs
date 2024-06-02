namespace BlossomServer.Datas.Whatsapp
{
    public class WhatsappNotificationProfile
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }
        public string? Uri { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
