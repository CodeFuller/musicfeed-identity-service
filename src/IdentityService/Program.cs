using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace IdentityService
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			await CreateHostBuilder(args).Build().RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostContext, builder) =>
				{
					if (hostContext.HostingEnvironment.IsDevelopment())
					{
						builder.AddUserSecrets<Startup>();
					}
				})
				.UseSerilog((hostingContext, loggerConfiguration) =>
				{
					loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
