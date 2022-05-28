using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

#pragma warning disable CA1716 // Identifiers should not match keywords
namespace IdentityService.Pages.Home.Error;
#pragma warning restore CA1716 // Identifiers should not match keywords

[AllowAnonymous]
[SecurityHeaders]
public class Index : PageModel
{
	private readonly IIdentityServerInteractionService interaction;

	private readonly IWebHostEnvironment environment;

	public ViewModel View { get; set; }

	public Index(IIdentityServerInteractionService interaction, IWebHostEnvironment environment)
	{
		this.interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
		this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
	}

	public async Task OnGet(string errorId)
	{
		View = new ViewModel();

		// Retrieve error details from IdentityServer.
		var message = await interaction.GetErrorContextAsync(errorId);
		if (message != null)
		{
			View.Error = message;

			if (!environment.IsDevelopment())
			{
				// Only show in development.
				message.ErrorDescription = null;
			}
		}
	}
}
