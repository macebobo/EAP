using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ElfCore
{
	public class ListBoxUtil
	{
		#region [ Constants ]

		public const string NOT_SELECTED_TEXT = "<Not Selected>";
		public const int NOT_SELECTED_KEY = -1;

		#endregion [ Constants ]

		#region [ Private Variables ]

		private bool _resetComboBox;
		private bool _controlKey = false;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		/// <summary>
		/// Only present because it is required
		/// </summary>
		/// <param name="setObjects"></param>
		protected void Initialize(bool setObjects)
		{ }

		[DebuggerHidden()]
		public ListBoxUtil()
		{
			Initialize(true);
		}

		#endregion [ Constructors ]

		#region [ Item ]

		public class Item 
		{
			private string _key;
			private string _text;
			private object _storage;

			#region [ Properties ]

			[DebuggerHidden()]
			public string Key
			{
				get { return _key; }
				set { _key = value; }
			}

			[DebuggerHidden()]
			public string Text
			{
				get { return _text; }
				set { _text = value; }
			}

			[DebuggerHidden()]
			public object StoredObject
			{
				get { return _storage; }
				set { _storage = value; }
			}

			#endregion

			#region [ Constructors ]

			[DebuggerHidden()]
			public Item()
			{
				Initialize(true);
			}

			[DebuggerHidden()]
			public Item(string text)
				: this()
			{
				_key = text;
				_text = text;
			}

			[DebuggerHidden()]
			public Item(string text, string key)
				: this()
			{
				_key = key;
				_text = text;
			}

			[DebuggerHidden()]
			public Item(string text, int key)
				: this()
			{
				_key = key.ToString();
				_text = text;
			}

			[DebuggerHidden()]
			public Item(string text, string key, object storage)
				: this(text, key)
			{
				_storage = storage;
			}

			[DebuggerHidden()]
			protected void Initialize(bool setObjects)
			{
				_key = string.Empty;
				_text = string.Empty;
				_storage = null;
			}

			#endregion

			[DebuggerHidden()]
			public override string ToString()
			{
				return _text;
			}
		}

		#endregion

		#region [ Sets ]

		#region [ Windows.Forms.ComboBox ]

		[DebuggerHidden()]
		public bool Set(ComboBox comboBox, bool value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		[DebuggerHidden()]
		public bool Set(ComboBox comboBox, string value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		[DebuggerHidden()]
		public bool Set(ComboBox comboBox, string value, bool useText, bool useStringFragment)
		{
			comboBox.SelectedIndex = Find(comboBox, value, useText, useStringFragment);
			return (comboBox.SelectedIndex > -1);
		}

		[DebuggerHidden()]
		public bool Set(ComboBox comboBox, int value)
		{
			return Set(comboBox, ((long)value));
		}

		[DebuggerHidden()]
		public bool Set(ComboBox comboBox, long value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		#endregion [ Windows.Forms.ComboBox ]

		#region [ Windows.Forms.ToolStripComboBox ]

		[DebuggerHidden()]
		public bool Set(ToolStripComboBox comboBox, bool value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		[DebuggerHidden()]
		public bool Set(ToolStripComboBox comboBox, string value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		[DebuggerHidden()]
		public bool Set(ToolStripComboBox comboBox, string value, bool useText, bool useStringFragment)
		{
			comboBox.SelectedIndex = Find(comboBox, value, useText, useStringFragment);
			return (comboBox.SelectedIndex > -1);
		}

		[DebuggerHidden()]
		public bool Set(ToolStripComboBox comboBox, int value)
		{
			return Set(comboBox, ((long)value));
		}

		[DebuggerHidden()]
		public bool Set(ToolStripComboBox comboBox, long value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#region [ Windows.Forms.ListBox ]

		[DebuggerHidden()]
		public bool Set(ListBox listBox, long value)
		{
			listBox.SelectedIndex = Find(listBox, value.ToString());
			return (listBox.SelectedIndex > -1);
		}

		[DebuggerHidden()]
		public bool Set(ListBox listBox, string value)
		{
			listBox.SelectedIndex = Find(listBox, value);
			return (listBox.SelectedIndex > -1);
		}

		[DebuggerHidden()]
		public bool Set(ListBox listBox, int value)
		{
			return Set(listBox, (value.ToString()));
		}

		#endregion [ Windows.Forms.ListBox ]

		#region [ Windows.Forms.CheckedListBox ]

		[DebuggerHidden()]
		public bool Set(CheckedListBox checkedListBox, long value)
		{
			return Set(checkedListBox, value, true);
		}

		[DebuggerHidden()]
		public bool Set(CheckedListBox checkedListBox, long value, bool checkedValue)
		{
			if (Find(checkedListBox, value.ToString()) > -1)
			{
				checkedListBox.SetItemChecked(Find(checkedListBox, value.ToString()), checkedValue);
				return true;
			}
			return false;
		}

		[DebuggerHidden()]
		public bool Set(CheckedListBox checkedListBox, string value, bool checkedValue)
		{
			if (Find(checkedListBox, value) > -1)
			{
				checkedListBox.SetItemChecked(Find(checkedListBox, value), checkedValue);
				return true;
			}
			return false;
		}

		#endregion [ Windows.Forms.CheckedListBox ]

		#endregion [ Sets ]

		#region [ Find ]

		#region [ Windows.Forms.ComboBox ]

		[DebuggerHidden()]
		public int Find(ComboBox comboBox, int value)
		{
			return Find(comboBox, value.ToString(), false, false);
		}

		public int Find(ComboBox comboBox, long value)
		{
			return Find(comboBox, value.ToString(), false, false);
		}

		public int Find(ComboBox comboBox, string value)
		{
			return Find(comboBox, value, false, false);
		}

		public int Find(ComboBox comboBox, string value, bool findFromName)
		{
			return Find(comboBox, value, findFromName, false);
		}

		public int Find(ComboBox comboBox, string value, bool findFromName, bool stringFragment)
		{
			if ((value == null) || (comboBox.Items.Count == 0))
				return NOT_SELECTED_KEY;

			if (findFromName || !(comboBox.Items[0] is Item))
			{
				if (!stringFragment)
					return comboBox.FindStringExact(value.Trim());
				else
					return comboBox.FindString(value.Trim());
			}

			for (int i = 0; i <= comboBox.Items.Count - 1; i++)
			{
				if (string.Compare(value.ToString(), ((Item)comboBox.Items[i]).Key, true) == 0)
					return i;
			}
			return NOT_SELECTED_KEY;
		}

		public int Find(ComboBox comboBox, bool value)
		{
			return Find(comboBox, (value ? "1" : "0"), false, false);
		}

		#endregion [ Windows.Forms.ComboBox ]

		#region [ Windows.Forms.ToolStripComboBox ]

		[DebuggerHidden()]
		public int Find(ToolStripComboBox comboBox, int value)
		{
			return Find(comboBox, value.ToString(), false, false);
		}

		public int Find(ToolStripComboBox comboBox, long value)
		{
			return Find(comboBox, value.ToString(), false, false);
		}

		public int Find(ToolStripComboBox comboBox, string value)
		{
			return Find(comboBox, value, false, false);
		}

		public int Find(ToolStripComboBox comboBox, string value, bool findFromName)
		{
			return Find(comboBox, value, findFromName, false);
		}

		public int Find(ToolStripComboBox comboBox, string value, bool findFromName, bool stringFragment)
		{
			if ((value == null) || (comboBox.Items.Count == 0))
				return NOT_SELECTED_KEY;

			if (findFromName || !(comboBox.Items[0] is Item))
			{
				if (!stringFragment)
					return comboBox.FindStringExact(value.Trim());
				else
					return comboBox.FindString(value.Trim());
			}

			for (int i = 0; i <= comboBox.Items.Count - 1; i++)
			{
				if (string.Compare(value.ToString(), ((Item)comboBox.Items[i]).Key, true) == 0)
					return i;
			}
			return NOT_SELECTED_KEY;
		}

		public int Find(ToolStripComboBox comboBox, bool value)
		{
			return Find(comboBox, (value ? "1" : "0"), false, false);
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#region [ Windows.Forms.ListBox ]

		[DebuggerHidden()]
		public int Find(ListBox listBox, string value)
		{
			try
			{
				for (int i = 0; i <= listBox.Items.Count - 1; i++)
				{
					if (value == ((Item)listBox.Items[i]).Key)
						return i;
				}
				return NOT_SELECTED_KEY;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}

		#endregion [ Windows.Forms.ListBox ]

		#region [ Windows.Forms.ListView ]

		[DebuggerHidden()]
		public int Find(ListView listView, string value)
		{
			try
			{
				for (int i = 0; i <= listView.Items.Count - 1; i++)
				{
					if (value == listView.Items[i].Text)
						return i;
				}
				return NOT_SELECTED_KEY;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}

		#endregion [ Windows.Forms.ListView ]

		#region [ Windows.Forms.CheckedListBox ]

		[DebuggerHidden()]
		public int Find(CheckedListBox listBox, string value)
		{
			try
			{
				for (int i = 0; i <= listBox.Items.Count - 1; i++)
				{
					if (value == ((Item)listBox.Items[i]).Key)
						return i;
				}
				return NOT_SELECTED_KEY;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}

		#endregion [ Windows.Forms.CheckedListBox ]

		#endregion [ Find ]

		#region [ Load ]

		#region [ Windows.Forms.ComboBox ]

		[DebuggerHidden()]
		public void Initialize(ComboBox comboBox, bool clear, bool loadNotSelectedEntry)
		{
			if (loadNotSelectedEntry)
				LoadNotSelected(comboBox, clear, NOT_SELECTED_TEXT, NOT_SELECTED_KEY);
			else
				ClearList(comboBox, clear);
		}

		[DebuggerHidden()]
		public void ClearList(ComboBox comboBox)
		{
			comboBox.Items.Clear();
		}

		[DebuggerHidden()]
		private void ClearList(ComboBox comboBox, bool clear)
		{
			if (clear)
				ClearList(comboBox);
		}

		[DebuggerHidden()]
		public void LoadNotSelected(ComboBox comboBox, bool clear, string desc, long index)
		{
			ClearList(comboBox, clear);
			if (Find(comboBox, index) == NOT_SELECTED_KEY)
			{
				Add(comboBox, desc, index.ToString(), false);
				SetDropDownWidth(comboBox);
			}
		}

		[DebuggerHidden()]
		public void Load(ComboBox comboBox, List<Item> list)
		{
			Initialize(comboBox, true, false);
			foreach (Item Item in list)
			{
				Add(comboBox, Item);
			}
			SetDropDownWidth(comboBox);
		}

		[DebuggerHidden()]
		public void Load(ComboBox comboBox, string[] list)
		{
			Load(comboBox, list, true, false, string.Empty);
		}

		[DebuggerHidden()]
		public void Load(ComboBox comboBox, string[] list, bool clear,
						 bool loadNotSelectedEntry, string formatString)
		{
			Initialize(comboBox, clear, loadNotSelectedEntry);

			for (int i = 0; i <= list.Length - 1; i++)
			{
				Add(comboBox, list[i], i.ToString(), formatString);
			}

			SetDropDownWidth(comboBox);
		}

		/// <summary>
		///  Populates a combobox with the contects of a list of strings
		/// </summary>
		/// <param name="comboBox">Control to populate</param>
		/// <param name="list">Generic list of entries to populate the control</param>
		/// <param name="loadNotSelectedEntry">Load an entry for not selected?</param>
		[DebuggerHidden()]
		public void Load(ComboBox comboBox, List<string> list, bool loadNotSelectedEntry)
		{
			int Width = comboBox.Width;
			Initialize(comboBox, true, loadNotSelectedEntry);

			foreach (string entry in list)
			{
				Add(comboBox, entry, entry, false);
			}
			SetDropDownWidth(comboBox);
		}

		[DebuggerHidden()]
		public void SetDropDownWidth(ComboBox comboBox)
		{
			//Found snippet on http://weblogs.asp.net/eporter/archive/2004/09/27/234773.aspx

			System.Drawing.Graphics g = comboBox.CreateGraphics();
			int WidestWidth = comboBox.DropDownWidth;
			string ValueToMeasure;
			int CurrentWidth;

			try
			{

				for (int i = 0; i <= comboBox.Items.Count - 1; i++)
				{
					ValueToMeasure = ((Item)comboBox.Items[i]).ToString();
					CurrentWidth = System.Convert.ToInt32(g.MeasureString(ValueToMeasure, comboBox.Font).Width);
					if (CurrentWidth > WidestWidth)
					{
						WidestWidth = CurrentWidth;
					}
				}

				if (WidestWidth > comboBox.DropDownWidth)
				{
					WidestWidth += 20; //Add a little for the scroll bar
				}

				//Make sure we are inbounds of the screen
				int left = comboBox.PointToScreen(new System.Drawing.Point(0, comboBox.Left)).X;
				if (WidestWidth > Screen.PrimaryScreen.WorkingArea.Width - left)
				{
					WidestWidth = Screen.PrimaryScreen.WorkingArea.Width - left;
				}

				comboBox.DropDownWidth = WidestWidth;

			}
			catch (Exception ex)
			{
				throw (ex);

			}
			finally
			{
				if (g != null)
				{
					g.Dispose();
					g = null;
				}
			}
		}

		/// <summary>
		/// Adjusts the width of the drop down section of comboboxes to show the widest member
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void AdjustedWidthComboBox_DropDown(object sender, System.EventArgs e)
		{
			ComboBox senderComboBox = (ComboBox)sender;
			int width = senderComboBox.DropDownWidth;
			Graphics g = senderComboBox.CreateGraphics();
			Font font = senderComboBox.Font;
			int vertScrollBarWidth = (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;
			string s;

			int newWidth;
			foreach (object o in ((ComboBox)sender).Items)
			{
				s = o.ToString();
				newWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;
				if (width < newWidth)
					width = newWidth;
			}
			senderComboBox.DropDownWidth = width;
		}

		[DebuggerHidden()]
		public void Load(ComboBox comboBox, string text, string key, bool clear, string formatString)
		{
			Initialize(comboBox, clear, false);
			Add(comboBox, text, key, formatString);
		}

		[DebuggerHidden()]
		public void LoadYesNo(ComboBox comboBox)
		{
			LoadYesNo(comboBox, true, false);
		}

		[DebuggerHidden()]
		public void LoadYesNo(ComboBox comboBox, bool clear, bool loadNotSelectedEntry)
		{
			Initialize(comboBox, clear, loadNotSelectedEntry);
			Add(comboBox, "Yes", "1", false);
			Add(comboBox, "No", "0", false);
			SetDropDownWidth(comboBox);
		}

		[DebuggerHidden()]
		public void Load(ComboBox comboBox, string text, string key, bool clear, bool loadNotSelectedEntry, string formatString)
		{
			Initialize(comboBox, clear, loadNotSelectedEntry);
			Add(comboBox, text, key, formatString);
			SetDropDownWidth(comboBox);
		}

		#endregion [ Windows.Forms.ComboBox ]

		#region [ Windows.Forms.ToolStripComboBox ]

		[DebuggerHidden()]
		public void Initialize(ToolStripComboBox ComboBox, bool clear, bool loadNotSelectedEntry)
		{
			if (loadNotSelectedEntry)
				LoadNotSelected(ComboBox, clear, NOT_SELECTED_TEXT, NOT_SELECTED_KEY);
			else
				ClearList(ComboBox, clear);
		}

		[DebuggerHidden()]
		public void ClearList(ToolStripComboBox ComboBox)
		{
			ComboBox.Items.Clear();
		}

		[DebuggerHidden()]
		private void ClearList(ToolStripComboBox ComboBox, bool clear)
		{
			if (clear)
				ClearList(ComboBox);
		}

		[DebuggerHidden()]
		public void LoadNotSelected(ToolStripComboBox ComboBox, bool clear, string desc, long index)
		{
			ClearList(ComboBox, clear);
			if (Find(ComboBox, index) == NOT_SELECTED_KEY)
			{
				Add(ComboBox, desc, index.ToString(), false);
				SetDropDownWidth(ComboBox);
			}
		}

		[DebuggerHidden()]
		public void Load(ToolStripComboBox ComboBox, List<Item> list)
		{
			Initialize(ComboBox, true, false);
			foreach (Item Item in list)
			{
				Add(ComboBox, Item);
			}
			SetDropDownWidth(ComboBox);
		}

		[DebuggerHidden()]
		public void Load(ToolStripComboBox ComboBox, string[] list)
		{
			Load(ComboBox, list, true, false, string.Empty);
		}

		[DebuggerHidden()]
		public void Load(ToolStripComboBox ComboBox, string[] list, bool clear,
						 bool loadNotSelectedEntry, string formatString)
		{
			Initialize(ComboBox, clear, loadNotSelectedEntry);

			for (int i = 0; i <= list.Length - 1; i++)
			{
				Add(ComboBox, list[i], i.ToString(), formatString);
			}
			SetDropDownWidth(ComboBox);
		}

		/// <summary>
		///  Populates a ToolStripComboBox with the contects of a list of strings
		/// </summary>
		/// <param name="ComboBox">Control to populate</param>
		/// <param name="list">Generic list of entries to populate the control</param>
		/// <param name="loadNotSelectedEntry">Load an entry for not selected?</param>
		[DebuggerHidden()]
		public void Load(ToolStripComboBox ComboBox, List<string> list, bool loadNotSelectedEntry)
		{
			int Width = ComboBox.Width;
			Initialize(ComboBox, true, loadNotSelectedEntry);

			foreach (string entry in list)
			{
				Add(ComboBox, entry, entry, false);
			}
			SetDropDownWidth(ComboBox);
		}

		[DebuggerHidden()]
		public static void SetDropDownWidth(ToolStripComboBox comboBox)
		{
			//Found snippet on http://weblogs.asp.net/eporter/archive/2004/09/27/234773.aspx

			System.Drawing.Graphics g = comboBox.GetCurrentParent().CreateGraphics();
			int WidestWidth = comboBox.DropDownWidth;
			string ValueToMeasure;
			int CurrentWidth;

			try
			{

				for (int i = 0; i <= comboBox.Items.Count - 1; i++)
				{
					ValueToMeasure = comboBox.Items[i].ToString();
					CurrentWidth = System.Convert.ToInt32(g.MeasureString(ValueToMeasure, comboBox.Font).Width);
					if (CurrentWidth > WidestWidth)
					{
						WidestWidth = CurrentWidth;
					}
				}

				if (WidestWidth > comboBox.DropDownWidth)
				{
					WidestWidth += 20; //Add a little for the scroll bar
				}

				comboBox.DropDownWidth = WidestWidth;

			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (g != null)
				{
					g.Dispose();
					g = null;
				}
			}
		}

		/// <summary>
		/// Adjusts the width of the drop down section of ToolStripComboBoxes to show the widest member
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void AdjustedWidthToolStripComboBox_DropDown(object sender, System.EventArgs e)
		{
			ToolStripComboBox ComboBox = (ToolStripComboBox)sender;
			int width = ComboBox.DropDownWidth;
			Graphics g = ComboBox.GetCurrentParent().CreateGraphics();
			Font font = ComboBox.Font;
			int vertScrollBarWidth = (ComboBox.Items.Count > ComboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;
			string s;

			int newWidth;
			foreach (object o in ((ToolStripComboBox)sender).Items)
			{
				s = o.ToString();
				newWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;
				if (width < newWidth)
					width = newWidth;
			}
			ComboBox.DropDownWidth = width;
		}

		[DebuggerHidden()]
		public void Load(ToolStripComboBox ComboBox, string text, string key, bool clear, string formatString)
		{
			Initialize(ComboBox, clear, false);
			Add(ComboBox, text, key, formatString);
		}

		[DebuggerHidden()]
		public void LoadYesNo(ToolStripComboBox ToolStripComboBox)
		{
			LoadYesNo(ToolStripComboBox, true, false);
		}

		[DebuggerHidden()]
		public void LoadYesNo(ToolStripComboBox ComboBox, bool clear, bool loadNotSelectedEntry)
		{
			Initialize(ComboBox, clear, loadNotSelectedEntry);
			Add(ComboBox, "Yes", "1", false);
			Add(ComboBox, "No", "0", false);
			//SetDropDownWidth(ComboBox);
		}

		[DebuggerHidden()]
		public void Load(ToolStripComboBox ComboBox, string text, string key, bool clear, bool loadNotSelectedEntry, string formatString)
		{
			Initialize(ComboBox, clear, loadNotSelectedEntry);
			Add(ComboBox, text, key, formatString);
			//SetDropDownWidth(ComboBox);
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#region [ Windows.Forms.ListBox ]

		/// <summary>
		/// Clears the listbox
		/// </summary>
		/// <param name="listBox"></param>
		[DebuggerHidden()]
		public void ClearList(ListBox listBox)
		{
			listBox.Items.Clear();
		}

		/// <summary>
		/// If the boolean is true, clears the listbox.
		/// </summary>
		/// <param name="listBox"></param>
		/// <param name="clear"></param>
		[DebuggerHidden()]
		public void ClearList(ListBox listBox, bool clear)
		{
			if (clear)
				ClearList(listBox);
		}

		/// <summary>
		/// Clears out items with the Deleted flag out of the listbox.
		/// </summary>
		/// <param name="listBox"></param>
		/// <returns></returns>
		[DebuggerHidden()]
		public bool ClearDeleted(ListBox listBox)
		{

			ArrayList ClearItems = new ArrayList();

			try
			{

				if (listBox == null)
					return false;

				foreach (Item item in ClearItems)
				{
					listBox.Items.Remove(item);
				}

				if (ClearItems.Count > 0)
					return true;

				return false;

			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (ClearItems == null)
				{
					ClearItems.Clear();
					ClearItems = null;
				}
			}
		}

		/// <summary>
		/// Possibly clears the listbox and adds the item indicated.
		/// </summary>
		/// <param name="listBox"></param>
		/// <param name="text"></param>
		/// <param name="key"></param>
		/// <param name="clear"></param>
		/// <param name="formatString"></param>
		/// <param name="isDeleted"></param>
		[DebuggerHidden()]
		public void Load(ListBox listBox, string text, string key, bool clear, string formatString)
		{
			ClearList(listBox, clear);
			Add(listBox, text, key, formatString);
		}

		/// <summary>
		/// Possibly clears the listbox and adds the item indicated.
		/// </summary>
		/// <param name="listBox"></param>
		/// <param name="text"></param>
		/// <param name="key"></param>
		/// <param name="clear"></param>
		/// <param name="storage"></param>
		/// <param name="formatString"></param>
		/// <param name="isDeleted"></param>
		[DebuggerHidden()]
		public void Load(ListBox listBox, string text, string key, object storage, bool clear, string formatString)
		{
			ClearList(listBox, clear);
			Add(listBox, text, key, storage, formatString);
		}

		/// <summary>
		/// Possibly clears the listbox and adds the items from the string array.
		/// </summary>
		/// <param name="listBox"></param>
		/// <param name="list"></param>
		/// <param name="clear"></param>
		/// <param name="formatString"></param>
		[DebuggerHidden()]
		public void Load(ListBox listBox, string[] list, bool clear, string formatString)
		{
			ClearList(listBox, clear);
			for (int i = 0; i <= list.Length - 1; i++)
			{
				Add(listBox, list[i], i.ToString(), formatString);
			}
		}

		#region [ Windows.Forms.CheckedListBox ]

		[DebuggerHidden()]
		public void ClearChecked(CheckedListBox listBox)
		{
			for (int i = 0; i <= listBox.Items.Count - 1; i++)
			{
				listBox.SetItemCheckState(i, CheckState.Unchecked);
			}
		}

		[DebuggerHidden()]
		public void ClearList(CheckedListBox listBox)
		{
			listBox.Items.Clear();
		}

		[DebuggerHidden()]
		private void ClearList(CheckedListBox listBox, bool clear)
		{
			if (clear)
			{
				ClearList(listBox);
			}
		}

		[DebuggerHidden()]
		public void Load(CheckedListBox listBox, string text, string key, bool clear, string formatString)
		{
			ClearList(listBox, clear);
			Add(listBox, text, key, formatString);
		}

		[DebuggerHidden()]
		public void Load(CheckedListBox listBox, string[] List, bool clear, string formatString)
		{
			ClearList(listBox, clear);
			for (int i = 0; i <= List.Length - 1; i++)
			{
				Add(listBox, List[i], i.ToString(), formatString);
			}
		}

		#endregion [ Windows.Forms.CheckedListBox ]

		#endregion [ Windows.Forms.ListBox ]

		#endregion [ Load ]

		#region [ Add ]

		#region [ Windows.Forms.ComboBox ]

		[DebuggerHidden()]
		public void Add(ComboBox comboBox, string text, string key)
		{
			Add(comboBox, text, key);
		}

		[DebuggerHidden()]
		public void Add(ComboBox comboBox, Item listItem)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(listItem);
			comboBox.EndUpdate();
		}

		[DebuggerHidden()]
		public void Add(ComboBox comboBox, string text, string key, string formatString)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(new Item(text, key));
			comboBox.EndUpdate();
		}

		[DebuggerHidden()]
		public void Add(ComboBox comboBox, string text, string key, object storage)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(new Item(text, key, storage));
			comboBox.EndUpdate();
		}

		public void Add(ComboBox control, string text)
		{
			control.BeginUpdate();
			control.Items.Add(new Item(text));
			control.EndUpdate();
		}

		#endregion [ Windows.Forms.ComboBox ]

		#region [ Windows.Forms.ToolStripComboBox ]

		[DebuggerHidden()]
		public void Add(ToolStripComboBox comboBox, string text, string key)
		{
			Add(comboBox, text, key);
		}

		[DebuggerHidden()]
		public void Add(ToolStripComboBox comboBox, Item listItem)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(listItem);
			comboBox.EndUpdate();
		}

		[DebuggerHidden()]
		public void Add(ToolStripComboBox comboBox, string text, string key, string formatString)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(new Item(text, key));
			comboBox.EndUpdate();
		}

		[DebuggerHidden()]
		public void Add(ToolStripComboBox comboBox, string text, string key, object storage)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(new Item(text, key, storage));
			comboBox.EndUpdate();
		}

		public void Add(ToolStripComboBox control, string text)
		{
			control.BeginUpdate();
			control.Items.Add(new Item(text));
			control.EndUpdate();
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#region [ Windows.Forms.ListBox ]

		[DebuggerHidden()]
		public void Add(ListBox listBox, string text, string key)
		{
			Add(listBox, text, key, string.Empty);
		}

		[DebuggerHidden()]
		public void Add(ListBox listBox, string text, string key, string formatString)
		{
			Add(listBox, text, key, null, formatString);
		}

		[DebuggerHidden()]
		public void Add(ListBox listBox, string text, string key, object storage, string formatString)
		{
			listBox.BeginUpdate();
			listBox.Items.Add(new Item(text, key, storage));
			listBox.EndUpdate();
		}

		#endregion [ Windows.Forms.ListBox ]

		#region [ Windows.Forms.CheckedListBox ]

		[DebuggerHidden()]
		public void Add(CheckedListBox listBox, string text, string key)
		{
			Add(listBox, text, key, string.Empty);
		}

		[DebuggerHidden()]
		public void Add(CheckedListBox checkedListBox, string text, string key, string formatString)
		{
			checkedListBox.BeginUpdate();
			//if (formatString.Trim().Length > 0)
			//	text = m_Formattor.Format(text, formatString);

			checkedListBox.Items.Add(new Item(text, key));
			checkedListBox.EndUpdate();
		}

		#endregion [ Windows.Forms.CheckedListBox ]

		#endregion [ Add ]

		#region [ Gets ]

		#region [ Windows.Forms.ComboBox ]

		[DebuggerHidden()]
		public Item GetItem(ComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]);
			}
			catch
			{
				return null;
			}
		}

		[DebuggerHidden()]
		public Item GetItem(ComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
			{
				return GetItem(comboBox, comboBox.SelectedIndex);
			}
			else
			{
				return null;
			}
		}

		[DebuggerHidden()]
		public string GetKey(ComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]).Key;
			}
			catch
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public string GetKey(ComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
			{
				return GetKey(comboBox, comboBox.SelectedIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public string GetText(ComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public string GetText(ComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
			{
				return GetText(comboBox, comboBox.SelectedIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public bool GetBoolean(ComboBox comboBox)
		{
			return GetBoolean(comboBox, comboBox.SelectedIndex);
		}

		[DebuggerHidden()]
		public bool GetBoolean(ComboBox comboBox, int index)
		{
			if (GetKey(comboBox, index) == "1")
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#region [ Windows.Forms.ToolStripComboBox ]

		[DebuggerHidden()]
		public Item GetItem(ToolStripComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Retrieves the currently selected item from a ToolStripComboBox
		/// </summary>
		/// <param name="comboBox"></param>
		/// <returns></returns>
		[DebuggerHidden()]
		public Item GetItem(ToolStripComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
			{
				return GetItem(comboBox, comboBox.SelectedIndex);
			}
			else
			{
				return null;
			}
		}

		[DebuggerHidden()]
		public string GetKey(ToolStripComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]).Key;
			}
			catch
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public string GetKey(ToolStripComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
			{
				return GetKey(comboBox, comboBox.SelectedIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public string GetText(ToolStripComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public string GetText(ToolStripComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
			{
				return GetText(comboBox, comboBox.SelectedIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public bool GetBoolean(ToolStripComboBox comboBox)
		{
			return GetBoolean(comboBox, comboBox.SelectedIndex);
		}

		[DebuggerHidden()]
		public bool GetBoolean(ToolStripComboBox comboBox, int index)
		{
			if (GetKey(comboBox, index) == "1")
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#region [ Windows.Forms.ListBox ]

		public Item GetItem(ListBox listBox, int index)
		{
			try
			{
				return ((Item)listBox.Items[index]);
			}
			catch
			{
				return null;
			}
		}

		public Item GetItem(ListBox listBox)
		{
			if (listBox.SelectedIndex > -1)
			{
				return GetItem(listBox, listBox.SelectedIndex);
			}
			else
			{
				return null;
			}
		}

		public string GetKey(ListBox listBox, int index)
		{
			try
			{
				return ((Item)listBox.Items[index]).Key;
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetKey(ListBox listBox)
		{
			return GetKey(listBox, listBox.SelectedIndex);
		}

		public string GetText(ListBox listBox, int index)
		{
			try
			{
				return ((Item)listBox.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetText(object listBox, int index)
		{
			try
			{
				return GetText(((ListBox)listBox), index);
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetKey(object listBox, int index)
		{
			return GetKey(((ListBox)listBox), index);
		}

		public string GetText(ListBox listBox)
		{
			return GetText(listBox, listBox.SelectedIndex);
		}

		#endregion [ Windows.Forms.ListBox ]

		#region [ Windows.Forms.CheckedListBox ]

		[DebuggerHidden()]
		public Item GetItem(CheckedListBox listBox, int index)
		{
			try
			{
				return ((Item)listBox.Items[index]);
			}
			catch
			{
				return null;
			}
		}

		[DebuggerHidden()]
		public Item GetItem(CheckedListBox listBox)
		{
			if (listBox.SelectedIndex > -1)
			{
				return GetItem(listBox, listBox.SelectedIndex);
			}
			else
			{
				return null;
			}
		}

		[DebuggerHidden()]
		public string GetKey(CheckedListBox listBox, int index)
		{
			try
			{
				return ((Item)listBox.Items[index]).Key;
			}
			catch
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public string GetKey(CheckedListBox listBox)
		{
			return GetKey(listBox, listBox.SelectedIndex);
		}

		[DebuggerHidden()]
		public string GetText(CheckedListBox listBox, int index)
		{
			try
			{
				return ((Item)listBox.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		[DebuggerHidden()]
		public string GetText(CheckedListBox listBox)
		{
			return GetText(listBox, listBox.SelectedIndex);
		}

		#endregion [ Windows.Forms.CheckedListBox ]

		#endregion [ Gets ]

		#region [ Event Overrides ]

		[DebuggerHidden()]
		public void ComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			_resetComboBox = false;
			// Determine whether the keystroke is a backspace.
			if (e.KeyCode == Keys.Back)
			{
				_resetComboBox = true; //A non-numerical keystroke was pressed.
			}
		}

		[DebuggerHidden()]
		public void ComboBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (_resetComboBox)
			{
				((ComboBox)sender).SelectedIndex = -1;
				e.Handled = true;
			}
		}

		// perform the text substitution
		[DebuggerHidden()]
		public void AutoComplete_KeyDown(object sender, KeyEventArgs e)
		{

			ComboBox Combo = (ComboBox)sender;

			if (Combo.Text != string.Empty && !_controlKey)
			{
				// search for matching entry
				string MatchText = Combo.Text;
				int Match = Combo.FindString(MatchText);

				// If a matching entry is found, insert it now
				if (Match != NOT_SELECTED_KEY)
				{

					Combo.SelectedIndex = Match;

					// select the added text so it can be replaced
					// if the user keeps typing.
					Combo.SelectionStart = MatchText.Length;
					Combo.SelectionLength = Combo.Text.Length - Combo.SelectionStart;
				}
			}
		}

		[DebuggerHidden()]
		public void AutoComplete_KeyPress(object sender, KeyPressEventArgs e)
		{

			ComboBox Combo = (ComboBox)sender;

			if (char.IsControl(e.KeyChar))
			{
				_controlKey = true;
			}
			else
			{
				_controlKey = false;
			}
		}

		#endregion [ Event Overrides ]

		#region [ Other Methods ]

		/// <summary>
		/// Found at http://rajeshkm.blogspot.com/2006/11/adjust-combobox-drop-down-list-width-c.html
		/// </summary>
		/// <param name="sender"></param>		
		[DebuggerHidden()]
		public void SetComboScrollWidth(ComboBox comboBox)
		{
			int newWidth;
			string s;

			try
			{
				int width = comboBox.Width;
				Graphics g = comboBox.CreateGraphics();
				Font font = comboBox.Font;

				// checks if a scrollbar will be displayed.
				// If yes, then get its width to adjust the size of the drop down list.
				int vertScrollBarWidth = (comboBox.Items.Count > comboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

				foreach (ListBoxUtil.Item Item in comboBox.Items)
				{
					if (Item != null)
					{
						s = Item.Text;
						newWidth = (int)g.MeasureString(s.Trim(), font).Width + vertScrollBarWidth;
						if (width < newWidth)
						{
							width = newWidth;
						}
					}
				}

				comboBox.DropDownWidth = width;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		#endregion [ Other Methods ]
	}
}