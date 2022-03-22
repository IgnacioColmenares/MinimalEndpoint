using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace JwtSigner;

public static class CryptoService
{
    public static RsaSecurityKey CreateRsaSecurityKey(int keySize = 3072)
    {
        var key = new RsaSecurityKey(RSA.Create(keySize))
        {
            KeyId = Guid.NewGuid().ToString()
        };

        return key;
    }

}
