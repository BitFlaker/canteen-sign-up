/*
    Source: https://github.com/htlvb/Htlvb.Http/blob/main/Htlvb.Http/HttpStreamReader.cs
 */

using System;
using System.IO;
using System.Text;

namespace Htlvb.Http
{
    public class HttpStreamReader
    {
        private readonly Stream stream;
        private Encoding encoding;
        private Decoder decoder;
        private readonly byte[] byteBuffer;
        private int byteBufferEnd;
        private readonly char[] charBuffer;
        private int charBufferEnd;

        public HttpStreamReader(Stream stream) : this(stream, Encoding.ASCII, 1024, 1024)
        {
        }

        internal HttpStreamReader(Stream stream, Encoding encoding, int byteBufferSize, int charBufferSize)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (byteBufferSize < 2)
            {
                throw new ArgumentException("Byte buffer size must be >= 2");
            }
            byteBuffer = new byte[byteBufferSize];
            if (charBufferSize < 1)
            {
                throw new ArgumentException("Char buffer size must be >= 1");
            }
            charBuffer = new char[charBufferSize];
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public Encoding Encoding
        {
            get
            {
                return encoding;
            }
            set
            {
                encoding = value;
                decoder = encoding.GetDecoder();
                decoder.Convert(byteBuffer, 0, byteBufferEnd, charBuffer, 0, charBuffer.Length, false, out var bytesConverted, out charBufferEnd, out var completed);
            }
        }

        public string ReadLine()
        {
            StringBuilder line = new StringBuilder();
            while (!ReadLineFromBuffer(line))
            {
                var bytesRead = stream.Read(byteBuffer, byteBufferEnd, byteBuffer.Length - byteBufferEnd);
                decoder.Convert(byteBuffer, byteBufferEnd, bytesRead, charBuffer, charBufferEnd, charBuffer.Length - charBufferEnd, false, out var bytesConverted, out var charsConverted, out var completed);
                byteBufferEnd += bytesRead;
                charBufferEnd += charsConverted;
            }
            return line.ToString();
        }

        public string ReadBytesAsText(int numberOfBytes)
        {
            StringBuilder result = new StringBuilder();
            int numberOfBytesToRead = numberOfBytes;

            if (byteBufferEnd >= numberOfBytesToRead)
            {
                int charBufferIndex = 0;
                while (true)
                {
                    numberOfBytesToRead -= encoding.GetByteCount(charBuffer, charBufferIndex, 1);
                    if (numberOfBytesToRead < 0)
                    {
                        break;
                    }
                    result.Append(charBuffer, charBufferIndex, 1);
                    charBufferIndex++;
                }
                ResetBuffer(charBufferIndex);
                return result.ToString();
            }

            result.Append(charBuffer, 0, charBufferEnd);
            numberOfBytesToRead -= encoding.GetByteCount(charBuffer, 0, charBufferEnd);
            ResetBuffer(charBufferEnd);

            while (numberOfBytesToRead > 0)
            {
                var bytesRead = stream.Read(byteBuffer, byteBufferEnd, Math.Min(byteBuffer.Length - byteBufferEnd, numberOfBytesToRead));
                numberOfBytesToRead -= bytesRead;
                decoder.Convert(byteBuffer, byteBufferEnd, bytesRead, charBuffer, charBufferEnd, charBuffer.Length, false, out var bytesConverted, out var charsConverted, out var completed);
                byteBufferEnd += bytesRead;
                charBufferEnd += charsConverted;
                result.Append(charBuffer, 0, charBufferEnd);
                ResetBuffer(charBufferEnd);
            }
            return result.ToString();
        }

        private bool ReadLineFromBuffer(StringBuilder line)
        {
            var charsConsumed = 0;
            while (charsConsumed < charBufferEnd)
            {
                if (charBuffer[charsConsumed] == '\r')
                {
                    if (charsConsumed + 1 >= charBufferEnd)
                    {
                        ResetBuffer(charsConsumed);
                        return false;
                    }
                    else if (charBuffer[charsConsumed + 1] == '\n')
                    {
                        charsConsumed += 2;
                        ResetBuffer(charsConsumed);
                        return true;
                    }
                }
                line.Append(charBuffer[charsConsumed]);
                charsConsumed++;
            }
            ResetBuffer(charsConsumed);
            return false;
        }

        private void ResetBuffer(int charsConsumed)
        {
            var consumedBytes = encoding.GetByteCount(charBuffer, 0, charsConsumed);

            charBufferEnd -= charsConsumed;
            Array.Copy(charBuffer, charsConsumed, charBuffer, 0, charBufferEnd);

            byteBufferEnd -= consumedBytes;
            Array.Copy(byteBuffer, consumedBytes, byteBuffer, 0, byteBufferEnd);
        }
    }
}