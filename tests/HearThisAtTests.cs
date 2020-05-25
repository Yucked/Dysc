using System.Threading.Tasks;
using Dysc.Providers.HearThis.At;
using Dysc.Search;
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
        public async Task PerformSearchAsync(string query) {
            var response = await _provider.SearchAsync(query)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.SearchResult, response.Status);
            Assert.IsNotNull(response.Tracks);
            Assert.IsTrue(response.Tracks.Count > 0);
            Assert.IsNull(response.Playlist.Name);
        }

        [DataTestMethod]
        [DataRow("https://hearthis.at/verossi/set/veroniada/")]
        [DataRow("https://hearthis.at/raggajungle/set/junglecasts/")]
        [DataRow("https://hearthis.at/raggajungle/set/bang-in-ya-face-2017/")]
        [DataRow("https://hearthis.at/verossi/set/ravebox/")]
        [DataRow("https://hearthis.at/dextar/set/sound-of-eastside/")]
        public async Task GetPlaylistAsync(string playlistUrl) {
            var response = await _provider.SearchAsync(playlistUrl)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.PlaylistLoaded, response.Status);
            Assert.IsTrue(response.Tracks.Count > 0);
            Assert.IsNull(response.Playlist.Name);
        }

        [DataTestMethod]
        [DataRow("https://hearthis.at/momix-bond-r3/love-yourz-to-grrove-lp/listen")]
        [DataRow("https://hearthis.at/robert-k.-yw/robert-k-Bve/")]
        [DataRow("https://hearthis.at/rdio-gilo-4a/nao-faz-sentido-no-28/listen")]
        [DataRow("https://hearthis.at/groby-stefangrob/the-terminal-frontier-april-1992/listen")]
        [DataRow("https://hearthis.at/mjnrxbth/together-again-jazz-stacks/listen")]
        public async Task GetTrackAsync(string trackUrl) {
            var response = await _provider.SearchAsync(trackUrl)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.TrackLoaded, response.Status);
            Assert.IsTrue(response.Tracks.Count == 1);
        }

        [DataTestMethod]
        [DataRow("https://hearthis.at/momix-bond-r3/love-yourz-to-grrove-lp/listen")]
        [DataRow("https://hearthis.at/robert-k.-yw/robert-k-Bve/")]
        [DataRow("https://hearthis.at/rdio-gilo-4a/nao-faz-sentido-no-28/listen")]
        [DataRow("https://hearthis.at/groby-stefangrob/the-terminal-frontier-april-1992/listen")]
        [DataRow("https://hearthis.at/mjnrxbth/together-again-jazz-stacks/listen")]
        public async Task GetStreamAsync(string trackUrl) {
            var stream = await _provider.GetStreamAsync(trackUrl);
            Assert.IsNotNull(stream);
            Assert.IsTrue(stream.Length != 0);
        }
    }
}