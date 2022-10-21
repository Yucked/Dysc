using System;

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
                case byte[] {Length: 0}:
                    throw new Exception("Array cannot be empty or have length of 0.");
                case null:
                    throw new ArgumentNullException(argumentName, "Argument cannot be null.");
                case <= -1:
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
    }
}