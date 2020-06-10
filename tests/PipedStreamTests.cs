using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Dysc.Stream;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
	[TestClass]
	public sealed class PipedStreamTests {
		private readonly HttpClient _httpClient;
		private PipedStream _pipedStream;

		public PipedStreamTests() {
			_httpClient = new HttpClient();
		}

		[TestMethod]
		public async Task BigTest() {
			await WriteToPipeAsync("http://k007.kiwi6.com/hotlink/4tfa00k99z/Time_To_Party_.mp3");
			await ReadFromPipeAsync();
		}

		public async Task WriteToPipeAsync(string url) {
			var responseMessage = await _httpClient.GetAsync(url)
			   .ConfigureAwait(false);

			if (!responseMessage.IsSuccessStatusCode) {
				Assert.Fail(responseMessage.ReasonPhrase);
			}

			var content = responseMessage.Content;
			if (!content.TryGetAudioType(out _)) {
				Assert.Fail("Requested resource doesn't return audio content.");
			}

			var stream = await content!.ReadAsStreamAsync();
			var streamOptions = new StreamOptions {
				Length = content.GetContentLength(),
				Stream = stream
			};

			_pipedStream = new PipedStream(streamOptions);
			Assert.IsNotNull(_pipedStream);
			Assert.IsFalse(_pipedStream.IsPlaying);

			if (!_pipedStream.Position.Equals(default)) {
				Assert.Fail();
			}

			await _pipedStream.ReadAsync();
		}

		public async Task ReadFromPipeAsync() {
			var memoryStream = new MemoryStream();
			await _pipedStream.WriteAsync(memoryStream);
			
			Assert.IsTrue(memoryStream.CanRead);
			Assert.IsTrue(memoryStream.CanWrite);
			Assert.IsTrue(memoryStream.CanSeek);
			Assert.IsTrue(memoryStream.CanRead);
			
			Assert.IsTrue(memoryStream.Length > 0);
			Assert.IsTrue(memoryStream.Position > 0);
		}
	}
}