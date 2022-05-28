using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Challenge : PageModel
{
	private readonly IIdentityServerInteractionService interactionService;

	public Challenge(IIdentityServerInteractionService interactionService)
	{
		this.interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
	}

#pragma warning disable CA1054 // URI-like parameters should not be strings
	public IActionResult OnGet(string scheme, string returnUrl)
#pragma warning restore CA1054 // URI-like parameters should not be strings
	{
		if (string.IsNullOrEmpty(returnUrl))
		{
			returnUrl = "~/";
		}

		// Validate returnUrl - either it is a valid OIDC URL or back to a local page.
		if (Url.IsLocalUrl(returnUrl) == false && interactionService.IsValidReturnUrl(returnUrl) == false)
		{
			// User might have clicked on a malicious link - should be logged.
			throw new InvalidOperationException("Invalid return URL");
		}

		// Start challenge and roundtrip the return URL and scheme.
		var props = new AuthenticationProperties
		{
			RedirectUri = Url.Page("/externallogin/callback"),

			Items =
			{
				{ "returnUrl", returnUrl },
				{ "scheme", scheme },
			},
		};

		return Challenge(props, scheme);
	}
}
