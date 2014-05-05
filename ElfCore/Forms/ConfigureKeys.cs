using ElfCore.Util;
using ElfCore.Controllers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Forms
{
	public partial class ConfigureKeys : Form
	{
		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;
		private bool _capturing = false;
		private ListBoxUtil _listBoxUtil = null;
		Dictionary<string, string> _dict = null; // <gesture, id>
		SortedList<string, MultiKeyGesture> _list = null; // <id, obj>

		#endregion [ Private Variables ]

		#region [ Properties ]

		private string CurrentID
		{
			get
			{
				if ((lstCommands.Items.Count == 0) || (lstCommands.SelectedIndex < 0))
					return string.Empty;
				return _listBoxUtil.GetKey(lstCommands);
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ConfigureKeys()
		{
			InitializeComponent();
			_listBoxUtil = new ListBoxUtil();
			txtExisting.Text = string.Empty;
			txtConflicts.Text = string.Empty;
			cmdAssign.Enabled = false;
			cmdRemove.Enabled = false;
			cmdAssign.Image = ImageHandler.AddAnnotation(ElfRes.keyboard, Annotation.Add, AnchorStyles.Top | AnchorStyles.Left);
			cmdRemove.Image = ImageHandler.AddAnnotation(ElfRes.keyboard, Annotation.Delete, AnchorStyles.Top | AnchorStyles.Left);
			BuildDictionary(false);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Assigns the gesture to the command associated with this id
		/// </summary>
		private void AssignGesture(string id, string gesture)
		{
			MultiKeyGesture m = _list[id];
			if (m == null)
				return;

			_dict.Add(gesture, id);
			m.Gesture = gesture;
		}

		/// <summary>
		/// Construct a copy of the list of MultiKeyGestures from the Keyboard controller and construct a gesture dictionary.
		/// </summary>
		private void BuildDictionary(bool getDefaults)
		{
			_list = new SortedList<string, MultiKeyGesture>();
			_dict = new Dictionary<string, string>();
			MultiKeyGesture m = null;

			foreach (KeyValuePair<string, MultiKeyGesture> KVP in (getDefaults ? _workshop.KeyboardController.DefaultGestures : _workshop.KeyboardController.CurrentGestures))
			{
				m = new MultiKeyGesture(KVP.Value.ID, KVP.Value.Gesture, KVP.Value.MenuItem);
				_list.Add(m.ID, m);
				if (m.Gesture.Length > 0)
					_dict.Add(m.Gesture, m.ID);
			}
			m = null;
			LoadGestures(_list);
		}

		/// <summary>
		/// Finds all the commands from the list whose ID at least partially matches the text.
		/// </summary>
		private SortedList<string, MultiKeyGesture> FindCommands(string text)
		{
			SortedList<string, MultiKeyGesture> Found = new SortedList<string, MultiKeyGesture>();
			MultiKeyGesture m = null;
			for (int i = 0; i < _list.Count; i++)
			{
				m = _list[_list.Keys[i]];
				if (m.ID.ToUpper().Contains(text.ToUpper()))
					Found.Add(m.ID, m);
			}
			return Found;
		}

		/// <summary>
		/// Loads the list of MultiKeyGesture ID's into the listbox.
		/// </summary>
		/// <param name="gestures">List of MultiKeyGestures</param>
		private void LoadGestures(SortedList<string, MultiKeyGesture> gestures)
		{
			lstCommands.Items.Clear();
			MultiKeyGesture m = null;
			for (int i = 0; i < gestures.Count; i++)
			{
				m = gestures[gestures.Keys[i]];
				_listBoxUtil.Add(lstCommands, m.ID, m.ID); 
			}
			if (lstCommands.Items.Count > 0)
				lstCommands.SelectedIndex = 0;
		}

		/// <summary>
		/// Finds a match in the current key configuration that matches the gesture, but is not currently selected in the list
		/// </summary>
		/// <param name="gesture">Gesture string to match.</param>
		private void FindMatch(string gesture)
		{
			string ID = string.Empty;
			txtConflicts.Text = string.Empty;

			// Check for a direct match
			if (_dict.TryGetValue(gesture, out ID))
			{
				txtConflicts.Text = ID;
				return;
			}
			
			// Check for a match of the first keystroke to any existing
			if (gesture.Contains(","))
			{
				// See if there is a gesture that only is the first keystroke. This is a conflict.
				gesture = gesture.Split(',')[0];
				if (_dict.TryGetValue(gesture, out ID))
				{
					txtConflicts.Text = ID;
					return;
				}
			}
			else
			{
				// The entered gesture was a single keystroke. See if there are any in the dictionary
				// that start with this keystroke.
				foreach (KeyValuePair<string, string> KVP in _dict)
				{ 
					if (KVP.Key.Split(',')[0] == gesture)
						txtConflicts.Text += (txtConflicts.Text.Length > 0 ? "|" : string.Empty) + KVP.Value;
				}
			}
		}

		/// <summary>
		/// Removes the gesture from the command indicated by the ID passed in.
		/// </summary>
		private void RemoveGesture(string id)
		{
			MultiKeyGesture m = _list[id];
			if (m == null)
				return;

			if (_dict.ContainsKey(m.Gesture))
				_dict.Remove(m.Gesture);
			m.Gesture = string.Empty;
			txtExisting.Text = string.Empty;			
		}

		#endregion [ Methods ]

		#region [ Events ]
		
		private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
		{
			_workshop.KeyboardController.ClearBuffer();
			txtExisting.Text = string.Empty;
			txtNewKey.Text = string.Empty;
			txtConflicts.Text = string.Empty;

			if (lstCommands.SelectedIndex < 0)
				return;

			string CurrentID = this.CurrentID;
			if (_list.Keys.Contains(CurrentID))
			{
				MultiKeyGesture m = _list[CurrentID];
				if (m == null)
					return;
				txtExisting.Text = m.Gesture;
			}
		}

		/// <summary>
		/// Assigns the currently entered gesture to the currently selected command.
		/// If there are conflicts present, prompts the user for instructions.
		/// </summary>
		private void cmdAssign_Click(object sender, EventArgs e)
		{
			_workshop.KeyboardController.ClearBuffer();
			string CurrentID = this.CurrentID;
			if ((txtConflicts.TextLength > 0) && (txtConflicts.Text != CurrentID))
			{
				if (MessageBox.Show("This gesture currently conflicts with the commands:\n" + txtConflicts.Text + "\nRemove the conflicts and assign the gesture?", "Assign Keystroke Gesture", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
					return;
				foreach (string id in txtConflicts.Text.Split('|'))
					RemoveGesture(id);
			}
			else if ((txtConflicts.TextLength > 0) && (txtConflicts.Text == CurrentID))
				RemoveGesture(CurrentID);

			txtExisting.Text = txtNewKey.Text;
			txtNewKey.Text = string.Empty;
			AssignGesture(CurrentID, txtExisting.Text);
		}

		/// <summary>
		/// Rebuild the list and dictionary to the original shortcut configuration.
		/// </summary>
		private void cmdDefault_Click(object sender, EventArgs e)
		{
			_workshop.KeyboardController.ClearBuffer();
			_list = new SortedList<string, MultiKeyGesture>();
			_dict = new Dictionary<string, string>();
			BuildDictionary(true);
		}

		/// <summary>
		/// Removes the gesture from the currently selected command.
		/// </summary>
		private void cmdRemove_Click(object sender, EventArgs e)
		{
			_workshop.KeyboardController.ClearBuffer();
			RemoveGesture(CurrentID);
		}

		private void Form_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_capturing)
				_workshop.KeyboardController.MessageOccurred -= KeyboardController_MessageOccurred;
		}

		private void KeyboardController_MessageOccurred(object sender, MessageEventArgs e)
		{
			txtNewKey.Text = e.Message;
			FindMatch(txtNewKey.Text);
			if (txtNewKey.Text.Contains(","))
				_workshop.KeyboardController.ClearBuffer();
		}

		private void txtNewKey_Enter(object sender, EventArgs e)
		{
			_workshop.KeyboardController.MessageOccurred += KeyboardController_MessageOccurred;
			_capturing = true;
		}

		private void txtNewKey_KeyDown(object sender, KeyEventArgs e)
		{
			_workshop.KeyboardController.Keyboard_KeyDownJustCapture(e);
		}

		private void txtNewKey_Leave(object sender, EventArgs e)
		{
			if (_capturing)
				_workshop.KeyboardController.MessageOccurred -= KeyboardController_MessageOccurred;
			_capturing = false;
			_workshop.KeyboardController.ClearBuffer();
		}

		private void txtNewKey_TextChanged(object sender, EventArgs e)
		{
			cmdAssign.Enabled = (txtNewKey.TextLength > 0);
			// Don't give the option to save if the new keystroke matches the existing one.
			if (cmdAssign.Enabled)
				cmdAssign.Enabled = (txtNewKey.Text != txtExisting.Text);
		}
		
		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			LoadGestures(FindCommands(txtSearch.Text));
		}

		private void txtExisting_TextChanged(object sender, EventArgs e)
		{
			cmdRemove.Enabled = (txtExisting.TextLength > 0);
		}

		#endregion [ Events ]

		private void cmdLoad_Click(object sender, EventArgs e)
		{

		}

		private void cmdSave_Click(object sender, EventArgs e)
		{

		}
	}
}