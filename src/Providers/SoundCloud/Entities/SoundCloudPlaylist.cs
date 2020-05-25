﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dysc.Infos;

namespace Dysc.Providers.SoundCloud.Entities {
    internal sealed class SoundCloudPlaylist : SoundCloudResult {
        [JsonPropertyName("tracks")]
        public IList<SoundCloudTrack> Tracks { get; set; }

        [JsonIgnore]
        public PlaylistInfo ToPlaylistInfo
            => new PlaylistInfo {
                Id = $"{Id}",
                Name = Title,
                Url = PermalinkUrl,
                Duration = Duration,
                ArtworkUrl = ArtworkUrl
            };
    }
}