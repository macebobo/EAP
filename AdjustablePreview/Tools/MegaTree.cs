using System;
using System.Collections.Generic;
using System.Text;
using ElfRes = ElfCore.Properties.Resources;
using ElfCore.PlugIn;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ElfCore.Tools
{
	[AdjPrevTool("MegaTree")]
	public class MegaTreeTool : MultiChannelBase, ITool
	{

		#region [ Enums ]

		enum TravelDirection
		{
			Anticlockwise = -1,
			NotSet = 0,
			Clockwise = +1
		}

		#endregion [ Enums ]

		#region [ Private Variables ]

		// Settings from the ToolStrip
		private int _numChannelsPerLineGroup = 0;
		private int _numLines = 0;
		private int _numLinesInGroup = 0;
		private float _numRotations = float.MaxValue;
		private TravelDirection _rotationDirection = TravelDirection.NotSet;

		// Controls from ToolStrip
		private ToolStripButton AntiClockwise = null;
		private ToolStripButton Clockwise = null;

		private ToolStripLabel Warning_NumLines = null;
		private ToolStripLabel Warning_NumLinesPerChan = null;
		private ToolStripLabel Warning_NumChannelsPerLineGroup = null;

		private ToolStripTextBox txtNumLines = null;
		private ToolStripTextBox txtNumLinesPerChan = null;
		private ToolStripTextBox txtNumChannelsPerLineGroup = null;
		private ToolStripTextBox txtNumRotations = null;

		private ToolStripLabel XChannels = null;
		
		private string _Error_InvalidValue = "Value cannot be zero.";
		private string _Error_TooManyChannels = "Insufficient number of Channels available at this position. Please use fewer Channels or start at a lower Channel number.";
		private string _Error_TooManyLinesInGroup = "Cannot have more lines in a group than there are lines to begin with.";

		#endregion [ Private Variables ]

		#region [ Constants ]

		private const string NUM_LINES2BASE = "NumberOfLinesToBase";
		private const string NUM_LINESGROUP = "NumberOfLinesInGroup";
		private const string NUM_CHANNELSGROUP = "NumberOfChannelsPerLineGroup";
		private const string NUM_ROTATIONS = "NumberOfRotations";
		private const string SPINDIRECTION = "SpinDirection";

		private const string DEFAULT_NUM_LINES2BASE = "8";
		private const string DEFAULT_NUM_LINESGROUP = "1";
		private const string DEFAULT_NUM_CHANNELSGROUP = "1";
		private const string DEFAULT_NUM_ROTATIONS = "0";

		private const string XChanText = " = ({0} Channels, in {1} group{2})";

		#endregion [ Constants ]
	
		#region [ Constructors ]

		public MegaTreeTool() : base()
		{
			this.ID = (int)Tool.MegaTree;
			this.Name = "MegaTree";
			this.ToolBoxImage = ElfRes.megatree;
			base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.tree_modifier, new Point(15, 15));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private string BadIntegerMathMessage()
		{
			return string.Format("Number of Lines ({0}) must be divided by the Number of Lines in a Group ({1}) evenly with no remainder.", _numLines, _numLinesInGroup);
		}

		/// <summary>
		/// Hides a particular warning on the settings toolstrip
		/// </summary>
		/// <param name="warningIndex"></param>
		private void ClearWarning(int warningIndex)
		{
			switch (warningIndex)
			{
				case 1:
					Warning_NumLines.Visible = false;
					break;
				case 2:
					Warning_NumLinesPerChan.Visible = false;
					break;
				case 3:
				default:
					Warning_NumChannelsPerLineGroup.Visible = false;
					break;
			}
		}

		/// <summary>
		/// Hides all the warnings from the settings toolstrip
		/// </summary>
		private void ClearWarnings()
		{
			Warning_NumLines.Visible = false;
			Warning_NumLinesPerChan.Visible = false;
			Warning_NumChannelsPerLineGroup.Visible = false;
		}

		/// <summary>
		/// Create the Channels for the MegaTree. If there is rotation given, this becomes a spiral tree.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected override List<GraphicsPath> CreateRenderPathList(Point p1, Point p2, bool finalRender)
		{
			Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
			PointF Top = new PointF();
			List<GraphicsPath> Paths = new List<GraphicsPath>();
			float PI = (float)Math.PI;
			float Theta;
			float DeltaTheta;
			int ChannelIndex = _workshop.Channels.Active.Index;
			int NumGroups = _numLines / _numLinesInGroup;

			DrawArea = _workshop.NormalizedRectangle(p1, p2);
			Top = new PointF(DrawArea.X + DrawArea.Width / 2, DrawArea.Top);

			if (finalRender)
			{
				DeltaTheta = 2 * PI / _numLines;
				float StartAngle = DeltaTheta / 3f;

				// Outer loop is the number of times we go around the tree, duplicating blocks of lines
				for (int d = 0; d < _numChannelsPerLineGroup; d++)
				{
					Theta = StartAngle;

					// Divide the number of lines in the tree by the number of Channels in a block of lines and loop
					// through that from the current Channel
					for (int c = ChannelIndex; c < ChannelIndex + NumGroups; c++)
					{
						if (c > _workshop.Channels.Count - 1)
							break;

						// Draw all the lines from top to base for this Channel
						for (int i = 0; i < _numLinesInGroup; i++)
						{
							Paths.Add(CreateChannelLine(Top, DrawArea.Width / 2f, Theta, DrawArea.Height));
							Theta += DeltaTheta;
						}
					}
					ChannelIndex += NumGroups;
				}
			}
			else
			{
				GraphicsPath PreviewPath = new GraphicsPath();
				PreviewPath.AddLine(Top, new PointF(DrawArea.Left, DrawArea.Bottom));
				PreviewPath.AddLine(new PointF(DrawArea.Left, DrawArea.Bottom), new PointF(DrawArea.Right, DrawArea.Bottom));
				PreviewPath.AddLine(new PointF(DrawArea.Right, DrawArea.Bottom), Top);
				Paths.Add(PreviewPath);
				PreviewPath = null;
			}

			return Paths;
		}

		/// <summary>
		/// Creates a GraphicsPath for a given Channel.
		/// </summary>
		/// <param name="topPoint">Top of the tree</param>
		/// <param name="maxInterations">Number of lines used to generate this curve</param>
		/// <param name="radius">Radius of the tree at the bottom (half the width of the drawing area)</param>
		/// <param name="startAngle">Unique starting angle for this Channel</param>
		/// <param name="height">Height of the tree</param>
		private GraphicsPath CreateChannelLine(PointF topPoint, float radius, float startAngle, float height)
		{
			PointF LastPoint = Point.Empty;
			PointF ThisPoint;
			float Radius;
			float X, Y;
			float Theta;
			float Rotation;
			float maxInterations = 30; // Number of line segements used to generate this "curve"
			float RotationDirection = (float)((int)_rotationDirection);
			float ThetaCalc = RotationDirection * 2 * (float)Math.PI * _numRotations / maxInterations;
			GraphicsPath Path = new GraphicsPath();

			for (int j = 0; j <= maxInterations; j++)
			{
				Radius = radius * (1 - j / maxInterations);
				Theta = j * ThetaCalc;
				Rotation = Theta + startAngle;
				X = Radius * (float)Math.Cos(Rotation);
				Y = j * height / maxInterations;

				X += topPoint.X;
				Y = height - Y + topPoint.Y;

				ThisPoint = new PointF(X, Y);
				if (!LastPoint.IsEmpty)
					Path.AddLine(LastPoint, ThisPoint);
				LastPoint = ThisPoint;
			}
			return Path;
		}

		/// <summary>
		/// Formats the channel count information for the end user
		/// </summary>
		private void FormatChannelCount()
		{ 
			float NumGroups = NumberOfGroups();
			string Groups = NumGroups.ToString("0.00").Replace(".00", string.Empty);
			string Plural = (NumGroups == 1) ? "" : "s";
			
			XChannels.Text = string.Format(XChanText, NumberOfChannels(), Groups, Plural);
		}

		/// <summary>
		/// Load in the saved values from the Settings Xml file. The path to be used should be 
		/// ToolSettings|[Name of this tool].
		/// We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
		/// format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
		/// for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
		/// </summary>
		/// <param name="settings">Settings object, handles getting and saving settings data</param>
		/// <param name="workshop">Workshop object, contains lots of useful methods and ways to hold data.</param>
		public override void Initialize()
		{
			base.Initialize();

			// Load the Settings values
			_numLines = LoadValue(NUM_LINES2BASE, 8);
			_numLinesInGroup = LoadValue(NUM_LINESGROUP, 1);
			_numChannelsPerLineGroup = LoadValue(NUM_CHANNELSGROUP, 1);
			_numRotations = LoadValue(NUM_ROTATIONS, 0);
			_rotationDirection = EnumHelper.GetEnumFromValue<TravelDirection>(LoadValue(SPINDIRECTION, (int)TravelDirection.Anticlockwise));

			// Get a pointer to the controls on the toolstrip that belongs to us.
			AntiClockwise = (ToolStripButton)GetItem<ToolStripButton>(1);
			Clockwise = (ToolStripButton)GetItem<ToolStripButton>(2);

			txtNumLines = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);
			txtNumLinesPerChan = (ToolStripTextBox)GetItem<ToolStripTextBox>(2);
			txtNumChannelsPerLineGroup = (ToolStripTextBox)GetItem<ToolStripTextBox>(3);
			txtNumRotations = (ToolStripTextBox)GetItem<ToolStripTextBox>(4);

			Warning_NumLines = (ToolStripLabel)GetItem<ToolStripLabel>(3);
			Warning_NumLinesPerChan = (ToolStripLabel)GetItem<ToolStripLabel>(5);
			Warning_NumChannelsPerLineGroup = (ToolStripLabel)GetItem<ToolStripLabel>(8);
			XChannels = (ToolStripLabel)GetItem<ToolStripLabel>(7);

			// Attach events to these controls
			AntiClockwise.Click += new System.EventHandler(this.AntiClockwise_Click);
			Clockwise.Click += new System.EventHandler(this.Clockwise_Click);

			txtNumLines.Leave += new System.EventHandler(this.txtNumLines_Leave);
			txtNumLines.TextChanged += new System.EventHandler(this.Settings_TextChanged);
			txtNumLinesPerChan.Leave += new System.EventHandler(this.txtNumLinesPerChan_Leave);
			txtNumLinesPerChan.TextChanged += new System.EventHandler(this.Settings_TextChanged);
			txtNumChannelsPerLineGroup.Leave += new System.EventHandler(this.txtNumChannelsPerLineGroup_Leave);
			txtNumChannelsPerLineGroup.TextChanged += new System.EventHandler(this.Settings_TextChanged);
			txtNumRotations.Leave += new System.EventHandler(this.txtNumRotations_Leave);

			// Set the initial value for the contol from what we had retrieve from Settings
			AntiClockwise.Checked = (_rotationDirection == TravelDirection.Anticlockwise);
			Clockwise.Checked = (_rotationDirection == TravelDirection.Clockwise);
			txtNumLines.Text = _numLines.ToString();
			txtNumLinesPerChan.Text = _numLinesInGroup.ToString();
			txtNumChannelsPerLineGroup.Text = _numChannelsPerLineGroup.ToString();
			txtNumRotations.Text = _numRotations.ToString();

			FormatChannelCount();
			ClearWarnings();
		}

		/// <summary>
		/// Calculate the number of Channels that are needed.
		/// </summary>
		protected override int NumberOfChannels()
		{
			return (_numLinesInGroup == 0) ? 0 : (_numLines / _numLinesInGroup) * _numChannelsPerLineGroup;
		}

		/// <summary>
		/// Determine the number of Channel groups we have.
		/// </summary>
		/// <returns></returns>
		private float NumberOfGroups()
		{
			return (float)_numLines / (float)_numLinesInGroup;
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		/// <param name="settings"></param>
		public override void SaveSettings()
		{
			SaveValue(NUM_LINES2BASE, _numLines);
			SaveValue(NUM_LINESGROUP, _numLinesInGroup);
			SaveValue(NUM_CHANNELSGROUP, _numChannelsPerLineGroup);
			SaveValue(NUM_ROTATIONS, _numRotations);
			SaveValue(SPINDIRECTION, (int)_rotationDirection);
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			AntiClockwise = null;
			Clockwise = null;

			Warning_NumLines = null;
			Warning_NumLinesPerChan = null;
			Warning_NumChannelsPerLineGroup = null;

			txtNumLines = null;
			txtNumLinesPerChan = null;
			txtNumChannelsPerLineGroup = null;
			txtNumRotations = null;

			XChannels = null;
		}

		/// <summary>
		/// Display a warning message on a particular label
		/// </summary>
		/// <param name="warningIndex"></param>
		/// <param name="message"></param>
		private void ShowWarning(int warningIndex, string message)
		{
			ToolStripLabel Warning = null;
			switch (warningIndex)
			{ 
				case 1:
					Warning = Warning_NumLines;
					break;
				case 2:
					Warning = Warning_NumLinesPerChan;
					break;
				case 3:
				default:
					Warning = Warning_NumChannelsPerLineGroup;
					break;
			}
			Warning.Visible = true;
			Warning.ToolTipText = message;
		}

		#endregion [ Methods ]

		#region [ ToolStrip Events ]

		private void AntiClockwise_Click(object sender, EventArgs e)
		{
			if (AntiClockwise.Checked)
				return;
			Clockwise.Checked = false;
			AntiClockwise.Checked = true;
			_rotationDirection = TravelDirection.Anticlockwise;
		}

		private void Clockwise_Click(object sender, EventArgs e)
		{
			if (Clockwise.Checked)
				return;
			AntiClockwise.Checked = false;
			Clockwise.Checked = true;
			_rotationDirection = TravelDirection.Clockwise;
		}

		private void Settings_TextChanged(object sender, EventArgs e)
		{
			ClearWarnings();
		}

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// Afterwards, perform validations to make sure the math is correct.
		/// </summary>
		private void txtNumLines_Leave(object sender, EventArgs e)
		{
			if (txtNumLines.TextLength == 0)
				txtNumLines.Text = DEFAULT_NUM_LINES2BASE;

			_numLines = ValidateInteger(txtNumLines, _numLines);

			FormatChannelCount();

			if (_numLines == 0)
				ShowWarning(1, _Error_InvalidValue);

			else if (NumberOfChannels() > _workshop.Channels.Count)
				ShowWarning(1, _Error_TooManyChannels);

			else if (_numLinesInGroup > _numLines)
				ShowWarning(1, _Error_TooManyLinesInGroup);

			else if ((_numLines % _numLinesInGroup) != 0)
				ShowWarning(1, BadIntegerMathMessage());
		}

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// Afterwards, perform validations to make sure the math is correct.
		/// </summary>
		private void txtNumLinesPerChan_Leave(object sender, EventArgs e)
		{
			if (txtNumLinesPerChan.TextLength == 0)
				txtNumLinesPerChan.Text = DEFAULT_NUM_LINESGROUP;

			_numLinesInGroup = ValidateInteger(txtNumLinesPerChan, _numLinesInGroup);

			FormatChannelCount();

			if (_numLinesInGroup == 0)
				ShowWarning(2, _Error_InvalidValue);

			else if (_numLinesInGroup > _numLines)
				ShowWarning(2, _Error_TooManyLinesInGroup);

			else if ((_numLines % _numLinesInGroup) != 0)
				ShowWarning(2, BadIntegerMathMessage());

			else if (NumberOfChannels() > _workshop.Channels.Count)
				ShowWarning(2, _Error_TooManyChannels);
		}

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// Afterwards, perform validations to make sure the math is correct.
		/// </summary>
		private void txtNumChannelsPerLineGroup_Leave(object sender, EventArgs e)
		{
			if (txtNumChannelsPerLineGroup.TextLength == 0)
				txtNumChannelsPerLineGroup.Text = DEFAULT_NUM_CHANNELSGROUP;

			_numChannelsPerLineGroup = ValidateInteger(txtNumChannelsPerLineGroup, _numChannelsPerLineGroup);

			FormatChannelCount();

			if (_numChannelsPerLineGroup == 0)
				ShowWarning(3, _Error_InvalidValue);

			else if (NumberOfChannels() > _workshop.Channels.Count)
				ShowWarning(3, _Error_TooManyChannels);
		}

		private void txtNumRotations_Leave(object sender, EventArgs e)
		{
			if (txtNumRotations.TextLength == 0)
				txtNumRotations.Text = DEFAULT_NUM_ROTATIONS;

			string Value = txtNumRotations.Text;
			if (Value.Length > 0)
				_numRotations = (float)Convert.ToDecimal(Value);
		}

		#endregion [ ToolStrip Events ]

	}
}



