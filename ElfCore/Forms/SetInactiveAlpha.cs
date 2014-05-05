using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Profiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ElfCore.Forms
{
	public partial class SetInactiveAlpha : Form
	{

		#region [ Private Variables ]

		private BaseProfile _tempProfile;
		private Workshop _workshop = Workshop.Instance;
		
		private int _selected = 0;
		private string PercentFormat = "0%;(0%);\"-\"";
		private byte _originalAlpha = 128;
		private byte _currentAlpha = 128;

		#endregion [ Private Variables ]

		#region [ Properties ]
				
		public byte Alpha
		{
			get { return _currentAlpha; }
			set 
			{
				hslAlpha.IndicatorMarks.Add((double)value);
				_originalAlpha = value;
				hslAlpha.Value = (double)value;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public SetInactiveAlpha()
		{
			InitializeComponent();

			_tempProfile = new BaseProfile(_workshop.Profile.ProfileDataLayer.GetType());
			_tempProfile.UnclipCanvasWindow();
			_tempProfile.SubstituteCanvas = pctPreview;
			hslAlpha.IndicatorMarks.Clear();
			pctPreview.BackColor = Color.Black;
			CreatePreview();
		}
		
		#endregion [ Constructors ]
		
		#region [ Methods ]

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

		#endregion [ Methods ]

		#region [ Events ]

		private void Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			_workshop.UI.InactiveChannelAlpha = _originalAlpha;
		}

		private void hslAlpha_Changed(object sender, EventArgs e)
		{
			lblAlpha.Text = (hslAlpha.Value / hslAlpha.MaxValue).ToString(PercentFormat);
			_currentAlpha = Convert.ToByte(Math.Round(hslAlpha.Value));
			_workshop.UI.InactiveChannelAlpha = _currentAlpha;
			pctPreview.Refresh();
		}

		private void pctPreview_Paint(object sender, PaintEventArgs e)
		{
			Matrix MoveMatrix = null;
			GraphicsPath Path = null;
			Point Offset;

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

		private void tmrAnimation_Tick(object sender, EventArgs e)
		{
			_selected++;
			if (_selected % 5 == 0)
				_selected = 0;
			_tempProfile.Channels.Select(_selected);
			pctPreview.Refresh();
		}

		#endregion [ Events ]

		
	}
}
