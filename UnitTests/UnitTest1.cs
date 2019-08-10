using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var mem = new MemoryStream())
            {
                mem.Position = 1030;
                byte[] buffer = mem.GetBuffer();

                Assert.AreEqual(0, mem.Capacity);
                Assert.AreEqual(0, mem.Length);
                Assert.AreEqual(0, buffer.Length);
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            using (var mem = new MemoryStream())
            {
                for (int i = 0; i < 1024; i++)
                {
                    mem.WriteByte(0);
                }

                Assert.IsTrue(mem.Capacity >= 1024);
                Assert.AreEqual(1024, mem.Length);
            }
        }

        /// <summary>
        /// Зануление правой части стрима после смещения позиции.
        /// </summary>
        [TestMethod]
        public void TestMemoryPoolStream1()
        {
            var ar = ArrayPool<byte>.Shared.Rent(16);
            Array.Fill<byte>(ar, 1);
            ArrayPool<byte>.Shared.Return(ar);

            using (var mem = new MemoryPoolStream())
            {
                for (int i = 0; i < 10; i++)
                {
                    mem.WriteByte(1);
                }

                // Пропускаем 5 байт.
                mem.Position = 15;

                mem.WriteByte(2);
            }
        }

        [TestMethod]
        public void TestMemoryPoolStream2()
        {
            using (var mem = new MemoryPoolStream())
            {
                for (int i = 0; i < 10; i++)
                {
                    mem.WriteByte(1);
                }
                mem.Position = 0;
                mem.Read(new byte[3], 0, 3);
                mem.Seek(0, SeekOrigin.End);
                mem.Capacity = 20;

                var ar = new byte[10];
                Array.Fill<byte>(ar, 1);
                mem.Write(ar, 0, 10);
            }
        }

        [TestMethod]
        public void TestReadByte()
        {
            using (var mem = new MemoryPoolStream())
            {
                var buf = new byte[19];
                Array.Fill<byte>(buf, 1);
                mem.Write(buf, 0, buf.Length);

                mem.Position = 0;

                int b = mem.ReadByte();
                Assert.AreEqual(1, b);
            }
        }
    }
}
