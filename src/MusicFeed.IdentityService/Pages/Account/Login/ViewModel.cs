namespace MusicFeed.IdentityService.Pages.Account.Login;

public class ViewModel
{
	public bool AllowRememberLogin { get; set; } = true;

	public IEnumerable<ViewModel.ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();

	public IEnumerable<ViewModel.ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

	public string ExternalLoginScheme => ExternalProviders.Single().AuthenticationScheme;

#pragma warning disable CA1034 // Nested types should not be visible
	public class ExternalProvider
#pragma warning restore CA1034 // Nested types should not be visible
	{
		public string DisplayName { get; set; }

		public string AuthenticationScheme { get; set; }
	}
}
