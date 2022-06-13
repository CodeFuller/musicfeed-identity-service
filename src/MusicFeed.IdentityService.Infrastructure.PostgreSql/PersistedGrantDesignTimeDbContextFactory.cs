using Microsoft.Extensions.DependencyInjection;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql
{
	internal class PersistedGrantDesignTimeDbContextFactory : DesignTimeDbContextFactory<CustomPersistedGrantDbContext>
	{
		protected override CustomPersistedGrantDbContext CreateDbContext(string connectionString)
		{
			var services = new ServiceCollection();

			// PersistedGrantDbContext creates instance of OperationalStoreOptions from IServiceProvider.
			// Without calling AddOperationalStore(), the command 'dotnet ef migrations add' will fail.
			var identityServerBuilder = services.AddIdentityServer();

			if (String.IsNullOrEmpty(connectionString))
			{
				identityServerBuilder.AddPostgreSqlDalForPersistedGrantStore();
			}
			else
			{
				identityServerBuilder.AddPostgreSqlDalForPersistedGrantStore(connectionString);
			}

			var serviceProvider = services.BuildServiceProvider();
			return serviceProvider.GetRequiredService<CustomPersistedGrantDbContext>();
		}
	}
}
