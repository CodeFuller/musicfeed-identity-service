// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace IdentityService.Pages.Ciba;

public class ViewModel
{
	public string ClientName { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
	public string ClientUrl { get; set; }

	public string ClientLogoUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

	public string BindingMessage { get; set; }

	public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }

	public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
}
