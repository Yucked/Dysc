using System;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dysc.Decoders;

namespace Dysc.Stream {
	using System.IO;

	/// <summary>
	/// 
	/// </summary>
	public sealed class PipedStream {
		/// <summary>
		/// 
		/// </summary>
		public IDecoder Decoder { get; }

		/// <summary>
		/// 
		/// </summary>
		public TimeSpan Position { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public int Volume { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsPlaying { get; private set; }

		private readonly Pipe _pipe;
		private readonly StreamOptions _streamOptions;

		private long _position;
		private bool _isPaused;
		private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
		private TaskCompletionSource<bool> _pauseTask = new TaskCompletionSource<bool>();

		public PipedStream(StreamOptions streamOptions) {
			_streamOptions = streamOptions;
			_pipe = new Pipe();

			if (_streamOptions.Decoder == null) {
				return;
			}

			Decoder = streamOptions.Decoder;
			Decoder.Decode();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async ValueTask ReadAsync() {
			Guard.NotNull(nameof(_streamOptions.Stream), _streamOptions.Stream);

			if (!_streamOptions.Stream.CanRead) {
				throw new Exception("Reading isn't possible from target stream.");
			}

			if (_streamOptions.Stream.Length <= 0) {
				throw new Exception("Stream length is less than or equal to 0.");
			}

			await using var stream = _streamOptions.Stream;
			var segmentSize = (int) _streamOptions.Length / 5;
			while (_streamOptions.Length > _position) {
				var memoryBuffer = _pipe.Writer.GetMemory(segmentSize);
				var readCount = stream.Read(memoryBuffer.Span);

				if (readCount <= 0) {
					await _pipe.Writer.FlushAsync();
					break;
				}

				_pipe.Writer.Advance(readCount);
				_position += readCount;
			}

			await _pipe.Writer.CompleteAsync();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async ValueTask WriteAsync(Stream target) {
			Guard.NotNull(nameof(target), target);

			if (!target.CanWrite) {
				throw new Exception("Can't write to target stream.");
			}

			do {
				var readResult = await _pipe.Reader.ReadAsync(_cancellationToken.Token);
				foreach (var memory in readResult.Buffer) {
					var scaledMemory = ScaleVolume(memory);
					await target.WriteAsync(scaledMemory);
				}

				if (readResult.IsCompleted) {
					break;
				}

				_pipe.Reader.AdvanceTo(readResult.Buffer.End);
			} while (!_cancellationToken.IsCancellationRequested);

			await _pipe.Reader.CompleteAsync();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Pause() {
			if (!Volatile.Read(ref _isPaused)) {
				return;
			}

			Volatile.Write(ref _isPaused, true);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Resume() {
			if (Volatile.Read(ref _isPaused)) {
				return;
			}

			if (!_pauseTask.TrySetResult(true)) {
				return;
			}

			_pauseTask = new TaskCompletionSource<bool>();
			Volatile.Write(ref _isPaused, false);
			IsPlaying = false;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Stop() {
			if (!Volatile.Read(ref _isPaused)) {
				return;
			}

			using var cancelToken = _cancellationToken;
			cancelToken.Cancel(false);
			_cancellationToken = new CancellationTokenSource();
			IsPlaying = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="volume"></param>
		public void SetVolume(int volume) {
			Guard.NotNull(nameof(volume), volume);
			Volume = volume;
		}

		private ReadOnlyMemory<byte> ScaleVolume(ReadOnlyMemory<byte> readOnlyMemory) {
			if (Math.Abs(Volume - 1f) < 0.0001f) {
				return readOnlyMemory;
			}

			var memory = Unsafe.As<ReadOnlyMemory<byte>, Memory<byte>>(ref readOnlyMemory);
			var volumeFixed = (int) Math.Round(Volume * 65536d);
			var asShorts = MemoryMarshal.Cast<byte, short>(memory.Span[..memory.Span.Length]);

			for (var i = 0; i < asShorts.Length; i++) {
				asShorts[i] = (short) ((asShorts[i] * volumeFixed) >> 16);
			}

			return Unsafe.As<Memory<byte>, ReadOnlyMemory<byte>>(ref memory);
		}
	}
}