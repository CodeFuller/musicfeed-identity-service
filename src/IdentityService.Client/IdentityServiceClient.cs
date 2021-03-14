using IdentityService.Client;

namespace IdentityService.Grpc
{
#pragma warning disable CA1724 // Type names should not match namespaces
	public static partial class IdentityService
#pragma warning restore CA1724 // Type names should not match namespaces
	{
#pragma warning disable CA1034 // Nested types should not be visible
		public partial class IdentityServiceClient : IIdentityServiceClient
#pragma warning restore CA1034 // Nested types should not be visible
		{
		}
	}
}
