using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using ImageMagick;

namespace ConsoleAppTest
{
    class Program
    {
        private static BufferStream _bufStream;

        static void Main()
        {
            byte[] data = File.ReadAllBytes("E:\\Temp\\st_ak_a101.png");

            Memory<byte> mem = data;

            var stream = new SpanStream((ReadOnlyMemory<byte>)mem);
            _bufStream = new BufferStream(new byte[10], writable: false, publiclyVisible: true);

            using (var image = new MagickImage(stream))
            {
                if (image.HasAlpha)
                {
                    // Сначала нужно задать цвет фона.
                    image.BackgroundColor = MagickColors.White;

                    // Затем удалим 4-й канал - прозрачность.
                    image.Alpha(AlphaOption.Remove);
                }
            var buf = new byte[5];
            int n = _bufStream.Read(buf);

                var colors = image.TotalColors;

                image.FilterType = ImageMagick.FilterType.Lanczos;
                image.Resize(500, 0);

                //image.Quantize(new QuantizeSettings
                //{
                //    Colors = 256,
                //    DitherMethod = DitherMethod.No
                //});
                
                image.Write("E:\\Temp\\test.png", MagickFormat.Png8);
            }
        }
    }
}
