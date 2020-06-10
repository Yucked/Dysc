using System.Threading.Tasks;
using Dysc.Providers.HearThisAt;
using Dysc.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
	[TestClass]
	public sealed class HearThisAtTests : IProviderTest {
		private readonly HearThisAtProvider _provider;

		public HearThisAtTests() {
			var serviceCollection = new ServiceCollection()
			   .AddHttpClient()
			   .AddSingleton<HearThisAtProvider>();
			var provider = serviceCollection.BuildServiceProvider();
			_provider = provider.GetRequiredService<HearThisAtProvider>();
		}

		[DataTestMethod]
		[DataRow("Hard Bass")]
		[DataRow("Techo House")]
		[DataRow("Shawne")]
		[DataRow("Dubstep Mix")]
		[DataRow("Deep House")]
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
		[DataRow("https://hearthis.at/momix-bond-r3/love-yourz-to-grrove-lp")]
		[DataRow("https://hearthis.at/robert-k.-yw/robert-k-Bve/")]
		[DataRow("https://hearthis.at/rdio-gilo-4a/nao-faz-sentido-no-28")]
		[DataRow("https://hearthis.at/groby-stefangrob/the-terminal-frontier-april-1992")]
		[DataRow("https://hearthis.at/mjnrxbth/together-again-jazz-stacks")]
		public async Task GetTrackAsync(string trackUrl) {
			var track = await _provider.GetTrackAsync(trackUrl)
			   .ConfigureAwait(false);

			track.IsValidTrack();
			track.Author.IsValidAuthor();
		}

		[DataTestMethod]
		[DataRow("https://hearthis.at/generik/set/generik-originals-fu/")]
		[DataRow("https://hearthis.at/generik/set/chat-noir/")]
		[DataRow("https://hearthis.at/slobodnyvysielac/set/ako-alej/")]
		[DataRow("https://hearthis.at/slobodnyvysielac/set/anarchokapitalizmus/")]
		[DataRow("https://hearthis.at/moddakerelearn/set/moddaker-level-1-1/")]
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
		[DataRow("https://hearthis.at/momix-bond-r3/love-yourz-to-grrove-lp/listen")]
		[DataRow("https://hearthis.at/robert-k.-yw/robert-k-Bve/")]
		[DataRow("https://hearthis.at/rdio-gilo-4a/nao-faz-sentido-no-28/listen")]
		[DataRow("https://hearthis.at/groby-stefangrob/the-terminal-frontier-april-1992/listen")]
		[DataRow("https://hearthis.at/mjnrxbth/together-again-jazz-stacks")]
		public async Task GetStreamAsync(string trackUrl) {
			var stream = await _provider.GetTrackStreamAsync(trackUrl);
			Assert.IsNotNull(stream);
			stream.Decoder.IsDecodingSuccessfull();
		}
	}
}