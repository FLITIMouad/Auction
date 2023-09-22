using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(op =>
{
    op.Authority = builder.Configuration["IdentityServiceUrl"];
    op.RequireHttpsMetadata = false;
    op.TokenValidationParameters.ValidateAudience = false;
    op.TokenValidationParameters.NameClaimType = "username";

});

var app = builder.Build();

app.MapReverseProxy();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
