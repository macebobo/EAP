using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Forms;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Profiles
{
	internal class PlaceholderProfile : IProfile
	{
		#region [ Properties ]

		/* IProfile Properties */

		public Core.Background Background { get; set; }

		public Controllers.ChannelController Channels { get; set; }

		public int ChannelCount
		{
			get { throw new NotImplementedException(); }
		}

		public System.Windows.Forms.Cursor Cursor{ get; set; }

		public string DefaultSavePath{ get; set; }

		public bool Enabled { get; set; }

		public string FileExtension { get; set; }

		public Forms.CanvasWindow Form { get; set; }

		public string FormatName { get; set; }

		public string Filename { get; set; }

		public bool HasMask
		{
			get { return false; }
		}

		public bool HasRedo
		{
			get { return false; }
		}

		public bool HasUndo
		{
			get { return false; }
		}

		public System.Drawing.Bitmap IconImage
		{
			get { return ElfRes.undefined; }
		}

		public int ID { get; set; }

		public string Name { get; set; }

		public Core.Scaling Scaling { get; set; }

		public System.Windows.Forms.PictureBox SubstituteCanvas { get; set; }

		/* IBase Properies */

		public bool Dirty { get; set; }

		public string GUID { get; set; }

		public string Serialized { get; set; }

		public bool SuppressEvents { get; set; }

		#endregion [ Properties ]

		#region [ Methods ]

		/* IProfile Methods */

		public void ClearMask()
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			throw new NotImplementedException();
		}

		public string Debug_RedoStack()
		{
			throw new NotImplementedException();
		}

		public string Debug_UndoStack()
		{
			throw new NotImplementedException();
		}

		public string Debug_UndoSnapshot()
		{
			throw new NotImplementedException();
		}

		public void DefineMask(Core.Mask newMask)
		{
			throw new NotImplementedException();
		}

		public void DefineMask(Core.Mask newMask, bool createOverlay)
		{
			throw new NotImplementedException();
		}

		public void DefineMask(System.Drawing.Drawing2D.GraphicsPath canvasOutline, System.Drawing.Drawing2D.GraphicsPath latticeOutline)
		{
			throw new NotImplementedException();
		}

		public System.Windows.Forms.PictureBox GetCanvas()
		{
			throw new NotImplementedException();
		}

		public System.Drawing.Graphics GetCanvasGraphics()
		{
			throw new NotImplementedException();
		}

		public System.Drawing.Drawing2D.GraphicsPath GetMaskOutline(Util.UnitScale scale)
		{
			throw new NotImplementedException();
		}

		public System.Drawing.Bitmap GetMaskOverlay()
		{
			throw new NotImplementedException();
		}

		public string GetUndoText(bool undo)
		{
			throw new NotImplementedException();
		}

		public void InitializeUndo()
		{
			throw new NotImplementedException();
		}

		public bool Load()
		{
			throw new NotImplementedException();
		}

		public bool Load(System.Xml.XmlNode setupData, List<Channels.Properties> rawChannels)
		{
			throw new NotImplementedException();
		}

		public bool Load(string filename)
		{
			throw new NotImplementedException();
		}

		public void MoveMask(System.Drawing.Point offset)
		{
			throw new NotImplementedException();
		}

		public void MoveMask(System.Drawing.PointF offset)
		{
			throw new NotImplementedException();
		}

		public void Refresh()
		{
			throw new NotImplementedException();
		}

		public void ReleaseMouse()
		{
			throw new NotImplementedException();
		}

		public void Redo()
		{
			throw new NotImplementedException();
		}

		public bool Save()
		{
			throw new NotImplementedException();
		}

		public bool Save(string filename)
		{
			throw new NotImplementedException();
		}

		public bool Save(System.Xml.XmlNode setupData)
		{
			throw new NotImplementedException();
		}

		public void SaveUndo(string action)
		{
			throw new NotImplementedException();
		}

		public string SerializeMask()
		{
			throw new NotImplementedException();
		}

		public void SetMask(Core.Mask maskData)
		{
			throw new NotImplementedException();
		}

		public void SetClickZoom(System.Drawing.Point zoomPoint, float zoomLevel)
		{
			throw new NotImplementedException();
		}

		public void TrapMouse()
		{
			throw new NotImplementedException();
		}

		public void UnclipCanvasWindow()
		{
			throw new NotImplementedException();
		}

		public void Undo()
		{
			throw new NotImplementedException();
		}

		public bool ValidateFile(string filename)
		{
			return false;
		}

		/* IBase Methods */

		public void SetClean()
		{
			throw new NotImplementedException();
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}

		public event Util.EventHandlers.DirtyEventHandler DirtyChanged;

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion [ Methods ]

		#region [ Events ]

		/* IProfile Events */

		public event EventHandler Canvas_MouseLeave;

		public event EventHandler Canvas_MouseEnter;

		public event Util.EventHandlers.ChannelListEventHandler ChannelPropertyChanged;

		public event Util.EventHandlers.ChannelEventHandler ChannelRemoved;

		public event Util.EventHandlers.ChannelListEventHandler ChannelsSelected;

		public event Util.EventHandlers.ZoomEventHandler ClickZoom;

		public event EventHandler Closing;

		public event EventHandler Loaded;

		public event EventHandler Mask_Cleared;

		public event EventHandler Mask_Defined;

		public event Util.EventHandlers.UndoEventHandler Redo_Changed;

		public event EventHandler ScalingChanged;

		public event Util.EventHandlers.ShuffleEventHandler ShuffleChanged;

		public event Util.EventHandlers.ShuffleEventHandler ShuffleSwitched;

		public event Util.EventHandlers.UndoEventHandler Undo_Changed;

		public event EventHandler Undo_Completed;

		/* IBase Events */

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		#endregion [ Events ]

		
	}
}
