using ElfCore.Channels;
using ElfCore.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ElfCore.Util
{
	/// <summary>
	/// RasterChannel enumerator class
	/// http://msdn.microsoft.com/en-us/library/system.collections.ienumerable.getenumerator.aspx
	/// </summary>
	public class ChannelEnum : IEnumerator
	{
		public ChannelList _list;

		// Enumerators are positioned before the first element 
		// until the first MoveNext() call. 
		int position = -1;

		public ChannelEnum(ChannelList list)
		{
			_list = list;
		}

		public bool MoveNext()
		{
			position++;
			return (position < _list.Count);
		}

		public void Reset()
		{
			position = -1;
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public Channel Current
		{
			get
			{
				try
				{
					return _list[position];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}
	}

	public class ChannelGroupEnum : IEnumerator
	{
		public List<ChannelGroup> _list;

		// Enumerators are positioned before the first element 
		// until the first MoveNext() call. 
		int position = -1;

		public ChannelGroupEnum(List<ChannelGroup> list)
		{
			_list = list;
		}

		public bool MoveNext()
		{
			position++;
			return (position < _list.Count);
		}

		public void Reset()
		{
			position = -1;
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public ChannelGroup Current
		{
			get
			{
				try
				{
					return _list[position];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}
	}

	public class ShuffleEnum : IEnumerator
	{
		public ShuffleList _list;

		// Enumerators are positioned before the first element 
		// until the first MoveNext() call. 
		int position = -1;

		public ShuffleEnum(ShuffleList list)
		{
			_list = list;
		}

		public bool MoveNext()
		{
			position++;
			return (position < _list.Count);
		}

		public void Reset()
		{
			position = -1;
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public Shuffle Current
		{
			get
			{
				try
				{
					return _list[position];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}
	}
}
