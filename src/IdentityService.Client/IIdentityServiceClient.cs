using System;
using System.Threading;
using IdentityService.Grpc;

using grpc = Grpc.Core;

namespace IdentityService.Client
{
	/// <summary>
	/// Client for IdentityService.
	/// </summary>
	public interface IIdentityServiceClient
	{
		/// <summary>
		/// Registers new user.
		/// </summary>
		/// <param name="request">The request to send to the server.</param>
		/// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
		/// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
		/// <param name="cancellationToken">An optional token for canceling the call.</param>
		/// <returns>The call object.</returns>
		public grpc::AsyncUnaryCall<RegisterUserReply> RegisterUserAsync(RegisterUserRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Checks user credentials.
		/// </summary>
		/// <param name="request">The request to send to the server.</param>
		/// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
		/// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
		/// <param name="cancellationToken">An optional token for canceling the call.</param>
		/// <returns>The call object.</returns>
		public grpc::AsyncUnaryCall<CheckUserReply> CheckUserAsync(CheckUserRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
	}
}
