using System;
using System.Buffers;
using System.IO;
using System.Threading;

namespace ConsoleAppTest
{
    class Program
    {
        private static BufferStream _bufStream;

        static void Main()
        {
            _bufStream = new BufferStream(new byte[10], writable: false, publiclyVisible: true);

            var buf = new byte[5];
            int n = _bufStream.Read(buf);

        }
    }
}
