using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MusicFeed.IdentityService.Abstractions;
using MusicFeed.IdentityService.Settings;

namespace MusicFeed.IdentityService.Stub
{
	[ApiController]
	[Route("stub/token")]
	public class StubTokenController : ControllerBase
	{
		private readonly ITokenService tokenService;

		private readonly IUserClaimsPrincipalFactory<ApplicationUser> principalFactory;

		private readonly IdentityServerSettings identityServerSettings;

		private readonly IdentityServerOptions options;

		public StubTokenController(ITokenService tokenService, IUserClaimsPrincipalFactory<ApplicationUser> principalFactory,
			IOptions<IdentityServerSettings> identityServerSettings, IdentityServerOptions options)
		{
			this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
			this.principalFactory = principalFactory ?? throw new ArgumentNullException(nameof(principalFactory));
			this.identityServerSettings = identityServerSettings?.Value ?? throw new ArgumentNullException(nameof(identityServerSettings));
			this.options = options ?? throw new ArgumentNullException(nameof(options));
		}

		// https://stackoverflow.com/a/44322425/5740031
		[HttpPost]
		public async Task<string> IssueToken(IssueTokenRequest request)
		{
			var user = new ApplicationUser
			{
				Id = request.UserId,
				UserName = "Test User",
			};

			var principal = await principalFactory.CreateAsync(user);

			var identityUser = new IdentityServerUser(user.Id)
			{
				AdditionalClaims = principal.Claims.ToArray(),
				DisplayName = user.UserName,
				AuthenticationTime = DateTime.UtcNow,
				IdentityProvider = IdentityServerConstants.LocalIdentityProvider,
			};

			var subject = identityUser.CreatePrincipal();

			var tokenCreationRequest = new TokenCreationRequest
			{
				Subject = subject,
				IncludeAllIdentityClaims = true,
				ValidatedRequest = new ValidatedRequest
				{
					Subject = subject,
					Options = options,
					ClientClaims = identityUser.AdditionalClaims,
				},

				ValidatedResources = new ResourceValidationResult(new Resources(Config.IdentityResources, Enumerable.Empty<ApiResource>(), Config.ApiScopes)),
			};

			tokenCreationRequest.ValidatedRequest.SetClient(identityServerSettings.Clients.Single());

			var token = await tokenService.CreateAccessTokenAsync(tokenCreationRequest);

			token.Issuer = options.IssuerUri;
			token.Audiences = new[]
			{
				new Uri(new Uri(options.IssuerUri), "resources").OriginalString,
			};

			var tokenValue = await tokenService.CreateSecurityTokenAsync(token);

			return tokenValue;
		}
	}
}
