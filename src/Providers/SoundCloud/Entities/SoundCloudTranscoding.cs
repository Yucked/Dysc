﻿using System.Text.Json.Serialization;

namespace Dysc.Providers.SoundCloud.Entities {
	internal sealed class SoundCloudTranscoding {
		[JsonPropertyName("format")]
		public SoundCloudFormat Format { get; set; }

		[JsonPropertyName("preset")]
		public string Preset { get; set; }

		[JsonPropertyName("quality")]
		public string Quality { get; set; }

		[JsonPropertyName("url")]
		public string Url { get; set; }
	}
}