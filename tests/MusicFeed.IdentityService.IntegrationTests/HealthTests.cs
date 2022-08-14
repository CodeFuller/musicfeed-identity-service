using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicFeed.IdentityService.IntegrationTests
{
	[TestClass]
	public class HealthTests
	{
		private const string HealthyPostgresConnectionString = "Server=localhost;Port=15432;Database=IdentityDB;User Id=postgres;Password=Qwerty123;";
		private const string UnhealthyPostgresConnectionString = "Server=localhost;Port=15433;Database=IdentityDB;User Id=postgres;Password=Qwerty123;";

		private static Action<IConfigurationBuilder> HealthyPostgresConfiguration => GetDbConfiguration(HealthyPostgresConnectionString);

		private static Action<IConfigurationBuilder> UnhealthyPostgresConfiguration => GetDbConfiguration(UnhealthyPostgresConnectionString);

		[TestMethod]
		public async Task LiveRequest_IfAllRequiredServicesAreHealthy_ReturnsHealthyResponse()
		{
			// Arrange

			await using var factory = new CustomWebApplicationFactory(HealthyPostgresConfiguration);
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/live", UriKind.Relative));

			// Assert

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[TestMethod]
		public async Task LiveRequest_IfPostgresDatabaseIsUnhealthy_ReturnsHealthyResponse()
		{
			// Arrange

			await using var factory = new CustomWebApplicationFactory(UnhealthyPostgresConfiguration);
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/live", UriKind.Relative));

			// Assert

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[TestMethod]
		public async Task ReadyRequest_IfAllRequiredServicesAreHealthy_ReturnsHealthyResponse()
		{
			// Arrange

			await using var factory = new CustomWebApplicationFactory(HealthyPostgresConfiguration);
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

			// Assert

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[TestMethod]
		public async Task ReadyRequest_IfPostgresDatabaseIsUnhealthy_ReturnsUnhealthyResponse()
		{
			// Arrange

			await using var factory = new CustomWebApplicationFactory(UnhealthyPostgresConfiguration);
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

			// Assert

			response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
		}

		private static Action<IConfigurationBuilder> GetDbConfiguration(string postgresConnectionString)
		{
			return configBuilder => configBuilder
				.AddInMemoryCollection(new[]
				{
					new KeyValuePair<string, string>("connectionStrings:identityDB", postgresConnectionString),
				});
		}
	}
}
