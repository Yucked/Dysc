using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dysc.Providers.SoundCloud {
	/// <summary>
	/// </summary>
	public sealed class SoundCloudParser {
		/// <summary>
		/// </summary>
		public static string ClientId { get; private set; }

		private readonly HttpClient _httpClient;
		private DateTimeOffset? _lastUpdate;

		/// <summary>
		/// </summary>
		/// <param name="httpClient"></param>
		public SoundCloudParser(HttpClient httpClient) {
			_httpClient = httpClient;
			_lastUpdate = null;
		}

		/// <summary>
		/// </summary>
		/// <param name="clientFactory"></param>
		public SoundCloudParser(IHttpClientFactory clientFactory)
			: this(clientFactory.CreateClient(nameof(SoundCloudProvider))) {
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NullReferenceException"></exception>
		public async Task ValidateClientIdAsync() {
			if (_lastUpdate.HasValue && _lastUpdate.Value.AddMinutes(50) < DateTimeOffset.Now) {
				return;
			}

			var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://soundcloud.com");
			var responseMessage = await _httpClient
			   .ChangeUserAgent()
			   .SendAsync(requestMessage)
			   .ConfigureAwait(false);

			var content = await responseMessage.Content!
			   .ReadAsStringAsync()
			   .ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(content)) {
				throw new NullReferenceException(nameof(content));
			}

			var matchScriptUrl = Patterns.SoundCloud.PageScript.Matches(content)[6].Groups[0].Value;

			requestMessage = new HttpRequestMessage(HttpMethod.Get, matchScriptUrl);
			responseMessage = await _httpClient
			   .SendAsync(requestMessage)
			   .ConfigureAwait(false);

			content = await responseMessage.Content!
			   .ReadAsStringAsync()
			   .ConfigureAwait(false);

			var match = Patterns.SoundCloud.ScriptClientId.Match(content);
			var id = match.Groups[1].Value;

			ClientId = id;
			_lastUpdate = DateTimeOffset.Now;
		}
	}
}