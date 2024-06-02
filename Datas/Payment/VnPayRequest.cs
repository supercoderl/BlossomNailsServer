namespace BlossomServer.Datas.Payment
{
    public class VnPayRequest
    {
        public long Money { get; set; }
        public bool Bankcode_Vnpayqr { get; set; } = false;
        public bool Bankcode_Vnbank { get; set; } = false;
        public bool Bankcode_Intcard { get; set; } = false;
        public bool Locale_Vn {  get; set; } = false;
        public bool Locale_En {  get; set; } = false;
    }
}
