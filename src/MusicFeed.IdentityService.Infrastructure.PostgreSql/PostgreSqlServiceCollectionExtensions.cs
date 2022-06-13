using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql;

public static class PostgreSqlServiceCollectionExtensions
{
	public static IServiceCollection AddPostgreSqlDal(this IServiceCollection services, Func<IServiceProvider, string> connectionStringFactory)
	{
		services.AddDbContext<CustomIdentityDbContext>((serviceProvider, options) =>
		{
			options.UseNpgsql(connectionStringFactory(serviceProvider));
		});

		return services;
	}

	public static IIdentityServerBuilder AddPostgreSqlDalForPersistedGrantStore(this IIdentityServerBuilder identityServerBuilder, string connectionString)
	{
		if (String.IsNullOrEmpty(connectionString))
		{
			throw new InvalidOperationException("Connection string is empty");
		}

		return identityServerBuilder.AddPostgreSqlDalForPersistedGrantStore(dbContextOptionsBuilder =>
		{
			dbContextOptionsBuilder.UseNpgsql(connectionString, GetNpgsqlOptionsAction());
		});
	}

	internal static IIdentityServerBuilder AddPostgreSqlDalForPersistedGrantStore(this IIdentityServerBuilder identityServerBuilder)
	{
		return identityServerBuilder.AddPostgreSqlDalForPersistedGrantStore(dbContextOptionsBuilder =>
		{
			dbContextOptionsBuilder.UseNpgsql(GetNpgsqlOptionsAction());
		});
	}

	private static IIdentityServerBuilder AddPostgreSqlDalForPersistedGrantStore(this IIdentityServerBuilder identityServerBuilder, Action<DbContextOptionsBuilder> configureDbContext)
	{
		return identityServerBuilder.AddOperationalStore<CustomPersistedGrantDbContext>(options =>
		{
			options.ConfigureDbContext = configureDbContext;
			options.EnableTokenCleanup = true;
		});
	}

	private static Action<NpgsqlDbContextOptionsBuilder> GetNpgsqlOptionsAction()
	{
		var migrationsAssembly = Assembly.GetExecutingAssembly();
		return npgsqlDbContextOptionsBuilder => npgsqlDbContextOptionsBuilder.MigrationsAssembly(migrationsAssembly.GetName().Name);
	}
}
