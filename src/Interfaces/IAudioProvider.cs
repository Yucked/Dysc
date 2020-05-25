using System;
using System.IO;
using System.Threading.Tasks;
using Dysc.Infos;
using Dysc.Search;

namespace Dysc.Interfaces {
    /// <summary>
    /// Represents an audio source.
    /// </summary>
    public interface IAudioProvider : IAsyncDisposable {
        /// <summary>
        /// Searches source for your <paramref name="query"/> and returns a <see cref="SearchResponse"/>.
        /// </summary>
        /// <param name="query">Link or raw search query.</param>
        /// <returns><see cref="SearchResponse"/></returns>
        ValueTask<SearchResponse> SearchAsync(string query);

        /// <summary>
        /// Gets the <see cref="Stream"/> for specified track url or id.
        /// </summary>
        /// <param name="trackId">Can be a url or track id</param>
        /// <returns><see cref="Stream"/></returns>
        ValueTask<Stream> GetStreamAsync(string trackId);

        /// <summary>
        /// Gets the <see cref="Stream"/> for specified <paramref name="trackInfo"/>.
        /// </summary>
        /// <param name="trackInfo"><see cref="TrackInfo"/></param>
        /// <returns><see cref="Stream"/></returns>
        ValueTask<Stream> GetStreamAsync(TrackInfo trackInfo);
    }
}