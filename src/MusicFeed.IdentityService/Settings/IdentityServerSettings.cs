using Duende.IdentityServer.Models;

namespace MusicFeed.IdentityService.Settings
{
	public class IdentityServerSettings
	{
		public Uri IssuerUri { get; set; }

		public string EndpointsScheme { get; set; }

		public IReadOnlyCollection<Client> Clients { get; set; }
	}
}
