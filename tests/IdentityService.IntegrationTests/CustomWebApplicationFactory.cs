using System;
using IdentityService.Client;
using IdentityService.Migrations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.IntegrationTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
	{
		private readonly Action<ApplicationDbContext> seedData;

		private ServiceProvider ServiceProvider { get; set; }

		public CustomWebApplicationFactory(Action<ApplicationDbContext> seedData = null)
		{
			this.seedData = seedData ?? (_ => { });
		}

		public TService CreateServiceClient<TService>()
		{
			InitializeServiceProvider();

			return ServiceProvider.GetRequiredService<TService>();
		}

		private void InitializeServiceProvider()
		{
			var services = new ServiceCollection();

			var httpClient = CreateClient();

			services.AddIdentityServiceClient(factoryOptions =>
			{
				factoryOptions.Address = httpClient.BaseAddress;
				factoryOptions.ChannelOptionsActions.Add(channelOptions => channelOptions.HttpClient = httpClient);
			});

			ServiceProvider = services.BuildServiceProvider();

			SeedData();
		}

		private void SeedData()
		{
			using var serviceScope = Services.CreateScope();
			var servicesProvider = serviceScope.ServiceProvider;
			using var context = servicesProvider.GetRequiredService<ApplicationDbContext>();

			context.Users.RemoveRange(context.Users);
			seedData(context);
			context.SaveChanges();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				ServiceProvider?.Dispose();
			}
		}
	}
}
