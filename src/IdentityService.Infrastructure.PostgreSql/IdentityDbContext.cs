using IdentityService.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.PostgreSql;

public class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
	public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
		: base(options)
	{
	}
}
