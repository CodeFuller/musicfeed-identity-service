using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Consent;

[Authorize]
[SecurityHeadersAttribute]
public class Index : PageModel
{
	private readonly IIdentityServerInteractionService interaction;
	private readonly IEventService events;
	private readonly ILogger<Index> logger;

	public ViewModel View { get; set; }

	[BindProperty]
	public InputModel Input { get; set; }

	public Index(IIdentityServerInteractionService interaction, IEventService events, ILogger<Index> logger)
	{
		this.interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
		this.events = events ?? throw new ArgumentNullException(nameof(events));
		this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

#pragma warning disable CA1054 // URI-like parameters should not be strings
	public async Task<IActionResult> OnGet(string returnUrl)
#pragma warning restore CA1054 // URI-like parameters should not be strings
	{
		View = await BuildViewModelAsync(returnUrl);

		if (View == null)
		{
			return RedirectToPage("/Home/Error/Index");
		}

		Input = new InputModel
		{
			ReturnUrl = returnUrl,
		};

		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		// Validate return url is still valid.
		var request = await interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
		if (request == null)
		{
			return RedirectToPage("/Home/Error/Index");
		}

		ConsentResponse grantedConsent = null;

		if (Input?.Button == "no")
		{
			// User clicked 'no' - send back the standard 'access_denied' response.
			grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

			// Emit event.
			await events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
		}
		else if (Input?.Button == "yes")
		{
			// User clicked 'yes' - validate the data.
			// If the user consented to some scope, build the response model
			if (Input.ScopesConsented != null && Input.ScopesConsented.Any())
			{
				var scopes = Input.ScopesConsented;
				if (ConsentOptions.EnableOfflineAccess == false)
				{
					scopes = scopes.Where(x => x != Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess);
				}

				grantedConsent = new ConsentResponse
				{
					RememberConsent = Input.RememberConsent,
					ScopesValuesConsented = scopes.ToArray(),
					Description = Input.Description,
				};

				// Emit event.
				await events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
			}
			else
			{
				ModelState.AddModelError(String.Empty, ConsentOptions.MustChooseOneErrorMessage);
			}
		}
		else
		{
			ModelState.AddModelError(String.Empty, ConsentOptions.InvalidSelectionErrorMessage);
		}

		if (grantedConsent != null)
		{
			// Communicate outcome of consent back to IdentityServer.
			await interaction.GrantConsentAsync(request, grantedConsent);

			// Redirect back to authorization endpoint.
			if (request.IsNativeClient())
			{
				// The client is native, so this change in how to return the response is for better UX for the end user.
				return this.LoadingPage(Input.ReturnUrl);
			}

			return Redirect(Input.ReturnUrl);
		}

		// We need to redisplay the consent UI.
		View = await BuildViewModelAsync(Input.ReturnUrl, Input);
		return Page();
	}

	private async Task<ViewModel> BuildViewModelAsync(string returnUrl, InputModel model = null)
	{
		var request = await interaction.GetAuthorizationContextAsync(returnUrl);
		if (request != null)
		{
			return CreateConsentViewModel(model, returnUrl, request);
		}

		logger.LogError($"No consent request matching request: {returnUrl}");

		return null;
	}

	private ViewModel CreateConsentViewModel(InputModel model, string returnUrl, AuthorizationRequest request)
	{
		var vm = new ViewModel
		{
			ClientName = request.Client.ClientName ?? request.Client.ClientId,
			ClientUrl = request.Client.ClientUri,
			ClientLogoUrl = request.Client.LogoUri,
			AllowRememberConsent = request.Client.AllowRememberConsent,
			IdentityScopes = request.ValidatedResources.Resources.IdentityResources
				.Select(x => CreateScopeViewModel(x, model?.ScopesConsented == null || model.ScopesConsented?.Contains(x.Name) == true))
				.ToArray(),
		};

		var resourceIndicators = request.Parameters.GetValues(OidcConstants.AuthorizeRequest.Resource) ?? Enumerable.Empty<string>();
		var apiResources = request.ValidatedResources.Resources.ApiResources.Where(x => resourceIndicators.Contains(x.Name));

		var apiScopes = new List<ScopeViewModel>();
		foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
		{
			var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
			if (apiScope != null)
			{
				var scopeVm = CreateScopeViewModel(parsedScope, apiScope, model == null || model.ScopesConsented?.Contains(parsedScope.RawValue) == true);
				scopeVm.Resources = apiResources.Where(x => x.Scopes.Contains(parsedScope.ParsedName))
					.Select(x => new ResourceViewModel
					{
						Name = x.Name,
						DisplayName = x.DisplayName ?? x.Name,
					}).ToArray();
				apiScopes.Add(scopeVm);
			}
		}

		if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
		{
			apiScopes.Add(GetOfflineAccessScope(model == null || model.ScopesConsented?.Contains(Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess) == true));
		}

		vm.ApiScopes = apiScopes;

		return vm;
	}

	private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
	{
		return new ScopeViewModel
		{
			Name = identity.Name,
			Value = identity.Name,
			DisplayName = identity.DisplayName ?? identity.Name,
			Description = identity.Description,
			Emphasize = identity.Emphasize,
			Required = identity.Required,
			Checked = check || identity.Required,
		};
	}

	public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
	{
		var displayName = apiScope.DisplayName ?? apiScope.Name;
		if (!String.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
		{
			displayName += ":" + parsedScopeValue.ParsedParameter;
		}

		return new ScopeViewModel
		{
			Name = parsedScopeValue.ParsedName,
			Value = parsedScopeValue.RawValue,
			DisplayName = displayName,
			Description = apiScope.Description,
			Emphasize = apiScope.Emphasize,
			Required = apiScope.Required,
			Checked = check || apiScope.Required,
		};
	}

	private static ScopeViewModel GetOfflineAccessScope(bool check)
	{
		return new ScopeViewModel
		{
			Value = Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess,
			DisplayName = ConsentOptions.OfflineAccessDisplayName,
			Description = ConsentOptions.OfflineAccessDescription,
			Emphasize = true,
			Checked = check,
		};
	}
}
