using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ClipperLib;
using ElfRes = ElfCore.Properties.Resources;
using System.Diagnostics;

namespace ElfCore.Tools
{
	using Polygon = List<IntPoint>;
	using Polygons = List<List<IntPoint>>;

	public abstract class MaskBase: BaseTool, ITool
	{
		#region [ Enums ]

		public enum MaskMode
		{
			NotSet = -1,
			Normal,
			Additive,
			Subtractive,
			Trim,
			XOR
		}

		#endregion [ Enums ]

		#region [ Protected Variables ]

		// Controls from ToolStrip
		protected ToolStripButton Mode_Normal = null;
		protected ToolStripButton Mode_Additive = null;
		protected ToolStripButton Mode_Subtractative = null;
		protected ToolStripButton Mode_Trim = null;
		protected ToolStripButton Mode_XOR = null;
		protected ToolStripButton ClearMask = null;
		protected ToolStripButton SelectAll = null;
		protected ToolStripButton InvertMask = null;

		protected ToolStripButton ExpandMask = null;
		protected ToolStripButton ReduceMask = null;
		protected ToolStripButton LoadMask = null;
		protected ToolStripButton SaveMask = null;

		protected ToolStripLabel ToolNameLabel = null;
		protected ToolStripSeparator MaskSepNib = null;
		protected ToolStripLabel _NibSize = null;
		protected ToolStripComboBox NibSize = null;
		protected ToolStripButton SquareNib = null;
		protected ToolStripButton RoundNib = null;

		protected ToolStripButton ShowMarquee = null;
		protected ToolStripButton ShowOverlay = null;

		protected ToolStripButton ShowSmooth = null;
		protected ToolStripButton ShowBlocky = null;

		protected Bitmap MaskTypeCursorModifier = null;

		// Used for rendering

		/// <summary>
		/// Bitmap of the mask for the Canvas
		/// </summary>
		protected Bitmap _canvasBuffer = null;	 

		/// <summary>
		/// Graphics object for the CanvasBuffer
		/// </summary>
		protected Graphics _canvasBufferGraphics = null;

		/// <summary>
		/// Indicates that this mask will not just clear the mask if the mouse down position of the cursor is the same
		/// as the mouse up position
		/// </summary>
		protected bool _freehandStyle = false;

		/// <summary>
		/// Used to temporarily disable the mask if we are doing normal mode and drawing a new area, so that the old mask
		/// does not remain on the screen.
		/// </summary>
		protected Mask _tempMask = null;

		// Mask file name
		protected string _maskFileName = "New Mask.mask";

		#endregion [ Protected Variables ]

		#region [ Static Fields ]

		public static MaskMode Mode = MaskMode.Normal;

		#endregion [ Static Fields ]

		#region [ Constants ]

		protected Color MASK_COLOR_ADDITIVE = Color.White;
		protected Color MASK_COLOR_NORMAL = Color.White;
		protected Color MASK_COLOR_SUBTRACTIVE = Color.Black;
		protected Color MASK_COLOR_CLEAR = Color.Black;

		// Settings Node Constants
		protected const string MASK = "MaskGeneral";
		protected const string MASK_MODE = "Mode";
		protected const string SHOW_MARQUEE = "ShowMarquee";
		protected const string SHOW_OVERLAY = "ShowOverlay";
		protected const string SHOW_BLOCKY = "Blocky";

		// Undo text constants
		protected const string REMOVE = "Mask Remove";
		protected const string INVERT = "Mask Invert";
		protected const string EXPAND = "Expand Mask";
		protected const string REDUCE = "Reduce Mask";
		protected const string LOAD = "Load Mask";
		protected const string SELECTALL = "Select All";

		#endregion [ Constants ]

		#region [ Properties ]

		/// <summary>
		/// True if this tool will do a marquee selection on the Canvas window. This flag is needed to display information in the Editor.
		/// </summary>
		public override bool DoesSelection
		{
			get { return true; }
		}

		/// <summary>
		/// Returns the name of the tool that is a Tool Group, in which this tool should be a child tool of
		/// </summary>
		public override string ToolGroupName
		{
			get { return "Mask_ToolGroup"; }
		}

		/// <summary>
		/// Shortcut to determine the current masking mode
		/// </summary>
		protected MaskMode CurrentMode
		{
			get { return MaskBase.Mode; }
			set { MaskBase.Mode = value; }
		}

		/// <summary>
		/// Create a cursor for this mask, based on the type of mask it is, and the MakeMode currently set.
		/// </summary>
		public override Cursor Cursor
		{
			get
			{
				Bitmap MaskCursor = new Bitmap(ElfRes.cross_base);
				using (Graphics g = Graphics.FromImage(MaskCursor))
				{
					if (MaskTypeCursorModifier != null)
						g.DrawImage(MaskTypeCursorModifier, MaskCursor.Width - MaskTypeCursorModifier.Width, 0, MaskTypeCursorModifier.Width, MaskTypeCursorModifier.Height);

					int Half = MaskCursor.Width / 2;
					Bitmap Mode = GetCursorModeBitmap();
					if (Mode != null)
						g.DrawImage(Mode, Half + (Half - Mode.Width) / 2, Half + (Half - Mode.Height) / 2, Mode.Width, Mode.Height);

					base.Cursor = CustomCursors.CreateCursor(MaskCursor, 15, 15);
					MaskCursor.Dispose();
					MaskCursor = null;

					return base.Cursor;
				}
			}
			set { base.Cursor = value; }
		}

		#endregion [ Properties ]

		#region [ Abstract Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		protected abstract GraphicsPath CreateRenderPath(Point p1, Point p2, Scaling scaling);

		#endregion [ Abstract Methods ]

		#region [ Methods ]

		/// <summary>
		/// Converts the curvy input path into one that resembles the cells on the Canvas
		/// </summary>
		protected void Blockify(ref GraphicsPath path)
		{
			Region Region = new Region(path);
			RectangleF Bounds = path.GetBounds();

			PointF HitPoint = PointF.Empty;
			PointF UpperLeft = PointF.Empty;
			PointF LowerRight = PointF.Empty;
			RectangleF Block = RectangleF.Empty;
			GraphicsPath NewBlock = new GraphicsPath();
			int Half = UISettings.ʃCellScale / 2;

			path.Reset();

			UpperLeft = Workshop.PixelPointF(Workshop.CellPointF(new PointF(Bounds.Left, Bounds.Top)));
			LowerRight = Workshop.PixelPointF(Workshop.CellPointF(new PointF(Bounds.Right, Bounds.Bottom)));
			
			for (float X = UpperLeft.X; X <= LowerRight.X; X += UISettings.ʃCellScaleF)
				for (float Y = UpperLeft.Y; Y <= LowerRight.Y; Y += UISettings.ʃCellScaleF)
				{
					// Check the center point of this test cell. 
					// If it is within the region, then add a rectangle representing the cell to the new graphics path.
					HitPoint = new PointF(X + Half, Y + Half);
					if (Region.IsVisible(HitPoint))
					{
						Block = new RectangleF(X, Y, UISettings.ʃCellScale, UISettings.ʃCellScale);
						//Debug.WriteLine(Block.ToString());

						// Create a rectangle that matches the Displayed cell + gridline
						if (path.PointCount == 0)
						{
							path.AddRectangle(Block);
						}
						else
						{
							//NewBlock.Reset();
							NewBlock.AddRectangle(Block);
							//JoinPaths(path, NewBlock);
						}
					}
				}
			
			JoinPaths(path, NewBlock);

			Region.Dispose();
			Region = null;

			NewBlock.Dispose();
			NewBlock = null;
		}

		/// <summary>
		/// Clears out the mask
		/// </summary>
		/// <param name="createUndo">If true, creates an Undo for the mask removal</param>
		protected void Clear(bool createUndo)
		{ 
			_workshop.ClearMask();
			if (createUndo)
				_workshop.UndoController.SaveUndo(REMOVE);
		}

		/// <summary>
		/// Blanks out the contents of the Mask buffers
		/// </summary>
		protected void ClearBuffers()
		{
			_canvasBufferGraphics.Clear(MASK_COLOR_CLEAR);
			_latticeBufferGraphics.Clear(MASK_COLOR_CLEAR);
		}

		/// <summary>
		/// Create the mask pane based on the mask stored in Workshop.
		/// Some of the UI settings might have changed, or an Undo event fired, or we switched over from one of the other mask tools.
		/// </summary>
		protected void CreateMaskBuffers()
		{
			// Grab a Graphics object from the CanvasPane PictureBox
			_canvasControlGraphics = Workshop.Canvas.CreateGraphics();

			_canvasBuffer = new Bitmap(Workshop.Canvas.Width, Workshop.Canvas.Height);
			_canvasBufferGraphics = Graphics.FromImage(_canvasBuffer);

			_latticeBuffer = new Bitmap(UISettings.ʃLatticeSize.Width, UISettings.ʃLatticeSize.Height);
			_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);

			ClearBuffers();

			// Fill the buffers with the contents of the existing mask (if any)
			if (_workshop.Mask.HasMask)
				using (Brush MaskBrush = new SolidBrush(MASK_COLOR_NORMAL))
				{
					_canvasBufferGraphics.FillPath(MaskBrush, _workshop.Mask.CanvasMask.Outline);
					_latticeBufferGraphics.FillPath(MaskBrush, _workshop.Mask.LatticeMask.Outline);
				}
		}

		///// <summary>
		///// Create the Undo for this masking event.
		///// </summary>
		//protected virtual void CreateUndo_Mask(string action)
		//{
		//    // Create Undo data to be able to reverse out our changes
		//    UndoData.v1 Undo = new UndoData.v1();
		//    Undo.Action = action;
		//    Undo.GeneralEvent = GeneralDataEvent.Mask;
		//    Undo.SpecificEvent = SpecificEventType.Mask_Defined;
		//    Undo.Mask.Set(_workshop.Mask, false);
		//    //_workshop.UndoController.Push(Undo);
		//    Undo = null;
		//}

		///// <summary>
		///// Create the Undo for this masking event.
		///// </summary>
		//public static void CreateUndo_MaskClear(Workshop workshop)
		//{
		//    // Create Undo data to be able to reverse out our changes
		//    UndoData.v1 Undo = new UndoData.v1();
		//    Undo.Action = MaskBase.REMOVE;
		//    Undo.GeneralEvent = GeneralDataEvent.Mask;
		//    Undo.SpecificEvent = SpecificEventType.Mask_Defined;
		//    Undo.Mask.Set(workshop.Mask, false);
		//    workshop.UndoController.Push(Undo);
		//    Undo = null;
		//}

		/// <summary>
		/// Attaches or detaches the click events to the buttons
		/// </summary>
		/// <param name="attach">Indicates that the events should be attached. If false, then detaches the events</param>
		protected virtual void EventAttachment(bool attach)
		{
			if (attach)
			{
				Mode_Normal.Click += new System.EventHandler(this.Mode_Normal_Click);
				Mode_Additive.Click += new System.EventHandler(this.Mode_Additive_Click);
				Mode_Subtractative.Click += new System.EventHandler(this.Mode_Subtractative_Click);
				Mode_Trim.Click += new System.EventHandler(this.Mode_Trim_Click);
				Mode_XOR.Click += new System.EventHandler(this.Mode_XOR_Click);

				ClearMask.Click += new System.EventHandler(this.ClearMask_Click);
				SelectAll.Click += new System.EventHandler(this.SelectAll_Click);
				InvertMask.Click += new System.EventHandler(this.InvertMask_Click);

				ExpandMask.Click += new System.EventHandler(this.ExpandMask_Click);
				ReduceMask.Click += new System.EventHandler(this.ReduceMask_Click);

				LoadMask.Click += new System.EventHandler(this.LoadMask_Click);
				SaveMask.Click += new System.EventHandler(this.SaveMask_Click);

				ShowMarquee.Click += new System.EventHandler(this.ShowMarquee_Click);
				ShowOverlay.Click += new System.EventHandler(this.ShowOverlay_Click);

				ShowSmooth.Click += new System.EventHandler(this.ShowSmooth_Click);
				ShowBlocky.Click += new System.EventHandler(this.ShowBlocky_Click);

				_workshop.Mask.Defined += new EventHandler(Mask_Defined);
				_workshop.Mask.Cleared += new EventHandler(Mask_Cleared);

			}
			else
			{
				// http://stackoverflow.com/questions/1307607/removing-event-handlers
				// Note: Mode_Normal.Click -= new System.EventHandler(this.Mode_Normal_Click); is functionally the same as
				//		 Mode_Normal.Click -= this.Mode_Normal_Click;

				Mode_Normal.Click -= new System.EventHandler(this.Mode_Normal_Click);
				Mode_Additive.Click -= new System.EventHandler(this.Mode_Additive_Click);
				Mode_Subtractative.Click -= new System.EventHandler(this.Mode_Subtractative_Click);
				Mode_Trim.Click -= new System.EventHandler(this.Mode_Trim_Click);
				Mode_XOR.Click -= new System.EventHandler(this.Mode_XOR_Click);

				ClearMask.Click -= new System.EventHandler(this.ClearMask_Click);
				SelectAll.Click -= new System.EventHandler(this.SelectAll_Click);
				InvertMask.Click -= new System.EventHandler(this.InvertMask_Click);

				ExpandMask.Click -= new System.EventHandler(this.ExpandMask_Click);
				ReduceMask.Click -= new System.EventHandler(this.ReduceMask_Click);

				LoadMask.Click -= new System.EventHandler(this.LoadMask_Click);
				SaveMask.Click -= new System.EventHandler(this.SaveMask_Click);

				ShowMarquee.Click -= new System.EventHandler(this.ShowMarquee_Click);
				ShowOverlay.Click -= new System.EventHandler(this.ShowOverlay_Click);

				ShowSmooth.Click -= new System.EventHandler(this.ShowSmooth_Click);
				ShowBlocky.Click -= new System.EventHandler(this.ShowBlocky_Click);

				_workshop.Mask.Defined -= new EventHandler(Mask_Defined);
				_workshop.Mask.Cleared -= new EventHandler(Mask_Cleared);
			}
		}

		/// <summary>
		/// Converts the MaskMode enumeration to the ClipType enumeration
		/// </summary>
		/// <returns>ClipType</returns>
		private ClipType GetClipType()
		{
			switch (CurrentMode)
			{ 
				case MaskMode.Subtractive:
					return ClipType.ctDifference;

				case MaskMode.XOR:
					return ClipType.ctXor;

				case MaskMode.Trim:
					return ClipType.ctIntersection;

				case MaskMode.Additive:
				case MaskMode.Normal:
				default:
					return ClipType.ctUnion;
			}
		}

		/// <summary>
		/// Determines the bitmap to use to indicate the current draw mode of the mask. This is appended onto the cursor image.
		/// </summary>
		protected Bitmap GetCursorModeBitmap()
		{
			switch (CurrentMode)
			{
				case MaskMode.Additive:
					return ElfRes.plus_modifier;

				case MaskMode.Subtractive:
					return ElfRes.minus_modifier;
					
				case MaskMode.Trim:
					return ElfRes.mask_trim_modifier;
					
				case MaskMode.XOR:
					return ElfRes.mask_xor_modifier;
					
				case MaskMode.Normal:
				default:
					return null;
			}
		}

		/// <summary>
		/// Changes the area of the mask as indicated by value
		/// </summary>
		/// <param name="value">Amount to inflate (or deflate if negative) the mask</param>
		protected void InflateMask(int value)
		{
			if (!_workshop.Mask.HasMask)
				return;

			_workshop.Mask.Define(ModifyMask(null, Scaling.Pixel, value * UISettings.ʃCellScale), 
							   ModifyMask(null, Scaling.Cell, value));

			Workshop.Canvas.Refresh();
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
			string Path = AppendPath(Constants.TOOLSETTINGS, MASK);

			CurrentMode = EnumHelper.GetEnumFromValue<MaskMode>(LoadValue(AppendPath(Path, MASK_MODE), (int)MaskMode.Normal, false));
			_workshop.UI.ShowMaskMarquee = LoadValue(AppendPath(Path, SHOW_MARQUEE), true, false);
			_workshop.UI.ShowMaskOverlay = LoadValue(AppendPath(Path, SHOW_OVERLAY), true, false);
			_workshop.UI.ShowMaskBlocky = LoadValue(AppendPath(Path, SHOW_BLOCKY), false, false);

			// Get a pointer to the controls on the toolstrip that belongs to us.
			ToolNameLabel = (ToolStripLabel)GetItem<ToolStripLabel>(1);
			Mode_Normal = (ToolStripButton)GetItem<ToolStripButton>("NormalMask");
			Mode_Additive = (ToolStripButton)GetItem<ToolStripButton>("AdditiveMask");
			Mode_Subtractative = (ToolStripButton)GetItem<ToolStripButton>("SubtractiveMask");
			Mode_Trim = (ToolStripButton)GetItem<ToolStripButton>("TrimMask");
			Mode_XOR = (ToolStripButton)GetItem<ToolStripButton>("XORMask");
			ClearMask = (ToolStripButton)GetItem<ToolStripButton>("RemoveMask");
			SelectAll = (ToolStripButton)GetItem<ToolStripButton>("Mask_SelectAll");
			InvertMask = (ToolStripButton)GetItem<ToolStripButton>("MaskInvert");
			ExpandMask = (ToolStripButton)GetItem<ToolStripButton>("ExpandMask");
			ReduceMask = (ToolStripButton)GetItem<ToolStripButton>("ReduceMask");
			LoadMask = (ToolStripButton)GetItem<ToolStripButton>("LoadMask");
			SaveMask = (ToolStripButton)GetItem<ToolStripButton>("SaveMask");

			MaskSepNib = (ToolStripSeparator)GetItem<ToolStripSeparator>("MaskSepNib");
			_NibSize = (ToolStripLabel)GetItem<ToolStripLabel>("_Mask_NibSize");
			NibSize = (ToolStripComboBox)GetItem<ToolStripComboBox>("Mask_NibSize");
			SquareNib = (ToolStripButton)GetItem<ToolStripButton>("Mask_SquareNib");
			RoundNib = (ToolStripButton)GetItem<ToolStripButton>("Mask_RoundNib");

			ShowMarquee = (ToolStripButton)GetItem<ToolStripButton>("Mask_ShowMarquee");
			ShowOverlay = (ToolStripButton)GetItem<ToolStripButton>("Mask_ShowOverlay");

			ShowSmooth = (ToolStripButton)GetItem<ToolStripButton>("Mask_Smooth");
			ShowBlocky = (ToolStripButton)GetItem<ToolStripButton>("Mask_Blocky");

			// Attach all events that would normally go within the form to methods in this class
			// Attaching on Selected, remove attachment on UnSelect
			
			// Set the initial value for the contol from what we had retrieve from Settings
			SetModeButton();
		}

		/// <summary>
		/// Inverts the masked selection
		/// </summary>
		protected virtual void InvertTheMask()
		{
			//CreateUndo_Mask(INVERT);
			
			// Create a path that matches the canvas size
			// and use the Clipper library to invert the current mask with it
			GraphicsPath InvertRect = new GraphicsPath();
			InvertRect.AddRectangle(new Rectangle(0, 0, UISettings.ʃCanvasSize.Width, UISettings.ʃCanvasSize.Height));
			GraphicsPath CanvasMask_Outline = (GraphicsPath)_workshop.Mask.CanvasMask.Outline.Clone();
			JoinPaths(CanvasMask_Outline, InvertRect, ClipType.ctXor);

			InvertRect.Reset();
			InvertRect.AddRectangle(new Rectangle(0, 0, UISettings.ʃLatticeSize.Width, _workshop.UI.LatticeSize.Height));
			GraphicsPath LatticeMask_Outline = (GraphicsPath)_workshop.Mask.LatticeMask.Outline.Clone();
			JoinPaths(LatticeMask_Outline, InvertRect, ClipType.ctXor);

			_workshop.Mask.Define(CanvasMask_Outline, LatticeMask_Outline);

			#if DEBUG
				CreateMaskBuffers();
				using (SolidBrush MaskBrush = new SolidBrush(MaskColor()))
				{
					_latticeBufferGraphics.Clear(MASK_COLOR_CLEAR);
					_latticeBufferGraphics.FillPath(MaskBrush, _workshop.Mask.LatticeMask.Outline);
					Editor.ExposePane(_latticeBuffer, Panes.MaskLattice);

					_canvasBufferGraphics.Clear(MASK_COLOR_CLEAR);
					_canvasBufferGraphics.FillPath(MaskBrush, _workshop.Mask.CanvasMask.Outline);
					Editor.ExposePane(_canvasBuffer, Panes.MaskCanvas);
				}
				PostDrawCleanUp();
			#endif

		}

		/// <summary>
		/// Join 2 Graphics path into 1
		/// </summary>
		/// <param name="mainPath">The initial path</param>
		/// <param name="newPath">The path to join to the mainPath</param>
		protected void JoinPaths(GraphicsPath mainPath, GraphicsPath newPath)
		{
			JoinPaths(mainPath, newPath, ClipType.ctUnion);
		}

		/// <summary>
		/// Join 2 Graphics path into 1
		/// </summary>
		/// <param name="mainPath">The initial path</param>
		/// <param name="newPath">The path to join to the mainPath</param>
		/// <param name="clipping">Enum to indicate the type of joining to do</param>
		protected void JoinPaths(GraphicsPath mainPath, GraphicsPath newPath, ClipType clipping)
		{
			Polygons Subject = new Polygons();
			Polygons Clips = new Polygons();
			Polygons Solution = new Polygons();
			Clipper c = new Clipper();
			int Scale = 1;

			mainPath.Flatten();
			Clipper.PathToPolygon(mainPath, Subject, Scale);
			mainPath.Reset();

			newPath.Flatten();
			Clipper.PathToPolygon(newPath, Clips, Scale);

			mainPath.FillMode = FillMode.Winding;

			c.AddPolygons(Subject, PolyType.ptSubject);
			c.AddPolygons(Clips, PolyType.ptClip);
			Solution.Clear();
			bool succeeded = c.Execute(clipping, Solution);
			if (!succeeded)
				return;

			foreach (Polygon pg in new Polygons(Solution))
			{
				PointF[] pts = Clipper.PolygonToPointFArray(pg, Scale);
				if (pts.Length > 2)
					mainPath.AddPolygon(pts);
				pts = null;
			}

			Subject = null;
			Clips = null;
			Solution = null;
			c = null;
		}

		/// <summary>
		/// Gets the color to draw in the Mask Pane, depending on the mode we are in.
		/// </summary>
		/// <returns></returns>
		protected Color MaskColor()
		{
			if (CurrentMode == MaskMode.Subtractive)
				return MASK_COLOR_SUBTRACTIVE;
			else
				return MASK_COLOR_NORMAL;
		}

		///// <summary>
		///// Called by the outside to inform this tool that the mask has been cleared and we are to set the tool settings toolbar
		///// buttons accordingly.
		///// </summary>
		//public void MaskIsCleared()
		//{
		//    SetFunctionButtons();
		//}

		/// <summary>
		/// Generate a new Mask outline based any existing mask and the rendered mask
		/// </summary>
		/// <param name="renderedPath">Mask the was just drawn by the user</param>
		/// <param name="scaleMode">Indicates which Mask to use, Lattice for Scaling.Cell, Canvas for Scaling.Pixel</param>
		/// <returns>Modified mask.</returns>
		protected GraphicsPath ModifyMask(GraphicsPath renderedPath, Scaling scaleMode, float polygonOffset)
		{
			Polygons Subject = null;
			Polygons Clips = null;
			Polygons solution = null;
			Polygons solution2 = null;
			GraphicsPath Path = null;
			Clipper c = null;
			int Scale = 1;

			// Load in the mask (if any).
			if (scaleMode == Scaling.Cell)
				Path = _workshop.Mask.LatticeMask.Outline;
			else
				Path = _workshop.Mask.CanvasMask.Outline;

			// If there is an existing mask, then modify it.
			if (Path.PointCount > 0)
			{
				Subject = new Polygons();
				Clips = new Polygons();
				solution = new Polygons();
				solution2 = new Polygons();
				c = new Clipper();

				Clipper.PathToPolygon(Path, Subject, 1);

				// take the rendered path and convert it into the Clipper Polygon object
				if (renderedPath != null)
				{
					renderedPath.Flatten();
					Clipper.PathToPolygon(renderedPath, Clips, Scale);
				}

				Path = new GraphicsPath();
				Path.FillMode = FillMode.Winding;

				c.AddPolygons(Subject, PolyType.ptSubject);
				c.AddPolygons(Clips, PolyType.ptClip);
				solution.Clear();
				bool succeeded = c.Execute(GetClipType(), solution);
				if (!succeeded)
					return Path;

				if (polygonOffset != 0)
					solution2 = Clipper.OffsetPolygons(solution, (double)polygonOffset * Scale, JoinType.jtMiter);
				else
					solution2 = new Polygons(solution);

				foreach (Polygon pg in solution2)
				{
					PointF[] pts = Clipper.PolygonToPointFArray(pg, Scale);
					if (pts.Length > 2)
						Path.AddPolygon(pts);
					pts = null;
				}

				Subject = null;
				Clips = null;
				solution = null;
				solution2 = null;
				c = null;

				return Path;
			}
			else
				// Otherwise just return the rendered path
				return renderedPath;
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_mouseDown = true;
			_mouseDownCell = mouseCell;
			_mouseDownCellPixel = Workshop.PixelPoint(mouseCell);

			_currentMouseCell = mouseCell;
			_currentMouseCellPixel = Workshop.PixelPoint(mouseCell);

			// Trap the mouse into the Canvas while we are working
			//TrapMouse();

			// Create mask pane objects
			CreateMaskBuffers();

			// If we are in normal mode, we need to stop the CanvasPane from rendering the marquee until we can properly clear out the mask
			// on MouseUp. Move the contents of the Workshop.Mask object into a temporary object and clear out the main one.
			if (_workshop.Mask.HasMask && (CurrentMode == MaskMode.Normal))
			{
			    _tempMask = new Mask();
			    _tempMask.Define(_workshop.Mask, false);
			    _workshop.Mask.Clear();
			    ClearBuffers();
				Workshop.Canvas.Refresh();

			    Editor.ExposePane(_latticeBuffer, Panes.MaskLattice);
			    Editor.ExposePane(_canvasBuffer, Panes.MaskCanvas);
			}

			// Grab the current screen so we can display the mask marquee correctly.
			CaptureCanvas();		
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <returns>Return true if the MouseDown flag was set.</returns>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!base.MouseMove(buttons, mouseCell, mousePixel))
				return false;

			// Draw the background (all Channels)
			DisplayCapturedCanvas();

			// Render the preview of the new mask onto the CanvasPane
			Render(_constrainedCellPixel, _mouseDownCellPixel, Scaling.Pixel, false);

			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <returns>Return true if the MouseDown flag was set.</returns>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!base.MouseUp(buttons, mouseCell, mousePixel))
				return false;

			if (_tempMask != null)
			{
				_workshop.Mask.Define(_tempMask, true);
				_tempMask = null;
			}

			bool HasMask = _workshop.Mask.HasMask;

			// If we don't already have a mask, and we are doing subtractive masking, then no new mask will be drawn, exit
			if (!HasMask)
			{
				if ((CurrentMode == MaskMode.Subtractive))
				{
					PostDrawCleanUp();
					Workshop.Canvas.Refresh();
					return true;
				}
			}

			if (!_freehandStyle && mouseCell.Equals(_mouseDownCell))
			{
				// Click event, clear the mask if we are outside of the masked area, create Undo event
				using (Region Region = new System.Drawing.Region(_workshop.Mask.CanvasMask.Outline))
					if (!Region.IsVisible(_mouseDownCell))
					{
						Clear(true);
					}

				PostDrawCleanUp();
				return true;
			}

			//CreateUndo_Mask(this.UndoText);

			if (HasMask && (CurrentMode == MaskMode.Normal))
			{
				// Remove the existing mask, but don't create an undo event for the removal
				Clear(false);
			}

			GraphicsPath RenderedPath = null;
			Point ConstrainedPoint;

			// First, Calculate the changes to the Lattice Mask
			ConstrainedPoint = _workshop.ConstrainLine(mouseCell, _mouseDownCell);
			RenderedPath = CreateRenderPath(_mouseDownCell, ConstrainedPoint, Scaling.Cell);
			GraphicsPath LatticeMask_Outline = ModifyMask(RenderedPath, Scaling.Cell, 0);

			// Next, Calculate the changes to the Canvas Mask
			ConstrainedPoint = _workshop.ConstrainLine(_currentMouseCellPixel, _mouseDownCellPixel);
			RenderedPath = CreateRenderPath(_mouseDownCellPixel, ConstrainedPoint, Scaling.Pixel);
			GraphicsPath CanvasMask_Outline = ModifyMask(RenderedPath, Scaling.Pixel, 0);

			// Make the Canvas Mask blocky
			if (_workshop.UI.ShowMaskBlocky)
				Blockify(ref CanvasMask_Outline);

			_workshop.Mask.Define(CanvasMask_Outline, LatticeMask_Outline);
			LatticeMask_Outline = null;
			CanvasMask_Outline = null;

#if DEBUG
			using (SolidBrush MaskBrush = new SolidBrush(MaskColor()))
			{
				_latticeBufferGraphics.Clear(MASK_COLOR_CLEAR);
				_latticeBufferGraphics.FillPath(MaskBrush, _workshop.Mask.LatticeMask.Outline);
				Editor.ExposePane(_latticeBuffer, Panes.MaskLattice);

				_canvasBufferGraphics.Clear(MASK_COLOR_CLEAR);
				_canvasBufferGraphics.FillPath(MaskBrush, _workshop.Mask.CanvasMask.Outline);
				Editor.ExposePane(_canvasBuffer, Panes.MaskCanvas);
			}
#endif

			Workshop.Canvas.Refresh();
			PostDrawCleanUp();
			RenderedPath = null;

			return true;
		}

		/// <summary>
		/// Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
		/// </summary>
		protected override void PostDrawCleanUp()
		{
			base.PostDrawCleanUp();

			if (_canvasBufferGraphics != null)
			{
				_canvasBufferGraphics.Dispose();
				_canvasBufferGraphics = null;
			}
			if (_canvasBuffer != null)
			{
				_canvasBuffer.Dispose();
				_canvasBuffer = null;
			}

			SetFunctionButtons();
		}

		/// <summary>
		/// Draw the shape of the new mask, points p1 and p2 are expressed in Pixels
		/// </summary>
		/// <param name="p1">Upper Left point in pixels</param>
		/// <param name="p2">Lower Right point in pixels</param>
		/// <param name="scaling">Indicates if we are rendering to the Canvas or Lattice</param>
		protected virtual void Render(Point p1, Point p2, Scaling scaling, bool finalRender)
		{
			if (p1.Equals(p2) && !p1.IsEmpty && !p2.IsEmpty)
			    return;

			using (GraphicsPath DrawPath = CreateRenderPath(p1, p2, scaling))
			{
				if (finalRender)
				{
					using (Brush MaskBrush = new SolidBrush(MaskColor()))
					{
						(scaling == Scaling.Pixel ? _canvasBufferGraphics : _latticeBufferGraphics).FillPath(MaskBrush, DrawPath);
					}
				}
				else
				{
					using (Pen DrawPen = _workshop.GetMarqueePen())
					{
						try
						{
							_canvasControlGraphics.DrawPath(DrawPen, DrawPath);
						}
						catch (OutOfMemoryException)
						{ }
					}
				}
			}
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		/// <param name="settings"></param>
		public override void SaveSettings()
		{
			string Path = AppendPath(Constants.TOOLSETTINGS, MASK);
			SaveValue(AppendPath(Path, MASK_MODE), (int)CurrentMode, false);
			SaveValue(AppendPath(Path, SHOW_MARQUEE), _workshop.UI.ShowMaskMarquee, false);
			SaveValue(AppendPath(Path, SHOW_OVERLAY), _workshop.UI.ShowMaskOverlay, false);
			SaveValue(AppendPath(Path, SHOW_BLOCKY), _workshop.UI.ShowMaskBlocky, false);
		}

		/// <summary>
		/// Method that fires when this Tool is selected in the ToolBox.
		/// Hide the Mask_Brush custom tools
		/// </summary>
		public override void Selected()
		{
			
			// Check the mode on select, in case one of the other mask tools changed it.
			if (this.Mode_Subtractative.Checked)
				CurrentMode = MaskMode.Subtractive;
			else if (Mode_Additive.Checked)
				CurrentMode = MaskMode.Additive;
			else if (Mode_XOR.Checked)
				CurrentMode = MaskMode.XOR;
			else if (Mode_Trim.Checked)
				CurrentMode = MaskMode.Trim;
			else
				CurrentMode = MaskMode.Normal;

			EventAttachment(true);

			ShowMarquee.Checked = _workshop.UI.ShowMaskMarquee;
			ShowOverlay.Checked = _workshop.UI.ShowMaskOverlay;
			
			ShowSmooth.Checked = !_workshop.UI.ShowMaskBlocky;
			ShowBlocky.Checked = _workshop.UI.ShowMaskBlocky;

			MaskSepNib.Visible = false;
			_NibSize.Visible = false;
			NibSize.Visible = false;
			SquareNib.Visible = false;
			RoundNib.Visible = false;

			ToolNameLabel.Image = this.ToolBoxImage;
			ToolNameLabel.Text = this.Name;

			SetFunctionButtons();

			// Call base.Selected at the end, so that the cursor can be built correctly.
			base.Selected();
		}

		/// <summary>
		/// Sets which of the mode buttons are checked based on the current masking mode
		/// </summary>
		/// <param name="mode">Mask mode to set</param>
		protected void SetModeButton(MaskMode mode)
		{
			if (CurrentMode != mode)
				CurrentMode = mode;
			Mode_Normal.Checked = (mode == MaskMode.Normal);
			Mode_Additive.Checked = (mode == MaskMode.Additive);
			Mode_Subtractative.Checked = (mode == MaskMode.Subtractive);
			Mode_Trim.Checked = (mode == MaskMode.Trim);
			Mode_XOR.Checked = (mode == MaskMode.XOR);

			Workshop.Canvas.Cursor = this.Cursor;
		}

		/// <summary>
		/// Sets which of the mode buttons are checked based on the current masking mode
		/// </summary>
		protected void SetModeButton()
		{
			SetModeButton(this.CurrentMode);
		}

		/// <summary>
		/// Sets the enabled property of the various function buttons if the mask is present.
		/// </summary>
		protected void SetFunctionButtons()
		{
			bool hasMask = _workshop.Mask.HasMask;

			ClearMask.Enabled = hasMask;
			InvertMask.Enabled = hasMask;
			SaveMask.Enabled = hasMask;
			ExpandMask.Enabled = hasMask;
			ReduceMask.Enabled = hasMask;
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			Mode_Normal = null;
			Mode_Additive = null;
			Mode_Subtractative = null;
			ClearMask = null;
			InvertMask = null;
			SquareNib = null;
			RoundNib = null;
			NibSize = null;
			ToolNameLabel = null;
			MaskSepNib = null;
			_NibSize = null;
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// Remove all the click events from these shared controls
		/// </summary>
		public override void Unselected()
		{
			base.Unselected();
			EventAttachment(false);
		}

		#endregion [ Methods ]

		#region [ ToolStrip Events ]

		protected virtual void Mode_Normal_Click(object sender, EventArgs e)
		{
			SetModeButton(MaskMode.Normal);
		}

		protected virtual void Mode_Additive_Click(object sender, EventArgs e)
		{
			SetModeButton(MaskMode.Normal);
			CurrentMode = MaskMode.Additive;
			SetModeButton();
			Workshop.Canvas.Cursor = this.Cursor;
		}

		protected virtual void Mode_Subtractative_Click(object sender, EventArgs e)
		{
			SetModeButton(MaskMode.Subtractive);
		}

		protected virtual void Mode_Trim_Click(object sender, EventArgs e)
		{
			SetModeButton(MaskMode.Trim);
		}

		protected virtual void Mode_XOR_Click(object sender, EventArgs e)
		{
			SetModeButton(MaskMode.XOR);
		}

		protected virtual void ShowMarquee_Click(object sender, EventArgs e)
		{
			if (!ShowMarquee.Checked && !ShowOverlay.Checked)
			{
				MessageBox.Show("Either the Mask Marquee or the Mask Overlay must be check, else you would never see the masked area.", "Hide Mask Marquee", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				ShowMarquee.Checked = true;
				return;
			}

			_workshop.UI.ShowMaskMarquee = ShowMarquee.Checked;
			Workshop.Canvas.Refresh();
		}

		protected virtual void ShowOverlay_Click(object sender, EventArgs e)
		{
			if (!ShowMarquee.Checked && !ShowOverlay.Checked)
			{
				MessageBox.Show("Either the Mask Marquee or the Mask Overlay must be check, else you would never see the masked area.", "Hide Mask Overlay", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				ShowOverlay.Checked = true;
				return;
			}

			_workshop.UI.ShowMaskOverlay = ShowOverlay.Checked;
			Workshop.Canvas.Refresh();
		}

		protected virtual void ShowSmooth_Click(object sender, EventArgs e)
		{
			_workshop.UI.ShowMaskBlocky = false;
			ShowSmooth.Checked = !_workshop.UI.ShowMaskBlocky;
			ShowBlocky.Checked = _workshop.UI.ShowMaskBlocky;
		}

		protected virtual void ShowBlocky_Click(object sender, EventArgs e)
		{
			_workshop.UI.ShowMaskBlocky = true;
			ShowSmooth.Checked = !_workshop.UI.ShowMaskBlocky;
			ShowBlocky.Checked = _workshop.UI.ShowMaskBlocky;
		}

		/// <summary>
		/// Remove the mask
		/// </summary>
		protected virtual void ClearMask_Click(object sender, EventArgs e)
		{
			Clear(true);
			Workshop.Canvas.Refresh();
		}

		/// <summary>
		/// Make the mask fit the entire area by first clearing it, then inverting the selection (ie none selected becomes all selected)
		/// </summary>
		protected virtual void SelectAll_Click(object sender, EventArgs e)
		{
			///CreateUndo_Mask(SELECTALL);
			Clear(false);
			InvertTheMask();
			_workshop.UndoController.SaveUndo(SELECTALL);
			Workshop.Canvas.Refresh();
		}

		/// <summary>
		/// Invert the masked area 
		/// </summary>
		protected virtual void InvertMask_Click(object sender, EventArgs e)
		{
			InvertTheMask();
			_workshop.UndoController.SaveUndo(INVERT);
			Workshop.Canvas.Refresh();
		}

		/// <summary>
		/// Expand the mask by 1 cell in every direction
		/// </summary>
		protected virtual void ExpandMask_Click(object sender, EventArgs e)
		{
			//CreateUndo_Mask(EXPAND);
			InflateMask(1);
			_workshop.UndoController.SaveUndo(EXPAND);
		}

		/// <summary>
		/// Reduce the mask by 1 cell in every direction
		/// </summary>
		protected virtual void ReduceMask_Click(object sender, EventArgs e)
		{
			//CreateUndo_Mask(REDUCE);
			InflateMask(-1);
			_workshop.UndoController.SaveUndo(REDUCE);
		}

		/// <summary>
		/// Save the current Mask to disk
		/// </summary>
		protected virtual void SaveMask_Click(object sender, EventArgs e)
		{
			SaveFileDialog SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			SaveFileDialog.DefaultExt = "mask";
			SaveFileDialog.Filter = "Mask Files (*.mask)|*.mask|All Files (*.*)|*.*";
			SaveFileDialog.Title = "Save Mask";
			SaveFileDialog.FileName = _maskFileName;

			if (SaveFileDialog.ShowDialog() == DialogResult.Cancel)
				return;

			_maskFileName = SaveFileDialog.FileName;

			Stream FileStream = File.Create(_maskFileName);
			BinaryFormatter Serializer = new BinaryFormatter();

			Mask Mask = new Mask(_workshop.Mask);
			Mask.Serialize();

			Serializer.Serialize(FileStream, Mask);
			FileStream.Close();
			FileStream.Dispose();
			FileStream = null;
			Serializer = null;
		}

		/// <summary>
		/// Load a previously saved Mask from disk
		/// </summary>
		protected virtual void LoadMask_Click(object sender, EventArgs e)
		{
			OpenFileDialog OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			OpenFileDialog.DefaultExt = "mask";
			OpenFileDialog.Filter = "Mask Files (*.mask)|*.mask|All Files (*.*)|*.*";
			OpenFileDialog.Title = "Save Mask";
			OpenFileDialog.FileName = _maskFileName;

			if (OpenFileDialog.ShowDialog() == DialogResult.Cancel)
				return;

			//CreateUndo_Mask(LOAD);

			_maskFileName = OpenFileDialog.FileName;

			Stream FileStream = File.OpenRead(_maskFileName);
			BinaryFormatter Deserializer = new BinaryFormatter();

			Mask Mask = (Mask)Deserializer.Deserialize(FileStream);
			Mask.Deserialize();
			_workshop.SetMask(Mask);
			SetFunctionButtons();

			FileStream.Dispose();
			FileStream = null;
			Deserializer = null;

			_workshop.UndoController.SaveUndo(LOAD);
		}
		
		#endregion [ ToolStrip Events ]

		#region [ Mask Events ]

		/// <summary>
		/// Occurs when the Mask is Defined
		/// </summary>
		public void Mask_Defined(object sender, EventArgs e)
		{
			SetFunctionButtons();
		}

		/// <summary>
		/// Occurs when the Mask is Cleared
		/// </summary>
		public void Mask_Cleared(object sender, EventArgs e)
		{
			SetFunctionButtons();
		}

		#endregion [ Mask Events ]
	}
}

