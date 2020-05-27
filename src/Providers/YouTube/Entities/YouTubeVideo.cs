using System.Text.Json.Serialization;
using Dysc.Infos;

namespace Dysc.Providers.YouTube.Entities {
	internal sealed class YouTubeVideo : YouTubeResult {
		[JsonIgnore]
		public TrackInfo ToTrackInfo
			=> new TrackInfo {
				Id = Id,
				Author = new AuthorInfo {
					Name = Author
				},
				Title = Title,
				Duration = Duration * 1000,
				Provider = ProviderType.YouTube,
				Url = $"https://www.youtube.com/watch?v={Id}",
				ArtworkUrl = $"https://img.youtube.com/vi/{Id}/maxresdefault.jpg"
			};

		[JsonPropertyName("length_seconds")]
		public long Duration { get; set; }
	}
}