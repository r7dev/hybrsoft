using Hybrsoft.FoundationAPI.Configuration;
using Hybrsoft.FoundationAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register the AppSettings configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
// Register custom services
builder.Services.Configure();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true
	};
	options.Events = new JwtBearerEvents
	{
		OnAuthenticationFailed = context =>
		{
			Console.WriteLine("Token inválido: " + context.Exception.Message);
			return Task.CompletedTask;
		},
		OnTokenValidated = context =>
		{
			Console.WriteLine("Token validado com sucesso.");
			return Task.CompletedTask;
		}
	};
});
// Add Authorization
builder.Services.AddAuthorization();

builder.WebHost.UseKestrel((context, options) =>
{
	options.ListenAnyIP(443, listenOptions =>
	{
		listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
		listenOptions.UseHttps();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
