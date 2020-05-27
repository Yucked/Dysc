using System.Text.Json.Serialization;
using Dysc.Infos;

namespace Dysc.Providers.SoundCloud.Entities {
	internal sealed class SoundCloudUser {
		[JsonIgnore]
		public AuthorInfo ToAuthorInfo
			=> new AuthorInfo {
				Name = Username,
				Url = PermalinkUrl,
				AvatarUrl = AvatarUrl
			};

		[JsonPropertyName("avatar_url")]
		public string AvatarUrl { get; set; }

		[JsonPropertyName("permalink_url")]
		public string PermalinkUrl { get; set; }

		[JsonPropertyName("username")]
		public string Username { get; set; }
	}
}