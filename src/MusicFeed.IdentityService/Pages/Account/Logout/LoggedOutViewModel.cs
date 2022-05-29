// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace MusicFeed.IdentityService.Pages.Account.Logout;

public class LoggedOutViewModel
{
#pragma warning disable CA1056 // URI-like properties should not be strings
	public string PostLogoutRedirectUri { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

	public string ClientName { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
	public string SignOutIframeUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

	public bool AutomaticRedirectAfterSignOut { get; set; }
}
