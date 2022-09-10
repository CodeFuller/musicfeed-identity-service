using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicFeed.IdentityService.Stub;
using static MusicFeed.IdentityService.IntegrationTests.TestHelpers;

namespace MusicFeed.IdentityService.IntegrationTests
{
	[TestClass]
	public class StubTests
	{
		[TestMethod]
		public async Task StubToken_InNonStubEnvironment_IsNotAvailable()
		{
			// Arrange

			await using var factory = new CustomWebApplicationFactory(setupConfiguration: AvailablePostgresConfiguration);
			using var httpClient = factory.CreateClient();

			// Act

			var issueTokenRequest = new IssueTokenRequest
			{
				UserId = "Some User Id",
			};

			using var issueTokenResponse = await httpClient.PostAsJsonAsync("stub/token", issueTokenRequest);

			// Assert

			issueTokenResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[TestMethod]
		public async Task StubToken_InStubEnvironment_ReturnsCorrectToken()
		{
			// Arrange

			// We configure incorrect connection string to make sure that DB is not used in stub mode.
			await using var factory = new CustomWebApplicationFactory(setupConfiguration: UnavailablePostgresConfiguration, environment: "Stub");
			using var httpClient = factory.CreateClient();

			// Act

			var issueTokenRequest = new IssueTokenRequest
			{
				UserId = "Some User Id",
			};

			using var issueTokenResponse = await httpClient.PostAsJsonAsync("stub/token", issueTokenRequest);

			// Assert

			issueTokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var jwtEncodedString = await issueTokenResponse.Content.ReadAsStringAsync();
			var jwtSecurityToken = new JwtSecurityToken(jwtEncodedString);

			jwtSecurityToken.Issuer.Should().Be("https://identity-service-stub/");
			jwtSecurityToken.Audiences.Should().BeEquivalentTo("https://identity-service-stub/resources");

			var expectedClaims = new[]
			{
				new Claim(JwtClaimTypes.Subject, "Some User Id", null, "https://identity-service-stub/", "https://identity-service-stub/"),
			};

			foreach (var expectedClaim in expectedClaims)
			{
				jwtSecurityToken.Claims.Should().ContainEquivalentOf(expectedClaim);
			}
		}
	}
}
