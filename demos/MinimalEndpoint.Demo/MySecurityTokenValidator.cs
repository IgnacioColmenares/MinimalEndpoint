using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

internal class MySecurityTokenValidator : ISecurityTokenValidator
{
    public bool CanValidateToken => true;

    public int MaximumTokenSizeInBytes { get ;set; }

    public bool CanReadToken(string securityToken)
    {
        return true;
    }

    public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt =  tokenHandler.ReadJwtToken(securityToken);  
        // TODO use var principal= tokenHandler.ValidateToken(token, Options.TokenValidationParameters, out SecurityToken validatedToken);        
        var ci = new List<ClaimsIdentity>();                
        ci.Add( new ClaimsIdentity ( jwt.Claims)); 

         var claimsIdentity = new ClaimsIdentity(jwt.Claims, JwtBearerDefaults.AuthenticationScheme);
         var principal = new ClaimsPrincipal(claimsIdentity);
         validatedToken= jwt;
         return principal;
    }
}