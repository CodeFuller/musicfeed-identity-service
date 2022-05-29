using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicFeed.IdentityService.Abstractions;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql;

public class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
	public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
		: base(options)
	{
	}
}
