using System;
using System.Text.Json.Serialization;
using Dysc.Infos;

namespace Dysc.Providers.BandCamp.Entities {
    internal sealed class BandCampTrack {
        [JsonPropertyName("streaming")]
        public int Streaming { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("track_id")]
        public long TrackId { get; set; }

        [JsonPropertyName("file")]
        public BandCampFile File { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }

        public TrackInfo AsTrackInfo(string author, string url, long artId) {
            return new TrackInfo {
                Id = $"{TrackId}",
                Title = Title,
                Url = url,
                Duration = (long) TimeSpan.FromSeconds(Duration)
                    .TotalMilliseconds,
                ArtworkUrl = artId == 0 ? "" : $"https://f4.bcbits.com/img/a{artId}_0.jpg",
                CanStream = Streaming == 1,
                Provider = ProviderType.BandCamp,
                Author = new AuthorInfo {
                    Name = author
                }
            };
        }
    }
}