using JwtSigner;
using Microsoft.IdentityModel.Tokens;

Console.WriteLine("Hello, World!");

var store = new SecurityKeyFileStore();
var securityKey =  await store.GetCurrent() ?? await store.Create();

Console.WriteLine(securityKey.KeyId);



/*
var key = new RsaSecurityKey(RSA.Create())
{
    KeyId = Guid.NewGuid().ToString()
};

var base64 = await System.IO.File.ReadAllTextAsync(@"C:\Users\Ignacio Colmenares\.ssh\endpoint-experiment");

key.Rsa.ImportFromPem(base64);

Console.WriteLine(key.KeyId);

var bytes = key.Rsa.ExportRSAPrivateKey();

var newBase64 =
@"-----BEGIN RSA PRIVATE KEY-----
" + Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks) + @"
-----END RSA PRIVATE KEY-----";

await System.IO.File.WriteAllTextAsync(key.KeyId, newBase64);

base64 = await System.IO.File.ReadAllTextAsync(key.KeyId);

key.Rsa.ImportFromPem(base64);

Console.WriteLine(key.KeyId);

*/



// 3072
