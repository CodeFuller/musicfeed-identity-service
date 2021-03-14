using System;
using System.Net.Http;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Client
{
	/// <summary>
	/// Extension methods for adding client to IdentityService.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds services required for client to IdentityService.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
		/// <param name="configureClient">A delegate that is used to configure the gRPC client.</param>
		/// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
		public static IServiceCollection AddIdentityServiceClient(this IServiceCollection services, Action<GrpcClientFactoryOptions> configureClient)
		{
			services.RegisterGrpcClient<IIdentityServiceClient, Grpc.IdentityService.IdentityServiceClient>(configureClient);

			return services;
		}

		private static void RegisterGrpcClient<TClientInterface, TClientImplementation>(this IServiceCollection services, Action<GrpcClientFactoryOptions> configureClient)
			where TClientInterface : class
			where TClientImplementation : class, TClientInterface
		{
			services.AddGrpcClient<TClientImplementation>(configureClient)

				// By default AddGrpcClient will use SocketsHttpHandler - https://github.com/grpc/grpc-dotnet/blob/08024e350d39394db6982f65528fb2e3653c7666/src/Shared/HttpHandlerFactory.cs#L27
				// This will break dependency detection by Application Insights.
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

			services.AddSingleton<TClientInterface, TClientImplementation>(sp => sp.GetRequiredService<TClientImplementation>());
		}
	}
}
