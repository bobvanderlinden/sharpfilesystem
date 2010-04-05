using System;
using System.IO;

namespace System.IO
{
    public class SafeNetworkStream : Stream
    {
        private Stream _stream;

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public SafeNetworkStream(Stream stream)
        {
            _stream = stream;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return _stream.Read(buffer, offset, count);
            }
            catch (IOException)
            {
                return 0;
            }
            catch (ObjectDisposedException)
            {
                return 0;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                _stream.Write(buffer, offset, count);
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override void Close()
        {
            try
            {
                _stream.Close();
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}