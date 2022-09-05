using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicFeed.IdentityService.Infrastructure.PostgreSql.Migrations;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql;

public static class PostgreSqlServiceCollectionExtensions
{
	public static IServiceCollection AddPostgreSqlDalForIdentityDb(this IServiceCollection services, Func<IServiceProvider, string> connectionStringFactory)
	{
		services.AddDbContext<CustomIdentityDbContext>((serviceProvider, options) =>
		{
			options.UseNpgsql(connectionStringFactory(serviceProvider));
		});

		return services;
	}
}
