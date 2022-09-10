using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MusicFeed.IdentityService.IntegrationTests.TestHelpers;

namespace MusicFeed.IdentityService.IntegrationTests
{
	[TestClass]
	public class HealthTests
	{
		[TestMethod]
		public async Task LiveRequest_IfAllRequiredServicesAreHealthy_ReturnsHealthyResponse()
		{
			// Arrange

			await using var factory = new CustomWebApplicationFactory(AvailablePostgresConfiguration);
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

			await using var factory = new CustomWebApplicationFactory(UnavailablePostgresConfiguration);
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

			await using var factory = new CustomWebApplicationFactory(AvailablePostgresConfiguration);
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

			await using var factory = new CustomWebApplicationFactory(UnavailablePostgresConfiguration);
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

			// Assert

			response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
		}
	}
}
