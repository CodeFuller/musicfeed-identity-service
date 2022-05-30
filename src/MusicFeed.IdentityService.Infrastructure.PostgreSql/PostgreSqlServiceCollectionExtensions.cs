using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MusicFeed.IdentityService.Infrastructure.PostgreSql;

public static class PostgreSqlServiceCollectionExtensions
{
	public static IServiceCollection AddPostgreSqlDal(this IServiceCollection services, Func<IServiceProvider, string> connectionStringFactory)
	{
		services.AddDbContext<IdentityDbContext>((serviceProvider, options) =>
		{
			options.UseNpgsql(connectionStringFactory(serviceProvider));
		});

		return services;
	}
}
