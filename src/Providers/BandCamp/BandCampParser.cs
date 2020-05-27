using System;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;
using Dysc.Infos;
using Dysc.Search;

namespace Dysc.Providers.BandCamp {
    /// <summary>
    /// </summary>
    public readonly struct BandCampParser {
        /// <summary>
        /// </summary>
        /// <param name="searchResponse"></param>
        /// <param name="htmlBytes"></param>
        public static void ScrapeHtml(ref SearchResponse searchResponse, byte[] htmlBytes) {
			var searchDiv = GetSearchDiv(htmlBytes);
			var searchDivDocument = Extensions.HtmlParser.ParseDocument(searchDiv);
			var trackResults = searchDivDocument.GetElementsByClassName("searchresult track");
			foreach (var trackResult in trackResults) {
				var trackInfo = new TrackInfo();

				foreach (var child in trackResult.Children) {
					switch (child) {
						case IHtmlAnchorElement anchorElement:
							var url = anchorElement.Origin + anchorElement.PathName;
							var artworkElement = anchorElement.FirstElementChild.FirstElementChild as IHtmlImageElement;
							var artworkUrl = artworkElement!.Source;

							trackInfo.Url = url;
							trackInfo.ArtworkUrl = artworkUrl;
							break;

						case IHtmlDivElement divElement:
							var headingContent = divElement.Children[1].FirstElementChild.TextContent;
							var subheadContent = divElement.Children[2].TextContent;

							var title = headingContent
							   .Trim()
							   .Trim('\n');

							var artistName = subheadContent.SubstringAfter("by")
							   .Trim()
							   .Trim('\n');

							trackInfo.Title = title;
							trackInfo.Author = new AuthorInfo {
								Name = artistName
							};
							break;
					}
				}

				searchResponse.AddTrack(trackInfo);
			}
		}

        /// <summary>
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool TryGetJson(Span<byte> rawData, out string json) {
			json = string.Empty;

			if (rawData.Length == 0) {
				return false;
			}

			var jsVar = "var TralbumData =".AsBytes();

			var startIndex = rawData.IndexOf(jsVar);
			rawData = rawData.Slice(startIndex + jsVar.Length);

			var endIndex = rawData.IndexOf("};".AsBytes());
			rawData = rawData.Slice(0, endIndex + 1);

			json = Encoding.UTF8.GetString(rawData);
			FixJson(ref json);

			return true;
		}

		private static void FixJson(ref string json) {
			json = Patterns.BandCamp.JsonCleanup.Replace(json, string.Empty);
			var matches = Patterns.BandCamp.JsonBuilder.Matches(json);
			foreach (Match match in matches) {
				if (match == null) {
					continue;
				}

				var val = $"\"{match.Value.Replace(": ", "")}\":";
				var regex = new Regex(Regex.Escape(match.Value), RegexOptions.Compiled | RegexOptions.IgnoreCase);
				json = regex.Replace(json, val, 1);
			}

			json = json.Replace("\" + \"", "");
		}

		private static string GetSearchDiv(Span<byte> htmlBytes) {
			var leftIndex = htmlBytes.IndexOf("<div class=\"leftcol\">".AsBytes());
			htmlBytes = htmlBytes.Slice(leftIndex);

			var rightIndex = htmlBytes.IndexOf("<div class=\"rightcol\">".AsBytes());
			htmlBytes = htmlBytes.Slice(0, rightIndex);

			var rawHtml = Encoding.UTF8.GetString(htmlBytes);
			return rawHtml;
		}
	}
}