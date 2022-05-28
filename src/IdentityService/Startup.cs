using System;
using CodeFuller.MusicFeed.ApplicationInsights;
using HealthChecks.UI.Client;
using IdentityService.Interfaces;
using IdentityService.Internal;
using IdentityService.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityService
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddGrpc();

			var connectionString = configuration.GetConnectionString("DefaultConnection");

			services.AddControllers();
			services.AddHealthChecks().AddSqlServer(connectionString, tags: new[] { "ready" });

			services.AddApplicationInsights(settings => configuration.Bind("applicationInsights", settings));

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString, x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)));

			// https://stackoverflow.com/questions/55361533/addidentity-vs-addidentitycore
			services.AddIdentity<IdentityUser, IdentityRole>(options =>
				{
					options.Password.RequireDigit = true;
					options.Password.RequireLowercase = true;
					options.Password.RequireNonAlphanumeric = true;
					options.Password.RequireUppercase = true;
					options.Password.RequiredLength = 8;
					options.Password.RequiredUniqueChars = 5;

					// Without this setting, Identity does not validate email format.
					options.User.RequireUniqueEmail = true;

					options.Lockout.AllowedForNewUsers = true;
					options.Lockout.MaxFailedAccessAttempts = 5;
					options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);

					options.SignIn.RequireConfirmedAccount = false;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddScoped<IMigrationsApplier, MigrationsApplier>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationsApplier migrationsApplier)
		{
			migrationsApplier.ApplyMigrations();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<Services.IdentityService>();

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

				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
				});
			});
		}
	}
}
