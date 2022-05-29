// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace MusicFeed.IdentityService.Pages.Account.Login;

public class ViewModel
{
	public bool AllowRememberLogin { get; set; } = true;

	public bool EnableLocalLogin { get; set; } = true;

	public IEnumerable<ViewModel.ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();

	public IEnumerable<ViewModel.ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

	public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;

	public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

#pragma warning disable CA1034 // Nested types should not be visible
	public class ExternalProvider
#pragma warning restore CA1034 // Nested types should not be visible
	{
		public string DisplayName { get; set; }

		public string AuthenticationScheme { get; set; }
	}
}
