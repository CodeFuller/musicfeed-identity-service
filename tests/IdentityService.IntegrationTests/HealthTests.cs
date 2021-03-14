using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using IdentityService.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IdentityService.IntegrationTests
{
	[TestClass]
	public class HealthTests
	{
		[TestMethod]
		public async Task LiveRequest_IdentityDatabaseIsAvailable_ReturnsHealthyResponse()
		{
			// Arrange

			using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/live", UriKind.Relative));

			// Assert

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod]
		public async Task LiveRequest_IdentityDatabaseIsUnavailable_ReturnsHealthyResponse()
		{
			// Arrange

			using var factory = CreateApplicationFactoryForUnavailableDatabase();
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/live", UriKind.Relative));

			// Assert

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod]
		public async Task ReadyRequest_IdentityDatabaseIsAvailable_ReturnsHealthyResponse()
		{
			// Arrange

			using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

			// Assert

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod]
		public async Task ReadyRequest_IdentityDatabaseIsUnavailable_ReturnsUnhealthyResponse()
		{
			// Arrange

			using var factory = CreateApplicationFactoryForUnavailableDatabase();
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

			// Assert

			Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
		}

		private static CustomWebApplicationFactory CreateApplicationFactoryForUnavailableDatabase()
		{
			return new(
				services => services.AddSingleton<IMigrationsApplier>(Mock.Of<IMigrationsApplier>()),
				configBuilder => configBuilder.AddInMemoryCollection(
					new[] { new KeyValuePair<string, string>("connectionStrings:defaultConnection", "Server=localhost,14330;Database=TestIdentityDB;Trusted_Connection=True;Integrated Security=True") }));
		}
	}
}
