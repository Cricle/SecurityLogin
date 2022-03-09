using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
#if UNIX_SOCKET
using System.Net.Sockets;
#endif

namespace SecurityLogin.Cache
{
    internal static class Format
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public static int ParseInt32(ReadOnlySpan<char> s) => int.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
        public static bool TryParseInt32(ReadOnlySpan<char> s, out int value) => int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);
#endif
        internal const int
            MaxInt32TextLen = 11, // -2,147,483,648 (not including the commas)
            MaxInt64TextLen = 20; // -9,223,372,036,854,775,808 (not including the commas)

        public static int ParseInt32(string s) => int.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

        public static long ParseInt64(string s) => long.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

        public static string ToString(int value) => value.ToString(NumberFormatInfo.InvariantInfo);

        public static bool TryParseBoolean(string s, out bool value)
        {
            if (bool.TryParse(s, out value)) return true;

            if (s == "1" || string.Equals(s, "yes", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "on", StringComparison.OrdinalIgnoreCase))
            {
                value = true;
                return true;
            }
            if (s == "0" || string.Equals(s, "no", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "off", StringComparison.OrdinalIgnoreCase))
            {
                value = false;
                return true;
            }
            value = false;
            return false;
        }

        public static bool TryParseInt32(string s, out int value) =>
            int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

        internal static string ToString(long value) => value.ToString(NumberFormatInfo.InvariantInfo);

        internal static string ToString(ulong value) => value.ToString(NumberFormatInfo.InvariantInfo);

        internal static string ToString(double value)
        {
            if (double.IsInfinity(value))
            {
                if (double.IsPositiveInfinity(value)) return "+inf";
                if (double.IsNegativeInfinity(value)) return "-inf";
            }
            return value.ToString("G17", NumberFormatInfo.InvariantInfo);
        }

        internal static string ToString(object value)
        {
            if (value == null) return "";
            if (value is long l) return ToString(l);
            if (value is int i) return ToString(i);
            if (value is float f) return ToString(f);
            if (value is double d) return ToString(d);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }


        internal static bool TryParseDouble(string s, out double value)
        {
            if (string.IsNullOrEmpty(s))
            {
                value = 0;
                return false;
            }
            if (s.Length == 1 && s[0] >= '0' && s[0] <= '9')
            {
                value = (int)(s[0] - '0');
                return true;
            }
            // need to handle these
            if (string.Equals("+inf", s, StringComparison.OrdinalIgnoreCase) || string.Equals("inf", s, StringComparison.OrdinalIgnoreCase))
            {
                value = double.PositiveInfinity;
                return true;
            }
            if (string.Equals("-inf", s, StringComparison.OrdinalIgnoreCase))
            {
                value = double.NegativeInfinity;
                return true;
            }
            return double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out value);
        }

        internal static bool TryParseUInt64(string s, out ulong value) =>
            ulong.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

        internal static bool TryParseUInt64(ReadOnlySpan<byte> s, out ulong value) =>
            Utf8Parser.TryParse(s, out value, out int bytes, standardFormat: 'D') & bytes == s.Length;

        internal static bool TryParseInt64(ReadOnlySpan<byte> s, out long value) =>
            Utf8Parser.TryParse(s, out value, out int bytes, standardFormat: 'D') & bytes == s.Length;

        internal static bool CouldBeInteger(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length > MaxInt64TextLen) return false;
            bool isSigned = s[0] == '-';
            for (int i = isSigned ? 1 : 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c < '0' | c > '9') return false;
            }
            return true;
        }
        internal static bool CouldBeInteger(ReadOnlySpan<byte> s)
        {
            if (s.IsEmpty | s.Length > MaxInt64TextLen) return false;
            bool isSigned = s[0] == '-';
            for (int i = isSigned ? 1 : 0; i < s.Length; i++)
            {
                byte c = s[i];
                if (c < (byte)'0' | c > (byte)'9') return false;
            }
            return true;
        }

        internal static bool TryParseInt64(string s, out long value) =>
            long.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

        internal static bool TryParseDouble(ReadOnlySpan<byte> s, out double value)
        {
            if (s.IsEmpty)
            {
                value = 0;
                return false;
            }
            if (s.Length == 1 && s[0] >= '0' && s[0] <= '9')
            {
                value = (int)(s[0] - '0');
                return true;
            }
            // need to handle these
            if (CaseInsensitiveASCIIEqual("+inf", s) || CaseInsensitiveASCIIEqual("inf", s))
            {
                value = double.PositiveInfinity;
                return true;
            }
            if (CaseInsensitiveASCIIEqual("-inf", s))
            {
                value = double.NegativeInfinity;
                return true;
            }
            return Utf8Parser.TryParse(s, out value, out int bytes) & bytes == s.Length;
        }

        private static bool CaseInsensitiveASCIIEqual(string xLowerCase, ReadOnlySpan<byte> y)
        {
            if (y.Length != xLowerCase.Length) return false;
            for (int i = 0; i < y.Length; i++)
            {
                if (char.ToLower((char)y[i]) != xLowerCase[i]) return false;
            }
            return true;
        }

        internal static string GetString(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment) return GetString(buffer.First.Span);

            var arr = ArrayPool<byte>.Shared.Rent(checked((int)buffer.Length));
            var span = new Span<byte>(arr, 0, (int)buffer.Length);
            buffer.CopyTo(span);
            string s = GetString(span);
            ArrayPool<byte>.Shared.Return(arr);
            return s;
        }

        internal static unsafe string GetString(ReadOnlySpan<byte> span)
        {
            if (span.IsEmpty) return "";
            fixed (byte* ptr = span)
            {
                return Encoding.UTF8.GetString(ptr, span.Length);
            }
        }
        internal static int WriteRaw(Span<byte> span, long value, bool withLengthPrefix = false, int offset = 0)
        {
            if (value >= 0 && value <= 9)
            {
                if (withLengthPrefix)
                {
                    span[offset++] = (byte)'1';
                    offset = WriteCrlf(span, offset);
                }
                span[offset++] = (byte)((int)'0' + (int)value);
            }
            else if (value >= 10 && value < 100)
            {
                if (withLengthPrefix)
                {
                    span[offset++] = (byte)'2';
                    offset = WriteCrlf(span, offset);
                }
                span[offset++] = (byte)((int)'0' + ((int)value / 10));
                span[offset++] = (byte)((int)'0' + ((int)value % 10));
            }
            else if (value >= 100 && value < 1000)
            {
                int v = (int)value;
                int units = v % 10;
                v /= 10;
                int tens = v % 10, hundreds = v / 10;
                if (withLengthPrefix)
                {
                    span[offset++] = (byte)'3';
                    offset = WriteCrlf(span, offset);
                }
                span[offset++] = (byte)((int)'0' + hundreds);
                span[offset++] = (byte)((int)'0' + tens);
                span[offset++] = (byte)((int)'0' + units);
            }
            else if (value < 0 && value >= -9)
            {
                if (withLengthPrefix)
                {
                    span[offset++] = (byte)'2';
                    offset = WriteCrlf(span, offset);
                }
                span[offset++] = (byte)'-';
                span[offset++] = (byte)((int)'0' - (int)value);
            }
            else if (value <= -10 && value > -100)
            {
                if (withLengthPrefix)
                {
                    span[offset++] = (byte)'3';
                    offset = WriteCrlf(span, offset);
                }
                value = -value;
                span[offset++] = (byte)'-';
                span[offset++] = (byte)((int)'0' + ((int)value / 10));
                span[offset++] = (byte)((int)'0' + ((int)value % 10));
            }
            else
            {
                // we're going to write it, but *to the wrong place*
                var availableChunk = span.Slice(offset);
                if (!Utf8Formatter.TryFormat(value, availableChunk, out int formattedLength))
                {
                    throw new InvalidOperationException("TryFormat failed");
                }
                if (withLengthPrefix)
                {
                    // now we know how large the prefix is: write the prefix, then write the value
                    if (!Utf8Formatter.TryFormat(formattedLength, availableChunk, out int prefixLength))
                    {
                        throw new InvalidOperationException("TryFormat failed");
                    }
                    offset += prefixLength;
                    offset = WriteCrlf(span, offset);

                    availableChunk = span.Slice(offset);
                    if (!Utf8Formatter.TryFormat(value, availableChunk, out int finalLength))
                    {
                        throw new InvalidOperationException("TryFormat failed");
                    }
                    offset += finalLength;
                    Debug.Assert(finalLength == formattedLength);
                }
                else
                {
                    offset += formattedLength;
                }
            }

            return WriteCrlf(span, offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int WriteCrlf(Span<byte> span, int offset)
        {
            span[offset++] = (byte)'\r';
            span[offset++] = (byte)'\n';
            return offset;
        }

    }
}
