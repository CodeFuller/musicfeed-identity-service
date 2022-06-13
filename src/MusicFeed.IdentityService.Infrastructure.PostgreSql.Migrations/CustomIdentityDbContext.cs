using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicFeed.IdentityService.Abstractions;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql.Migrations;

public class CustomIdentityDbContext : IdentityDbContext<ApplicationUser>
{
	public CustomIdentityDbContext(DbContextOptions<CustomIdentityDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.HasDefaultSchema("identity");

		base.OnModelCreating(builder);
	}
}
