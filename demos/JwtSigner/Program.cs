using System.Security.Claims;
using JwtSigner;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

Console.WriteLine("Hello, World!");

var store = new SecurityKeyFileStore();
var securityKey =  await store.GetCurrent() ?? await store.Create();

Console.WriteLine(securityKey.KeyId);
var claims = new List<Claim>(new Claim[] { new Claim ("name", "ignacio")});
var signingCredentials = new SigningCredentials( securityKey, securityKey.Alg);

 var handler = new JsonWebTokenHandler();
       var now = DateTime.Now;
       var descriptor = new SecurityTokenDescriptor
       {
           Issuer = "https://localhost:6051/auth",
           Audience = "AsymetricKey",
           IssuedAt = now,
           NotBefore = now,
           Expires = now.AddMinutes(5),
           Subject = new ClaimsIdentity(claims),
           SigningCredentials = signingCredentials,
       };

       var token =  handler.CreateToken(descriptor);
Console.WriteLine(token);

var handler_1 = new JsonWebTokenHandler();

    var result = handler.ValidateToken(token,
        new TokenValidationParameters
        {
            ValidIssuer = "https://localhost:6051/auth",
            ValidAudience = "AsymetricKey",
            RequireSignedTokens = true,
            IssuerSigningKey = securityKey,
        });

      var name= result?.Claims.FirstOrDefault(f=> f.Key=="name").Value.ToString();

      Console.WriteLine(name);



//https://cognito-idp.us-east-1.amazonaws.com/us-east-1_gMChMjwSW/.well-known/openid-configuration

/*
{"authorization_endpoint":"https://test-aosp-members-area.auth.us-east-1.amazoncognito.com/oauth2/authorize",
"id_token_signing_alg_values_supported":["RS256"],
"issuer":"https://cognito-idp.us-east-1.amazonaws.com/us-east-1_gMChMjwSW",
"jwks_uri":"https://cognito-idp.us-east-1.amazonaws.com/us-east-1_gMChMjwSW/.well-known/jwks.json",
"response_types_supported":["code","token"],
"scopes_supported":["openid","email","phone","profile"],
"subject_types_supported":["public"],
"token_endpoint":"https://test-aosp-members-area.auth.us-east-1.amazoncognito.com/oauth2/token",
"token_endpoint_auth_methods_supported":["client_secret_basic","client_secret_post"],
"userinfo_endpoint":"https://test-aosp-members-area.auth.us-east-1.amazoncognito.com/oauth2/userInfo"}
*/
