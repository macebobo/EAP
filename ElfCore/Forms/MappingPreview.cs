using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Forms
{
	public partial class MappingPreview : Form
	{
		#region [ Constants ]

		private const int TARGET = 0;
		private const int IMPORTED = 1;
		private const int ALREADY_IMPORTED = 2;

		#endregion [ Constants ]

		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;
		private BaseProfile _tempProfile;
		private Rectangle _originalBounds = Rectangle.Empty;
		private MappingList _mapList = null;
		public Point MaxSetBack = Point.Empty;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Set the CanvasPane to equal the maximum size for the imported plus current channel.
		/// </summary>
		public Size LatticeSize
		{
			set 
			{ 
				_tempProfile.Scaling.LatticeSize = value;
				_tempProfile.Background.Set();
				UpdateDisplay();
			}
		}

		/// <summary>
		/// List of all the pre-existing Mappings.
		/// </summary>
		public MappingList MapList
		{
			set { _mapList = value; }
		}

		/// <summary>
		/// Current mapping being created/edited
		/// </summary>
		private Mapping PreviewMap
		{
			get 
			{ 
				//return _mapList.Where(m => m.PreviewFlag == true).FirstOrDefault(); 
				return _mapList.Where(true);
			}
		}

		/// <summary>
		/// Internal property to get the Active Profile.
		/// </summary>
		[DebuggerHidden]
		private BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public MappingPreview()
		{
			InitializeComponent();
			_tempProfile = new BaseProfile(_workshop.Profile.ProfileDataLayer.GetType());
			_tempProfile.UnclipCanvasWindow();
			_tempProfile.SubstituteCanvas = CanvasPane;
			_tempProfile.Scaling.CellSize = _workshop.Profile.Scaling.CellSize;
			_tempProfile.Scaling.ShowGridLines = true;
			_tempProfile.Scaling.Zoom = 1f;
			_tempProfile.Scaling.LatticeSize = Profile.Scaling.LatticeSize;
			_originalBounds = new Rectangle(0, 0, Profile.Scaling.CanvasSize.Width - 1, Profile.Scaling.CanvasSize.Height - 1);

			_tempProfile.Channels.Add(new Channel(TARGET));
			_tempProfile.Channels.Add(new Channel(IMPORTED));
			_tempProfile.Channels.Add(new Channel(ALREADY_IMPORTED));

			Background Background = _tempProfile.Background;
			Background.SuppressTempFiles = true;
			Background.Color = Color.Black;
			Background.GridColor = Color.FromArgb(25, 25, 25);
			Background.OverlayGrid = true;
			Background.Set();
			Background = null;
		}

		#endregion [ Constructors ]

		#region [ Methods ]
		
		/// <summary>
		/// Draws the particular channel on the Canvas
		/// </summary>
		/// <param name="g">Graphics object used to draw.</param>
		/// <param name="channel">Channel to draw.</param>
		/// <param name="brushColor">Color to draw the channel.</param>
		/// <param name="effectiveLatticeOffset">Amount the cells are to be offset.</param>
		private void DrawChannel(Graphics g, Channel channel, Color brushColor, Color outlineColor, LatticePoint effectiveLatticeOffset)
		{
			if (!channel.Visible)
				return;

			if (!channel.HasLatticeData)
				return;

			GraphicsPath Path = (GraphicsPath)channel.GetGraphicsPath().Clone();
			Rectangle Bounds = channel.GetBounds();

			Bounds = new Rectangle((Bounds.X + effectiveLatticeOffset.X) * _tempProfile.Scaling.CellScale,
								   (Bounds.Y + effectiveLatticeOffset.Y) * _tempProfile.Scaling.CellScale,
								   Bounds.Width * _tempProfile.Scaling.CellScale, Bounds.Height * _tempProfile.Scaling.CellScale);

			CanvasPoint CanvasOffset = _workshop.CalcCanvasPoint(effectiveLatticeOffset, _tempProfile);
			Matrix MoveMatrix = new Matrix();
			MoveMatrix.Translate(CanvasOffset.X, CanvasOffset.Y);
			Path.Transform(MoveMatrix);

			using (Brush ChannelBrush = new SolidBrush(brushColor))
				g.FillPath(ChannelBrush, Path);

			using (Pen BoundsPen = new Pen(brushColor))
			{
				BoundsPen.DashStyle = DashStyle.Dot;
				g.DrawRectangle(BoundsPen, Bounds);
			}

			MoveMatrix.Dispose();
			MoveMatrix = null;
			Path.Dispose();
			Path = null;
		}

		private MappingList GetOtherMappings()
		{
			//return _mapList.Where(m => m.PreviewFlag == false &&
			//						   Object.ReferenceEquals(m.TargetChannel, PreviewMap.TargetChannel) &&
			//						   m.MapID != PreviewMap.MapID).ToList();
			return _mapList.Where(false, PreviewMap.TargetChannel, PreviewMap.ID);
		}

		/// <summary>
		/// Changes the size of the form to match that of the canvas size and refreshes the display.
		/// </summary>
		public void UpdateDisplay()
		{
			if (!Visible)
				return;

			int BorderWidth = Width - CanvasPane.Width;
			int BorderHeight = Height - CanvasPane.Height;

			Size Size = _tempProfile.Scaling.CanvasSize;
			this.Size = new Size(Size.Width + BorderWidth, Size.Height + BorderHeight);

			if (PreviewMap != null)
			{
				_tempProfile.Channels[TARGET].Visible = !PreviewMap.ClearTargetChannel;
				_tempProfile.Channels[IMPORTED].Origin = PreviewMap.ImportedOffset;
			}

			CanvasPane.Refresh();
		}

		#endregion [ Methods ]

		#region [ Events ]

		/// <summary>
		/// Paint the preview of the mapping on the CanvasPane
		/// </summary>
		private void CanvasPane_Paint(object sender, PaintEventArgs e)
		{
			Channel Channel = null;
			Color DrawColor = Color.Empty;
			Color ImportedColor = Color.Empty;
			Color TargetColor = Color.Empty;

			Rectangle SetBackBounds = _originalBounds;
			SetBackBounds.Offset(new CanvasPoint(MaxSetBack.X * _tempProfile.Scaling.CellScale, MaxSetBack.Y * _tempProfile.Scaling.CellScale));

			// Draw the original boundary of the Canvas
			e.Graphics.DrawRectangle(Pens.White, SetBackBounds);

			if (PreviewMap == null)
				return;

			if ((PreviewMap.ImportedChannel == null) && (PreviewMap.TargetChannel == null))
				return;

			// Dump the contents of the mapping Channels into the channels of the temp Profile
			Channel = _tempProfile.Channels[IMPORTED];
			if (PreviewMap.ImportedChannel != null)
			{
				Channel.DeserializeLattice(PreviewMap.ImportedChannel.SerializeLattice(), true);
				ImportedColor = PreviewMap.ImportedChannel.GetColor();
			}
			else
				Channel.Empty();

			Channel = _tempProfile.Channels[TARGET];
			if (PreviewMap.TargetChannel != null)
			{
				Channel.DeserializeLattice(PreviewMap.TargetChannel.SerializeLattice(), true);
				TargetColor = PreviewMap.TargetChannel.GetColor();
			}
			else
				Channel.Empty();

			try
			{
				// Try to determine the proper color to use.
				if (PreviewMap.OverrideColor)
					DrawColor = ImportedColor;
				else
					DrawColor = TargetColor;

				// If there still is no color, the one or the other Channel is missing, use whichever color is present.
				if (DrawColor.IsEmpty)
				{
					if (PreviewMap.TargetChannel == null)
						DrawColor = ImportedColor;
					else if (PreviewMap.ImportedChannel == null)
						DrawColor = TargetColor;
				}

				// This should NEVER happen.
				if (DrawColor.IsEmpty)
					DrawColor = Color.Red;

				// Draw the original channel and the one we are currently editing.
				DrawChannel(e.Graphics, _tempProfile.Channels[TARGET], DrawColor, Color.White, MaxSetBack);
				DrawChannel(e.Graphics, _tempProfile.Channels[IMPORTED], DrawColor, Color.Yellow, PreviewMap.EffectiveOffset);

				Channel = _tempProfile.Channels[ALREADY_IMPORTED];

				Color FadedDrawColor = Color.FromArgb(_workshop.UI.InactiveChannelAlpha, DrawColor);
				
				// Find all the existing mappings that point to the same target Channel and display their data as well, in order to show the composite of all the mappings to this Channel.
				foreach (Mapping map in GetOtherMappings())
				{ 
					// Grab the contents of the import channel of this mapping and inject it into the "ALREADY_IMPORTED" channel of our temp Profile.
					Channel.DeserializeLattice(map.ImportedChannel.SerializeLattice(), true);
					DrawChannel(e.Graphics, Channel, FadedDrawColor, Color.Yellow, map.EffectiveOffset);
				}

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				//_workshop.WriteTraceMessage(ex.ToString(), TraceLevel.Error);
				throw;
			}
		}

		#endregion [ Events ]
	}
}
