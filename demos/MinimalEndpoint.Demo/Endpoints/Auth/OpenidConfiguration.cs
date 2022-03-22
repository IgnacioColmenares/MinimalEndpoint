namespace MinimalEndpoint.Demo.Endpoints.Auth;

public class OpenidConfiguration : EndpointGet
{
    public OpenidConfiguration()
    {
        AllowAnonymous();
    }

    protected override string Pattern => "auth/.well-known/openid-configuration";
    protected override Delegate Handler => Handle;

    private IResult Handle(CancellationToken cancellationToken = default)
    {
        return Results.Ok(new 
        {
            id_token_signing_alg_values_supported = new string[]{ "RS256"},
            issuer= "https://localhost:6051/auth",
            jwks_uri= "https://localhost:6051/auth/.well-known/jwks.json"
        });
    }
}
