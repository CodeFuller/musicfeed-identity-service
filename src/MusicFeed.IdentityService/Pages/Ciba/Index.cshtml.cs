using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFeed.IdentityService.Pages.Ciba;

[AllowAnonymous]
[SecurityHeaders]
public class IndexModel : PageModel
{
	public BackchannelUserLoginRequest LoginRequest { get; set; }

	private readonly IBackchannelAuthenticationInteractionService backchannelAuthenticationInteraction;
	private readonly ILogger<IndexModel> logger;

	public IndexModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService, ILogger<IndexModel> logger)
	{
		backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService ?? throw new ArgumentNullException(nameof(backchannelAuthenticationInteractionService));
		this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<IActionResult> OnGet(string id)
	{
		LoginRequest = await backchannelAuthenticationInteraction.GetLoginRequestByInternalIdAsync(id);
		if (LoginRequest == null)
		{
			logger.LogWarning($"Invalid backchannel login id {id}");
			return RedirectToPage("/Home/Error/Index");
		}

		return Page();
	}
}
