using System;

namespace Dysc.Decoders {
	/// <summary>
	/// 
	/// </summary>
	public interface IDecoder {
		/// <summary>
		/// 
		/// </summary>
		int BitsPerSample { get; }

		/// <summary>
		/// 
		/// </summary>
		int BytesPerSample { get; }

		/// <summary>
		/// 
		/// </summary>
		int BytesPerSecond { get; }

		/// <summary>
		/// 
		/// </summary>
		int Channels { get; }
		
		/// <summary>
		/// 
		/// </summary>
		int SampleRate { get; }
		
		/// <summary>
		/// 
		/// </summary>
		int BlockAlign { get; }
		
		/// <summary>
		/// 
		/// </summary>
		TimeSpan Length { get; }

		/// <summary>
		/// 
		/// </summary>
		void Decode();
	}
}