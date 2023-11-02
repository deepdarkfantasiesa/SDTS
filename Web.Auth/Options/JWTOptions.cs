namespace Web.Auth.Options
{
    public class JWTOptions
    {
        public string SigningKey { get; set; }
        public int ExpireSeconds {  get; set; }
    }
}
