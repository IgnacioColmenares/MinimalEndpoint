using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
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

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
 {
    o.TokenValidationParameters = new TokenValidationParameters
    {
        
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,        
        IssuerValidator= (string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters)
        => 
        {
            Console.WriteLine(issuer);
            return issuer;
        },
        //AudienceValidator = null,
    };
    
    builder.Configuration.GetSection("JwtBearerOptions").Bind(o);
    o.Events = new JwtBearerEvents{
         OnMessageReceived= (ctx)=>
         {
             Console.WriteLine(ctx);
             return Task.CompletedTask;
         },
         OnTokenValidated= (ctx)=>
         {
             Console.WriteLine(ctx);
             return Task.CompletedTask;
         },
         OnChallenge = (ctx)=>
         {
             Console.WriteLine(ctx);
             return Task.CompletedTask;
         },
         };
});



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
