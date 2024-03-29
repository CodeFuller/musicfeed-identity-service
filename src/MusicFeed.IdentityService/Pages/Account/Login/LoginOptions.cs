namespace MusicFeed.IdentityService.Pages.Account.Login;

public static class LoginOptions
{
	public static bool AllowRememberLogin => true;

	public static TimeSpan RememberMeLoginDuration => TimeSpan.FromDays(30);

	public static string InvalidCredentialsErrorMessage => "Invalid username or password";
}
