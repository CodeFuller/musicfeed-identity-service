using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MusicFeed.IdentityService.Stub
{
	[DataContract]
	public class IssueTokenRequest
	{
		[Required]
		[DataMember]
		public string UserId { get; set; }
	}
}
