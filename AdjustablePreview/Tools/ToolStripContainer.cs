using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ElfCore.Tools
{
	public partial class ToolStripContainer : Form
	{
		// Create methods to retrieve the various tool strip controls. For ones similar across tools, pass in the Tool enum

		#region [ Properties ]

		public ToolStrip GenericToolStrip
		{
			get { return this.Generic_ToolStrip; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ToolStripContainer()
		{
			InitializeComponent();
			AddCustomControls();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public void AddCustomControls()
		{
			TrackBar tb = new TrackBar();
			tb.AutoSize = false;
			tb.Size = new System.Drawing.Size(60, 20);
			tb.Maximum = 3;
			tb.LargeChange = 1;
			tb.TickStyle = TickStyle.None;

			ToolStripControlHost item = new ToolStripControlHost(tb);
			item.AutoSize = true;

			Spray_ToolStrip.Items.Insert(Spray_ToolStrip.Items.Count - 1, item);
		}

		public ToolStrip GetToolStrip(int ownerTool)
		{
			switch (ownerTool)
			{
				case (int)Tool.Crop:
					//return Crop_ToolStrip;
					return null;

				case (int)Tool.Ellipse:
					return Ellipse_ToolStrip;

				case (int)Tool.Mask_Ellipse:
				case (int)Tool.Mask_Freehand:
				case (int)Tool.Mask_Paint:
				case (int)Tool.Mask_Rectangle:
				case (int)Tool.Mask_Lasso:
					return Mask_ToolStrip;

				case (int)Tool.Erase:
					return Eraser_ToolStrip;

				case (int)Tool.Fill:
					return null;

				case (int)Tool.Icicles:
					return Icicles_ToolStrip;

				case (int)Tool.ImageStamp:
					return ImageStamp_ToolStrip;

				case (int)Tool.Line:
					return Line_ToolStrip;

				case (int)Tool.MegaTree:
					return MegaTree_ToolStrip;

				case (int)Tool.MoveChannel:
					return MoveChannel_ToolStrip;

				case (int)Tool.MultiChannelLine:
					return MultiChannelLine_ToolStrip;

				case (int)Tool.Paint:
					return Paint_ToolStrip;

				case (int)Tool.Polygon:
					return Polygon_ToolStrip;

				case (int)Tool.Rectangle:
					return Rectangle_ToolStrip;

				case (int)Tool.SingingFace:
					return SingingFace_ToolStrip;

				case (int)Tool.Spray:
					return Spray_ToolStrip;

				case (int)Tool.Text:
					return Text_ToolStrip;

				case (int)Tool.Zoom:
					return Zoom_ToolStrip;

				default:
					return null;
			}
		}

		/// <summary>
		/// Returns an item from a toolstrip of a given type and position
		/// </summary>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="strip">ToolStrip object to find the child control on.</param>
		/// <param name="index">Index of the object to return, Example, if the 3rd button is desired, it will return the 3rd object of that type that is found, regardless of the number of other controls that preceeded it.</param>
		public ToolStripItem GetItem<T>(ToolStrip strip, int index)
		{
			if (strip == null)
				return null;

			int Counter = 0;

			for (int i = 0; i < strip.Items.Count; i++)
			{
				if (strip.Items[i] is T)
				{
					Counter++;
					if (Counter == index)
						return strip.Items[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Returns an item from a toolstrip of a given type and position
		/// </summary>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="ownerTool">ID of the Tool that is associated with a ToolStrip</param>
		/// <param name="index">Index of the object to return, Example, if the 3rd button is desired, it will return the 3rd object of that type that is found, regardless of the number of other controls that preceeded it.</param>
		public ToolStripItem GetItem<T>(int ownerTool, int index)
		{
			ToolStrip strip = GetToolStrip(ownerTool);
			if (strip == null)
				return null;

			int Counter = 0;

			for (int i = 0; i < strip.Items.Count; i++)
			{
				if (strip.Items[i] is T)
				{
					Counter++;
					if (Counter == index)
						return strip.Items[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Returns an item from a toolstrip of a given type and name
		/// </summary>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="strip">ToolStrip object to find the child control on.</param>
		/// <param name="name">Name of the control</param>
		public ToolStripItem GetItem<T>(ToolStrip strip, string name)
		{
			if (strip == null)
				return null;

			for (int i = 0; i < strip.Items.Count; i++)
			{
				if (strip.Items[i] is T)
				{
					if (string.Compare(strip.Items[i].Name, name, true) == 0)
						return strip.Items[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Returns an item from a toolstrip of a given type and name
		/// </summary>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="ownerTool">ID of the Tool that is associated with a ToolStrip</param>
		/// <param name="name">Name of the control</param>
		public ToolStripItem GetItem<T>(int ownerTool, string name)
		{
			ToolStrip strip = GetToolStrip(ownerTool);
			if (strip == null)
				return null;

			for (int i = 0; i < strip.Items.Count; i++)
			{
				if (strip.Items[i] is T)
				{
					if (string.Compare(strip.Items[i].Name, name, true) == 0)
						return strip.Items[i];
				}
			}

			return null;
		}

		#endregion [ Methods ]

		#region [ Events ]

		private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		private void SignedFloatOnly_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsDigit(e.KeyChar) ||
				char.IsControl(e.KeyChar) ||
				(e.KeyChar == '-') ||
				(e.KeyChar == '+') ||
				(e.KeyChar == '.') ||
				(e.KeyChar == '°'))
			{
				// We like these, do nothing
				e.Handled = false;
			}
			else
				e.Handled = true;
		}

		private void SignedNumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsDigit(e.KeyChar) ||
				char.IsControl(e.KeyChar) ||
				(e.KeyChar == '-') ||
				(e.KeyChar == '+'))
			{
				// We like these, do nothing
				e.Handled = false;
			}
			else
				e.Handled = true;
		}

		#endregion [ Events ]
	}
}
