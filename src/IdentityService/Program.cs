using Duende.IdentityServer;
using IdentityService;
using IdentityService.Internal;
using IdentityService.Pages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services
	.AddIdentityServer()
	.AddInMemoryIdentityResources(Config.IdentityResources)
	.AddInMemoryApiScopes(Config.ApiScopes)
	.AddInMemoryClients(Config.Clients)
	.AddTestUsers(TestUsers.Users)
	.AddProfileService<ProfileService>();

builder.Services
	.AddAuthentication()
	.AddGoogle("Google", options =>
	{
		options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

		options.ClientId = builder.Configuration["authentication:google:clientId"];
		options.ClientSecret = builder.Configuration["authentication:google:clientSecret"];
	});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
