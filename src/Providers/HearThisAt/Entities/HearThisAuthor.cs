using System.Text.Json.Serialization;
using Dysc.Interfaces;

namespace Dysc.Providers.HearThisAt.Entities {
	internal struct HearThisAuthor : ISourceAuthor {
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("username")]
		public string Name { get; set; }

		[JsonPropertyName("permalink_url")]
		public string Url { get; set; }

		[JsonPropertyName("avatar_url")]
		public string AvatarUrl { get; set; }
	}
}