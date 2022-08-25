using Microsoft.IdentityModel.Tokens;

const string demoSpaProjectOriginPolicyName = "DemoSpaProject";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
	.AddAuthentication("Bearer")
	.AddJwtBearer("Bearer", options =>
	{
		options.Authority = "https://localhost:5001";

		options.TokenValidationParameters = new TokenValidationParameters
		{
#pragma warning disable CA5404 // Do not disable token validation checks
			// TODO: Enable audience validation.
			ValidateAudience = false,
#pragma warning restore CA5404 // Do not disable token validation checks
		};
	});

builder.Services.AddCors(options =>
{
	options.AddPolicy(
		demoSpaProjectOriginPolicyName,
		policy =>
		{
			policy.WithOrigins("https://localhost:7001", "https://localhost:44456");
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
