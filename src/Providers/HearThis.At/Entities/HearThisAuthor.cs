using System.Text.Json.Serialization;
using Dysc.Infos;

namespace Dysc.Providers.HearThis.At.Entities {
	internal struct HearThisAuthor {
		[JsonPropertyName("username")]
		public string Username { get; set; }

		[JsonPropertyName("permalink_url")]
		public string PermalinkUrl { get; set; }

		[JsonPropertyName("avatar_url")]
		public string AvatarUrl { get; set; }

		public AuthorInfo ToAuthorInfo
			=> new AuthorInfo {
				Name = Username,
				AvatarUrl = AvatarUrl,
				Url = PermalinkUrl
			};
	}
}