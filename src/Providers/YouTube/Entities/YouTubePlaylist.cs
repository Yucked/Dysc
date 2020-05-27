using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Dysc.Infos;

namespace Dysc.Providers.YouTube.Entities {
	internal sealed class YouTubePlaylist : YouTubeResult {
		[JsonPropertyName("video")]
		public IEnumerable<YouTubeVideo> Videos { get; set; }

		public PlaylistInfo BuildPlaylistInfo(string url) {
			return new PlaylistInfo {
				Id = Id,
				Url = url,
				Name = Title,
				Duration = Videos.Sum(x => x.Duration * 1000)
			};
		}
	}
}