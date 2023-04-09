namespace BookApi.models
{
    public class JwtOption
    {
        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string secret_key { get; set; }
    }
}