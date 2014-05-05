using ElfCore.Channels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Util
{
	/// <summary>
	/// Channel mapping class used for Importing of Channels from another Profile.
	/// </summary>
	public class Mapping : IComparable
	{
		#region [ Public Variables ]

		/// <summary>
		/// Indicates whether the cells in the target channel should be removed first.
		/// </summary>
		public bool ClearTargetChannel = false;

		/// Indicates whether we are currently working on this particular Mapping at the moment.
		/// </summary>
		public bool EditFlag = false;

		/// <summary>
		/// (Max Negative Offset - Negative Offset) + ImportedOffset
		/// </summary>
		public LatticePoint EffectiveOffset = LatticePoint.Empty;

		/// <summary>
		/// Amount the imported channel goes over the sides of the original lattice size.
		/// </summary>
		public LatticePoint Extension = LatticePoint.Empty;

		/// <summary>
		/// Unique ID for this mapping.
		/// </summary>
		public int ID = -1;

		/// <summary>
		/// Indicates whether the color of the target channel should be overwritten by the imported channel.
		/// </summary>
		public bool OverrideColor = false;

		/// <summary>
		/// Indicates whether the name of the target channel should be overwritten by the imported channel.
		/// </summary>
		public bool OverrideName = false;

		/// <summary>
		/// Channel whose data is being import into the target Channel.
		/// </summary>
		public Channel ImportedChannel = null;

		/// <summary>
		/// Original bounds of the data within the imported Channel.
		/// </summary>
		public Rectangle ImportedChannel_Bounds = Rectangle.Empty;

		/// <summary>
		/// Position offset of the imported data.
		/// </summary>
		public Point ImportedOffset = Point.Empty;

		/// <summary>
		/// Amount the canvas has been shoved left or up, need to offset all channels by this.
		/// </summary>
		public Point SetBack = Point.Empty;

		/// <summary>
		/// Indicates whether we are currently previewing this Mapping.
		/// </summary>
		public bool PreviewFlag = false;

		/// <summary>
		/// Indicates whether the lattice should be resized for this mapping.
		/// </summary>
		public bool ResizeLattice = false;

		/// <summary>
		/// Channel that is the target for importing data into.
		/// </summary>
		public Channel TargetChannel = null;

		/// <summary>
		/// Position offset of the target data.
		/// </summary>
		public Point TargetOffset = Point.Empty;

		#endregion [ Public Variables ]

		#region [ Constructors ]

		public Mapping()
		{ }

		public Mapping(Mapping map)
		{
			ClearTargetChannel = map.ClearTargetChannel;
			EditFlag = map.EditFlag;
			ImportedChannel = map.ImportedChannel;
			ID = map.ID;
			OverrideColor = map.OverrideColor;
			OverrideName = map.OverrideName;
			ImportedOffset = map.ImportedOffset;
			PreviewFlag = map.PreviewFlag;
			ResizeLattice = map.ResizeLattice;
			TargetChannel = map.TargetChannel;
			TargetOffset = map.TargetOffset;
			SetBack = map.SetBack;
			Extension = map.Extension;
			EffectiveOffset = map.EffectiveOffset;
			ImportedChannel_Bounds = map.ImportedChannel_Bounds;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Calculate the Extension by adding the effective offset to the width of the imported channel and see if it goes beyond the 
		/// edge of the Profile's lattice size. The difference is the Extension.
		/// </summary>
		public void CalcExtension(Size latticeSize)
		{
			if (!ResizeLattice)
			{
				Extension = Point.Empty;
				return;
			}

			int Value = ImportedChannel_Bounds.Right + ImportedOffset.X;
			if (Value > latticeSize.Width)
				Value -= latticeSize.Width;
			else
				Value = 0;
			Extension.X = Value;

			Value = ImportedChannel_Bounds.Bottom + ImportedOffset.Y;
			if (Value > latticeSize.Height)
				Value -= latticeSize.Height;
			else
				Value = 0;
			Extension.Y = Value;
		}

		/// <summary>
		/// Calculate the Set Back by adding the bounds of the imported channel to the offset. If the new value is less that 0, then
		/// the negative offset is the absolute value of that, otherwise, the offset is 0.
		/// </summary>
		public void CalcSetBack()
		{
			if (!ResizeLattice)
			{
				SetBack = Point.Empty;
				return;
			}

			int Value = ImportedChannel_Bounds.Left + ImportedOffset.X;
			if (Value < 0)
				Value = Math.Abs(Value);
			else
				Value = 0;
			SetBack.X = Value;

			Value = ImportedChannel_Bounds.Top + ImportedOffset.Y;
			if (Value < 0)
				Value = Math.Abs(Value);
			else
				Value = 0;
			SetBack.Y = Value;
		}

		/// <summary>
		/// Determine the effective offset
		/// </summary>
		/// <param name="maxSetBack">Maximum negative offset for all the mappings thus far.</param>
		public void CalcEffectiveOffset(LatticePoint maxSetBack)
		{
			EffectiveOffset.X = ImportedOffset.X + SetBack.X + (maxSetBack.X - SetBack.X);
			EffectiveOffset.Y = ImportedOffset.Y + SetBack.Y + (maxSetBack.Y - SetBack.Y);
		}

		public override string ToString()
		{
			if ((ImportedChannel == null) || (TargetChannel == null))
				return "INVALID MAPPING";

			string ToString = ToString = ID + ": " + ImportedChannel.Name + " → " + TargetChannel.Name;
			ToString += ", Resize: " + ResizeLattice;
			ToString += ", Override Color: " + OverrideColor;
			ToString += ", Override Name: " + OverrideName;
			ToString += ", Clear Cells: " + ClearTargetChannel;
			ToString += ", Offset: " + ImportedOffset;

			return ToString;
		}

		/// <summary>
		/// Implement the IComparable interface method
		/// </summary>
		public int CompareTo(object obj)
		{
			if (obj is Mapping)
			{
				return ID.CompareTo((obj as Mapping).ID);
			}
			else
				throw new ArgumentException("Object is not a Mapping");
		}

		#endregion [ Methods ]
	}

	public class MappingList : CollectionBase, IList<Mapping>, ICollection<Mapping>
	{ 

		public MappingList()
		{ }

		public void Add(Mapping item)
		{
			List.Add(item);
		}

		public bool Contains(Mapping item)
		{
			return List.Contains(item);
		}

		public void CopyTo(Mapping[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int IndexOf(Mapping item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, Mapping item)
		{
			List.Insert(index, item);
		}

		public bool IsReadOnly
		{
			get { return List.IsReadOnly; }
		}

		public MappingList OrderByDescending()
		{
			MappingList ReturnList = new MappingList();
			Mapping[] Arr = ToArray();
			Array.Sort(Arr);
			Stack<Mapping> Stack = new Stack<Mapping>();
			for (int i = 0; i < List.Count; i++)
				Stack.Push(Arr[i]);
			while (Stack.Count > 0)
				ReturnList.Add(Stack.Pop());
			Stack = null;
			return ReturnList;
		}

		public void Remove(Mapping item)
		{
			List.Remove(item);
		}

		bool ICollection<Mapping>.Remove(Mapping item)
		{
			if (!List.Contains(item))
				return false;
			List.Remove(item);
			return true;
		}

		public Mapping this[int index]
		{
			get { return (Mapping)List[index]; }
			set { List[index] = value; }
		}

		public Mapping[] ToArray()
		{
			Mapping[] Arr = new Mapping[List.Count];
			for (int i = 0; i < List.Count; i++)
				Arr[i] = this[i];
			return Arr;
		}

		public Mapping Where(int id)
		{
			foreach (Mapping item in List)
				if (item.ID == id)
					return item;
			return null;
		}

		public Mapping Where(bool previewFlag)
		{
			foreach (Mapping item in List)
				if (item.PreviewFlag == previewFlag)
					return item;
			return null;
		}

		public MappingList Where(bool previewFlag, object channel, int id)
		{
			MappingList Filtered = new MappingList();
			
			foreach (Mapping item in Filtered.List)
				if ((item.PreviewFlag == previewFlag) &&
					ReferenceEquals(item.TargetChannel, channel) &&
					(item.ID != id))
					Filtered.List.Add(item);

			return Filtered;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		IEnumerator<Mapping> IEnumerable<Mapping>.GetEnumerator()
		{
			return new MappingListEnumerator(List.GetEnumerator());
		}

		private class MappingListEnumerator : IEnumerator<Mapping>
		{
			private IEnumerator _enumerator;

			public MappingListEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			public Mapping Current
			{
				get { return (Mapping)_enumerator.Current; }
			}

			object IEnumerator.Current
			{
				get { return _enumerator.Current; }
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public void Dispose()
			{
			}
		}
	}
}
