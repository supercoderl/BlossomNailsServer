namespace BlossomServer.Datas.Authentication
{
	public class JWTConfiguration
	{
		public string SecretKey { get; set; }
		public string Audience { get; set; }
		public string Issuer { get; set; }
		public int AccessTokenExpirationMinutes { get; set; }
		public int RefreshTokenExpirationDays { get; set; }
	}
}
