using System.Text.RegularExpressions;

namespace Dysc {
    /// <summary>
    /// </summary>
    public readonly struct Patterns {
        /// <summary>
        /// </summary>
        public readonly struct YouTube {
            /// <summary>
            ///     Youtube ID regex.
            /// </summary>
            public static readonly Regex VideoId
				= new Regex("(?!videoseries)[a-zA-Z0-9_-]{11,42}",
					RegexOptions.Compiled | RegexOptions.IgnoreCase);

            /// <summary>
            /// </summary>
            public static readonly Regex DecipherName
				= new Regex(
					"(\\w+)=function\\(\\w+\\){(\\w+)=\\2\\.split\\(\x22{2}\\);.*?return\\s+\\2\\.join\\(\x22{2}\\)}"
					, RegexOptions.Compiled);
		}

        /// <summary>
        /// </summary>
        public readonly struct SoundCloud {
            /// <summary>
            /// </summary>
            public static readonly Regex PageScript
				= new Regex("https://[A-Za-z0-9-.]+/assets/[a-f0-9-]+\\.js",
					RegexOptions.Compiled | RegexOptions.IgnoreCase);

            /// <summary>
            /// </summary>
            public static readonly Regex ScriptClientId
				= new Regex(",client_id:\"([a-zA-Z0-9-_]+)\"",
					RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

        /// <summary>
        /// </summary>
        public readonly struct BandCamp {
            /// <summary>
            /// </summary>
            public static readonly Regex JsonBuilder = new Regex(@"([a-zA-Z0-9_]*:\s)(?!\s)",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);

            /// <summary>
            /// </summary>
            public static readonly Regex JsonCleanup =
				new Regex("\"(about|credits)\":\"([^\"]+)\",", RegexOptions.Compiled);

            /// <summary>
            /// </summary>
            public static readonly Regex TrackUrl = new Regex(
				"^https?://(?:[^.]+\\.|)bandcamp\\.com/track/([a-zA-Z0-9-_]+)/?(?:\\?.*|)$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);

            /// <summary>
            /// </summary>
            public static readonly Regex AlbumUrl = new Regex(
				"^https?://(?:[^.]+\\.|)bandcamp\\.com/album/([a-zA-Z0-9-_]+)/?(?:\\?.*|)$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}
	}
}