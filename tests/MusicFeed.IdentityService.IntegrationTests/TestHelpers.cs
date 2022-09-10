using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MusicFeed.IdentityService.IntegrationTests
{
	internal class TestHelpers
	{
		private const string AvailablePostgresConnectionString = "Server=localhost;Port=15432;Database=IdentityDB;User Id=postgres;Password=Qwerty123;";
		private const string UnavailablePostgresConnectionString = "Server=localhost;Port=15433;Database=IdentityDB;User Id=postgres;Password=Qwerty123;";

		public static Action<IConfigurationBuilder> AvailablePostgresConfiguration => GetDbConfiguration(AvailablePostgresConnectionString);

		public static Action<IConfigurationBuilder> UnavailablePostgresConfiguration => GetDbConfiguration(UnavailablePostgresConnectionString);

		private static Action<IConfigurationBuilder> GetDbConfiguration(string postgresConnectionString)
		{
			return configBuilder => configBuilder
				.AddInMemoryCollection(new[]
				{
					new KeyValuePair<string, string>("connectionStrings:identityDB", postgresConnectionString),
				});
		}
	}
}
