using Duende.IdentityServer.Models;

namespace MusicFeed.IdentityService.Settings
{
	public class IdentityServerSettings
	{
		// This mode is used by IT in other services.
		public bool UseInMemoryDatabase { get; set; }

		public IReadOnlyCollection<Client> Clients { get; set; }
	}
}
