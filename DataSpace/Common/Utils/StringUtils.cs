//-----------------------------------------------------------------------
// <copyright file="StringUtils.cs" company="GRAU DATA AG">
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General private License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//   GNU General private License for more details.
//
//   You should have received a copy of the GNU General private License
//   along with this program. If not, see http://www.gnu.org/licenses/.
//
// </copyright>
//-----------------------------------------------------------------------

namespace DataSpace.Common.Utils {
    using System;
    using System.Text;

    /// <summary>
    /// String utils.
    /// </summary>
    public static class StringUtils {
        /// <summary>
        /// Tos the hex string.
        /// </summary>
        /// <returns>The hex string.</returns>
        /// <param name="data">byte array.</param>
        public static string ToHexString(this byte[] data) {
            if (data == null) {
                return "(null)";
            } else {
                return BitConverter.ToString(data).Replace("-", string.Empty);
            }
        }

        /// <summary>
        /// Determines whether this instance is valid ISO-8859-15 specified input.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is valid ISO-8859-15 specified input; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='input'>
        /// If set to <c>true</c> input.
        /// </param>
        public static bool IsValidISO885915(this string input) {
            byte[] bytes = Encoding.GetEncoding(28605).GetBytes(input);
            string result = Encoding.GetEncoding(28605).GetString(bytes);
            return string.Equals(input, result);
        }

        /// <summary>
        /// Format a file size nicely.
        /// Example: 1048576 becomes "1 MB"
        /// </summary>
        /// <param name="byteCount">byte count</param>
        /// <returns>Formatted file size</returns>
        public static string AsFormattedFileSize(this double byteCount) {
            if (byteCount >= 1099511627776) {
                return string.Format("{0:##.##} TB", Math.Round(byteCount / 1099511627776, 1));
            } else if (byteCount >= 1073741824) {
                return string.Format("{0:##.##} GB", Math.Round(byteCount / 1073741824, 1));
            } else if (byteCount >= 1048576) {
                return string.Format("{0:##.##} MB", Math.Round(byteCount / 1048576, 0));
            } else if (byteCount >= 1024) {
                return string.Format("{0:##.##} KB", Math.Round(byteCount / 1024, 0));
            } else {
                return byteCount.ToString() + " bytes";
            }
        }

        /// <summary>
        /// Formats the bandwidth in typical 10 based calculation
        /// </summary>
        /// <returns>
        /// The bandwidth.
        /// </returns>
        /// <param name='bitsPerSecond'>
        /// Bits per second.
        /// </param>
        public static string AsFormattedBandwidth(this double bitsPerSecond) {
            if (bitsPerSecond >= (1000d * 1000d * 1000d * 1000d)) {
                return string.Format("{0:##.##} TBit/s", Math.Round(bitsPerSecond / (1000d * 1000d * 1000d * 1000d), 1));
            } else if (bitsPerSecond >= (1000d * 1000d * 1000d)) {
                return string.Format("{0:##.##} GBit/s", Math.Round(bitsPerSecond / (1000d * 1000d * 1000d), 1));
            } else if (bitsPerSecond >= (1000d * 1000d)) {
                return string.Format("{0:##.##} MBit/s", Math.Round(bitsPerSecond / (1000d * 1000d), 1));
            } else if (bitsPerSecond >= 1000d) {
                return string.Format("{0:##.##} KBit/s", Math.Round(bitsPerSecond / 1000d, 1));
            } else {
                return bitsPerSecond.ToString() + " Bit/s";
            }
        }

        /// <summary>
        /// Formats the bandwidth in typical 10 based calculation
        /// </summary>
        /// <returns>
        /// The bandwidth.
        /// </returns>
        /// <param name='bitsPerSecond'>
        /// Bits per second.
        /// </param>
        public static string AsFormattedBandwidth(this long bitsPerSecond) {
            return ((double)bitsPerSecond).AsFormattedBandwidth();
        }

        /// <summary>
        /// Formats the given double with a leading and tailing zero and appends percent char
        /// </summary>
        /// <returns>
        /// The formatted percent.
        /// </returns>
        /// <param name='p'>
        /// the percentage
        /// </param>
        public static string AsFormattedPercent(this double p) {
            return string.Format("{0:0.0} %", Math.Truncate(p * 10) / 10);
        }

        /// <summary>
        /// Formats the given int with a leading and tailing zero and appends percent char
        /// </summary>
        /// <returns>
        /// The formatted percent.
        /// </returns>
        /// <param name='p'>
        /// the percentage
        /// </param>
        public static string AsFormattedPercent(this int p) {
            return ((double)p).AsFormattedPercent();
        }

        /// <summary>
        /// Format a file size nicely.
        /// Example: 1048576 becomes "1 MB"
        /// </summary>
        /// <param name="byteCount">byte count</param>
        /// <returns>The formatted size</returns>
        public static string AsFormattedFileSize(this long byteCount) {
            return ((double)byteCount).AsFormattedFileSize();
        }
    }
}