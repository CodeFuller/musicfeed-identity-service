using DemoApi.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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
				Token = Request.Headers[HeaderNames.Authorization]
					.ToString()
					.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase),

				Claims = User.Claims.Select(c => new ClaimDataContract
				{
					Type = c.Type,
					Value = c.Value,
				}).ToList(),
			};
		}
	}
}
