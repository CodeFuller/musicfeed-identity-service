using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql.Migrations
{
	public class CustomPersistedGrantDbContext : PersistedGrantDbContext<CustomPersistedGrantDbContext>
	{
		public CustomPersistedGrantDbContext(DbContextOptions<CustomPersistedGrantDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("identity_server");

			base.OnModelCreating(modelBuilder);
		}
	}
}
