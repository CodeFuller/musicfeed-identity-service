using System;
using IdentityService.Client;
using IdentityService.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.IntegrationTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
	{
		private readonly Action<IServiceCollection> setupServices;

		private readonly Action<IConfigurationBuilder> setupConfiguration;

		private ServiceProvider ServiceProvider { get; set; }

		public CustomWebApplicationFactory(Action<IServiceCollection> setupServices = null, Action<IConfigurationBuilder> setupConfiguration = null)
		{
			this.setupServices = setupServices ?? (_ => { });
			this.setupConfiguration = setupConfiguration ?? (_ => { });
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureServices(setupServices);
			builder.ConfigureAppConfiguration(setupConfiguration);
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

			ClearIdentityDatabase();
		}

		private void ClearIdentityDatabase()
		{
			using var serviceScope = Services.CreateScope();
			var servicesProvider = serviceScope.ServiceProvider;
			using var context = servicesProvider.GetRequiredService<ApplicationDbContext>();

			context.Users.RemoveRange(context.Users);
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
