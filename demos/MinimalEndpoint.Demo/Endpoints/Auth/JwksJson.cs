namespace MinimalEndpoint.Demo.Endpoints.Auth;

public class JwksJson : EndpointGet
{
    public JwksJson()
    {
        AllowAnonymous();
    }

//https://localhost:6051/auth/.well-known/jwks.json

    protected override string Pattern => "auth/.well-known/jwks.json";

    protected override Delegate Handler => Handle;

    private IResult Handle(CancellationToken cancellationToken = default)
    {
        return Results.Ok(new 
        {
            keys= new dynamic[]{ new {
            kty= "RSA",
            alg= "RS256",
            e= "AQAB",
            kid= "3b2d4db7-b14c-4c6d-8ecf-42b7bd43e1b4",
            n= "tJQ9PWiwZoDma-zCOXmEogNsb7yH3E50ZP3LJDnoT4c12fyogw4c5g7tgDAq07Bt0L5rfZieKLeckR3v4lKjlTTtVJsJsT1J0S-mg7jPipENf8A0owyBlRzUC54DhBXhkKhumvGknJ9qiDZWOlii7BweRNWm9q6vmPlEmZGRi9WBb-Ob21OY6JM_A-YfNnTXTzyxRajC2ACV-i2sVVIur91H3ao12sGdae4rrcnA136K2Zfy3BXPRQmDIPEctccotHZLJ-wvI9VH6Uu0KUcgaW4gdxW62YybqNfFvbJwT_ICZPxsh7NcMisvFwxom-bCntDDWz6d1cziDiIEjjRjEQ",
            use= "sig"}}
        });
    }
}
