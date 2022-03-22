using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace JwtSigner;

public class SecurityKeyFileStore
{
    private readonly DirectoryInfo _filesPath;

    public SecurityKeyFileStore(DirectoryInfo? filesPath= null)
    {
        _filesPath = filesPath 
        ??
         new DirectoryInfo(
             Path.Combine(
                 Environment.GetFolderPath(
                     Environment.SpecialFolder.LocalApplicationData), nameof(SecurityKeyFileStore)));
        _filesPath.Create();
    }

    public async Task<RsaSecurityKey> Create( int keySize= 3072)
    {
        var securityKey = CryptoService.CreateRsaSecurityKey(keySize);
        await  Save(securityKey);
        return securityKey;
    }

    public async Task Save( RsaSecurityKey securityKey)
    {
        var bytes = securityKey.Rsa.ExportRSAPrivateKey();
        var base64 =
@"-----BEGIN RSA PRIVATE KEY-----
" + Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks) + @"
-----END RSA PRIVATE KEY-----";
        
        var fullFilename =  GetFullFileName(securityKey.KeyId);
        await System.IO.File.WriteAllTextAsync(fullFilename, base64);        
    }

    public async Task<RsaSecurityKey> Get( string keyId)
    {                
        var fullFileName = GetFullFileName(keyId);        
        return await Get(keyId, fullFileName);
    }

    public async Task<RsaSecurityKey?> GetCurrent()
    {
        var fileInfo = _filesPath.GetFiles().OrderBy(f=>f.CreationTime).FirstOrDefault();
        if( fileInfo==default) return default;
        return await  Get(fileInfo.Name,fileInfo.FullName);
    }

    public async Task<bool> HasCurrent()=> await Task.FromResult( _filesPath.GetFiles().Any());


    private string GetFullFileName(string keyId)=>  Path.Combine(_filesPath.FullName, keyId);

    private async Task<RsaSecurityKey> Get(string keyId, string fullFileName)
    {
        var securityKey = new RsaSecurityKey(RSA.Create())
        {
            KeyId = keyId
        };
        var base64 = await System.IO.File.ReadAllTextAsync(fullFileName);
        securityKey.Rsa.ImportFromPem(base64);        
        return securityKey;
    }


}
