﻿using System.Text.Json.Serialization;

namespace Dysc.Providers.SoundCloud.Entities {
	internal class SoundCloudResult {
		[JsonPropertyName("artwork_url")]
		public string ArtworkUrl { get; set; }

		[JsonPropertyName("duration")]
		public long Duration { get; set; }

		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("streamable")]
		public bool IsStreamable { get; set; }

		[JsonPropertyName("permalink_url")]
		public string PermalinkUrl { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("user")]
		public SoundCloudUser User { get; set; }
	}
}