using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.ServerSideSessions
{
	public class IndexModel : PageModel
	{
		private readonly ISessionManagementService sessionManagementService;

		public QueryResult<UserSession> UserSessions { get; set; }

		[BindProperty(SupportsGet = true)]
		public string Filter { get; set; }

		[BindProperty(SupportsGet = true)]
		public string Token { get; set; }

		[BindProperty(SupportsGet = true)]
		public string Prev { get; set; }

		[BindProperty]
		public string SessionId { get; set; }

		public IndexModel(ISessionManagementService sessionManagementService = null)
		{
			this.sessionManagementService = sessionManagementService;
		}

		public async Task OnGet()
		{
			if (sessionManagementService != null)
			{
				UserSessions = await sessionManagementService.QuerySessionsAsync(new SessionQuery
				{
					ResultsToken = Token,
					RequestPriorResults = Prev == "true",
					DisplayName = Filter,
					SessionId = Filter,
					SubjectId = Filter,
				});
			}
		}

		public async Task<IActionResult> OnPost()
		{
			await sessionManagementService.RemoveSessionsAsync(new RemoveSessionsContext
			{
				SessionId = SessionId,
			});

			return RedirectToPage("/ServerSideSessions/Index", new { Token, Filter, Prev });
		}
	}
}
