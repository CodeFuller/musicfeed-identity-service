using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityService.Grpc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityService.Services
{
	internal class IdentityService : Grpc.IdentityService.IdentityServiceBase
	{
		private readonly UserManager<IdentityUser> userManager;

		private readonly SignInManager<IdentityUser> signInManager;

		private readonly ILogger<IdentityService> logger;

		public IdentityService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<IdentityService> logger)
		{
			this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override async Task<RegisterUserReply> RegisterUser(RegisterUserRequest request, ServerCallContext context)
		{
			var newUserId = Guid.NewGuid().ToString("N");

			logger.LogInformation("Registering new user {UserName} with id {UserId} ...", request.Email, newUserId);

			var newUser = new IdentityUser
			{
				Id = newUserId,
				UserName = request.Email,
				Email = request.Email,
			};

			var result = await userManager.CreateAsync(newUser, request.Password);
			if (result.Succeeded)
			{
				logger.LogInformation("Successfully registered new user {UserName} with id {UserId}", request.Email, newUserId);

				return new RegisterUserReply
				{
					UserId = newUserId,
				};
			}

			logger.LogWarning("Failed to register user {UserName}. Errors: {@RegisterUserErrors}", request.Email, result.Errors);

			return new RegisterUserReply
			{
				UserId = String.Empty,
				Errors =
				{
					result.Errors.Select(error => new IdentityServiceError
					{
						ErrorCode = error.Code,
						ErrorDescription = error.Description,
					}),
				},
			};
		}
	}
}
