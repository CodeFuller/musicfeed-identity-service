using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Migrations
{
	// To update migrations, run the following command from src/IdentityService folder:
	//   dotnet ef migrations add InitialCreate --project ../IdentityService.Migrations
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}
}
