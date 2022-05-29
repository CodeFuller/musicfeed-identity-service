using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityService.Infrastructure.PostgreSql;

internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
	public IdentityDbContext CreateDbContext(string[] args)
	{
		if (args.Length > 1)
		{
			Console.Error.WriteLine("Usage:");
			Console.Error.WriteLine("dotnet ef migrations add InitialCreate -- \"<Connection String>\"");
			Console.Error.WriteLine("dotnet ef database update -- \"<Connection String>\"");
			Environment.Exit(1);
		}

		var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

		if (args.Length == 1)
		{
			var connectionString = args.Single();
			optionsBuilder.UseNpgsql(connectionString);
		}
		else
		{
			optionsBuilder.UseNpgsql();
		}

		return new IdentityDbContext(optionsBuilder.Options);
	}
}
