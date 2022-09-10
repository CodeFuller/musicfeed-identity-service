using Duende.IdentityServer.Models;

namespace MusicFeed.IdentityService
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

		public static IEnumerable<ApiResource> ApiResources
		{
			get
			{
				yield return new(name: ApiName, displayName: "Music Feed API")
				{
					Scopes = new[] { ApiName },
				};
			}
		}

		public static IEnumerable<ApiScope> ApiScopes
		{
			get
			{
				yield return new(name: ApiName, displayName: "Music Feed API");
			}
		}
	}
}
