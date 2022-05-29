// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace IdentityService.Pages.Consent;

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
