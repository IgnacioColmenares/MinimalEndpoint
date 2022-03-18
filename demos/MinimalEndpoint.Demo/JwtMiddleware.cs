using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        var jwt =  tokenHandler.ReadJwtToken(token);  
        // TODO use var principal= tokenHandler.ValidateToken(token, Options.TokenValidationParameters, out SecurityToken validatedToken);        
        var ci = new List<ClaimsIdentity>();                
        ci.Add( new ClaimsIdentity ( jwt.Claims)); 

         var claimsIdentity = new ClaimsIdentity(jwt.Claims, JwtBearerDefaults.AuthenticationScheme);
         var principal = new ClaimsPrincipal(claimsIdentity);
         context.User = principal;                

        await Task.CompletedTask;
        
    }
}
