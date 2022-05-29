using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFeed.IdentityService.Pages.Ciba;

[SecurityHeaders]
[Authorize]
public class AllModel : PageModel
{
	private readonly IBackchannelAuthenticationInteractionService backchannelAuthenticationInteraction;

	public IEnumerable<BackchannelUserLoginRequest> Logins { get; set; }

	[Required]
	[BindProperty]
	public string Id { get; set; }

	[Required]
	[BindProperty]
	public string Button { get; set; }

	public AllModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService)
	{
		backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService ?? throw new ArgumentNullException(nameof(backchannelAuthenticationInteractionService));
	}

	public async Task OnGet()
	{
		Logins = await backchannelAuthenticationInteraction.GetPendingLoginRequestsForCurrentUserAsync();
	}
}
