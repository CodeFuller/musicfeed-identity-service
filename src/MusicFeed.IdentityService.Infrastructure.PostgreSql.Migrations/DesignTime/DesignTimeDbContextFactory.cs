using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql.Migrations.DesignTime;

internal abstract class DesignTimeDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>
	where TContext : DbContext
{
	protected abstract TContext CreateDbContext(string connectionString);

	public TContext CreateDbContext(string[] args)
	{
		if (args.Length > 1)
		{
			Console.Error.WriteLine("Usage:");
			Console.Error.WriteLine("dotnet ef migrations add <Migration Name> -- <Connection String>");
			Console.Error.WriteLine("dotnet ef database update -- <Connection String>");
			Environment.Exit(1);
		}

		return CreateDbContext(args.SingleOrDefault());
	}
}
