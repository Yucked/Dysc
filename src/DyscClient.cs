using System.Collections.Generic;
using System.Net.Http;
using Dysc.Interfaces;
using Dysc.Providers;
using Dysc.Providers.BandCamp;
using Dysc.Providers.HearThis.At;
using Dysc.Providers.Http;
using Dysc.Providers.SoundCloud;
using Dysc.Providers.YouTube;

namespace Dysc {
    /// <summary>
    /// 
    /// </summary>
    public sealed class DyscClient {
        /// <summary>
        ///     Number of current providers.
        /// </summary>
        public int Providers
            => _audioProviders.Count;

        private readonly IDictionary<ProviderType, IAudioProvider> _audioProviders;

        /// <summary>
        /// Creates a new instance of <see cref="DyscClient"/>.
        /// </summary>
        public DyscClient(IHttpClientFactory clientFactory) {
            _audioProviders = new Dictionary<ProviderType, IAudioProvider> {
                {ProviderType.SoundCloud, new SoundCloudProvider(clientFactory)},
                {ProviderType.Http, new HttpProvider(clientFactory)},
                {ProviderType.HearThisAt, new HearThisAtProvider(clientFactory)},
                {ProviderType.BandCamp, new BandCampProvider(clientFactory)},
                {ProviderType.YouTube, new YouTubeProvider(clientFactory)}
            };
        }

        /// <summary>
        /// Checks if a certain provider exists.
        /// </summary>
        /// <param name="providerType">Which provider to look for.</param>
        /// <returns><see cref="bool"/></returns>
        public bool HasProvider(ProviderType providerType) {
            return _audioProviders.ContainsKey(providerType);
        }

        /// <summary>
        /// Gets an <see cref="IAudioProvider"/> from cache.
        /// </summary>
        /// <param name="providerType">Which provider to look for.</param>
        /// <returns><see cref="IAudioProvider"/></returns>
        public IAudioProvider GetProvider(ProviderType providerType) {
            return _audioProviders[providerType];
        }

        /// <summary>
        /// A safe way to check and get an <see cref="IAudioProvider"/> from cache.
        /// </summary>
        /// <param name="providerType">Which provider to look for.</param>
        /// <param name="audioProvider">Provider returned based on <paramref name="audioProvider"/>.</param>
        /// <returns><see cref="bool"/></returns>
        public bool TryGetProvider(ProviderType providerType, out IAudioProvider audioProvider) {
            return _audioProviders.TryGetValue(providerType, out audioProvider!);
        }
    }
}