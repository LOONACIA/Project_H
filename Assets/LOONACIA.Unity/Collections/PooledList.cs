using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LOONACIA.Unity.Collections
{
	/// <summary>
	/// Implementation of a list that uses <see cref="System.Buffers.ArrayPool{T}"/> to rent and return the underlying array.
	/// This list must be disposed after use to return the underlying array to the pool.
	/// </summary>
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class PooledList<T> : IList<T>, IList, IReadOnlyList<T>, IDisposable
	{
		private const int MaxLength = 0X7FFFFFC7;
		
		private const int DefaultCapacity = 4;

		private readonly ArrayPool<T> _pool = ArrayPool<T>.Shared;

		private readonly bool _clearOnReturn;

		private T[] _items;

		private int _size;

		private int _capacity;

		public PooledList() : this(DefaultCapacity)
		{
		}

		public PooledList(int capacity, ClearMode clearMode = ClearMode.ReferenceTypeOnly)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative.");
			}

			_capacity = capacity;
			_clearOnReturn = IsClearOnReturn(clearMode);
			_items = _pool.Rent(capacity);
		}

		public PooledList(IEnumerable<T> collection, ClearMode clearMode = ClearMode.ReferenceTypeOnly)
		{
			_clearOnReturn = IsClearOnReturn(clearMode);
			
			if (collection is ICollection<T> c)
			{
				int length = c.Count;
				_items = _pool.Rent(length);
				_capacity = length;
				c.CopyTo(_items, 0);
				_size = length;
			}
			else
			{
				_capacity = DefaultCapacity;
				_items = _pool.Rent(_capacity);

				using var enumerator = collection.GetEnumerator();
				while (enumerator.MoveNext())
				{
					InsertItemCore(Count, enumerator.Current);
				}
			}
		}

		public T this[int index]
		{
			get
			{
				if (index > _size)
				{
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				return _items[index];
			}
			set
			{
				if (index > _size)
				{
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				SetItem(index, value);
			}
		}

		object IList.this[int index]
		{
			get => this[index];
			set
			{
				if (value is not T item)
				{
					throw new ArgumentException($"{value} is invalid type for this list.");
				}

				this[index] = item;
			}
		}

		public int Count => _size;

		public bool IsReadOnly => false;

		public bool IsFixedSize => false;

		public bool IsSynchronized => false;

		public object SyncRoot => this;

		public int Capacity
		{
			get => _capacity;
			set => EnsureCapacity(value);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			_pool.Return(_items, _clearOnReturn);
			_size = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			int index = Count;
			InsertItem(index, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Add(object value)
		{
			if (value is not T item)
			{
				throw new ArgumentException($"{value} is invalid type for this list.");
			}

			Add(item);

			return Count - 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			ClearItems();
		}

		public bool Contains(T item) => _items.Contains(item);

		public bool Contains(object value)
		{
			if (value is not T item)
			{
				throw new ArgumentException($"{value} is invalid type for this list.");
			}

			return Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, _size);

		public void CopyTo(Array array, int index) => Array.Copy(_items, 0, array, index, _size);

		public Enumerator GetEnumerator() => new(_items, _size);

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(_items, _size);

		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_items, _size);

		public int IndexOf(T item) => Array.IndexOf(_items, item);

		public int IndexOf(object value)
		{
			if (value is not T item)
			{
				throw new ArgumentException($"{value} is invalid type for this list.");
			}

			return IndexOf(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, T item)
		{
			InsertItem(index, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, object value)
		{
			if (value is not T item)
			{
				throw new ArgumentException($"{value} is invalid type for this list.");
			}

			Insert(index, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(T item)
		{
			int index = IndexOf(item);

			if (index < 0)
			{
				return false;
			}

			RemoveItem(index, item);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(object value)
		{
			if (value is not T item)
			{
				throw new ArgumentException($"{value} is invalid type for this list.");
			}

			Remove(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int index)
		{
			T item = this[index];
			RemoveItem(index, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAll(Predicate<T> predicate)
		{
			int index = 0;
			int length = _size;
			var span = _items.AsSpan();

			while (index < length && !predicate(span[index]))
			{
				index++;
			}

			if (index >= length)
			{
				return;
			}

			int cursor = index;
			while (cursor < length)
			{
				if (predicate(span[cursor]))
				{
					cursor++;
				}
				else
				{
					span[index++] = span[cursor++];
				}
			}

			RemoveItems(index, length - index, span[index..length]);
		}

		public Span<T> AsSpan() => _items.AsSpan(0, _size);
		
		public ReadOnlyCollection<T> AsReadOnly() => new(this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void InsertItem(int index, T item)
		{
			InsertItemCore(index, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void ClearItems()
		{
			int count = _size;

			if (count > 0 && _clearOnReturn)
			{
				Array.Clear(_items, 0, _items.Length);
			}

			_size = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void RemoveItem(int index, T item)
		{
			RemoveItemCore(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void RemoveItems(int index, int count, ReadOnlySpan<T> items)
		{
			var span = _items.AsSpan();
			int end = index + count;
			if (end < _size)
			{
				span[(end + 1).._size].CopyTo(span[index..]);
			}
			span[end.._size].Clear();
			_size -= count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void SetItem(int index, T item)
		{
			_items[index] = item;
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
			ResizeIfRequired();
			var span = _items.AsSpan();
			if (index != _size)
			{
				span[index.._size].CopyTo(span[(index + 1).._size]);
			}
			span[index] = item;
			++_size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveItemCore(int index)
		{
			var span = _items.AsSpan();
			int count = _size;
			
			span[(index + 1)..count].CopyTo(span[index..count]);
			if (index >= count - 1 && _clearOnReturn)
			{
				span[index..(index + 1)].Clear();
			}
			--_size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ResizeIfRequired()
		{
			if (_size >= _capacity)
			{
				Grow(_size + 1);
			}
		}

		public void EnsureCapacity(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative.");
			}

			if (_capacity < capacity)
			{
				Grow(capacity);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Grow(int capacity)
		{
			int newCapacity = _items.Length == 0 ? DefaultCapacity : _size * 2;

			if (newCapacity > MaxLength)
			{
				newCapacity = MaxLength;
			}
			else if (newCapacity < capacity)
			{
				newCapacity = capacity;
			}

			T[] oldItems = _items;
			T[] newItems = _pool.Rent(newCapacity);

			Array.Copy(oldItems, 0, newItems, 0, _size);

			_items = newItems;
			_capacity = newCapacity;

			_pool.Return(oldItems, _clearOnReturn);
		}

		public struct Enumerator : IEnumerator<T>
		{
			private readonly T[] _source;

			private readonly int _count;

			private int _index;

			internal Enumerator(T[] source, int count)
			{
				_source = source;
				_count = count;
				_index = -1;
			}

			public readonly ref T Current => ref _source[_index];

			T IEnumerator<T>.Current => _source[_index];

			object IEnumerator.Current => _source[_index];

			public readonly void Dispose()
			{

			}

			public bool MoveNext() => ++_index < _count;

			public void Reset()
			{
				_index = -1;
			}
		}
	}
}