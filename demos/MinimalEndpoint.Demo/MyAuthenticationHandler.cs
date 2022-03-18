using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class MyAuthenticationHandler : AuthenticationHandler<JwtBearerOptions>
{
    public MyAuthenticationHandler(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
    : base(options, logger, encoder, clock)
    {
    }

    protected override object? Events { get => base.Events; set => base.Events = value; }

    protected override string ClaimsIssuer => base.ClaimsIssuer;

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }

    protected override Task<object> CreateEventsAsync()
    {
        return base.CreateEventsAsync();
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {        
        
        var token =( Context.Request.Headers.
        TryGetValue("Authorization", out var _value)? _value.ToString(): string.Empty)
        ?.Split(' ').Last();
        
        if (string.IsNullOrEmpty(token)) return Task.FromResult( AuthenticateResult.Fail("Unauthorized"));
 
        var tokenHandler = new JwtSecurityTokenHandler();                
                
        var principal= tokenHandler.ValidateToken(token, Options.TokenValidationParameters, out SecurityToken validatedToken);        
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));

    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        return base.HandleChallengeAsync(properties);
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        return base.HandleForbiddenAsync(properties);
    }

    protected override Task InitializeEventsAsync()
    {
        return base.InitializeEventsAsync();
    }

    protected override Task InitializeHandlerAsync()
    {
        return base.InitializeHandlerAsync();
    }

    protected override string? ResolveTarget(string? scheme)
    {
        return base.ResolveTarget(scheme);
    }
}
