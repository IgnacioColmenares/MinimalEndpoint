using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace JwtSigner;

public class SecurityKeyFileStore
{
    private readonly DirectoryInfo _filesPath;

    public SecurityKeyFileStore(DirectoryInfo? filesPath = null)
    {
        _filesPath = filesPath
        ??
         new DirectoryInfo(
             Path.Combine(
                 Environment.GetFolderPath(
                     Environment.SpecialFolder.LocalApplicationData), nameof(SecurityKeyFileStore)));
        _filesPath.Create();
    }

    public async Task<JsonWebKey> Create(int keySize = 2048)  //3072
    {
        var securityKey = CryptoService.CreateRsaSecurityKey(keySize);
        var jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey((RsaSecurityKey)securityKey);
        jsonWebKey.Alg="RS256";
        jsonWebKey.Use = "sig";        
        await Save(jsonWebKey);
        return jsonWebKey;
    }

    public async Task Save(JsonWebKey jsonWebKey)
    {

        var fullFilename = GetFullFileName(jsonWebKey.KeyId);
        await Save(jsonWebKey, fullFilename);        
        
        string publicKeyFileFullFileName = GetPublicKeyFileFullFileName(jsonWebKey.KeyId);
        await SavePublicKey(jsonWebKey, publicKeyFileFullFileName);       

    }

    private async Task SavePublicKey( JsonWebKey jsonWebKey, string publicKeyfullFilename)
    {
        var publickey = new PublicJsonWebKey
        {
            kid = jsonWebKey.KeyId,
            kty = jsonWebKey.Kty,
            n = jsonWebKey.N,
            use = jsonWebKey.Use,
            alg = jsonWebKey.Alg
        };

        var publicJwkeyAsString = JsonSerializer.Serialize(publickey, typeof(PublicJsonWebKey), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            WriteIndented = true,
        });

        await System.IO.File.WriteAllTextAsync(publicKeyfullFilename, publicJwkeyAsString);   
    }
    private async Task Save( JsonWebKey jsonWebKey, string fullFilename)
    {
        var jwkeyAsString = JsonSerializer.Serialize(jsonWebKey, typeof(JsonWebKey), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            WriteIndented = true,
        });

        await System.IO.File.WriteAllTextAsync(fullFilename, jwkeyAsString);
    }

    private string GetPublicKeyFileFullFileName(string keyId)
    =>
    Path.Combine(_filesPath.FullName, keyId) + "-public.json";

    public async Task<JsonWebKey?> Get(string keyId)
    {
        var fullFileName = GetFullFileName(keyId);
        return await Get(keyId, fullFileName);
    }

    public async Task<JsonWebKey?> GetCurrent()
    {
        var fileInfo = _filesPath.GetFiles().OrderBy(f => f.CreationTime).FirstOrDefault();
        if (fileInfo == default) return default;
        return await Get(fileInfo.Name, fileInfo.FullName);
    }

    public async Task<bool> HasCurrent() => await Task.FromResult(_filesPath.GetFiles().Any());


    private string GetFullFileName(string keyId) => Path.Combine(_filesPath.FullName, keyId) + "-private.json";

    private async Task<JsonWebKey?> Get(string keyId, string fullFileName)
    {

        var text = await System.IO.File.ReadAllTextAsync(fullFileName);
        var jwkey = JsonSerializer.Deserialize<JsonWebKey>(text);
        return await Task.FromResult(jwkey);

    }


}
