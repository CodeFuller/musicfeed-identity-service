namespace IdentityService.Pages.Device;

public class ViewModel
{
	public string ClientName { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
	public string ClientUrl { get; set; }

	public string ClientLogoUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

	public bool AllowRememberConsent { get; set; }

	public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }

	public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
}
