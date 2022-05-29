using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Account.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class LoggedOut : PageModel
{
	private readonly IIdentityServerInteractionService interactionService;

	public LoggedOutViewModel View { get; set; }

	public LoggedOut(IIdentityServerInteractionService interactionService)
	{
		this.interactionService = interactionService;
	}

	public async Task OnGet(string logoutId)
	{
		// Get context information (client name, post logout redirect URI and iframe for federated signout).
		var logout = await interactionService.GetLogoutContextAsync(logoutId);

		View = new LoggedOutViewModel
		{
			AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut,
			PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
			ClientName = String.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
			SignOutIframeUrl = logout?.SignOutIFrameUrl,
		};
	}
}
