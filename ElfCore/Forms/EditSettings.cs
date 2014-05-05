using ElfControls;

using ElfCore.Channels;
using ElfCore.Util;
using ElfCore.Controllers;
using ElfCore.Profiles;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Collections.Generic;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Forms
{
	public partial class EditUISettings : Form
	{
		#region [ Constants ]

		private const string Group_Logging = "Logging";
		private const string Group_General = "General";
		private const string Group_Keyboard = "Keyboard";
		private const string Group_Alpha = "Alpha";
		private const string Group_Profile = "Profile";
		
		#endregion [ Constants ]

		#region [ Private Variables ]

		/// <summary>
		/// Workshop is a Singleton type of class. Simply getting the Instance variable from the Static object will get the object already loaded with our data
		/// </summary>
		private Workshop _workshop = Workshop.Instance;
		private Settings _settings = Settings.Instance;
		
		// Alpha
		private BaseProfile _tempProfile;
		private string PercentFormat = "0%;(0%);\"-\"";
		private byte _originalAlpha = 128;
		private byte _currentAlpha = 128;
		private int _selected = 0;

		// Keyboard
		private bool _capturing = false;
		private ListBoxUtil _listBoxUtil = null;
		private Dictionary<string, string> _dict = null; // <gesture, id>
		private SortedList<string, MultiKeyGesture> _gesturesList = null; // <id, obj>
		private string _keyboardFilename = string.Empty;

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

		public EditUISettings()
		{
			InitializeComponent();
			_listBoxUtil = new ListBoxUtil();

			ImageListItem Item = null;

			Item = new ImageListItem("General", Group_General, ElfRes.settings);
			lstGrouping.Items.Add(Item);
			Item = new ImageListItem("Keyboard", Group_Keyboard, ElfRes.keyboard);
			lstGrouping.Items.Add(Item);
			Item = new ImageListItem("Inactive Channel Transparency", Group_Alpha, ElfRes.transparency);
			lstGrouping.Items.Add(Item);
			lstGrouping.SelectedIndex = 0;
			
			//lstCommands.Items.Add(new ElfControls.ImageListItem("Logging", Group_Logging, ElfRes.settings));
			//lstCommands.Items.Add(new ElfControls.ImageListItem("Profiles", Group_Profile, ElfRes.profile));

			// General
			chkShowRulers.Checked = _workshop.UI.ShowRuler;
			chkDisableUndo.Checked = _workshop.UI.DisableUndo;
			chkShowExcludedChannels.Checked = _workshop.UI.ShowExcludedChannels;

			chkShowExcludedChannels.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Exclude, AnchorStyles.Bottom | AnchorStyles.Right);

			// Logging
			/*
			cboTraceLevel.SelectedIndex = (int)_workshop.TraceLevel;
			txtFileName.Text = _workshop.TraceLogFilename;			
			chkNoLogPlayback.Checked = _workshop.NoTraceDuringPlayback;
			*/

			//// Alpha
			_tempProfile = new BaseProfile(_workshop.AvailableProfiles[0].Profile.GetType());
			_tempProfile.UnclipCanvasWindow();
			_tempProfile.SubstituteCanvas = pctPreview;
			hslAlpha.IndicatorMarks.Clear();
			pctPreview.BackColor = Color.Black;
			_originalAlpha = _workshop.UI.InactiveChannelAlpha;
			_currentAlpha = _originalAlpha;
			hslAlpha.IndicatorMarks.Add((double)_originalAlpha);
			hslAlpha.Value = (double)_originalAlpha;
			CreatePreview();

			// Keyboard
			_keyboardFilename = _workshop.KeyboardController.Filename;
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
			MultiKeyGesture m = _gesturesList[id];
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
			_gesturesList = new SortedList<string, MultiKeyGesture>();
			_dict = new Dictionary<string, string>();
			MultiKeyGesture m = null;

			foreach (KeyValuePair<string, MultiKeyGesture> KVP in (getDefaults ? _workshop.KeyboardController.DefaultGestures : _workshop.KeyboardController.CurrentGestures))
			{
				m = new MultiKeyGesture(KVP.Value.ID, KVP.Value.Gesture, KVP.Value.MenuItem);
				_gesturesList.Add(m.ID, m);
				if (m.Gesture.Length > 0)
				{
					if (!_dict.ContainsKey(m.Gesture))
						_dict.Add(m.Gesture, m.ID);
				}
			}
			m = null;
			LoadGestures(_gesturesList);
		}

		/// <summary>
		/// Build the dictionary of gestures from the passed in list and reload the controls
		/// </summary>
		private void BuildDictionary(SortedList<string, MultiKeyGesture> list)
		{
			_dict = new Dictionary<string, string>();
			MultiKeyGesture m = null;

			foreach (KeyValuePair<string, MultiKeyGesture> KVP in list)
			{
				m = new MultiKeyGesture(KVP.Value.ID, KVP.Value.Gesture, KVP.Value.MenuItem);
				if (m.Gesture.Length > 0)
				{
					if (!_dict.ContainsKey(m.Gesture))
						_dict.Add(m.Gesture, m.ID);
				}
			}
			m = null;
			//LoadGestures(list);
			LoadGestures(FindCommands(txtSearch.Text));
		}

		/// <summary>
		/// Populate the temp Profile with some channels.
		/// </summary>
		private void CreatePreview()
		{
			Channel Channel = null;

			// Add Cells to make up a mini tree
			List<Point> MiniTree = new List<Point>();
			MiniTree.Add(new Point(0, 9));
			MiniTree.Add(new Point(1, 7));
			MiniTree.Add(new Point(1, 9));
			MiniTree.Add(new Point(2, 5));
			MiniTree.Add(new Point(2, 7));
			MiniTree.Add(new Point(2, 8));
			MiniTree.Add(new Point(2, 9));
			MiniTree.Add(new Point(3, 1));
			MiniTree.Add(new Point(3, 3));
			MiniTree.Add(new Point(3, 4));
			MiniTree.Add(new Point(3, 5));
			MiniTree.Add(new Point(3, 6));
			MiniTree.Add(new Point(3, 7));
			MiniTree.Add(new Point(3, 8));
			MiniTree.Add(new Point(3, 9));
			MiniTree.Add(new Point(4, 0));
			MiniTree.Add(new Point(4, 1));
			MiniTree.Add(new Point(4, 2));
			MiniTree.Add(new Point(4, 3));
			MiniTree.Add(new Point(4, 4));
			MiniTree.Add(new Point(4, 5));
			MiniTree.Add(new Point(4, 6));
			MiniTree.Add(new Point(4, 7));
			MiniTree.Add(new Point(4, 8));
			MiniTree.Add(new Point(4, 9));
			MiniTree.Add(new Point(5, 1));
			MiniTree.Add(new Point(5, 3));
			MiniTree.Add(new Point(5, 4));
			MiniTree.Add(new Point(5, 5));
			MiniTree.Add(new Point(5, 6));
			MiniTree.Add(new Point(5, 7));
			MiniTree.Add(new Point(5, 8));
			MiniTree.Add(new Point(5, 9));
			MiniTree.Add(new Point(6, 5));
			MiniTree.Add(new Point(6, 7));
			MiniTree.Add(new Point(6, 8));
			MiniTree.Add(new Point(6, 9));
			MiniTree.Add(new Point(7, 7));
			MiniTree.Add(new Point(7, 9));
			MiniTree.Add(new Point(8, 9));

			// Create a test profile and populate it with some test channels to create the image for the preview window
			_tempProfile.Scaling.CellSize = 7;
			_tempProfile.Scaling.Zoom = 1f;
			_tempProfile.Scaling.ShowGridLines = (_workshop.Profile != null) ? _workshop.Profile.Scaling.ShowGridLines : false;
			_tempProfile.Scaling.LatticeSize = _workshop.GetSizeInCells(pctPreview.Size, _tempProfile);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.Red;
			Channel.Origin = new Point(10, 3);
			Channel.Paint(MiniTree);
			Channel.IsSelected = true;
			Channel.SuppressEvents = false;
			Channel.Name = "Red Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.Yellow;
			Channel.Origin = new Point(24, 3);
			Channel.Paint(MiniTree);
			Channel.SuppressEvents = false;
			Channel.Name = "Yellow Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.Green;
			Channel.Origin = new Point(3, 15);
			Channel.Paint(MiniTree);
			Channel.SuppressEvents = false;
			Channel.Name = "Green Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.Blue;
			Channel.Origin = new Point(17, 15);
			Channel.Paint(MiniTree);
			Channel.SuppressEvents = false;
			Channel.Name = "Blue Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.White;
			Channel.Origin = new Point(31, 15);
			Channel.Paint(MiniTree);
			Channel.SuppressEvents = false;
			Channel.Name = "White Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = null;
		}

		/// <summary>
		/// Finds all the commands from the list whose ID at least partially matches the text.
		/// </summary>
		private SortedList<string, MultiKeyGesture> FindCommands(string text)
		{
			SortedList<string, MultiKeyGesture> Found = new SortedList<string, MultiKeyGesture>();
			MultiKeyGesture m = null;
			for (int i = 0; i < _gesturesList.Count; i++)
			{
				m = _gesturesList[_gesturesList.Keys[i]];
				if (m.ID.ToUpper().Contains(text.ToUpper()))
					Found.Add(m.ID, m);
			}
			return Found;
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
		/// Removes the gesture from the command indicated by the ID passed in.
		/// </summary>
		private void RemoveGesture(string id)
		{
			MultiKeyGesture m = _gesturesList[id];
			if (m == null)
				return;

			if (_dict.ContainsKey(m.Gesture))
				_dict.Remove(m.Gesture);
			m.Gesture = string.Empty;
			txtExisting.Text = string.Empty;
		}

		#endregion [ Methods ]

		//#region [ Events ]

		private void cboTraceLevel_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (cboTraceLevel.SelectedIndex)
			{
				case (int)TraceLevel.Off:
					lblTraceDesc.Text = "Output no tracing and debugging messages.";
					break;
				case (int)TraceLevel.Error:
					lblTraceDesc.Text = "Output only error-handling messages.";
					break;
				case (int)TraceLevel.Warning:
					lblTraceDesc.Text = "Output only warnings and error-handling messages.";
					break;
				case (int)TraceLevel.Info:
					lblTraceDesc.Text = "Output informational messages, warnings, and error-handling messages.";
					break;
				case (int)TraceLevel.Verbose:
					lblTraceDesc.Text = "Output all debugging and tracing messages.";
					break;
			}
		}

		private void cmdOk_Click(object sender, EventArgs e)
		{
			_workshop.UI.ShowRuler = chkShowRulers.Checked;
			_workshop.UI.InactiveChannelAlpha = _currentAlpha;
			_workshop.UI.DisableUndo = chkDisableUndo.Checked;

			//_workshop.TraceLevel = Util.EnumHelper.GetEnumFromValue<TraceLevel>(cboTraceLevel.SelectedIndex);
			//_workshop.TraceLogFilename = txtFileName.Text;			
			//_workshop.NoTraceDuringPlayback = chkNoLogPlayback.Checked;

			// Move the working key configuration into the keyboard controller's current list.
			_workshop.KeyboardController.AssignList(_gesturesList);

			_workshop.UI.SaveSettings();
			_workshop.KeyboardController.SaveSettings();
			_settings.Save();

			DialogResult = DialogResult.OK;
			Close();
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
			_gesturesList = new SortedList<string, MultiKeyGesture>();
			_dict = new Dictionary<string, string>();
			BuildDictionary(true);
		}

		private void cmdLoadKeyboard_Click(object sender, EventArgs e)
		{
			openFileDialog1.Title = "Open Keyboard Configuration";
			if (_keyboardFilename.Length == 0)
				_keyboardFilename = KeyboardController.KeyboardDefaultName;
			openFileDialog1.FileName = _keyboardFilename;
			openFileDialog1.Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*";

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				_keyboardFilename = openFileDialog1.FileName;
				// Read in the configuration saved to disk.
				KeyboardController.LoadConfigFile(_keyboardFilename, _gesturesList);
				BuildDictionary(_gesturesList);
			}
		}

		private void cmdLogFileBrowse_Click(object sender, EventArgs e)
		{
			saveFileDialog1.Title = "Select Log File";
			saveFileDialog1.FileName = txtLogFileName.Text;
			saveFileDialog1.Filter = "Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				txtLogFileName.Text = saveFileDialog1.FileName;
		}

		/// <summary>
		/// Removes the gesture from the currently selected command.
		/// </summary>
		private void cmdRemove_Click(object sender, EventArgs e)
		{
			_workshop.KeyboardController.ClearBuffer();
			RemoveGesture(CurrentID);
		}

		private void cmdSaveKeyboard_Click(object sender, EventArgs e)
		{
			saveFileDialog1.Title = "Save Keyboard Configuration";
			if (_keyboardFilename.Length == 0)
				_keyboardFilename = KeyboardController.KeyboardDefaultName;
			saveFileDialog1.FileName = _keyboardFilename;
			saveFileDialog1.Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*";

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				_keyboardFilename = saveFileDialog1.FileName;
				KeyboardController.Save(_keyboardFilename, _gesturesList);
			}
		}

		private void Form_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_capturing)
				_workshop.KeyboardController.MessageOccurred -= KeyboardController_MessageOccurred;
		}

		private void hslAlpha_Changed(object sender, EventArgs e)
		{
			lblAlpha.Text = (hslAlpha.Value / hslAlpha.MaxValue).ToString(PercentFormat);
			_currentAlpha = Convert.ToByte(Math.Round(hslAlpha.Value));
			_workshop.UI.InactiveChannelAlpha = _currentAlpha;
			pctPreview.Refresh();
		}

		private void KeyboardController_MessageOccurred(object sender, MessageEventArgs e)
		{
			txtNewKey.Text = e.Message;
			FindMatch(txtNewKey.Text);
			if (txtNewKey.Text.Contains(","))
				_workshop.KeyboardController.ClearBuffer();
		}

		private void pctPreview_Paint(object sender, PaintEventArgs e)
		{
			Matrix MoveMatrix = null;
			GraphicsPath Path = null;
			Point Offset;
			if (_tempProfile == null)
				return;

			try
			{
				Channel Channel = null;
				for (int i = 0; i < _tempProfile.Channels.Count; i++)
				{
					Channel = _tempProfile.Channels[i];

					// If the Channel is one of the selected Channels, do not draw it here in this block
					if (Channel.IsSelected)
						continue;

					using (SolidBrush ChannelBrush = new SolidBrush(Color.FromArgb(_workshop.UI.InactiveChannelAlpha, Channel.GetColor())))
					{
						Path = (GraphicsPath)Channel.GetGraphicsPath().Clone();

						Offset = _workshop.CalcCanvasPoint(Channel.Origin, _tempProfile);
						MoveMatrix = new Matrix();
						MoveMatrix.Translate(Offset.X, Offset.Y);
						Path.Transform(MoveMatrix);
						MoveMatrix.Dispose();

						e.Graphics.FillPath(ChannelBrush, Path);
					}
				}

				Offset = Point.Empty;

				// Draw the Selected Channels on top of the unselected ones (for clarity)
				if (_tempProfile.Channels.Selected.Count > 0)
				{
					foreach (Channel SelectedChannel in _tempProfile.Channels.Selected.OrderByDescending())
					{
						using (SolidBrush ChannelBrush = new SolidBrush(SelectedChannel.GetColor()))
						{
							Path = (GraphicsPath)SelectedChannel.GetGraphicsPath().Clone();

							Offset = _workshop.CalcCanvasPoint(SelectedChannel.Origin, _tempProfile);
							MoveMatrix = new Matrix();
							MoveMatrix.Translate(Offset.X, Offset.Y);
							Path.Transform(MoveMatrix);
							MoveMatrix.Dispose();

							e.Graphics.FillPath(ChannelBrush, Path);
						}
					}
				}
			}
			finally
			{
				Path = null;
				MoveMatrix = null;
			}
		}

		private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
		{
			_workshop.KeyboardController.ClearBuffer();
			txtExisting.Text = string.Empty;
			txtNewKey.Text = string.Empty;
			txtConflicts.Text = string.Empty;

			if (lstCommands.SelectedIndex < 0)
				return;

			string CurrentID = this.CurrentID;
			if (_gesturesList.Keys.Contains(CurrentID))
			{
				MultiKeyGesture m = _gesturesList[CurrentID];
				if (m == null)
					return;
				txtExisting.Text = m.Gesture;
			}
		}

		/// <summary>
		/// Display the panel that corresponds to the setting group name.
		/// </summary>
		private void lstGrouping_SelectedIndexChanged(object sender, EventArgs e)
		{
			pnlGeneral.Visible = false;
			pnlAlpha.Visible = false;
			pnlKeyboard.Visible = false;
			pnlLogging.Visible = false;

			switch (_listBoxUtil.GetKey(lstGrouping))
			{
				case Group_General:
					pnlGeneral.Visible = true;
					pnlGeneral.Focus();
					break;
				case Group_Keyboard:
					pnlKeyboard.Visible = true;
					pnlKeyboard.Focus();
					break;
				case Group_Alpha:
					pnlAlpha.Visible = true;
					pnlAlpha.Focus();
					break;
				case Group_Logging:
					pnlLogging.Visible = true;
					pnlLogging.Focus();
					break;
			}
		}

		private void tmrAnimation_Tick(object sender, EventArgs e)
		{
			_selected++;
			if (_selected % 5 == 0)
				_selected = 0;
			_tempProfile.Channels.Select(_selected);
			pctPreview.Refresh();
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

		private void txtNewKey_TextChanged(object sender, EventArgs e)
		{
			cmdAssign.Enabled = (txtNewKey.TextLength > 0);
			// Don't give the option to save if the new keystroke matches the existing one.
			if (cmdAssign.Enabled)
				cmdAssign.Enabled = (txtNewKey.Text != txtExisting.Text);
		}

		private void txtNewKey_Leave(object sender, EventArgs e)
		{
			if (_capturing)
				_workshop.KeyboardController.MessageOccurred -= KeyboardController_MessageOccurred;
			_capturing = false;
			_workshop.KeyboardController.ClearBuffer();
		}

		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			LoadGestures(FindCommands(txtSearch.Text));
		}

		private void txtExisting_TextChanged(object sender, EventArgs e)
		{
			cmdRemove.Enabled = (txtExisting.TextLength > 0);
		}



		//		#endregion [ Events ]

	}
}
