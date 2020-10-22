using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.IO
{
    public class BufferStream : MemoryStream
    {
        public BufferStream(ReadOnlyMemory<byte> buffer, bool writable, bool publiclyVisible)
        {

        }

        public override byte[] GetBuffer()
        {
            return base.GetBuffer();
        }

        public override bool TryGetBuffer(out ArraySegment<byte> buffer)
        {
            return base.TryGetBuffer(out buffer);
        }
    }
}
