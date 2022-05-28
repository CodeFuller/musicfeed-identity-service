using System.Text.Json;
using IdentityModel.Client;

try
{
	using var client = new HttpClient();
	var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
	if (discoveryDocumentResponse.IsError)
	{
		throw new InvalidOperationException($"Failed to get discovery document: {discoveryDocumentResponse.Error}");
	}

	using var tokenRequest = new ClientCredentialsTokenRequest
	{
		Address = discoveryDocumentResponse.TokenEndpoint,

		ClientId = "client",
		ClientSecret = "secret",
		Scope = "musicfeed-api",
	};

	var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
	if (tokenResponse.IsError)
	{
		throw new InvalidOperationException($"Failed to issue token: {tokenResponse.Error}");
	}

	Console.WriteLine($"Issued a token: {tokenResponse.AccessToken}");

	using var apiClient = new HttpClient();
	apiClient.SetBearerToken(tokenResponse.AccessToken);

	var response = await apiClient.GetAsync(new Uri("https://localhost:6001/identity"));
	response.EnsureSuccessStatusCode();

	var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
	Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
}
#pragma warning disable CA1031 // Do not catch general exception types
catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
{
	Console.WriteLine(e);
}
