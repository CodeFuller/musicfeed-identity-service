using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.PostgreSql;

public static class PostgreSqlServiceCollectionExtensions
{
	public static IServiceCollection AddPostgreSqlDal(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionString));

		return services;
	}
}
