using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dysc.Providers.YouTube {
    /// <summary>
    /// </summary>
    public sealed class YouTubeParser {
		internal const string BASE_URL = "https://youtube.com/";
		private const string API_URL = "https://youtube.googleapis.com/v/";
		private readonly HttpClient _httpClient;

        /// <summary>
        /// </summary>
        /// <param name="httpClient"></param>
        public YouTubeParser(HttpClient httpClient) {
			_httpClient = httpClient;
		}

        /// <summary>
        /// </summary>
        /// <param name="clientFactory"></param>
        public YouTubeParser(IHttpClientFactory clientFactory)
			: this(clientFactory.CreateClient(nameof(YouTubeProvider))) {
		}

        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="videoId"></param>
        /// <param name="playlistId"></param>
        public static void ParseId(string url, out string videoId, out string playlistId) {
			var matches = Patterns.YouTube.VideoId.Matches(url);
			var (vidId, plyId) = ("", "");

			foreach (Match match in matches) {
				if (!match!.Success) {
					continue;
				}

				if (match.Length == 11) {
					vidId = match.Value;
				}
				else {
					plyId = match.Value;
				}
			}

			videoId = vidId;
			playlistId = plyId;
		}

		public async ValueTask<IDictionary<string, string>> GetVideoInfoAsync(string videoId) {
			var requestMessage = new HttpRequestMessage(HttpMethod.Get,
				BASE_URL
				   .WithPath("get_video_info")
				   .WithParameter("video_id", videoId)
				   .WithParameter("eurl", $"{API_URL}{videoId}")
				   .WithParameter("el", "embedded"));

			using var responseMessage = await _httpClient
			   .SendAsync(requestMessage)
			   .ConfigureAwait(false);

			using var content = responseMessage.Content;
			var byteArray = await content.ReadAsByteArrayAsync()
			   .ConfigureAwait(false);

			var rawResponse = Encoding.UTF8.GetString(byteArray);
			return rawResponse.ToKeyValuePair();
		}
	}
}