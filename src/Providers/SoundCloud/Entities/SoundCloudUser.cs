using System.Text.Json.Serialization;
using Dysc.Interfaces;

namespace Dysc.Providers.SoundCloud.Entities {
	internal class SoundCloudUser : ISourceAuthor {
		[JsonPropertyName("avatar_url")]
		public string AvatarUrl { get; set; }

		[JsonPropertyName("id")]
		public long RawId { get; set; }

		string ISourceAuthor.Id
			=> $"{RawId}";

		[JsonPropertyName("permalink_url")]
		public string Url { get; set; }

		[JsonPropertyName("username")]
		public string Name { get; set; }
	}
}