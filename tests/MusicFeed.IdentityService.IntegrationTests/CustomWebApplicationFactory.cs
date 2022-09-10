using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MusicFeed.IdentityService.IntegrationTests
{
	internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
	{
		private readonly Action<IConfigurationBuilder> setupConfiguration;

		private readonly string environment;

		public CustomWebApplicationFactory(Action<IConfigurationBuilder> setupConfiguration = null, string environment = null)
		{
			this.setupConfiguration = setupConfiguration ?? (_ => { });
			this.environment = environment;
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			if (!String.IsNullOrEmpty(environment))
			{
				builder.UseEnvironment(environment);
			}

			builder.ConfigureAppConfiguration(setupConfiguration);
		}
	}
}
