namespace DemoApi.Contracts
{
	public class IdentityResponse
	{
		public IReadOnlyCollection<ClaimDataContract> Claims { get; set; }
	}
}
