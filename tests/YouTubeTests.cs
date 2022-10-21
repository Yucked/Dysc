using System.Threading.Tasks;
using Dysc.Providers.YouTube;
using Dysc.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
	[TestClass]
	public class YouTubeTests : IProviderTest {
		private readonly YouTubeProvider _provider;

		public YouTubeTests() {
			var serviceCollection = new ServiceCollection()
			   .AddHttpClient()
			   .AddSingleton<YouTubeProvider>();

			var provider = serviceCollection.BuildServiceProvider();
			_provider = provider.GetRequiredService<YouTubeProvider>();
		}

		[DataTestMethod]
		[DataRow("SiR Hair Down")]
		[DataRow("J. Cole No Role Modelz")]
		[DataRow("The Weeknd Wicked Games")]
		[DataRow("Eminem Lucky You")]
		[DataRow("$UICIDEBOY$ x POUYA - SOUTH SIDE $UICIDE")]
		public async Task SearchAsync(string query) {
			var trackResults = await _provider.SearchAsync(query)
			   .ConfigureAwait(false);

			Assert.IsNotNull(trackResults);
			Assert.IsTrue(trackResults.Count != 0);

			foreach (var track in trackResults) {
				track.IsValidTrack();
				track.Author.IsValidAuthor();
			}
		}


		[DataTestMethod]
		[DataRow("https://www.youtube.com/watch?v=ygTZZpVkmKg")]
		[DataRow("https://www.youtube.com/watch?v=lKqUggbSlHw")]
		[DataRow("https://www.youtube.com/watch?v=PW6Wvg_fxAQ")]
		[DataRow("https://www.youtube.com/watch?v=8J_0wkmqq8o")]
		[DataRow("https://www.youtube.com/watch?v=imOUKPruPHk")]
		public async Task GetTrackAsync(string trackUrl) {
			var track = await _provider.GetTrackAsync(trackUrl)
			   .ConfigureAwait(false);

			track.IsValidTrack();
			track.Author.IsValidAuthor();
		}

		[DataTestMethod]
		[DataRow("https://www.youtube.com/watch?v=vYPIOaqNlyg&list=RDCLAK5uy_n7QjhERM2Q4Ha5B6t6ZmzyhOtRYjQtxKk")]
		[DataRow("https://www.youtube.com/watch?v=hlFOLS7vKaQ&list=OLAK5uy_nyPpHefgBGBt_CgAaqT3PV6BLxh_tU59s&index=1")]
		[DataRow("https://www.youtube.com/watch?v=_FU8xyVC-tk&list=PL-2HG0C5jJQG5n1GVlif-9S-FnQ1dIuKM")]
		[DataRow("https://www.youtube.com/watch?v=iE4_dtz8u28&list=RDCLAK5uy_lWC9PGFnzJWI7RvkLcFUBICdb3UhfzLbs")]
		[DataRow("https://www.youtube.com/watch?v=JH398xAYpZA&list=OLAK5uy_lwaD8UXRautA8W9eWT4zZOvwf5Ktxpax8")]
		public async Task GetPlaylistAsync(string playlistUrl) {
			var playlist = await _provider.GetPlaylistAsync(playlistUrl)
			   .ConfigureAwait(false);

			playlist.IsValidPlaylist();
			playlist.Author.IsValidAuthor();

			foreach (var track in playlist.Tracks) {
				track.IsValidTrack();
				track.Author.IsValidAuthor();
			}
		}

		[DataTestMethod]
		[DataRow("https://www.youtube.com/watch?v=ygTZZpVkmKg")]
		[DataRow("https://www.youtube.com/watch?v=lKqUggbSlHw")]
		[DataRow("https://www.youtube.com/watch?v=PW6Wvg_fxAQ")]
		[DataRow("https://www.youtube.com/watch?v=8J_0wkmqq8o")]
		[DataRow("https://www.youtube.com/watch?v=imOUKPruPHk")]
		public async Task GetStreamAsync(string trackUrl) {
			var stream = await _provider.GetTrackStreamAsync(trackUrl);
			Assert.IsNotNull(stream);
		}
	}
}