using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = "Cookies";
		options.DefaultChallengeScheme = "oidc";
	})
	.AddCookie("Cookies")
	.AddOpenIdConnect("oidc", options =>
	{
		options.Authority = "https://localhost:5001";

		options.ClientId = "web";
		options.ClientSecret = "secret";
		options.ResponseType = "code";

		options.Scope.Clear();
		options.Scope.Add("openid");
		options.Scope.Add("profile");
		options.GetClaimsFromUserInfoEndpoint = true;

		options.SaveTokens = true;
	});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages()
	.RequireAuthorization();

app.Run();
