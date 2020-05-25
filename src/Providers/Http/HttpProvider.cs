using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Dysc.Infos;
using Dysc.Interfaces;
using Dysc.Search;

namespace Dysc.Providers.Http {
    /// <summary>
    /// </summary>
    public sealed class HttpProvider : IAudioProvider {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        public HttpProvider(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        /// <summary>
        /// </summary>
        /// <param name="clientFactory"></param>
        public HttpProvider(IHttpClientFactory clientFactory)
            : this(clientFactory.CreateClient(nameof(HttpProvider))) { }

        /// <inheritdoc />
        public ValueTask<SearchResponse> SearchAsync(string query) {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public async ValueTask<Stream> GetStreamAsync(string url) {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url), "Url cannot be null or empty.");

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new UriFormatException($"{nameof(url)} must be a proper URL type.");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            var responseMessage = await _httpClient.SendAsync(requestMessage)
                .ConfigureAwait(false);

            var content = responseMessage.Content;
            if (!content.Headers.ContentType.MediaType.Contains("audio"))
                throw new Exception("URL missing audio headers.");

            var stream = await content.ReadAsStreamAsync()
                .ConfigureAwait(false);

            stream.Position = 0;
            return stream;
        }

        /// <inheritdoc />
        public ValueTask<Stream> GetStreamAsync(TrackInfo track) {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync() {
            _httpClient.Dispose();
            return default;
        }
    }
}