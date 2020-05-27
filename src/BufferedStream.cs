using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dysc {
    /// <summary>
    /// </summary>
    public sealed class BufferedStream : Stream {
		/// <inheritdoc />
		public override bool CanRead
			=> true;

		/// <inheritdoc />
		public override bool CanSeek
			=> true;

		/// <inheritdoc />
		public override bool CanWrite
			=> false;

		/// <inheritdoc />
		public override long Length
			=> _length;

		/// <inheritdoc />
		public override long Position {
			get => _position;
			set {
				if (value < 0) {
					throw new IOException(
						"An attempt was made to move the position before the beginning of the stream.");
				}

				if (_position == value) {
					return;
				}

				_position = value;
				_baseStream!.Dispose();
				_baseStream = null;
			}
		}

        /// <summary>
        /// </summary>
        public long Segment { get; private set; }

		private readonly HttpClient _httpClient;
		private readonly string _url;

		private Stream _baseStream;
		private long _length;
		private long _position;

        /// <summary>
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        public BufferedStream(HttpClient httpClient, string url) {
			_httpClient = httpClient;
			_url = url;
		}

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public void SetSegment(long value) {
			Segment = value;
		}

		/// <inheritdoc />
		public override void SetLength(long value) {
			_length = value;
		}

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count) {
			var readAsync = ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
			return readAsync;
		}

		/// <inheritdoc />
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
			CancellationToken cancellationToken) {
			if (Position >= Length) {
				return 0;
			}

			if (_baseStream == null) {
				using var requestMessage = new HttpRequestMessage(HttpMethod.Get, _url)
				   .WithRange(Position, Position + Segment - 1);

				using var responseMessage = await _httpClient
				   .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
				   .ConfigureAwait(false);

				using var content = responseMessage.Content;
				var stream = await content.ReadAsStreamAsync()
				   .ConfigureAwait(false);

				_baseStream = stream;
			}

			var bytesRead = await _baseStream.ReadAsync(buffer, offset, count, cancellationToken)
			   .ConfigureAwait(false);

			_position += bytesRead;

			if (bytesRead != 0) {
				return bytesRead;
			}

			_baseStream.Dispose();
			_baseStream = null;

			bytesRead = await ReadAsync(buffer, offset, count, cancellationToken)
			   .ConfigureAwait(false);

			return bytesRead;
		}

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					return offset;
				case SeekOrigin.Current:
					return Position + offset;
				case SeekOrigin.End:
					return Length + offset;
				default:
					throw new ArgumentOutOfRangeException(nameof(origin));
			}
		}

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override void Flush() {
			throw new NotImplementedException();
		}
	}
}