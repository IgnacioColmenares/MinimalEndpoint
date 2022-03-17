
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context is null) return;

        Console.WriteLine("............... hi there from JwtMiddleware..................................................");

        if( !context.User?.Identity?.IsAuthenticated??false)
        {
            await TryAuthenticateFromToken(context);
        }

        await _next(context);
    }

    private async Task TryAuthenticateFromToken(HttpContext context)
    {
        var token =( context.Request.Headers.
        TryGetValue("Authorization", out var _value)? _value.ToString(): string.Empty)
        ?.Split(' ').Last();
        
        if (string.IsNullOrEmpty(token)) return;

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt =  tokenHandler.ReadJwtToken(token); //tokenHandler.ReadToken()
        var ci = new List<ClaimsIdentity>();                
        ci.Add( new ClaimsIdentity ( jwt.Claims)); 

         var claims = new ClaimsIdentity(jwt.Claims, JwtBearerDefaults.AuthenticationScheme);
         var principal = new ClaimsPrincipal(claims);
         context.User = principal;        
         
        
        //var key = Encoding.ASCII.GetBytes(_jwtMiddlewareConfig.StringForIssuerSigningKey);  // TODO : ??MOVE GETTING TOKEN INTO IUserService so no need to know "THE SECRET"
        /*tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            
            ValidateIssuerSigningKey = false,
            //IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        Console.WriteLine(validatedToken);

        var vt = (JwtSecurityToken)validatedToken;*/


        /*//var base64Payload= token.Split(".").Skip(1).Take(1).FirstOrDefault();
        //if (string.IsNullOrEmpty(base64Payload)) return;

        //var bytes = System.Convert.FromBase64String(base64Payload);
        //var jsonPayload = System.Text.Encoding.UTF8.GetString (bytes);
        //var claimsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,string>>(jsonPayload);
        //*/

        await Task.CompletedTask;
        
    }
}
