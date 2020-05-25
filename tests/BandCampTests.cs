using System.Linq;
using System.Threading.Tasks;
using Dysc.Providers.BandCamp;
using Dysc.Search;
using Dysc.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
    [TestClass]
    public sealed class BandCampTests : IProviderTest {
        private readonly BandCampProvider _provider;

        public BandCampTests() {
            var serviceCollection = new ServiceCollection()
                .AddHttpClient()
                .AddSingleton<BandCampProvider>();

            var provider = serviceCollection.BuildServiceProvider();
            _provider = provider.GetRequiredService<BandCampProvider>();
        }

        [DataTestMethod]
        [DataRow("Travis Scott Through The Late Night")]
        [DataRow("Daniel Ceaser Get You")]
        public async Task PerformSearchAsync(string query) {
            var response = await _provider.SearchAsync(query)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.SearchResult, response.Status);
            Assert.IsNotNull(response.Tracks);
            Assert.IsTrue(response.Tracks.Count > 0);
            Assert.IsNull(response.Playlist.Name);
        }

        [DataTestMethod]
        [DataRow("https://slimk4.bandcamp.com/album/astrochops")]
        [DataRow("https://illuminatihotties.bandcamp.com/album/ppl-plzr-single")]
        [DataRow("https://glennastro.bandcamp.com/album/naturals")]
        [DataRow("https://nikiistrefi.bandcamp.com/album/euromantic001")]
        [DataRow("https://speakspellrecords.bandcamp.com/album/all-good-things-come-in-threes")]
        public async Task GetPlaylistAsync(string playlistUrl) {
            var response = await _provider.SearchAsync(playlistUrl)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.PlaylistLoaded, response.Status);
            Assert.IsNotNull(response.Playlist);
            Assert.IsNotNull(response.Playlist.Name);
            Assert.IsNotNull(response.Tracks);
            Assert.IsTrue(response.Tracks.Count > 0);
        }

        [DataTestMethod]
        [DataRow("https://bragacircuit1.bandcamp.com/track/without-ya")]
        [DataRow("https://inchaoscn.bandcamp.com/track/you-and-me")]
        [DataRow("https://luvless.bandcamp.com/track/oceans-and-mountains")]
        [DataRow("https://danheim.bandcamp.com/track/lifa-dau-r")]
        [DataRow("https://bluehoursounds.bandcamp.com/track/blackmoon-rising")]
        public async Task GetTrackAsync(string trackUrl) {
            var response = await _provider.SearchAsync(trackUrl)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.TrackLoaded, response.Status);
            Assert.IsTrue(response.Tracks.Count == 1);
        }

        [DataTestMethod]
        [DataRow("https://bragacircuit1.bandcamp.com/track/without-ya")]
        [DataRow("https://inchaoscn.bandcamp.com/track/you-and-me")]
        [DataRow("https://luvless.bandcamp.com/track/oceans-and-mountains")]
        [DataRow("https://danheim.bandcamp.com/track/lifa-dau-r")]
        [DataRow("https://bluehoursounds.bandcamp.com/track/blackmoon-rising")]
        public async Task GetStreamAsync(string trackUrl) {
            var search = await _provider
                .SearchAsync(trackUrl)
                .ConfigureAwait(false);

            var track = search.Tracks.FirstOrDefault();
            Assert.IsNotNull(track);

            var stream = await _provider.GetStreamAsync(track)
                .ConfigureAwait(false);

            Assert.IsNotNull(stream);
            Assert.IsFalse(stream.Length == 0);
        }
    }
}