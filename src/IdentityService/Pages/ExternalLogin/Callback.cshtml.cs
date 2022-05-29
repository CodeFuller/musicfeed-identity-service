using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
	private readonly UserManager<ApplicationUser> userManager;

	private readonly SignInManager<ApplicationUser> signInManager;

	private readonly IIdentityServerInteractionService interaction;

	private readonly IEventService events;

	private readonly ILogger<Callback> logger;

	public Callback(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
		IIdentityServerInteractionService interaction, IEventService events, ILogger<Callback> logger)
	{
		this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
		this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
		this.interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
		this.events = events ?? throw new ArgumentNullException(nameof(events));
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
		var user = await userManager.FindByLoginAsync(provider, providerUserId);
		if (user == null)
		{
			// This might be where you might initiate a custom workflow for user registration.
			// In this sample we don't show how that would be done, as our sample implementation.
			// Simply auto-provisions new external user.
			user = await AutoProvisionUserAsync(provider, providerUserId, externalUser.Claims.ToList());
		}

		// This allows us to collect any additional claims or properties.
		// For the specific protocols used and store them in the local auth cookie.
		// This is typically used to store data needed for sign-out from those protocols.
		var additionalLocalClaims = new List<Claim>();
		var localSignInProps = new AuthenticationProperties();
		CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

		// Issue authentication cookie for user.
		await signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

		// Delete temporary cookie used during external authentication.
		await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

		// Retrieve return URL.
		var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

		// Check if external login is in the context of an OIDC request.
		var context = await interaction.GetAuthorizationContextAsync(returnUrl);
		await events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName, true, context?.Client.ClientId));

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

	private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IReadOnlyCollection<Claim> claims)
	{
		var sub = Guid.NewGuid().ToString();

		var user = new ApplicationUser
		{
			Id = sub,

			// Don't need a username, since the user will be using an external provider to login.
			UserName = sub,
		};

		// Email.
		var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
		            claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
		if (email != null)
		{
			user.Email = email;
		}

		// Create a list of claims that we want to transfer into our store.
		var filtered = new List<Claim>();

		// User's display name.
		var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
		           claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
		if (name != null)
		{
			filtered.Add(new Claim(JwtClaimTypes.Name, name));
		}
		else
		{
			var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
			            claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
			var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
			           claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
			if (first != null && last != null)
			{
				filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
			}
			else if (first != null)
			{
				filtered.Add(new Claim(JwtClaimTypes.Name, first));
			}
			else if (last != null)
			{
				filtered.Add(new Claim(JwtClaimTypes.Name, last));
			}
		}

		var identityResult = await userManager.CreateAsync(user);
		if (!identityResult.Succeeded)
		{
			throw new InvalidOperationException(identityResult.Errors.First().Description);
		}

		if (filtered.Any())
		{
			identityResult = await userManager.AddClaimsAsync(user, filtered);
			if (!identityResult.Succeeded)
			{
				throw new InvalidOperationException(identityResult.Errors.First().Description);
			}
		}

		identityResult = await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
		if (!identityResult.Succeeded)
		{
			throw new InvalidOperationException(identityResult.Errors.First().Description);
		}

		return user;
	}

	// If the external login is OIDC-based, there are certain things we need to preserve to make logout work.
	// This will be different for WS-Fed, SAML2p or other protocols
	private static void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
	{
		// Capture the idp used to login, so the session knows where the user came from.
		localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties.Items["scheme"]));

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
