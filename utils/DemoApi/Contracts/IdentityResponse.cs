namespace DemoApi.Contracts
{
	public class IdentityResponse
	{
		public string Token { get; set; }

		public IReadOnlyCollection<ClaimDataContract> Claims { get; set; }
	}
}
