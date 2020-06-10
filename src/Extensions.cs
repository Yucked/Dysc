using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Dysc.Decoders;
using Dysc.Stream;

namespace Dysc {
	/// <summary>
	/// 
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string WithPath(this string str, string path) {
			return $"{str}/{path}";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string WithParameter(this string str, string key, string value) {
			return str.Contains("?")
				? str + $"&{key}={value}"
				: str + $"?{key}={value}";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		public static HttpClient ChangeUserAgent(this HttpClient httpClient) {
			httpClient.DefaultRequestHeaders.UserAgent.Clear();
			if (httpClient.DefaultRequestHeaders.Contains("User-Agent")) {
				return httpClient;
			}

			httpClient.DefaultRequestHeaders.Add("User-Agent",
				"Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
			return httpClient;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpClient"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		/// <exception cref="HttpRequestException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static async ValueTask<PipedStream> GetPipedStreamAsync(this HttpClient httpClient, string url) {
			var responseMessage = await httpClient.GetAsync(url)
			   .ConfigureAwait(false);

			if (!responseMessage.IsSuccessStatusCode) {
				throw new HttpRequestException(responseMessage.ReasonPhrase);
			}

			var content = responseMessage.Content;
			if (!content.TryGetAudioType(out var audioType)) {
				throw new InvalidOperationException("Requested resource doesn't return audio content.");
			}

			var stream = await content!.ReadAsStreamAsync();
			var streamOptions = new StreamOptions {
				Length = content.GetContentLength(),
				Stream = stream,
				HttpClient = httpClient,
				Decoder = audioType.GetDecoder(stream)
			};

			var pipedStream = new PipedStream(streamOptions);
			await pipedStream.ReadAsync();
			return pipedStream;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content"></param>
		/// <param name="audioType"></param>
		/// <returns></returns>
		public static bool TryGetAudioType(this HttpContent content, out string audioType) {
			audioType = default;

			var contentType = content?.Headers.ContentType;
			var mediaType = contentType?.MediaType;
			if (mediaType == null) {
				return false;
			}

			if (!mediaType.Contains("audio")) {
				return false;
			}

			audioType = mediaType;
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static long GetContentLength(this HttpContent content) {
			var contentLength = content?.Headers.ContentLength;
			return contentLength ?? throw new Exception("Invalid content length.");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestMessage"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static HttpRequestMessage WithRange(this HttpRequestMessage requestMessage, long from, long to) {
			requestMessage.Headers.Range = new RangeHeaderValue(from, to);
			return requestMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpClient"></param>
		/// <param name="url"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static async ValueTask<T> ReadFromJsonAsync<T>(this HttpClient httpClient, string url) {
			using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
			var responseMessage = await httpClient.SendAsync(requestMessage)
			   .ConfigureAwait(false);

			Guard.IsSuccessfulResponse(responseMessage);
			var content = responseMessage.Content;
			Guard.NotNull(nameof(content), content);
			var byteArray = await content!.ReadAsByteArrayAsync()
			   .ConfigureAwait(false);

			return JsonSerializer.Deserialize<T>(byteArray);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpClient"></param>
		/// <param name="url"></param>
		/// <param name="propertyName"></param>
		/// <typeparam name="TReturn"></typeparam>
		/// <returns></returns>
		/// <exception cref="JsonException"></exception>
		public static async ValueTask<TReturn> GetTokenFromJsonAsync<TReturn>(this HttpClient httpClient,
			string url, string propertyName) where TReturn : class {
			using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
			var responseMessage = await httpClient.SendAsync(requestMessage)
			   .ConfigureAwait(false);

			Guard.IsSuccessfulResponse(responseMessage);
			var content = responseMessage.Content;
			Guard.NotNull(nameof(content), content);
			var stream = await content!.ReadAsStreamAsync()
			   .ConfigureAwait(false);

			var document = await JsonDocument.ParseAsync(stream);
			if (!document.RootElement.TryGetProperty(propertyName, out var element)) {
				throw new JsonException("");
			}

			return $"{element}" as TReturn;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="audioType"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static IDecoder GetDecoder(this string audioType, System.IO.Stream stream) {
			return audioType switch {
				_ when audioType.Contains("mpeg") => new Mp3Decoder(stream),
				_                                 => default
			};
		}

		internal static Uri AsUri(this string str) {
			return new Uri(str);
		}
	}
}