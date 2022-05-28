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

	const string token = @"<Put access token here>";

	using var apiClient = new HttpClient();
	apiClient.SetBearerToken(token);

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
