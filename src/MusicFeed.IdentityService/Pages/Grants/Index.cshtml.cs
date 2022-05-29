using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFeed.IdentityService.Pages.Grants;

[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
	private readonly IIdentityServerInteractionService interaction;
	private readonly IClientStore clients;
	private readonly IResourceStore resources;
	private readonly IEventService events;

	public ViewModel View { get; set; }

	[BindProperty]
	[Required]
	public string ClientId { get; set; }

	public Index(IIdentityServerInteractionService interaction, IClientStore clients, IResourceStore resources, IEventService events)
	{
		this.interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
		this.clients = clients ?? throw new ArgumentNullException(nameof(clients));
		this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
		this.events = events ?? throw new ArgumentNullException(nameof(events));
	}

	public async Task OnGet()
	{
		var grants = await interaction.GetAllUserGrantsAsync();

		var list = new List<GrantViewModel>();
		foreach (var grant in grants)
		{
			var client = await clients.FindClientByIdAsync(grant.ClientId);
			if (client != null)
			{
				var resources = await this.resources.FindResourcesByScopeAsync(grant.Scopes);

				var item = new GrantViewModel
				{
					ClientId = client.ClientId,
					ClientName = client.ClientName ?? client.ClientId,
					ClientLogoUrl = client.LogoUri,
					ClientUrl = client.ClientUri,
					Description = grant.Description,
					Created = grant.CreationTime,
					Expires = grant.Expiration,
					IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
					ApiGrantNames = resources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray(),
				};

				list.Add(item);
			}
		}

		View = new ViewModel
		{
			Grants = list,
		};
	}

	public async Task<IActionResult> OnPost()
	{
		await interaction.RevokeUserConsentAsync(ClientId);
		await events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), ClientId));

		return RedirectToPage("/Grants/Index");
	}
}
