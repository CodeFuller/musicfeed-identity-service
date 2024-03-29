using System.Text;
using System.Text.Json;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;

namespace MusicFeed.IdentityService.Pages.Diagnostics;

public class ViewModel
{
	public AuthenticateResult AuthenticateResult { get; }

	public IEnumerable<string> Clients { get; } = new List<string>();

	public ViewModel(AuthenticateResult result)
	{
		AuthenticateResult = result;

		if (result.Properties.Items.ContainsKey("client_list"))
		{
			var encoded = result.Properties.Items["client_list"];
			var bytes = Base64Url.Decode(encoded);
			var value = Encoding.UTF8.GetString(bytes);

			Clients = JsonSerializer.Deserialize<string[]>(value);
		}
	}
}
