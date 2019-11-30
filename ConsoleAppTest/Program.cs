using System;
using System.Buffers;
using System.IO;
using System.Threading;

namespace ConsoleAppTest
{
    class Program
    {
        private static MemoryPoolStream _poolStream = new MemoryPoolStream(16);

        static void Main()
        {
            var mem = new MemoryStream(16);
            byte[] buf = mem.GetBuffer();

            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)i;
            }

            _poolStream = null;
            Test();
            return;
        }

        static void Test()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(-1);
        }
    }
}
