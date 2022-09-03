using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql.Migrations
{
	public class CustomPersistedGrantDbContext : PersistedGrantDbContext<CustomPersistedGrantDbContext>, IDataProtectionKeyContext
	{
		public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

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
