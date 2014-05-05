using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Drawing;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfCore.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Tools
{
	[ElfEditTool("Spray")]
	public class SprayTool : BaseTool, ITool
	{
		#region [ Private Variables ]

		// Settings from the ToolStrip
		private int _radius = 5;
		private int _intensity = 3;

		// Controls from ToolStrip
		private ToolStripTextBox Radius = null;
		private Timer SprayTimer = null;
		private TrackBar IntensityTracker = null;

		// Used for rendering
		private Random _random = new Random();
		private SolidBrush _paintBrush = null;
		private Rectangle _nibRect;
		private Rectangle _bounds = Rectangle.Empty;
		private LockBitmap _lockBitmap = null;

		// Masking variables
		private Region _maskRegion = null;
		private bool _hasMask = false;

		#endregion [ Private Variables ]

		#region [ Constants ]

		private const string INTENSITY = "Intensity";
		private const string DEFAULT_RADIUS = "5";

		#endregion [ Constants ]

		#region [ Properties ]

		/// <summary>
		/// Cursor to use on the Canvas window when the mouse is within its bounds. A safe cursor to use might be: Cursors.Cross
		/// </summary>
		public override Cursor Cursor
		{
			get 
			{
				float Radius = (float)_radius * Scaling.CellScaleF;
				float Width = 0;
				float Height = 0;
				PointF CenterPt;

				if (Radius >= 16)
				{
					Width = 2 * (Radius + 1);
					Height = 2 * (Radius + 1);
					CenterPt = new PointF(Radius + 1, Radius + 1);
				}
				else
				{
					// compositing the fuzzy circle with the spray can image
					Width = 2 * (Radius + 1) + 16;
					Height = 2 * (Radius + 1) + 13;
					CenterPt = new PointF((Radius + 1) + 16, (Radius + 1));
				}

				Bitmap CursorBmp = new Bitmap((int)Width, (int)Height);

				Graphics g = Graphics.FromImage(CursorBmp);
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.Clear(Color.Transparent);

				// Draw concentric circles of white with increasing transparency towards the outside
				float AlphaRad = (float)Math.Ceiling(255 / Radius);

				byte Alpha = 0;
				if (!Byte.TryParse(AlphaRad.ToString(), out Alpha))
					Alpha = 128;

				Color AlphaColor = Color.FromArgb(Alpha, Color.White);

				for (int i = 0; i <= Radius; i++)
				{
					g.FillEllipse(new SolidBrush(AlphaColor), new RectangleF(CenterPt.X - i, CenterPt.Y - i, i * 2, i * 2));
				}
				g.DrawImage(ElfRes.spray, new Point(0, (int)CenterPt.Y - 3));
				g.Dispose();

				if (base.Cursor != null)
					CustomCursors.DestroyCreatedCursor(base.Cursor.Handle);

				base.Cursor = CustomCursors.CreateCursor(CursorBmp, (int)CenterPt.X, (int)CenterPt.Y);

				return base.Cursor;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public SprayTool()
		{
			this.ID = (int)ToolID.Spray;
			this.Name = "Spray";
			this.ToolBoxImage = ElfRes.spray;
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
				Radius.Leave += new EventHandler(Radius_Leave);
				IntensityTracker.Scroll += new EventHandler(IntensityTracker_Scroll);
				SprayTimer.Tick += new EventHandler(SprayTimer_Tick);
				if (Profile != null)
					Profile.ScalingChanged += new EventHandler(Profile_ScalingChanged);
			}
			else
			{
				Radius.Leave -= this.Radius_Leave;
				IntensityTracker.Scroll -= IntensityTracker_Scroll;
				SprayTimer.Tick -= SprayTimer_Tick;
				if (Profile != null)
					Profile.ScalingChanged -= Profile_ScalingChanged;
			}
			base.AttachEvents(attach);
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
			_radius = LoadValue(Constants.RADIUS, _radius);
			_intensity = LoadValue(INTENSITY, _intensity);

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			Radius = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);
			SprayTimer = _toolStrip_Form.ToolTimer;
			IntensityTracker = ((ToolStripControlHost)GetItem<ToolStripControlHost>(2)).Control as TrackBar;

			// Set the initial value for the contol from what we had retrieve from Settings
			Radius.Text = _radius.ToString();
			IntensityTracker.Value = _intensity;
		}

		/// <summary>
		/// Checks to see if there is a mask in place. If so, is this point visible therein?
		/// </summary>
		/// <param name="pt">Point to check</param>
		private bool IsValidMaskPoint(Point pt)
		{
			if (!_hasMask)
				return true;
			else
				return _maskRegion.IsVisible(pt);
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			base.MouseDown(buttons, latticePoint, actualCanvasPoint);

			_hasMask = Profile.HasMask;
			if (_hasMask)
				_maskRegion = new Region(Profile.GetMaskOutline(UnitScale.Lattice));

			_paintBrush = Profile.Channels.Active.GetBrush();
			_nibRect = new Rectangle(0, 0, 1, 1);

			Size LatticeSize = Scaling.LatticeSize;

			_latticeBuffer = new Bitmap(LatticeSize.Width, LatticeSize.Height);
			_bounds = new Rectangle(new Point(0, 0), LatticeSize);

			SetMaskClip();

			_lockBitmap = new LockBitmap(_latticeBuffer);

			#if DEBUG
				Workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
			#endif

			SprayTimer.Enabled = true;
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (Profile == null)
				return false;
			
			if (!_isMouseDown)
				return false;

			_currentLatticePoint = latticePoint;
			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (!base.MouseUp(buttons, latticePoint, actualCanvasPoint))
				return false;

			SprayTimer.Enabled = false;
			_canvasControlGraphics.ResetClip();

			foreach (BaseChannel Channel in Profile.Channels.Selected)
			{
				Profile.Channels.PopulateChannelFromBitmap(_latticeBuffer, Channel, false);
				Channel.DedupeData();
			}

			Profile.Refresh();

			#if DEBUG
				Workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
			#endif

			PostDrawCleanUp();
			return true;
		}

		/// <summary>
		/// Paint the spray point onto the painting bitmap and the canvas
		/// </summary>
		/// <param name="pt">Location (in cells) in which to paint</param>
		private void PaintPoint(Point pt)
		{
			if (!(_bounds.Contains(pt) && IsValidMaskPoint(pt)))
				return;

			_lockBitmap.SetPixel(pt.X, pt.Y, Color.White);
			int CellZoom = Scaling.CellZoom;
			Point P = _workshop.CalcCanvasPoint(pt);
			int Grid = (int)Scaling.GridLineWidthZoom;
			P.Offset(Grid, Grid);
			_canvasControlGraphics.FillRectangle(_paintBrush, new Rectangle(P, new Size(CellZoom, CellZoom)));
		}
		
		/// <summary>
		/// Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
		/// </summary>
		protected override void PostDrawCleanUp()
		{
			if (_paintBrush != null)
			{
				_paintBrush.Dispose();
				_paintBrush = null;
			}
			if (_maskRegion != null)
			{
				_maskRegion.Dispose();
				_maskRegion = null;
			}
			_lockBitmap = null;
			base.PostDrawCleanUp();
		}

		/// <summary>
		/// Method that fires when this Tool is selected in the ToolBox.
		/// For this tool, it sets the timer interval to 1
		/// </summary>
		public override void OnSelected()
		{
			base.OnSelected();

			SprayTimer.Enabled = false;
			SprayTimer.Interval = 1;
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			SaveValue(Constants.RADIUS, _radius);
			SaveValue(INTENSITY, _intensity);
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();
			Radius = null;
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// </summary>
		public override void OnUnselected()
		{
			SprayTimer.Enabled = false;
			base.OnUnselected();
		}

		#endregion [ Methods ]

		#region [ ToolStrip Event Delegates ]

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// </summary>
		private void Radius_Leave(object sender, EventArgs e)
		{
			if (Radius.TextLength == 0)
				Radius.Text = DEFAULT_RADIUS;

			_radius = ValidateInteger(Radius, _radius);
			Profile.Cursor = this.Cursor;
		}

		/// <summary>
		/// Spray out a number of points onto the Channel
		/// </summary>
		private void SprayTimer_Tick(object sender, EventArgs e)
		{
			int X = 0;
			int Y = 0;
			int R = 0;
			int Theta = 0;

			float Darkness = ((float)_radius * (float)_intensity / 4f);
			if (Darkness < 1)
				Darkness = 1f;

			_lockBitmap.LockBits();
			
			for (int i = 0; i < (int)Math.Round(Darkness); i++)
			{
				R = _random.Next(_radius);
				Theta = _random.Next(360);
				X = (int)((float)_currentLatticePoint.X + R * (float)Math.Cos(_workshop.DegreeToRadian(Theta)));
				Y = (int)((float)_currentLatticePoint.Y + R * (float)Math.Sin(_workshop.DegreeToRadian(Theta)));
				PaintPoint(new Point(X, Y));
			}

			_lockBitmap.UnlockBits();
		}

		/// <summary>
		/// Event fires when the slider of the track bar is moved.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IntensityTracker_Scroll(object sender, EventArgs e)
		{
			_intensity = IntensityTracker.Value;
		}

		#endregion [ ToolStrip Event Delegates ]

		#region [ Event Delegates ]

		/// <summary>
		/// Occurs when one of the scaling variables within the Profile changes. 
		/// </summary>
		private void Profile_ScalingChanged(object sender, EventArgs e)
		{
			Profile.Cursor = this.Cursor;
		}

		#endregion [ Event Delegates ]
	}
}
