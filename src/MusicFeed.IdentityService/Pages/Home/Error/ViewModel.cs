using Duende.IdentityServer.Models;

namespace MusicFeed.IdentityService.Pages.Home.Error;

public class ViewModel
{
	public ErrorMessage Error { get; set; }

	public ViewModel()
	{
	}

	public ViewModel(string error)
	{
		Error = new ErrorMessage
		{
			Error = error,
		};
	}
}
