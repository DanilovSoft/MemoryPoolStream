using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.IO
{
    public class SpanStream : Stream
    {
        private readonly Memory<byte> _writableMemory;
        private readonly ReadOnlyMemory<byte> _readonlyMem;
        private int _position;
        private int _disposed;

        public SpanStream(Memory<byte> memory)
        {
            _readonlyMem = memory;
            _writableMemory = memory;
            CanWrite = true;
        }

        public SpanStream(ReadOnlyMemory<byte> memory)
        {
            CanWrite = false;
            _readonlyMem = memory;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite { get; }

        public override long Length => _readonlyMem.Length;

        public override long Position { get => _position; set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();

            // Проверить пользовательские параметры на выход за границы массива.
            if (count <= buffer.Length)
            {
                // Сколько данных осталось в стриме.
                int sizeLeft = _readonlyMem.Length - _position;
                if (sizeLeft > 0)
                {
                    if (count > sizeLeft)
                        count = sizeLeft;

                    // Копируем из внутреннего буфера в пользовательский.
                    _readonlyMem.Slice(_position, count).CopyTo(buffer.AsMemory(offset, count));

                    // Увеличить позицию стрима.
                    _position += count;

                    // Вернуть сколько байт было скопировано.
                    return count;
                }
                else
                {
                    // Достигнут конец стрима.
                    return 0;
                }
            }
            else
            {
                // Входные параметры невалидны.
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer size less than requested count.");
            }
        }

        public override int Read(Span<byte> buffer)
        {
            return base.Read(buffer);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();

            int newPosition;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        newPosition = (int)offset;
                    }
                    break;
                case SeekOrigin.Current:
                    {
                        newPosition = (int)(_position + offset);
                    }
                    break;
                case SeekOrigin.End:
                    // Сдвинуть указатель вперед начиная с конца.
                    {
                        newPosition = (int)(_readonlyMem.Length + offset);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }

            // Позиция не может быть отрицательной.
            if (newPosition >= 0)
            {
                _position = newPosition;

                // Нужно вернуть итоговую позицию.
                return newPosition;
            }
            else
            // newPosition меньше нуля.
            {
                throw new IOException("An attempt was made to move the position before the beginning of the stream.");
            }
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <exception cref="ObjectDisposedException"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfDisposed()
        {
            if (_disposed == 0)
                return;

            throw new ObjectDisposedException(GetType().FullName);
        }

        public override bool CanTimeout => false;
    }
}
