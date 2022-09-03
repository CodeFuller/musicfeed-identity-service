using Duende.IdentityServer.Models;

namespace MusicFeed.IdentityService.Settings
{
	public class IdentityServerSettings
	{
		public IReadOnlyCollection<Client> Clients { get; set; }
	}
}
