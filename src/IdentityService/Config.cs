using Duende.IdentityServer.Models;

namespace IdentityService
{
	public static class Config
	{
		private const string ApiName = "musicfeed-api";

		public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
		{
			new(name: ApiName, displayName: "Music Feed API"),
		};

		public static IEnumerable<Client> Clients
		{
			get
			{
				yield return new()
				{
					ClientId = "client",
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					ClientSecrets = { new Secret("secret".Sha256()) },
					AllowedScopes = { ApiName },
				};
			}
		}
	}
}
