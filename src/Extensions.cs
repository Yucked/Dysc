using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AngleSharp;
using AngleSharp.Html.Parser;
using Dysc.Search;

namespace Dysc {
	internal static class Extensions {
		private static readonly IBrowsingContext HtmlContext
			= BrowsingContext.New(Configuration.Default);

		public static readonly IHtmlParser HtmlParser
			= HtmlContext.GetService<IHtmlParser>();

		public static string WithPath(this string str, string path) {
			return $"{str}/{path}";
		}

		public static string WithParameter(this string str, string key, string value) {
			return str.Contains("?")
				? str + $"&{key}={value}"
				: str + $"?{key}={value}";
		}

		public static HttpClient ChangeUserAgent(this HttpClient httpClient) {
			httpClient.DefaultRequestHeaders.UserAgent.Clear();
			httpClient.DefaultRequestHeaders.Add("User-Agent",
				"Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
			return httpClient;
		}

		public static SearchResponse WithError(this SearchResponse searchResponse) {
			searchResponse.Status = SearchStatus.SearchError;
			return searchResponse;
		}

		public static SearchResponse WithNoMatches(this SearchResponse searchResponse) {
			searchResponse.Status = SearchStatus.NoMatches;
			return searchResponse;
		}

		public static string SubstringAfter(this string str, string subStr) {
			var index = str.IndexOf(subStr, StringComparison.Ordinal);
			return index < 0 ? string.Empty : str.Substring(index + subStr.Length, str.Length - index - subStr.Length);
		}

		public static string SubstringUntil(this string str, string subStr) {
			var index = str.IndexOf(subStr, StringComparison.Ordinal);
			return index < 0 ? str : str.Substring(0, index);
		}

		public static IDictionary<string, string> ToKeyValuePair(this string rawString) {
			var parameters = rawString
			   .TrimStart('?')
			   .Split('&');

			var dictionary = new Dictionary<string, string>(parameters.Length);
			foreach (var param in parameters) {
				var parameter = WebUtility.UrlDecode(param);

				var indexOf = parameter.IndexOf('=');
				if (indexOf <= 0) {
					continue;
				}

				var key = parameter.Substring(0, indexOf);
				var value = indexOf < parameter.Length
					? parameter.Substring(indexOf + 1)
					: string.Empty;

				dictionary.Add(key, value);
			}

			return dictionary;
		}

		public static byte[] AsBytes(this string str) {
			return Encoding.UTF8.GetBytes(str);
		}

		public static bool IsStrongMatch(this string str, string matchAgainst) {
			if (str.Equals(matchAgainst, StringComparison.InvariantCultureIgnoreCase)) {
				return true;
			}

			var mainSplits = str.Split(' ');
			var matcherSplits = matchAgainst.Split(' ');

			if (mainSplits.Length == 1 && matcherSplits.Length == 1) {
				return mainSplits[0].Equals(matcherSplits[0], StringComparison.InvariantCultureIgnoreCase);
			}

			var matchCount = mainSplits.Sum(main => matcherSplits.Count(main.IsStrongMatch));
			var percentage = matchCount / mainSplits.Length * 100;
			return percentage > 80;
		}

		public static HttpRequestMessage WithRange(this HttpRequestMessage requestMessage, long from, long to) {
			requestMessage.Headers.Range = new RangeHeaderValue(from, to);
			return requestMessage;
		}

		public static BufferedStream GetBufferedStream(this HttpClient httpClient, string url, long length,
			long bufferSize = 1024) {
			var bufferedStream = new BufferedStream(httpClient, url);
			bufferedStream.SetLength(length);
			bufferedStream.SetSegment(bufferSize);
			return bufferedStream;
		}
	}
}