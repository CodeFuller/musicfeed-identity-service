using System;
using IdentityService.Interfaces;
using IdentityService.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityService.Internal
{
	public class MigrationsApplier : IMigrationsApplier
	{
		private readonly ApplicationDbContext dbContext;

		private readonly ILogger<MigrationsApplier> logger;

		public MigrationsApplier(ApplicationDbContext dbContext, ILogger<MigrationsApplier> logger)
		{
			this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void ApplyMigrations()
		{
			logger.LogInformation("Migrating the database ...");

			dbContext.Database.Migrate();

			logger.LogInformation("The database was migrated successfully");
		}
	}
}
