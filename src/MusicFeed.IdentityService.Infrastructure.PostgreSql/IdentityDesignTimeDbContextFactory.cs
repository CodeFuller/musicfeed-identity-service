using Microsoft.EntityFrameworkCore;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql
{
	internal class IdentityDesignTimeDbContextFactory : DesignTimeDbContextFactory<CustomIdentityDbContext>
	{
		protected override CustomIdentityDbContext CreateDbContext(string connectionString)
		{
			var optionsBuilder = new DbContextOptionsBuilder<CustomIdentityDbContext>();

			if (String.IsNullOrEmpty(connectionString))
			{
				optionsBuilder.UseNpgsql();
			}
			else
			{
				optionsBuilder.UseNpgsql(connectionString);
			}

			return new CustomIdentityDbContext(optionsBuilder.Options);
		}
	}
}
