// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages;

public static class PageExtensions
{
	public static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
	{
		var provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
		var handler = await provider.GetHandlerAsync(context, scheme);
		return handler is IAuthenticationSignOutHandler;
	}

	public static bool IsNativeClient(this AuthorizationRequest context)
	{
		return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal) &&
		       !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
	}

#pragma warning disable CA1054 // URI-like parameters should not be strings
	public static IActionResult LoadingPage(this PageModel page, string redirectUri)
#pragma warning restore CA1054 // URI-like parameters should not be strings
	{
		page.HttpContext.Response.StatusCode = 200;
		page.HttpContext.Response.Headers["Location"] = String.Empty;

		return page.RedirectToPage("/Redirect/Index", new { RedirectUri = redirectUri });
	}
}
