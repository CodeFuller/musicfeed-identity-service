using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoWebClient.Pages
{
	public class SignOutModel : PageModel
	{
		public IActionResult OnGet()
		{
			return SignOut("Cookies", "oidc");
		}
	}
}
