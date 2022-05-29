// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace MusicFeed.IdentityService.Pages.Account.Login;

public class InputModel
{
	[Required]
	public string Username { get; set; }

	[Required]
	public string Password { get; set; }

	public bool RememberLogin { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
	public string ReturnUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

	public string Button { get; set; }
}
