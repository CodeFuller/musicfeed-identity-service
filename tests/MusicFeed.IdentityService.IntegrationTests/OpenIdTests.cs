using System.IO;
using System.Threading.Tasks;
using FluentAssertions.Json;
using IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using static MusicFeed.IdentityService.IntegrationTests.TestHelpers;

namespace MusicFeed.IdentityService.IntegrationTests
{
	[TestClass]
	public class OpenIdTests
	{
		[TestMethod]
		public async Task GetDiscoveryDocument_InNonStubEnvironment_ReturnsCorrectDiscoverDocument()
		{
			// Arrange

			await using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateClient();

			// Act

			var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();

			// Assert

			var expectedDocumentJson = await File.ReadAllTextAsync("ExpectedDiscoveryDocument.json");
			var expectedDocument = JToken.Parse(expectedDocumentJson);

			var actualDocument = JToken.Parse(discoveryDocumentResponse.Raw);

			actualDocument.Should().BeEquivalentTo(expectedDocument);
		}

		[TestMethod]
		public async Task GetDiscoveryDocument_InStubEnvironment_ReturnsCorrectDiscoverDocument()
		{
			// Arrange

			await using var factory = new CustomWebApplicationFactory(setupConfiguration: UnavailablePostgresConfiguration, environment: "Stub");
			using var client = factory.CreateClient();

			// Act

			var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();

			// Assert

			var expectedDocumentJson = await File.ReadAllTextAsync("ExpectedDiscoveryDocument.Stub.json");
			var expectedDocument = JToken.Parse(expectedDocumentJson);

			var actualDocument = JToken.Parse(discoveryDocumentResponse.Raw);

			actualDocument.Should().BeEquivalentTo(expectedDocument);
		}
	}
}
