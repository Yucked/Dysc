using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dysc.Providers.SoundCloud.Entities {
	internal class SoundCloudSearch {
		[JsonPropertyName("collection")]
		public IReadOnlyList<SoundCloudTrack> Collection { get; set; }
	}
}