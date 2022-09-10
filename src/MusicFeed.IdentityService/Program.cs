using Duende.IdentityServer;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MusicFeed.IdentityService;
using MusicFeed.IdentityService.Abstractions;
using MusicFeed.IdentityService.Infrastructure.PostgreSql;
using MusicFeed.IdentityService.Infrastructure.PostgreSql.Migrations;
using MusicFeed.IdentityService.Settings;
using MusicFeed.IdentityService.Stub;

var builder = WebApplication.CreateBuilder(args);

var identityServerSettings = new IdentityServerSettings();
builder.Configuration.Bind(identityServerSettings);

ConfigureServices(builder);

var app = builder.Build();
ConfigureMiddleware(app, app.Environment);
ConfigureEndpoints(app);

app.Run();

void ConfigureServices(WebApplicationBuilder webApplicationBuilder)
{
	var services = webApplicationBuilder.Services;
	var configuration = webApplicationBuilder.Configuration;

	var isStub = webApplicationBuilder.Environment.IsStub();

	services.AddRazorPages();

	services.AddHealthChecks()
		.AddNpgSql(ConnectionStringFactory(), failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready" }, timeout: TimeSpan.FromSeconds(5));

	services
		.AddIdentity<ApplicationUser, IdentityRole>()
		.AddEntityFrameworkStores<CustomIdentityDbContext>()
		.AddDefaultTokenProviders();

	services.Configure<IdentityServerSettings>(configuration.Bind);

	if (isStub)
	{
		services.AddDbContext<CustomIdentityDbContext>(options =>
		{
			options.UseInMemoryDatabase("IdentityDB");
		});
	}
	else
	{
		services.AddPostgreSqlDalForIdentityDb(ConnectionStringFactory());
	}

	var identityServerBuilder = services
		.AddIdentityServer(options =>
		{
			options.IssuerUri = identityServerSettings.IssuerUri?.OriginalString;

			options.Events.RaiseErrorEvents = true;
			options.Events.RaiseInformationEvents = true;
			options.Events.RaiseFailureEvents = true;
			options.Events.RaiseSuccessEvents = true;
			options.EmitStaticAudienceClaim = true;
		})
		.AddInMemoryIdentityResources(Config.IdentityResources)
		.AddInMemoryApiResources(Config.ApiResources)
		.AddInMemoryApiScopes(Config.ApiScopes)
		.AddInMemoryClients(identityServerSettings.Clients)
		.AddAspNetIdentity<ApplicationUser>();

	if (!isStub)
	{
		identityServerBuilder.AddPostgreSqlDalForPersistedGrantStore(GetIdentityDbConnectionString(configuration));
	}

	var dataProtectionBuilder = services.AddDataProtection();

	if (!isStub)
	{
		dataProtectionBuilder.PersistKeysToDbContext<CustomPersistedGrantDbContext>();
	}

	services
		.AddAuthentication()
		.AddGoogle("Google", options =>
		{
			options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

			options.ClientId = configuration["authentication:google:clientId"];
			options.ClientSecret = configuration["authentication:google:clientSecret"];
		});
}

string GetIdentityDbConnectionString(IConfiguration configuration)
{
	return configuration.GetConnectionString("identityDB");
}

Func<IServiceProvider, string> ConnectionStringFactory()
{
	return serviceProvider =>
	{
		var configuration = serviceProvider.GetRequiredService<IConfiguration>();
		return GetIdentityDbConnectionString(configuration);
	};
}

void ConfigureMiddleware(IApplicationBuilder appBuilder, IWebHostEnvironment environment)
{
	if (!environment.IsDevelopment())
	{
		appBuilder.UseExceptionHandler("/Error");
	}

	app.Use((context, next) =>
	{
		context.Request.Scheme = identityServerSettings.EndpointsScheme;
		return next();
	});

	app.UsePathBase("/identity");

	appBuilder.UseStaticFiles();

	appBuilder.UseRouting();
	appBuilder.UseIdentityServer();
	appBuilder.UseAuthorization();
}

void ConfigureEndpoints(WebApplication webApplication)
{
	webApplication
		.MapRazorPages()
		.RequireAuthorization();

	if (webApplication.Environment.IsStub())
	{
		webApplication.MapControllers();
	}

	app.UseEndpoints(endpoints =>
	{
		endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
		{
			Predicate = check => check.Tags.Contains("ready"),
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
		});

		endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
		{
			Predicate = check => check.Tags.Contains("live"),
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
		});
	});
}
