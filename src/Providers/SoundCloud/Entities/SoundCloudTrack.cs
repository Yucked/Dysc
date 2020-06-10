using System.Linq;
using System.Text.Json.Serialization;
using Dysc.Interfaces;

namespace Dysc.Providers.SoundCloud.Entities {
	internal class SoundCloudTrack : ITrackResult {
		[JsonIgnore]
		string ITrackResult.Id
			=> $"{RawId}";

		[JsonIgnore]
		string ITrackResult.StreamUrl {
			get {
				if (Media?.Transcodings == null || Media.Transcodings.Length == 0) {
					return string.Empty;
				}

				var opusTranscoding = Media.Transcodings.FirstOrDefault(x => x.Preset == "opus_0_0");
				return opusTranscoding != null ? opusTranscoding.Url : Media.Transcodings[0].Url;
			}
		}

		[JsonIgnore]
		ISourceAuthor ITrackResult.Author
			=> User;

		[JsonIgnore]
		long ITrackResult.Duration {
			get {
				if (FullDuration != 0) {
					return FullDuration;
				}

				return TempDuration != 0 ? TempDuration : 0;
			}
		}

		[JsonPropertyName("media")]
		public SoundCloudMedia Media { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("full_duration")]
		public long FullDuration { get; set; }

		[JsonPropertyName("duration")]
		public long TempDuration { get; set; }

		[JsonPropertyName("artwork_url")]
		public string ArtworkUrl { get; set; }

		[JsonPropertyName("streamable")]
		public bool IsStreamable { get; set; }

		[JsonPropertyName("id")]
		public long RawId { get; set; }

		[JsonPropertyName("permalink_url")]
		public string Url { get; set; }

		[JsonPropertyName("user")]
		public SoundCloudUser User { get; set; }
	}
}