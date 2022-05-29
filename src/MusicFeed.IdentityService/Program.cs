using Duende.IdentityServer;
using Microsoft.AspNetCore.Identity;
using MusicFeed.IdentityService;
using MusicFeed.IdentityService.Abstractions;
using MusicFeed.IdentityService.Infrastructure.PostgreSql;
using MusicFeed.IdentityService.Settings;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
ConfigureMiddleware(app, app.Environment);
ConfigureEndpoints(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
	services.AddRazorPages();

	services.AddPostgreSqlDal(configuration.GetConnectionString("identityDB"));

	services
		.AddIdentity<ApplicationUser, IdentityRole>()
		.AddEntityFrameworkStores<IdentityDbContext>()
		.AddDefaultTokenProviders();

	var identityServerSettings = new IdentityServerSettings();
	configuration.Bind(identityServerSettings);

	services
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
		.AddInMemoryClients(identityServerSettings.Clients)
		.AddAspNetIdentity<ApplicationUser>();

	services
		.AddAuthentication()
		.AddGoogle("Google", options =>
		{
			options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

			options.ClientId = configuration["authentication:google:clientId"];
			options.ClientSecret = configuration["authentication:google:clientSecret"];
		});
}

void ConfigureMiddleware(IApplicationBuilder appBuilder, IWebHostEnvironment environment)
{
	if (!environment.IsDevelopment())
	{
		appBuilder.UseExceptionHandler("/Error");
	}

	appBuilder.UseStaticFiles();

	appBuilder.UseRouting();
	appBuilder.UseIdentityServer();
	appBuilder.UseAuthorization();
}

void ConfigureEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
{
	endpointRouteBuilder
		.MapRazorPages()
		.RequireAuthorization();
}
