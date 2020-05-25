using System;
using Dysc.Providers;

namespace Dysc.Infos {
    /// <summary>
    /// </summary>
    public struct TrackInfo {
        /// <summary>
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// </summary>
        public AuthorInfo Author { get; internal set; }

        /// <summary>
        /// </summary>
        public long Duration { get; internal set; }

        /// <summary>
        /// </summary>
        public string ArtworkUrl { get; internal set; }

        /// <summary>
        /// </summary>
        public bool CanStream { get; internal set; }

        /// <summary>
        /// </summary>
        public ProviderType Provider { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackInfo"></param>
        /// <returns></returns>
        public bool IsEqual(TrackInfo trackInfo) {
            return Url.Equals(trackInfo.Url, StringComparison.InvariantCultureIgnoreCase) ||
                   Id.IsStrongMatch(trackInfo.Id) ||
                   $"{Author} {Title}".IsStrongMatch($"{trackInfo.Author} {trackInfo.Title}");
        }
    }
}