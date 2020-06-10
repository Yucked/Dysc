using System;
using System.Net.Http;

namespace Dysc {
	/// <summary>
	/// 
	/// </summary>
	public readonly struct Guard {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="argumentName"></param>
		/// <param name="argument"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void NotNull(string argumentName, object argument) {
			switch (argument) {
				case string str when string.IsNullOrWhiteSpace(str):
					throw new ArgumentNullException(argumentName, "String cannot be null/empty/whitespace.");
				case byte[] byteArray when byteArray.Length == 0:
					throw new Exception("Array cannot be empty or have length of 0.");
				case var _ when argument == null:
					throw new ArgumentNullException(argumentName, "Argument cannot be null.");
				case int num when num <= -1:
					throw new ArgumentOutOfRangeException(argumentName, "Value must be higher than or equal to 0.");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argumentName"></param>
		/// <param name="argument"></param>
		/// <exception cref="ArgumentException"></exception>
		public static void IsValidUrl(string argumentName, string argument) {
			if (Uri.IsWellFormedUriString(argument, UriKind.Absolute)) {
				return;
			}

			throw new ArgumentException("A valid URI string must be provided.", argumentName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="responseMessage"></param>
		/// <exception cref="HttpRequestException"></exception>
		public static void IsSuccessfulResponse(HttpResponseMessage responseMessage) {
			if (responseMessage.IsSuccessStatusCode) {
				return;
			}

			throw new HttpRequestException(responseMessage.ReasonPhrase);
		}
	}
}