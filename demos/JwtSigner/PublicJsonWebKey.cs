namespace JwtSigner;

public class PublicJsonWebKey
{
    public string kty { get; set; } = string.Empty;

    public string alg { get; set; } = "RSA256";
    public string e { get; set; } = "AQAB";
    public string kid { get; set; } = string.Empty;
    public string n { get; set; } = string.Empty;
    public string use { get; set; } = "sig";
}
