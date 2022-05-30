using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

			var expectedJson = await File.ReadAllTextAsync("ExpectedDiscoveryDocument.json");

			var actualJson = ToIndentedString(discoveryDocumentResponse.Json);

			actualJson.Should().Be(expectedJson);
		}

		private static string ToIndentedString(JsonElement element)
		{
			if (element.ValueKind == JsonValueKind.Undefined)
			{
				return String.Empty;
			}

			return JsonSerializer.Serialize(element, new JsonSerializerOptions { WriteIndented = true });
		}
	}
}
