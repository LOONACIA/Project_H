using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LOONACIA.Unity.Collections
{
    /// <summary>
    /// Implementations of a list that uses the given Span or <see cref="System.Buffers.ArrayPool{T}"/> to minimize heap allocations.
    /// This list must be disposed after use to return the underlying array to the pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public ref struct ValueList<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// This enum determines the behavior of <see cref="ValueList{T}"/> when a given array is used as a buffer.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Use the given array as a buffer and set the size to zero.
            /// </summary>
            Clear,

            /// <summary>
            /// Use the given array as a buffer and set the size to the length of the array.
            /// </summary>
            Reference,

            /// <summary>
            /// Copy the given array and use the copied array as a buffer.
            /// </summary>
            Copy
        }
        
        private const int MaxLength = 0X7FFFFFC7;
        
        private const int DefaultCapacity = 4;
        
        private readonly bool _clearOnReturn;

        private T[] _items;

        private Span<T> _buffer;

        private int _capacity;

        private int _size;
        
        public ValueList(int capacity = DefaultCapacity, ClearMode clearMode = ClearMode.ReferenceTypeOnly)
        {
            _clearOnReturn = IsClearOnReturn(clearMode);
            _items = ArrayPool<T>.Shared.Rent(capacity);
            _buffer = _items;
            _capacity = capacity;
            _size = 0;
        }

        public ValueList(Span<T> source, Mode mode = Mode.Clear, ClearMode clearMode = ClearMode.ReferenceTypeOnly)
        {
            _capacity = source.Length;
            _clearOnReturn = IsClearOnReturn(clearMode);

            switch (mode)
            {
                case Mode.Clear:
                    _items = null;
                    _buffer = source;
                    _size = 0;
                    break;
                case Mode.Reference:
                    _items = null;
                    _buffer = source;
                    _size = _capacity;
                    break;
                case Mode.Copy:
                    _items = ArrayPool<T>.Shared.Rent(_capacity);
                    _buffer = _items;
                    _size = source.Length;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Capacity cannot be negative.");
                }

                ResizeBuffer(value);
            }
        }

        public int Count => _size;

        public readonly ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return ref _buffer[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            int count = _size;
            InsertItemCore(count, item);
        }

        public void AddRange(T[] items)
        {
            var span = _buffer;
            int count = _size;
            int newCapacity = count + items.Length;
            EnsureCapacity(newCapacity);
            items.CopyTo(span[count..]);
        }

        public void AddRange(ReadOnlySpan<T> items)
        {
            var span = _buffer;
            int count = _size;
            int newCapacity = count + items.Length;
            EnsureCapacity(newCapacity);
            items.CopyTo(span[count..]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            if (index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            RemoveItemCore(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int IndexOf(T item) => _buffer[.._size].IndexOf(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(T item) => IndexOf(item) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Span<T> AsSpan() => _buffer[.._size];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator GetEnumerator() => new(_buffer[.._size]);

        public void EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative.");
            }

            if (_capacity < capacity)
            {
                Capacity = capacity;
            }
        }

        public T[] ToArray()
        {
            return _size == 0 ? Array.Empty<T>() : _buffer[.._size].ToArray();
        }

        public void Dispose()
        {
            T[] buffer = _items;
            if (buffer is not null)
            {
                ArrayPool<T>.Shared.Return(buffer, _clearOnReturn);
            }

            _size = 0;
        }
        
        private static bool IsClearOnReturn(ClearMode clearMode)
        {
            return clearMode switch
            {
                ClearMode.ReferenceTypeOnly => RuntimeHelpers.IsReferenceOrContainsReferences<T>(),
                ClearMode.Always => true,
                ClearMode.Never => false,
                _ => throw new ArgumentOutOfRangeException(nameof(clearMode))
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertItemCore(int index, T item)
        {
            ResizeBufferIfRequired();
            var span = _buffer;
            int count = _size;

            if (index != count)
            {
                span[index..count].CopyTo(span[(index + 1)..count]);
            }

            span[index] = item;
            _size = count + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveItemCore(int index)
        {
            var span = _buffer;
            int count = _size;

            span[(index + 1)..count].CopyTo(span[index..count]);
            if (index >= count - 1 && _clearOnReturn)
            {
                span[index..(index + 1)].Clear();
            }

            _size = count - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResizeBufferIfRequired()
        {
            if (_size >= _capacity)
            {
                ResizeBuffer(_size + 1);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ResizeBuffer(int capacity)
        {
            int newCapacity = _buffer.Length == 0 ? DefaultCapacity : _size * 2;

            if (newCapacity > MaxLength)
            {
                newCapacity = MaxLength;
            }

            if (newCapacity < capacity)
            {
                newCapacity = capacity;
            }

            var arrayPool = ArrayPool<T>.Shared;
            if (_items is null)
            {
                _items = arrayPool.Rent(newCapacity);
            }
            else
            {
                T[] oldBuffer = _items;
                T[] newItems = arrayPool.Rent(newCapacity);

                Array.Copy(oldBuffer, 0, newItems, 0, _size);

                _items = newItems;

                arrayPool.Return(oldBuffer, _clearOnReturn);
            }

            _buffer = _items.AsSpan();
            _capacity = newCapacity;
        }

        public ref struct Enumerator
        {
            private readonly Span<T> _source;

            private int _index;

            internal Enumerator(Span<T> source)
            {
                _source = source;
                _index = -1;
            }

            public readonly ref T Current => ref _source[_index];

            public bool MoveNext() => ++_index < _source.Length;

            public void Reset()
            {
                _index = -1;
            }
        }
    }
}