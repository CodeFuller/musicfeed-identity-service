using DemoApi.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class IdentityController : ControllerBase
	{
		[HttpGet]
		public IdentityResponse GetIdentity()
		{
			return new IdentityResponse
			{
				Claims = User.Claims.Select(c => new ClaimDataContract
				{
					Type = c.Type,
					Value = c.Value,
				}).ToList(),
			};
		}
	}
}
