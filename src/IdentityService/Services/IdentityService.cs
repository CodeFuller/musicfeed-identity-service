using System;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityService.Grpc;
using Microsoft.Extensions.Logging;

namespace IdentityService.Services
{
	internal class IdentityService : Grpc.IdentityService.IdentityServiceBase
	{
		private readonly ILogger<IdentityService> logger;

		public IdentityService(ILogger<IdentityService> logger)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override Task<RegisterUserReply> RegisterUser(RegisterUserRequest request, ServerCallContext context)
		{
			logger.LogInformation("Registering new user {UserName} ...", request.Email);

			return Task.FromResult(new RegisterUserReply
			{
				UserId = "Test",
			});
		}
	}
}
