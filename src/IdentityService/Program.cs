using Duende.IdentityServer;
using IdentityService;
using IdentityService.Abstractions;
using IdentityService.Infrastructure.PostgreSql;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddPostgreSqlDal(builder.Configuration.GetConnectionString("identityDB"));

builder.Services
	.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<IdentityDbContext>()
	.AddDefaultTokenProviders();

builder.Services
	.AddIdentityServer(options =>
	{
		options.Events.RaiseErrorEvents = true;
		options.Events.RaiseInformationEvents = true;
		options.Events.RaiseFailureEvents = true;
		options.Events.RaiseSuccessEvents = true;
		options.EmitStaticAudienceClaim = true;
	})
	.AddInMemoryIdentityResources(Config.IdentityResources)
	.AddInMemoryApiScopes(Config.ApiScopes)
	.AddInMemoryClients(Config.Clients)
	.AddAspNetIdentity<ApplicationUser>();

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
