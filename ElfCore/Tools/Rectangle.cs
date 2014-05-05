using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[ElfEditTool("Rectangle")]
	public class RectangleTool : ShapeBase, ITool
	{
		#region [ Enums ]

		enum CornerStyle
		{
			NotSet,
			Square,
			Round
		}

		[Flags]
		enum Corners
		{
			None = 0,
			TopLeft = 1,
			TopRight = 2,
			BottomLeft = 4,
			BottomRight = 8,
			All = TopLeft | TopRight | BottomLeft | BottomRight
		}

		#endregion [ Enums ]

		#region [ Private Variables ]

		// Settings from the ToolStrip
		private CornerStyle _cornerStyle = CornerStyle.NotSet;
		private int _cornerRadius = -1;

		// Controls from ToolStrip
		private ToolStripButton SquareCorner = null;
		private ToolStripButton RoundCorner = null;
		private ToolStripTextBox Radius = null;
		private ToolStripLabel RadiusLabel = null;

		#endregion [ Private Variables ]

		#region [ Constants ]

		public const string CORNER_STYLE = "CornerStyle";

		#endregion [ Constants ]

		#region [ Constructors ]

		public RectangleTool() : base()
		{			
			this.ID = (int)ToolID.Rectangle;
			this.Name = "Rectangle";
			this.ToolBoxImage = ElfRes.rectangle;
			//this.Cursor = CustomCursors.MemoryCursor(ElfRes.cross_rectangle);
			base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.rectangle_modifier, new Point(15, 15));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Attaches or detaches events to objects, such as Click events to buttons.
		/// </summary>
		/// <param name="attach">Indicates that the events should be attached. If false, then detaches the events</param>
		protected override void AttachEvents(bool attach)
		{
			// If we've already either attached or detached, exit out.
			if (attach && _eventsAttached)
				return;

			if (attach)
			{
				SquareCorner.Click += new EventHandler(SquareCorner_Click);
				RoundCorner.Click += new EventHandler(RoundCorner_Click);
				Radius.Leave += new EventHandler(Radius_Leave);
			}
			else
			{
				SquareCorner.Click -= SquareCorner_Click;
				RoundCorner.Click -= RoundCorner_Click;
				Radius.Leave -= Radius_Leave;
			}
			base.AttachEvents(attach);
		}

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, bool finalRender)
		{
			GraphicsPath DrawPath = new GraphicsPath();
			Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);

			//if (finalRender)
			//{
			//    System.Diagnostics.Debug.WriteLine(p1);
			//    System.Diagnostics.Debug.WriteLine(p2);
			//    System.Diagnostics.Debug.WriteLine(DrawArea);
			//}

			float CellScale = Scaling.CellScaleF;
			float Radius = _cornerRadius * CellScale;

			if (!finalRender)
				Radius *= CellScale;

			if ((_cornerStyle == CornerStyle.Round) && (_cornerRadius > 0))
				DrawPath = RoundedRectangle(DrawArea, Radius, Corners.All);
			else
				DrawPath.AddRectangle(DrawArea);

			return DrawPath;
		}

		/// <summary>
		/// Load in the saved values from the Settings Xml file. The path to be used should be 
		/// ToolSettings|[Name of this tool].
		/// We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
		/// format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
		/// for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			// Load the Settings values
			_cornerRadius = LoadValue(Constants.RADIUS, 0);
			_cornerStyle = EnumHelper.GetEnumFromValue<CornerStyle>(LoadValue(CORNER_STYLE, (int)CornerStyle.Square));

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			Radius = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);
			RadiusLabel = (ToolStripLabel)GetItem<ToolStripLabel>(4);

			SquareCorner = (ToolStripButton)GetItem<ToolStripButton>(3);
			RoundCorner = (ToolStripButton)GetItem<ToolStripButton>(4);

			// Assign the DashStyle enum value to the tag of each dash menu item
			SetDashStyleDropDownButton(DashStyleDD);
			
			// Set the initial value for the contol from what we had retrieve from Settings
			SquareCorner.Checked = (_cornerStyle == CornerStyle.Square);
			RoundCorner.Checked = (_cornerStyle == CornerStyle.Round);
			Radius.Enabled = RoundCorner.Checked;
			RadiusLabel.Enabled = RoundCorner.Checked;

			Radius.Text = _cornerRadius.ToString();
		}

		// http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/d805a3fd-79a7-42c2-9416-83df5df04ca8
		private GraphicsPath RoundedRectangle(Rectangle bounds, float radius, Corners corners)
		{
			// Make sure the Path fits inside the rectangle
			//bounds.Width--;
			//bounds.Height--;

			// Scale the radius if it's too large to fit.
			if (radius > (bounds.Width))
				radius = bounds.Width;
			if (radius > (bounds.Height))
				radius = bounds.Height;

			GraphicsPath path = new GraphicsPath();

			if (radius <= 0)
				path.AddRectangle(bounds);
			else
			{
				if ((corners & Corners.TopLeft) == Corners.TopLeft)
					path.AddArc(bounds.Left, bounds.Top, radius, radius, 180, 90);
				else
					path.AddLine(bounds.Left, bounds.Top, bounds.Left, bounds.Top);

				if ((corners & Corners.TopRight) == Corners.TopRight)
					path.AddArc(bounds.Right - radius, bounds.Top, radius, radius, 270, 90);
				else
					path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Top);

				if ((corners & Corners.BottomRight) == Corners.BottomRight)
					path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
				else
					path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);

				if ((corners & Corners.BottomLeft) == Corners.BottomLeft)
					path.AddArc(bounds.Left, bounds.Bottom - radius, radius, radius, 90, 90);
				else
					path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Bottom);
			}

			path.CloseFigure();
			return path;
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			SaveValue(Constants.LINE_THICKNESS, _lineThickness);
			SaveValue(Constants.FILL, _fill);
			SaveValue(Constants.DASH_STYLE, (int)_dashStyle);
			SaveValue(CORNER_STYLE, (int)_cornerStyle);
			SaveValue(Constants.RADIUS, _cornerRadius);
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			DoFill = null;
			DoNotFill = null;
			SquareCorner = null;
			RoundCorner = null;
			LineThickness = null;
			DashStyleDD = null;
			Radius = null;
		}		

		#endregion [ Methods ]

		#region [ ToolStrip Event Delegates ]

		private void SquareCorner_Click(object sender, EventArgs e)
		{
			if (SquareCorner.Checked)
				return;
			RoundCorner.Checked = false;
			SquareCorner.Checked = true;
			_cornerStyle = CornerStyle.Square;
			Radius.Enabled = false;
			RadiusLabel.Enabled = false;
		}

		private void RoundCorner_Click(object sender, EventArgs e)
		{
			if (RoundCorner.Checked)
				return;
			SquareCorner.Checked = false;
			RoundCorner.Checked = true;
			_cornerStyle = CornerStyle.Round;
			Radius.Enabled = true;
			RadiusLabel.Enabled = true;
		}

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// </summary>
		private void Radius_Leave(object sender, EventArgs e)
		{
			_cornerRadius = ValidateInteger((ToolStripTextBox)sender, _cornerRadius);
		}

		#endregion [ ToolStrip Event Delegates ]
			
	}
}
