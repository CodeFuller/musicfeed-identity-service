namespace MusicFeed.IdentityService.Pages.Grants;

public class GrantViewModel
{
	public string ClientId { get; set; }

	public string ClientName { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
	public string ClientUrl { get; set; }

	public string ClientLogoUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

	public string Description { get; set; }

	public DateTime Created { get; set; }

	public DateTime? Expires { get; set; }

	public IEnumerable<string> IdentityGrantNames { get; set; }

	public IEnumerable<string> ApiGrantNames { get; set; }
}
