using System;
using System.Collections.Generic;

namespace Dysc.Decoders {
	using System.IO;

	/// <inheritdoc />
	public sealed class Mp3Decoder : IDecoder {
		/// <inheritdoc />
		public int BitsPerSample { get; private set; }

		/// <inheritdoc />
		public int BytesPerSample
			=> BitsPerSample / 8;

		/// <inheritdoc />
		public int BytesPerSecond
			=> BlockAlign * SampleRate;

		/// <inheritdoc />
		public int Channels { get; private set; }

		/// <inheritdoc />
		public int SampleRate { get; private set; }

		/// <inheritdoc />
		public int BlockAlign
			=> BitsPerSample / 8 * Channels;

		/// <inheritdoc />
		public TimeSpan Length { get; private set; }

		private ulong _someVariable;
		private bool _isVBitRate;
		private int _vFrames;
		private long _fileSize;
		private readonly Stream _stream;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		/// <exception cref="Exception"></exception>
		public Mp3Decoder(Stream stream) {
			Guard.NotNull(nameof(stream), stream);

			if (!stream.CanRead) {
				throw new Exception("Provided stream isn't readable.");
			}

			_stream = stream;
		}

		/// <inheritdoc />
		public void Decode() {
			_fileSize = _stream.Length;
			var byteHeader = new byte[4];
			var byteVBitRate = new byte[12];
			var intPos = 0;

			do {
				_stream.Position = intPos;
				_stream.Read(byteHeader, 0, 4);
				intPos++;
				LoadMP3Header(byteHeader);
			} while (!IsValidHeader() && _stream.Position != _stream.Length);

			if (_stream.Position == _stream.Length) {
				return;
			}

			intPos += 3;
			if (GetVersionIndex() == 3) // MPEG Version 1
			{
				// If single Channel then 17
				intPos += GetModeIndex() == 3 ? 17 : 32;
			}
			else // MPEG Version 2.0 or 2.5
			{
				// If single Channel then 17
				intPos += GetModeIndex() == 3 ? 9 : 17;
			}

			_stream.Position = intPos;
			_stream.Read(byteVBitRate, 0, 12);
			_isVBitRate = IsVBRHeader(byteVBitRate);

			BitsPerSample = GetBitrate();
			SampleRate = GetFrequency();
			Channels = GetChannels();
			Length = GetLength();

			_stream.Seek(0, SeekOrigin.Begin);
		}

		private void LoadMP3Header(IReadOnlyList<byte> c) {
			// this thing is quite interesting, it works like the following
			// c[0] = 00000011
			// c[1] = 00001100
			// c[2] = 00110000
			// c[3] = 11000000
			// the operator << means that we'll move the bits in that direction
			// 00000011 << 24 = 00000011000000000000000000000000
			// 00001100 << 16 =         000011000000000000000000
			// 00110000 << 24 =                 0011000000000000
			// 11000000       =                         11000000
			//                +_________________________________
			//                  00000011000011000011000011000000
			_someVariable = (ulong) (((c[0] & 255) << 24) | ((c[1] & 255) << 16) | ((c[2] & 255) << 8) | (c[3] & 255));
		}

		private int GetLayerIndex() {
			return (int) ((_someVariable >> 17) & 3);
		}

		private int GetBitrateIndex() {
			return (int) ((_someVariable >> 12) & 15);
		}

		private int GetFrequencyIndex() {
			return (int) ((_someVariable >> 10) & 3);
		}

		private int GetVersionIndex() {
			return (int) ((_someVariable >> 19) & 3);
		}

		private int GetModeIndex() {
			return (int) ((_someVariable >> 6) & 3);
		}

		private bool IsValidHeader() {
			int GetFrameSync() {
				return (int) ((_someVariable >> 21) & 2047);
			}

			int GetEmphasisIndex() {
				return (int) (_someVariable & 3);
			}

			return (GetFrameSync() & 2047) == 2047 &&
			       (GetVersionIndex() & 3) != 1 &&
			       (GetLayerIndex() & 3) != 0 &&
			       (GetBitrateIndex() & 15) != 0 &&
			       (GetBitrateIndex() & 15) != 15 &&
			       (GetFrequencyIndex() & 3) != 3 &&
			       (GetEmphasisIndex() & 3) != 2;
		}

		private bool IsVBRHeader(IReadOnlyList<byte> inputheader) {
			if (inputheader[0] != 88 || inputheader[1] != 105 || inputheader[2] != 110 || inputheader[3] != 103) {
				return false;
			}

			var flags = ((inputheader[4] & 255) << 24) | ((inputheader[5] & 255) << 16) | ((inputheader[6] & 255) << 8) |
			            (inputheader[7] & 255);
			if ((flags & 0x0001) == 1) {
				_vFrames = ((inputheader[8] & 255) << 24) | ((inputheader[9] & 255) << 16) | ((inputheader[10] & 255) << 8) |
				           (inputheader[11] & 255);
				return true;
			}

			_vFrames = -1;
			return true;
		}

		private int GetBitrate() {
			if (_isVBitRate) {
				var medFrameSize = (double) _fileSize / GetNumberOfFrames();
				return (int) (medFrameSize * GetFrequency() / (1000.0 * (GetLayerIndex() == 3 ? 12.0 : 144.0)));
			}

			int[,,] table = {
				// 0
				{
					{
						0,
						8,
						16,
						24,
						32,
						40,
						48,
						56,
						64,
						80,
						96,
						112,
						128,
						144,
						160,
						0
					}, {
						0,
						8,
						16,
						24,
						32,
						40,
						48,
						56,
						64,
						80,
						96,
						112,
						128,
						144,
						160,
						0
					}, {
						0,
						32,
						48,
						56,
						64,
						80,
						96,
						112,
						128,
						144,
						160,
						176,
						192,
						224,
						256,
						0
					}
				},

				// 1
				{
					{
						0,
						32,
						40,
						48,
						56,
						64,
						80,
						96,
						112,
						128,
						160,
						192,
						224,
						256,
						320,
						0
					}, {
						0,
						32,
						48,
						56,
						64,
						80,
						96,
						112,
						128,
						160,
						192,
						224,
						256,
						320,
						384,
						0
					}, {
						0,
						32,
						64,
						96,
						128,
						160,
						192,
						224,
						256,
						288,
						320,
						352,
						384,
						416,
						448,
						0
					}
				}
			};

			return table[GetVersionIndex() & 1, GetLayerIndex() - 1, GetBitrateIndex()];
		}

		private int GetFrequency() {
			int[,] table = {
				{
					32000,
					16000,
					8000
				}, // MPEG 2.5
				{
					0,
					0,
					0
				}, // reserved
				{
					22050,
					24000,
					16000
				}, // MPEG 2
				{
					44100,
					48000,
					32000
				} // MPEG 1
			};

			return table[GetVersionIndex(), GetFrequencyIndex()];
		}

		private int GetChannels() {
			return GetModeIndex() switch {
				1 => 2, // Joint Stereo
				2 => 2, // Dual Channel
				3 => 1, // Single Channel
				_ => 2 // Stereo
			};
		}

		private TimeSpan GetLength() {
			var lengthInSeconds = (int) (8 * _fileSize / 1000) / GetBitrate();
			var seconds = lengthInSeconds % 60;
			var totalMinutes = (lengthInSeconds - seconds) / 60;
			var minutes = totalMinutes % 60;
			var totalHours = (totalMinutes - minutes) / 60;
			return new TimeSpan(totalHours, minutes, seconds);
		}

		private int GetNumberOfFrames() {
			if (_isVBitRate) {
				return _vFrames;
			}

			var medFrameSize =
				(GetLayerIndex() == 3 ? 12 : 144) * (1000.0 * GetBitrate() / GetFrequency());

			return (int) (_fileSize / medFrameSize);
		}
	}
}