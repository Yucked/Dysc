using System.Text.RegularExpressions;

namespace Dysc.Providers.YouTube {
	/// <summary>
	/// 
	/// </summary>
	public static class YouTubeParser {
		
		/// <summary>
		/// 
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
	}
}