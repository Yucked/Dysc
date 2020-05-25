using System.Text.Json.Serialization;
using Dysc.Infos;

namespace Dysc.Providers.SoundCloud.Entities {
    internal sealed class SoundCloudTrack : SoundCloudResult {
        [JsonIgnore]
        public TrackInfo ToTrackInfo
            => new TrackInfo {
                Id = $"{Id}",
                Title = Title,
                Url = PermalinkUrl,
                Duration = Duration,
                ArtworkUrl = ArtworkUrl,
                CanStream = IsStreamable,
                Provider = ProviderType.SoundCloud,
                Author = User.ToAuthorInfo
            };
    }
}