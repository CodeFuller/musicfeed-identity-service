using Microsoft.IdentityModel.Tokens;

const string demoSpaProjectOriginPolicyName = "DemoSpaProject";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
	.AddAuthentication("Bearer")
	.AddJwtBearer("Bearer", options =>
	{
		options.Authority = "https://localhost:5001/identity/";
		options.Audience = "musicfeed-api";

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidateActor = true,
			ValidateTokenReplay = true,

			ValidTypes = new[] { "at+jwt" },
		};
	});

builder.Services.AddCors(options =>
{
	options.AddPolicy(
		demoSpaProjectOriginPolicyName,
		policy =>
		{
			policy
				.WithOrigins("https://localhost:7001", "https://localhost:44456")
				.AllowAnyHeader();
		});
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("MusicFeedApiScope", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("scope", "musicfeed-api");
	});
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors(demoSpaProjectOriginPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
	.RequireAuthorization("MusicFeedApiScope");

app.Run();
