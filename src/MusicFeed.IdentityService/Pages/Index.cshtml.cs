using System.Reflection;
using Duende.IdentityServer.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFeed.IdentityService.Pages;

[AllowAnonymous]
public class Index : PageModel
{
	public string Version { get; set; }

	public void OnGet()
	{
		Version = typeof(IdentityServerMiddleware).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').First();
	}
}
