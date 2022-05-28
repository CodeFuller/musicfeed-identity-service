using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
	private readonly IIdentityServerInteractionService interaction;

	private readonly IEventService events;

	private readonly TestUserStore userStore;

	private readonly ILogger<Callback> logger;

	public Callback(IIdentityServerInteractionService interaction, IEventService events, TestUserStore userStore, ILogger<Callback> logger)
	{
		this.interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
		this.events = events ?? throw new ArgumentNullException(nameof(events));

		// TODO: Replace with ASP.NET Core Identity.
		this.userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
		this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<IActionResult> OnGet()
	{
		// Read external identity from the temporary cookie
		var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
		if (result.Succeeded != true)
		{
			throw new InvalidOperationException("External authentication error");
		}

		var externalUser = result.Principal;

		if (logger.IsEnabled(LogLevel.Debug))
		{
			var externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
			logger.LogDebug("External claims: {@claims}", externalClaims);
		}

		// Lookup our user and external provider info.
		// Try to determine the unique id of the external user (issued by the provider).
		// The most common claim type for that are the sub claim and the NameIdentifier.
		// Depending on the external provider, some other claim type might be used.
		var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
						  externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
						  throw new InvalidOperationException("Unknown userid");

		var provider = result.Properties.Items["scheme"];
		var providerUserId = userIdClaim.Value;

		// Find external user.
		var user = userStore.FindByExternalProvider(provider, providerUserId);
		if (user == null)
		{
			// This might be where you might initiate a custom workflow for user registration.
			// In this sample we don't show how that would be done, as our sample implementation.
			// Simply auto-provisions new external user.
			//
			// Remove the user id claim so we don't include it as an extra claim if/when we provision the user.
			var claims = externalUser.Claims.ToList();
			claims.Remove(userIdClaim);
			user = userStore.AutoProvisionUser(provider, providerUserId, claims.ToList());
		}

		// This allows us to collect any additional claims or properties.
		// For the specific protocols used and store them in the local auth cookie.
		// This is typically used to store data needed for sign-out from those protocols.
		var additionalLocalClaims = new List<Claim>();
		var localSignInProps = new AuthenticationProperties();
		CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

		// Issue authentication cookie for user.
		var identityServerUser = new IdentityServerUser(user.SubjectId)
		{
			DisplayName = user.Username,
			IdentityProvider = provider,
			AdditionalClaims = additionalLocalClaims,
		};

		await HttpContext.SignInAsync(identityServerUser, localSignInProps);

		// Delete temporary cookie used during external authentication.
		await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

		// Retrieve return URL.
		var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

		// Check if external login is in the context of an OIDC request.
		var context = await interaction.GetAuthorizationContextAsync(returnUrl);
		await events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.SubjectId, user.Username, true, context?.Client.ClientId));

		if (context != null)
		{
			if (context.IsNativeClient())
			{
				// The client is native, so this change in how to return the response is for better UX for the end user.
				return this.LoadingPage(returnUrl);
			}
		}

		return Redirect(returnUrl);
	}

	// If the external login is OIDC-based, there are certain things we need to preserve to make logout work.
	// This will be different for WS-Fed, SAML2p or other protocols
	private static void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
	{
		// If the external system sent a session id claim, copy it over so we can use it for single sign-out.
		var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
		if (sid != null)
		{
			localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
		}

		// If the external provider issued an id_token, we'll keep it for sign-out.
		var idToken = externalResult.Properties.GetTokenValue("id_token");
		if (idToken != null)
		{
			localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
		}
	}
}
