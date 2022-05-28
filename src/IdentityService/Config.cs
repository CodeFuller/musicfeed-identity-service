using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityService
{
	public static class Config
	{
		private const string ApiName = "musicfeed-api";

		public static IEnumerable<IdentityResource> IdentityResources
		{
			get
			{
				yield return new IdentityResources.OpenId();
				yield return new IdentityResources.Profile();
			}
		}

		public static IEnumerable<ApiScope> ApiScopes
		{
			get
			{
				yield return new(name: ApiName, displayName: "Music Feed API");
			}
		}

		public static IEnumerable<Client> Clients
		{
			get
			{
				// Machine to machine client.
				yield return new()
				{
					ClientId = "client",
					ClientSecrets = { new Secret("secret".Sha256()) },
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					AllowedScopes = { ApiName },
				};

				// Interactive ASP.NET Core Web App.
				yield return new()
				{
					ClientId = "web",
					ClientSecrets = { new Secret("secret".Sha256()) },

					AllowedGrantTypes = GrantTypes.Code,

					RedirectUris = { "https://localhost:5002/signin-oidc" },

					PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

					AllowOfflineAccess = true,

					AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						ApiName,
					},
				};
			}
		}
	}
}
