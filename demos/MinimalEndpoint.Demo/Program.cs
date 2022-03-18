using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinimalApis.Extensions.Binding;
using MinimalEndpoint.Demo.Endpoints.Orders.GetOrderById;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthorization(o =>
{
   o.AddPolicy("AdminsOnly", b => b.RequireClaim(ClaimTypes.Role, "admin"));
});

builder.Services.AddAuthentication("DefaultAuth")
 .AddScheme<JwtBearerOptions, MyAuthenticationHandler>
(
    "DefaultAuth", 
    o =>
    {
        var keys = new   JsonWebKeySet();
        builder.Configuration.GetSection("JwtBearerOptions:TokenValidationParameters:IssuerSigningKeys:keys").Bind(keys.Keys);

        o.TokenValidationParameters = new TokenValidationParameters
        {                        
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,  
            IssuerSigningKeys = keys.Keys,      
            IssuerSigningKeyValidator= (SecurityKey securityKey, SecurityToken securityToken, TokenValidationParameters validationParameters)
            =>
            {
                return true;
            },
            IssuerValidator= (string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters)
            => 
            {
                return issuer;
            },

        };        
        builder.Configuration.GetSection("JwtBearerOptions").Bind(o);
        
    }
 );


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
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();

app.MapGet("test", (ModelBinder<GetOrderByIdRequest> request) => request.Model);

app.MapEndpointsFromCurrentAssembly();

app.Run();
