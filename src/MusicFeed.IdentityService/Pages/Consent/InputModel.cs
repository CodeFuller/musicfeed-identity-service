namespace MusicFeed.IdentityService.Pages.Consent;

public class InputModel
{
	public string Button { get; set; }

	public IEnumerable<string> ScopesConsented { get; set; }

	public bool RememberConsent { get; set; } = true;

#pragma warning disable CA1056 // URI-like properties should not be strings
	public string ReturnUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

	public string Description { get; set; }
}
