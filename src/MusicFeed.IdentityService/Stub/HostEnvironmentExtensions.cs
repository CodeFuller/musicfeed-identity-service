namespace MusicFeed.IdentityService.Stub
{
	internal static class HostEnvironmentExtensions
	{
		public static bool IsStub(this IHostEnvironment hostEnvironment)
		{
			return hostEnvironment.IsEnvironment("Stub");
		}
	}
}
