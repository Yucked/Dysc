using System.Text.Json.Serialization;
using Dysc.Interfaces;

namespace Dysc.Providers.YouTube.Entities {
	internal class YouTubeVideo : ITrackResult {
		long ITrackResult.Duration
			=> LengthInSeconds * 1000;

		ISourceAuthor ITrackResult.Author
			=> new YouTubeAuthor {
				Id = UserId,
				Name = AuthorName
			};

		[JsonPropertyName("encrypted_id")]
		public string Id { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonIgnore]
		public string Url
			=> $"https://www.youtube.com/watch?v={Id}";


		[JsonIgnore]
		public bool IsStreamable
			=> true;

		[JsonIgnore]
		public string ArtworkUrl
			=> $"https://img.youtube.com/vi/{Id}/maxresdefault.jpg";

		public string StreamUrl { get; set; }

            [JsonPropertyName("author")]
		public string AuthorName { get; set; }

		[JsonPropertyName("user_id")]
		public string UserId { get; set; }

		[JsonPropertyName("length_seconds")]
		public long LengthInSeconds { get; set; }
	}
}