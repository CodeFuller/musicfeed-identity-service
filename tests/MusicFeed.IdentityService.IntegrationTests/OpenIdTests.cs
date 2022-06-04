using System.IO;
using System.Threading.Tasks;
using FluentAssertions.Json;
using IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace MusicFeed.IdentityService.IntegrationTests
{
	[TestClass]
	public class OpenIdTests
	{
		[TestMethod]
		public async Task GetDiscoveryDocument_ReturnsCorrectDiscoverDocument()
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

			expectedDocument.Should().BeEquivalentTo(actualDocument);
		}
	}
}
