using System;
using System.Threading.Tasks;
using Dysc.Providers.Http;
using Dysc.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
    [TestClass]
    public sealed class HttpTests : IProviderTest {
        private readonly HttpProvider _provider;

        public HttpTests() {
            var serviceCollection = new ServiceCollection()
                .AddHttpClient()
                .AddSingleton<HttpProvider>();

            var provider = serviceCollection.BuildServiceProvider();
            _provider = provider.GetRequiredService<HttpProvider>();
        }

        [DataTestMethod]
        [DataRow("TEST")]
        public async Task PerformSearchAsync(string query) {
            await Assert.ThrowsExceptionAsync<NotSupportedException>(async
                () => await _provider.SearchAsync(query));
        }

        [DataTestMethod]
        [DataRow("TEST")]
        public async Task GetPlaylistAsync(string playlistUrl) {
            await Assert.ThrowsExceptionAsync<NotSupportedException>(async
                () => await _provider.SearchAsync(playlistUrl));
        }

        [DataTestMethod]
        [DataRow("TEST")]
        public async Task GetTrackAsync(string trackUrl) {
            await Assert.ThrowsExceptionAsync<NotSupportedException>(async
                () => await _provider.SearchAsync(trackUrl));
        }

        [DataTestMethod]
        [DataRow("http://k007.kiwi6.com/hotlink/4tfa00k99z/Time_To_Party_.mp3")]
        [DataRow("http://k002.kiwi6.com/hotlink/643z63l90f/Hood_BeatZzZ.mp3")]
        public async Task GetStreamAsync(string trackUrl) {
            var stream = await _provider.GetStreamAsync(trackUrl)
                .ConfigureAwait(false);

            Assert.IsNotNull(stream);
            Assert.IsFalse(stream.Length == 0);
        }
    }
}