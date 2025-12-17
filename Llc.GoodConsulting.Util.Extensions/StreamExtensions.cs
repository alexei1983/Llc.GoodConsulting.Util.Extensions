using System.Text;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// Extension methods for working with <see cref="Stream"/> objects.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this byte[] bytes) => new(bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this string? value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return new MemoryStream(encoding.GetBytes(value ?? string.Empty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static MemoryStream ToMemoryStreamFromBase64String(this string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                throw new ArgumentException("Base-64 string cannot be null or empty.", nameof(base64String));

            byte[] bytes;

            try
            {
                bytes = Convert.FromBase64String(base64String);
            }
            catch (FormatException fex)
            {
                throw new ArgumentException("Base-64 string is invalid.", nameof(base64String), fex);
            }
            return bytes.ToMemoryStream();
        }

        /// <summary>
        /// Reads the entire content of the <see cref="Stream"/> as a string using the specified encoding.
        /// </summary>
        /// <remarks>This method does not close the <see cref="Stream"/>.</remarks>
        public static string ReadAllText(this Stream stream, Encoding? encoding = null, bool rewind = true)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("Stream is not readable.");

            encoding ??= Encoding.UTF8;

            if (rewind && stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Asynchronously reads the entire content of the <see cref="Stream"/> as a string using the specified encoding.
        /// </summary>
        /// <remarks>This method does not close the <see cref="Stream"/>.</remarks>
        public static async Task<string> ReadAllTextAsync(this Stream stream, Encoding? encoding = null, bool rewind = true)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("Stream is not readable.");

            encoding ??= Encoding.UTF8;

            if (rewind && stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Writes a string into the <see cref="Stream"/> using the specified encoding.
        /// </summary>
        /// <remarks>This method does not close the <see cref="Stream"/>.</remarks>
        public static void WriteAllText(this Stream stream, string text, Encoding? encoding = null, bool rewind = true)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("Stream is not writable.");

            encoding ??= Encoding.UTF8;

            if (rewind && stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            using var writer = new StreamWriter(stream, encoding, bufferSize: 1024, leaveOpen: true);
            writer.Write(text);
            writer.Flush();

            if (rewind && stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Asynchronously writes a string into the <see cref="Stream"/> using the specified encoding.
        /// </summary>
        /// <remarks>This method does not close the <see cref="Stream"/>.</remarks>
        public static async Task WriteAllTextAsync(this Stream stream, string text, Encoding? encoding = null, bool rewind = true)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("Stream is not writable.");

            encoding ??= Encoding.UTF8;

            if (rewind && stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            using var writer = new StreamWriter(stream, encoding, bufferSize: 1024, leaveOpen: true);
            await writer.WriteAsync(text);
            await writer.FlushAsync();

            if (rewind && stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
