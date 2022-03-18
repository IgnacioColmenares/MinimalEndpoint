using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MinimalApis.Extensions.Binding;
using MinimalEndpoint.Demo.Endpoints.Orders.GetOrderById;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminsOnly", b => b.RequireClaim(ClaimTypes.Role, "admin"));
});


builder.Services.AddAuthentication(o=>{
    o.DefaultAuthenticateScheme="JWT_OR_MYAUTH";
    o.DefaultChallengeScheme="JWT_OR_MYAUTH";
})
.AddJwtBearer(o =>
{
    var keys = new JsonWebKeySet();
    builder.Configuration.GetSection("JwtBearerOptions:TokenValidationParameters:IssuerSigningKeys:keys").Bind(keys.Keys);
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKeys = keys.Keys,
        IssuerSigningKeyValidator = (SecurityKey securityKey, SecurityToken securityToken, TokenValidationParameters validationParameters)
         =>
         {
             return true;
         },
        IssuerValidator = (string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters)
         =>
         {
             return issuer;
         },

    };
    builder.Configuration.GetSection("JwtBearerOptions").Bind(o);

})
.AddScheme<JwtBearerOptions, MyAuthenticationHandler>(
    "MyAuth",
    o =>
    {   
        o.Audience="";
        o.Authority="";
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = false,
            ValidateLifetime = false,
            ValidateActor=false,
            IssuerSigningKeyValidator = (SecurityKey securityKey, SecurityToken securityToken, TokenValidationParameters validationParameters)
            =>
            {
                return true;
            },
            IssuerValidator = (string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters)
            =>
            {
                return issuer;
            },
        };        
    }
 )
 .AddPolicyScheme("JWT_OR_MYAUTH", "JWT_OR_MYAUTH", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string authorization = context.Request.Headers[HeaderNames.Authorization];
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
            var token = authorization.Substring("Bearer ".Length).Trim();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken= jwtHandler.ReadJwtToken(token);
            var canReadToken = jwtHandler.CanReadToken(token);

            return (canReadToken 
            && 
            (string.IsNullOrEmpty(jwtSecurityToken.Issuer) || jwtSecurityToken.Issuer.Equals("")))
                ? "MyAuth" : JwtBearerDefaults.AuthenticationScheme;
        }
        return JwtBearerDefaults.AuthenticationScheme;
    };
});
 ;


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOrdersServices();

builder.Services.AddMvcCore();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
//app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();

app.MapGet("test", (ModelBinder<GetOrderByIdRequest> request) => request.Model);

app.MapEndpointsFromCurrentAssembly();

app.Run();
