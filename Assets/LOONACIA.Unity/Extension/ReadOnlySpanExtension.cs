using System;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity
{
	public static class ReadOnlySpanExtension
	{
		public static SpanSplitEnumerator Split(this ReadOnlySpan<char> src, char separator) => new(src, new char[] { separator });

		public static SpanSplitEnumerator Split(this ReadOnlySpan<char> src, ReadOnlySpan<char> separator) => new(src, separator);

		public ref struct SpanSplitEnumerator
		{
			private readonly ReadOnlySpan<char> _source;

			private ReadOnlySpan<char> _string;

			private readonly ReadOnlySpan<char> _separator;

			private int _index;

			private bool _isEnd;

			public SpanSplitEnumerator(ReadOnlySpan<char> source, ReadOnlySpan<char> separator)
			{
				_source = _string = source;
				_separator = separator;
				_index = 0;
				_isEnd = false;
				Current = default;
			}

			public SpanSplitEntry Current { get; private set; }

			public readonly SpanSplitEnumerator GetEnumerator() => this;

			public bool MoveNext()
			{
				if (_isEnd)
				{
					return false;
				}

				int index = _string.IndexOf(_separator);
				if (index >= 0)
				{
					Current = new(_string[..index], _index++);
					_string = _string[(index + _separator.Length)..];
				}
				else
				{
					Current = new(_string, _index++);
					_string = ReadOnlySpan<char>.Empty;
					_isEnd = true;
				}

				return true;
			}

			public void Reset()
			{
				_index = 0;
				_isEnd = false;
				Current = default;
				_string = _source;
			}

			public readonly ref struct SpanSplitEntry
			{
				internal SpanSplitEntry(ReadOnlySpan<char> chars, int index)
				{
					Chars = chars;
					Index = index;
				}

				public readonly ReadOnlySpan<char> Chars;

				public readonly int Index;
			}
		}
	}
}
