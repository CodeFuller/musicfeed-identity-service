using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicFeed.IdentityService.Abstractions;

namespace MusicFeed.IdentityService.Pages.Account.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
	private readonly SignInManager<ApplicationUser> signInManager;

	private readonly IIdentityServerInteractionService interaction;

	private readonly IEventService events;

	[BindProperty]
	public string LogoutId { get; set; }

	public Index(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction, IEventService events)
	{
		this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
		this.interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
		this.events = events ?? throw new ArgumentNullException(nameof(events));
	}

	public async Task<IActionResult> OnGet(string logoutId)
	{
		LogoutId = logoutId;

		var showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

		if (!User.Identity.IsAuthenticated)
		{
			// If the user is not authenticated, then just show logged out page.
			showLogoutPrompt = false;
		}
		else
		{
			var context = await interaction.GetLogoutContextAsync(LogoutId);
			if (context?.ShowSignoutPrompt == false)
			{
				// It's safe to automatically sign-out.
				showLogoutPrompt = false;
			}
		}

		if (showLogoutPrompt == false)
		{
			// If the request for logout was properly authenticated from IdentityServer,
			// then we don't need to show the prompt and can just log the user out directly.
			return await OnPost();
		}

		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		if (User.Identity.IsAuthenticated)
		{
			// If there's no current logout context, we need to create one.
			// This captures necessary info from the current logged in user.
			// This can still return null if there is no context needed.
			LogoutId ??= await interaction.CreateLogoutContextAsync();

			// Delete local authentication cookie.
			await signInManager.SignOutAsync();

			// Raise the logout event.
			await events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

			// See if we need to trigger federated logout.
			var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

			// If it's a local login we can ignore this workflow.
			if (idp != null && idp != Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
			{
				// We need to see if the provider supports external logout.
				if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
				{
					// Build a return URL so the upstream provider will redirect back to us after the user has logged out.
					// This allows us to then complete our single sign-out processing.
					var url = Url.Page("/Account/Logout/Loggedout", new { logoutId = LogoutId });

					// This triggers a redirect to the external provider for sign-out.
					return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
				}
			}
		}

		return RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = LogoutId });
	}
}
