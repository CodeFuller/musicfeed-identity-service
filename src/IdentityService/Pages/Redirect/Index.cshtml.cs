using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Redirect;

[AllowAnonymous]
public class IndexModel : PageModel
{
#pragma warning disable CA1056 // URI-like properties should not be strings
	public string RedirectUri { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

#pragma warning disable CA1054 // URI-like parameters should not be strings
	public IActionResult OnGet(string redirectUri)
#pragma warning restore CA1054 // URI-like parameters should not be strings
	{
		if (!Url.IsLocalUrl(redirectUri))
		{
			return RedirectToPage("/Home/Error/Index");
		}

		RedirectUri = redirectUri;
		return Page();
	}
}
