using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityService.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Account.Login;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
	private readonly UserManager<ApplicationUser> userManager;

	private readonly SignInManager<ApplicationUser> signInManager;

	private readonly IIdentityServerInteractionService interaction;

	private readonly IAuthenticationSchemeProvider schemeProvider;

	private readonly IIdentityProviderStore identityProviderStore;

	private readonly IEventService events;

	public ViewModel View { get; set; }

	[BindProperty]
	public InputModel Input { get; set; }

	public Index(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
		IIdentityServerInteractionService interaction, IAuthenticationSchemeProvider schemeProvider,
		IIdentityProviderStore identityProviderStore, IEventService events)
	{
		this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
		this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
		this.interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
		this.schemeProvider = schemeProvider ?? throw new ArgumentNullException(nameof(schemeProvider));
		this.identityProviderStore = identityProviderStore ?? throw new ArgumentNullException(nameof(identityProviderStore));
		this.events = events ?? throw new ArgumentNullException(nameof(events));
	}

#pragma warning disable CA1054 // URI-like parameters should not be strings
	public async Task<IActionResult> OnGet(string returnUrl)
#pragma warning restore CA1054 // URI-like parameters should not be strings
	{
		await BuildModelAsync(returnUrl);

		if (View.IsExternalLoginOnly)
		{
			// We only have one option for logging in and it's an external provider.
			return RedirectToPage("/ExternalLogin/Challenge", new { scheme = View.ExternalLoginScheme, returnUrl });
		}

		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		// Check if we are in the context of an authorization request.
		var context = await interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

		// The user clicked the "cancel" button.
		if (Input.Button != "login")
		{
			if (context != null)
			{
				// If the user cancels, send a result back into IdentityServer as if they
				// denied the consent (even if this client does not require consent).
				// This will send back an access denied OIDC error response to the client.
				await interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

				// We can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null.
				if (context.IsNativeClient())
				{
					// The client is native, so this change in how to return the response is for better UX for the end user.
					return this.LoadingPage(Input.ReturnUrl);
				}

				return Redirect(Input.ReturnUrl);
			}

			// Since we don't have a valid context, then we just go back to the home page.
			return Redirect("~/");
		}

		if (ModelState.IsValid)
		{
			var result = await signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberLogin, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				var user = await userManager.FindByNameAsync(Input.Username);
				await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

				if (context != null)
				{
					if (context.IsNativeClient())
					{
						// The client is native, so this change in how to return the response is for better UX for the end user.
						return this.LoadingPage(Input.ReturnUrl);
					}

					// We can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null.
					return Redirect(Input.ReturnUrl);
				}

				// Request for a local page.
				if (Url.IsLocalUrl(Input.ReturnUrl))
				{
					return Redirect(Input.ReturnUrl);
				}

				if (string.IsNullOrEmpty(Input.ReturnUrl))
				{
					return Redirect("~/");
				}

				// User might have clicked on a malicious link - should be logged.
				throw new InvalidOperationException("Invalid return URL");
			}

			await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
			ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
		}

		// Something went wrong, show form with error.
		await BuildModelAsync(Input.ReturnUrl);
		return Page();
	}

	private async Task BuildModelAsync(string returnUrl)
	{
		Input = new InputModel
		{
			ReturnUrl = returnUrl,
		};

		var context = await interaction.GetAuthorizationContextAsync(returnUrl);
		if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP) != null)
		{
			var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

			// This is meant to short circuit the UI and only trigger the one external IdP.
			View = new ViewModel
			{
				EnableLocalLogin = local,
			};

			Input.Username = context?.LoginHint;

			if (!local)
			{
				View.ExternalProviders = new[] { new ViewModel.ExternalProvider { AuthenticationScheme = context.IdP } };
			}

			return;
		}

		var schemes = await schemeProvider.GetAllSchemesAsync();

		var providers = schemes
			.Where(x => x.DisplayName != null)
			.Select(x => new ViewModel.ExternalProvider
			{
				DisplayName = x.DisplayName ?? x.Name,
				AuthenticationScheme = x.Name,
			}).ToList();

		var dynamicSchemes = (await identityProviderStore.GetAllSchemeNamesAsync())
			.Where(x => x.Enabled)
			.Select(x => new ViewModel.ExternalProvider
			{
				AuthenticationScheme = x.Scheme,
				DisplayName = x.DisplayName,
			});

		providers.AddRange(dynamicSchemes);

		var allowLocal = true;
		var client = context?.Client;
		if (client != null)
		{
			allowLocal = client.EnableLocalLogin;
			if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
			{
				providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
			}
		}

		View = new ViewModel
		{
			AllowRememberLogin = LoginOptions.AllowRememberLogin,
			EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
			ExternalProviders = providers.ToArray(),
		};
	}
}
