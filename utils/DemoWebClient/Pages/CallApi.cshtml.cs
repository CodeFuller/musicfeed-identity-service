using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoWebClient.Pages
{
	public class CallApiModel : PageModel
	{
		public string Json { get; set; } = String.Empty;

		public async Task OnGet()
		{
			var accessToken = await HttpContext.GetTokenAsync("access_token");

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			var content = await client.GetStringAsync(new Uri("https://localhost:6001/identity"));

			var parsed = JsonDocument.Parse(content);
			Json = JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true });
		}
    }
}
