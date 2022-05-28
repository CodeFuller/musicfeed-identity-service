using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;

namespace IdentityService.Internal
{
	internal class ProfileService : IProfileService
	{
		private readonly TestUserStore userStore;

		public ProfileService(TestUserStore userStore)
		{
			this.userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
		}

		public Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var user = userStore.FindBySubjectId(context.Subject.GetSubjectId());

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.SubjectId),
			};

			context.IssuedClaims.AddRange(claims);

			return Task.CompletedTask;
		}

		public Task IsActiveAsync(IsActiveContext context)
		{
			context.IsActive = true;

			return Task.CompletedTask;
		}
	}
}
