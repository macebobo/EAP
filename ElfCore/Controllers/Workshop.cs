using ElfControls;

using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Forms;
using ElfCore.PlugIn;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using CanvasPointF = System.Drawing.PointF;
using Clipboard = ElfCore.Core.Clipboard;
using LatticePoint = System.Drawing.Point;
using LatticePointF = System.Drawing.PointF;
using Stopwatch = ElfCore.Util.Stopwatch;

namespace ElfCore.Controllers
{
	public sealed class Workshop : ElfBase
	{
		#region [ Private Static Variables ]

		private static readonly Workshop _instance = new Workshop();

		#endregion [ Private Static Variables ]

		#region [ Private Variables ]
		
		/// <summary>
		/// Offset for the dashes on the MarqueePen
		/// </summary>
		private int _marqueeDashedLineOffset = 0;
		private Keys _uniformConstrainKeys = Keys.Shift;
		private Palette _corelPalette = null;
		private Palette _standardPalette = null;
		private Palette _customColors = new Palette();
		private List<string> _tempFiles = new List<string>();
		private bool _initialized = false;
		private Clipboard _clipboard = null;
		private ChannelMover _mover = null;
		private List<ImageStampChannel> _stampChannels = new List<ImageStampChannel>();

		//private bool _noTraceDuringPlayback = false;
		//private ImageStampChannel _imageStampChannel = null;
		//private ImageStampChannel _textChannel = null;
		//private bool _inPlayback = false;
		//private Keys _drawFromCenterConstrainKeys = Keys.Control;
		//private Channel _moveChannel = null;
		//private TraceSwitch _traceSwitch = new TraceSwitch("General", "Entire Application");
		//private Stream _logFileStream = null;
		//private string _logFilename = "ElfCore.log";

		#endregion [ Private Variables ]

		#region [ Field Variables ]

		#if DEBUG
			/// <summary>
			/// Debugging viewing panes. Not visible during runtime when not debugging.
			/// </summary>
			public ViewBufferPane BufferPaneForm = null;
		#endif

		public Cursor LastCursor = Cursors.Default;

		/// <summary>
		/// Tells the CanvasWindow to paint the ImageStamp Channel. Used for the ImageStamp and Text tools
		/// </summary>
		//public bool ShowImageStamp = false;

		public PlugInProfileList AvailableProfiles = null;

		#endregion [ Field Variables ]

		#region [ Properties ]

		/// <summary>
		/// Shortcut to the Active Profile
		/// </summary>
		[DebuggerHidden()]
		public BaseProfile Profile
		{
			get { return ProfileController.Active; }
		}
		
		/// <summary>
		/// Clipboard controller
		/// </summary>
		public Clipboard Clipboard
		{
			get
			{
				if (_clipboard == null)
					_clipboard = new Clipboard();
				return _clipboard;
			}
		}

		/// <summary>
		/// Returns the CorelPAINT color palette. If this palette is not already generated, creates it first.
		/// </summary>
		public Palette CorelPalette
		{
			get
			{
				if (_corelPalette == null)
					LoadCorelPalette();
				return _corelPalette;
			}
		}

		/// <summary>
		/// Rectangle of cells defined to be the cropping area of the canvas.
		/// </summary>
		public Rectangle CropArea { get; set; }
		
		/// <summary>
		/// Tool that has been currently selected
		/// </summary>
		internal PlugInTool CurrentTool { get; set; }

		/// <summary>
		/// Gets or Sets the custom color palette
		/// </summary>
		public Palette CustomColors
		{
			get { return _customColors; }
			set { _customColors = value; }
		}

		/// <summary>
		/// Temporary Channel used to hold and display the ImageStamp
		/// </summary>
		//public ImageStampChannel ImageStampChannel 
		//{
		//	get
		//	{
		//		if (_imageStampChannel == null)
		//			_imageStampChannel = new ImageStampChannel();
		//		return _imageStampChannel;
		//	}
		//}

		///// <summary>
		///// Temporary Channel, used to hold cells that are being manipulated
		///// </summary>
		//public Channel MoveChannel
		//{
		//	get
		//	{
		//		if (_moveChannel == null)
		//			_moveChannel = new Channel();
		//		return _moveChannel;
		//	}
		//}

		/// <summary>
		/// Class have holds cells for the various channels that are being moved after selected.
		/// </summary>
		public ChannelMover ChannelMover
		{
			get 
			{
				if (_mover == null)
					_mover = new ChannelMover();
				return _mover;
			}
		}

		/// <summary>
		/// Indicates whether the program is currently in Playback mode.
		/// </summary>
		//public bool InPlayback
		//{
		//	get { return _inPlayback; }
		//	set { _inPlayback = value; }
		//}

		/// <summary>
		/// Returns the Instance of this class.
		/// </summary>
		public static Workshop Instance
		{
			get { return _instance; }
		}

		public KeyboardController KeyboardController { get; private set; }

		/// <summary>
		/// Object that controls the list of Profiles and their various functions
		/// </summary>
		public ProfileController ProfileController { get; private set; }

		/// <summary>
		/// Are we running under the assumed name of AdjustablePreview, or in Elf mode
		/// </summary>
		public RunMode RunMode { get; set; }

		/// <summary>
		/// List of temporary channels various tools can use for editing.
		/// </summary>
		public List<ImageStampChannel> StampChannels
		{
			get { return _stampChannels; }
		}

		/// <summary>
		/// Returns the "standard" color palette. If this palette is not already generated, creates it first.
		/// </summary>
		public Palette StandardPalette
		{
			get
			{
				if (_standardPalette == null)
					LoadStandardPalette();
				return _standardPalette;
			}
		}

		/// <summary>
		/// Stopwatch, used for timing various methods, to determine efficiencies
		/// </summary>
		public Stopwatch Stopwatch = new Stopwatch();

		/// <summary>
		/// List of all temporary files created during the execution of this assembly. Delete these files when shutting down.
		/// </summary>
		public List<string> TempFileNames
		{
			get { return _tempFiles; }
		}

		/// <summary>
		/// Temporary Channel used to hold and display the Text
		/// </summary>
		//public ImageStampChannel TextChannel
		//{
		//	get
		//	{
		//		if (_textChannel == null)
		//			_textChannel = new ImageStampChannel();
		//		return _textChannel;
		//	}
		//}

		/// <summary>
		/// List of PlugIn Tools to use.
		/// </summary>
		internal PlugInToolList Tools { get; set; }

		/// <summary>
		/// List of PlugIn ToolGroups to use.
		/// </summary>
		internal PlugInToolGroupList ToolGroups { get; set; }

		/// <summary>
		/// Gets or Sets the level of Tracing the application does.
		/// </summary>
		//public System.Diagnostics.TraceLevel TraceLevel
		//{
		//	get { return _traceSwitch.Level; }
		//	set { _traceSwitch.Level = value; }
		//}

		/// <summary>
		/// Indicates whether tracing should occur during the playback.
		/// </summary>
		//public bool NoTraceDuringPlayback
		//{
		//	get { return _noTraceDuringPlayback; }
		//	set { _noTraceDuringPlayback = value; }
		//}

		/// <summary>
		/// Gets or Sets the filename of the trace log. If the value is being set, closes and restarts the trace log at the new location.
		/// Creates the file if it does not already exist.
		/// </summary>
		//public string TraceLogFilename
		//{
		//	get { return _logFilename; }
		//	set
		//	{
		//		if (string.Compare(_logFilename, value, true) != 0)
		//		{
		//			CloseTraceLog();
		//			_logFilename = value;
		//			InitializeTraceLog();
		//		}
		//	}
		//}

		/// <summary>
		/// Object that contains data and methods specific to the User Interface
		/// </summary>
		public UISettings UI { get; private set; }
			
		/// <summary>
		/// Path to the user's Windows Profile folder
		/// </summary>
		public string UserDirectory { get; private set; }
		
		#endregion [ Properties ]

		#region [ Constructors ]

		static Workshop()
		{ }

		private Workshop()
		{ }

		#endregion [ Constructors ]

		#region [ Static Methods ]

		/// <summary>
		/// Compare two arrays for equality
		/// http://stackoverflow.com/questions/713341/comparing-arrays-in-c-sharp
		/// </summary>
		/// <typeparam name="T">Type of the data stored in the array</typeparam>
		/// <param name="a1">First array to compare</param>
		/// <param name="a2">Second array to compare</param>
		/// <returns>Returns true if both arrays exactly match each other. Also returns true if both parameters point to the exact same data</returns>
		public static bool ArraysEqual<T>(T[] a1, T[] a2)
		{
			if (ReferenceEquals(a1, a2))
				return true;

			if (a1 == null || a2 == null)
				return false;

			if (a1.Length != a2.Length)
				return false;

			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Length; i++)
			{
				if (!comparer.Equals(a1[i], a2[i]))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Compares two bitmaps on a per-pixel basis
		/// http://social.msdn.microsoft.com/Forums/en-US/vbgeneral/thread/bd0eec9e-f811-4fab-a245-08b2882d005c
		/// </summary>
		/// <param name="b1">First Bitmap to compare</param>
		/// <param name="b2">Second Bitmap to compare</param>
		/// <returns>Returns true if both are null, both share the same pointer, or if both match exactly at the pixel level</returns>
		public static bool BitmapEquals(Bitmap b1, Bitmap b2)
		{
			// Verify that both are not null
			if ((b1 == null) && (b2 == null))
				return true;

			// If one or the other are null, then we don't match
			if ((b1 == null) || (b2 == null))
				return false;

			// Verify that they are not using the same pointer
			if (ReferenceEquals(b1, b2))
				return true;

			// Verify the dimensions are the same
			if (b1.Size != b2.Size)
				return false;

			// Verify that the pixel formats are the same
			if (b1.PixelFormat != b2.PixelFormat)
				return false;

			LockBitmap lb1 = new LockBitmap(b1);
			lb1.FillPixelArray();
			LockBitmap lb2 = new LockBitmap(b2);
			lb2.FillPixelArray();

			bool Result = ArraysEqual<byte>(lb1.Pixels, lb2.Pixels);

			lb1 = null;
			lb2 = null;

			return Result;
		}
		
		/// <summary>
		/// Clones a list, so that one gets a copy of a list by value, not by reference
		/// http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/5c9b4c31-850d-41c4-8ea3-fae734b348c4
		/// </summary>
		/// <typeparam name="T">Type of object in the list</typeparam>
		/// <param name="oldList">List of objects to be cloned</param>
		/// <returns>A duplicate of the original list with the same values, but without the same object references</returns>
		public static List<T> CloneList<T>(List<T> oldList)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, oldList);
			stream.Position = 0;
			return (List<T>)formatter.Deserialize(stream);
		}

		/// <summary>
		/// Flushes and closes out the trace log and removes the listener.
		/// </summary>
		public void CloseTraceLog()
		{
			//if (_logFileStream != null)
			//{
			//	Trace.WriteLine("END TRACE\n");
			//	Trace.Flush();
			//	_logFileStream.Flush();
			//	_logFileStream.Close();
			//	_logFileStream.Dispose();
			//	_logFileStream = null;
			//}
			//Trace.Listeners.Clear();
		}

		/// <summary>
		/// Pukes out the last error message to a text file.
		/// </summary>
		/// <param name="ex">Exception to dump to file.</param>
		public static void CrashLog(Exception ex)
		{
			string FileName = "Elf Crash [" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + "].log";
			
			FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
			StreamWriter sw = new StreamWriter(fs);
			
			sw.BaseStream.Seek(0, SeekOrigin.End);
			sw.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());

			CrashLog(ex, sw);

			sw.Flush();
			sw.Dispose();
			sw = null;

			fs.Close();
			fs = null;

			Debug.WriteLine(ex.Message);
			Debug.WriteLine(ex.Source);
			Debug.WriteLine(ex.StackTrace);
		}

		/// <summary>
		/// Writes out the contents of hte exception to the stream writer object. If there is an inner exception within the 
		/// passed in exception, then recursively calls this method to printout that and other nested exceptions
		/// </summary>
		/// <param name="ex">Exception to dump to file.</param>
		/// <param name="sw">StreamWriter object to the file.</param>
		public static void CrashLog(Exception ex, StreamWriter sw)
		{
			if ((ex == null) || (sw == null))
				return;

			sw.WriteLine("Message:" + ex.Message ?? string.Empty);
			sw.WriteLine("Source:" + ex.Source ?? string.Empty);
			sw.WriteLine("StackTrace:" + ex.StackTrace ?? string.Empty);
			if (ex.InnerException != null)
			{
				sw.WriteLine("-------------------------------------------");
				CrashLog(ex.InnerException, sw);
			}
		}

		/// <summary>
		/// Restores the Canvas cursor to what it was before the static method WaitCursor was called.
		/// </summary>
		public void EndWaitCursor(Control control)
		{
			control.Cursor = LastCursor;
		}

		/// <summary>
		/// Restores the Canvas cursor to what it was before the static method WaitCursor was called.
		/// </summary>
		public void EndWaitCursor(BaseProfile profile)
		{
			profile.Cursor = LastCursor;
		}

		/// <summary>
		/// Display the bitmap on the diagnostic pane in the ViewBufferPane form.
		/// </summary>
		/// <param name="bmp">Bitmap to display</param>
		/// <param name="pane">Tab to display the bitmap upon</param>
		public void ExposePane(Bitmap bmp, Panes pane)
		{
			#if DEBUG
				BufferPaneForm.SetBitmap(bmp, pane);
			#endif
		}

		/// <summary>
		/// Display the Channel data on the diagnostic pane in the ViewBufferPane form.
		/// </summary>
		/// <param name="Channel">Channel to display</param>
		/// <param name="pane">Tab to display the Channel upon</param>
		public void ExposePane(Channel Channel, Panes pane)
		{
			#if DEBUG
				BufferPaneForm.SetBitmap(Channel.LatticeBuffer, pane);
			#endif
		}

		/// <summary>
		/// Create an MD5 hash for a stream. This is a way of identifing if data has changed. If the hash differs, the data is not the same.
		/// </summary>
		/// <param name="stream">Data stream to hash</param>
		public string GetMD5HashFromStream(Stream stream)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(stream);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}

		/// <summary>
		/// Gets the path to the user's Window's Profile directory
		/// </summary>
		public static string GetProfilePath()
		{
			string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen");
			DirectoryInfo DInfo = new DirectoryInfo(SavePath);
			if (!DInfo.Exists)
			{
				// Create the folder
				DInfo.Create();
			}
			return SavePath;
		}

		/// <summary>
		/// Creates the file stream object and establishes a trace listener to this stream.
		/// </summary>
		public void InitializeTraceLog()
		{
			//if (_logFileStream == null)
			//{
			//	_logFileStream = File.Open(_logFilename, FileMode.OpenOrCreate);
			//	_logFileStream.Seek(_logFileStream.Length, SeekOrigin.Begin);
			//	Trace.Listeners.Add(new TextWriterTraceListener(_logFileStream));
			//}
		}

		/// <summary>
		/// Compare Lists for equality
		/// http://stackoverflow.com/questions/713341/comparing-arrays-in-c-sharp
		/// </summary>
		/// <typeparam name="T">Type of the data stored in the array</typeparam>
		/// <param name="a1">First List to compare</param>
		/// <param name="a2">Second List to compare</param>
		/// <returns>Returns true if both Lists exactly match each other. Also returns true if both parameters point to the exact same data</returns>
		public static bool ListEqual<T>(List<T> a1, List<T> a2)
		{
			if (ReferenceEquals(a1, a2))
				return true;

			if (a1 == null || a2 == null)
				return false;

			if (a1.Count != a2.Count)
				return false;

			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Count; i++)
			{
				if (!comparer.Equals(a1[i], a2[i]))
					return false;
			}
			return true;
		}
				
		/// <summary>
		/// Brings up the file save dialog to save the bitmap file to disk
		/// </summary>
		/// <param name="bmp">Bitmap object to save</param>
		/// <param name="filename">Default file name for the file</param>
		/// <returns>If the save is successful, returns the new file name, otherwise returns an empty string</returns>
		public static string SaveBitmap(Bitmap bmp, string filename, string title = null)
		{
			SaveFileDialog SaveImageFileDialog = new SaveFileDialog();
			SaveImageFileDialog.Filter = "Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*";
			SaveImageFileDialog.Title = title ?? "Save Image";
			SaveImageFileDialog.FileName = filename;
			string Ext = string.Empty;

			if (filename.Length > 0)
				Ext = filename.Substring(filename.Length - 3).ToLower();
			else
				Ext = "png";

			string[] Filters = SaveImageFileDialog.Filter.Split('|');

			for (int i = 0; i < Filters.Length; i++)
			{
				// there will be a matched pair for each of these elements, look at the even numbered element
				// Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*
				if (i % 2 == 0)
					i++;
				if (i >= Filters.Length)
					break;

				if (Filters[i].Replace("*.", "") == Ext)
				{
					SaveImageFileDialog.FilterIndex = (i / 2) + 1;
					break;
				}
			}

			if (SaveImageFileDialog.ShowDialog() != DialogResult.OK)
				return string.Empty;

			filename = SaveImageFileDialog.FileName;
			ImageFormat Format;

			switch (SaveImageFileDialog.FilterIndex)
			{
				case 1: // Bitmap
					Format = ImageFormat.Bmp;
					filename = Path.ChangeExtension(filename, ".bmp");
					break;
				case 3: // PNG
					Format = ImageFormat.Png;
					filename = Path.ChangeExtension(filename, ".png");
					break;
				case 4: // GIF
					Format = ImageFormat.Gif;
					filename = Path.ChangeExtension(filename, ".gif");
					break;
				default:
					Format = ImageFormat.Jpeg;
					filename = Path.ChangeExtension(filename, ".jpg");
					break;
			}

			try
			{
				Bitmap b = new Bitmap(bmp);
				b.Save(filename, Format);
				b.Dispose();
				b = null;
			}
			catch
			{
				MessageBox.Show("Unable to save this file, possibly due to where it is being saved.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return string.Empty;
			}

			MessageBox.Show("File saved.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return filename;
		}

		/// <summary>
		/// Sets the Canvas cursor to the system Wait cursor
		/// </summary>
		public void WaitCursor(Control control)
		{
			LastCursor = control.Cursor;
			control.Cursor = Cursors.WaitCursor;
		}

		/// <summary>
		/// Sets the Canvas cursor to the system Wait cursor
		/// </summary>
		public void WaitCursor(BaseProfile profile)
		{
			LastCursor = profile.Cursor;
			profile.Cursor = Cursors.WaitCursor;
		}

		///// <summary>
		///// Write out the message stream to the tracelog
		///// </summary>
		///// <param name="message">Message to write.</param>
		///// <param name="traceLevel">Level of the message. This is compared to the threshold value in _traceLevel to see if the message gets written.</param>
		//public void WriteTraceMessage(string message, TraceLevel traceLevel)
		//{
		//	if (_inPlayback && _noTraceDuringPlayback)
		//		return;

		//	try
		//	{
		//		if ((_logFileStream == null) || (traceLevel > _traceSwitch.Level))
		//			return;

		//		const string Bullet = "; ";

		//		MethodBase Method = (new StackTrace()).GetFrame(1).GetMethod();
		//		string MethodName = Bullet + "Method: " + Method.DeclaringType.Name + "." + Method.Name + "()" + Bullet;
		//		string TraceLevel = Bullet + traceLevel.ToString().PadRight(7);
		//		string TraceMessage = string.Empty;
		//		string Date = String.Format("{0:MM/dd/yyyy HH:mm:ss.fff}", DateTime.Now);

		//		// Strip out extra whitespacing
		//		message = message.Replace("\t", " ").Replace("\r", string.Empty).Replace("\n", " ");
		//		while (message.Contains("  "))
		//			message = message.Replace("  ", " ");

		//		TraceMessage = (Date + TraceLevel + MethodName.PadRight(50) + message.Trim());

		//		Trace.WriteLine(TraceMessage);
		//		Trace.Flush();

		//		Method = null;
		//	}
		//	catch
		//	{ }
		//}	

		#endregion [ Static Methods ]

		#region [ Private Methods ]

		/// <summary>
		/// Returns the PlugInTool based on ID
		/// </summary>
		/// <param name="toolID">Id of the PlugInTool object in the Tools list.</param>
		internal PlugInTool GetPlugInTool(int toolID)
		{
			//return Tools.Where(t => t.ID == toolID).FirstOrDefault();
			return Tools.Where(toolID);
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();

			CloseTraceLog();

			#if DEBUG
			if (BufferPaneForm != null)
			{
				BufferPaneForm.Close();
				BufferPaneForm = null;
			}
			#endif

			if (Tools != null)
			{
				foreach (PlugInTool pTool in Tools)
				{
					pTool.ShutDown();
					pTool.Dispose();
				}
				Tools.Clear();
				Tools = null;
			}

			if (ToolGroups != null)
			{
				foreach (PlugInToolGroup pToolGroup in ToolGroups)
				{
					pToolGroup.ShutDown();
					pToolGroup.Dispose();
				}
				ToolGroups.Clear();
				ToolGroups = null;
			}

			if (_tempFiles != null)
			{
				foreach (string tempFileName in _tempFiles)
				{
					File.Delete(tempFileName);
				}
				_tempFiles.Clear();
				_tempFiles = null;
			}

			if (UI != null)
			{
				UI.Dispose();
				UI = null;
			}
			if (_mover != null)
			{
				_mover.Dispose();
				_mover = null;
			}
			//if (_imageStampChannel != null)
			//{
			//	_imageStampChannel.Dispose();
			//	_imageStampChannel = null;
			//}
			if (_stampChannels != null)
			{
				_stampChannels.Clear();
				_stampChannels = null;
			}
			//if (_textChannel != null)
			//{
			//	_textChannel.Dispose();
			//	_textChannel = null;
			//}
			if (ProfileController != null)
			{
				ProfileController.Dispose();
				ProfileController = null;
			}
			if (KeyboardController != null)
			{
				//KeyboardController.Stop();
				KeyboardController.Dispose();
				KeyboardController = null;
			}
			if (AvailableProfiles != null)
			{
				AvailableProfiles.Clear();
				AvailableProfiles = null;
			}
			if (_clipboard != null)
			{
				_clipboard.Dispose();
				_clipboard = null;
			}
			if (_mover != null)
			{
				_mover.Dispose();
				_mover = null;
			}
			Stopwatch = null;
		}

		private void LoadCorelPalette()
		{
			_corelPalette = new Palette();
			_corelPalette.Add(new NamedColor("Black", 0, 0, 0));
			_corelPalette.Add(new NamedColor("90% Black", 25, 25, 25));
			_corelPalette.Add(new NamedColor("80% Black", 51, 51, 51));
			_corelPalette.Add(new NamedColor("70% Black", 76, 76, 76));
			_corelPalette.Add(new NamedColor("60% Black", 102, 102, 102));
			_corelPalette.Add(new NamedColor("50% Black", 127, 127, 127));
			_corelPalette.Add(new NamedColor("40% Black", 153, 153, 153));
			_corelPalette.Add(new NamedColor("30% Black", 178, 178, 178));
			_corelPalette.Add(new NamedColor("20% Black", 204, 204, 204));
			_corelPalette.Add(new NamedColor("10% Black", 229, 229, 229));
			_corelPalette.Add(new NamedColor("White", 255, 255, 255));
			_corelPalette.Add(new NamedColor("Blue", 0, 0, 255));
			_corelPalette.Add(new NamedColor("Cyan", 0, 255, 255));
			_corelPalette.Add(new NamedColor("Green", 0, 255, 0));
			_corelPalette.Add(new NamedColor("Yellow", 255, 255, 0));
			_corelPalette.Add(new NamedColor("Red", 255, 0, 0));
			_corelPalette.Add(new NamedColor("Magenta", 255, 0, 255));
			_corelPalette.Add(new NamedColor("Purple", 153, 0, 204));
			_corelPalette.Add(new NamedColor("Orange", 255, 102, 0));
			_corelPalette.Add(new NamedColor("Pink", 255, 153, 204));
			_corelPalette.Add(new NamedColor("Dark Brown", 102, 51, 51));
			_corelPalette.Add(new NamedColor("Powder Blue", 204, 204, 255));
			_corelPalette.Add(new NamedColor("Pastel Blue", 153, 153, 255));
			_corelPalette.Add(new NamedColor("Baby Blue", 102, 153, 255));
			_corelPalette.Add(new NamedColor("Electric Blue", 102, 102, 255));
			_corelPalette.Add(new NamedColor("Twilight Blue", 102, 102, 204));
			_corelPalette.Add(new NamedColor("Navy Blue", 0, 51, 153));
			_corelPalette.Add(new NamedColor("Deep Navy Blue", 0, 0, 102));
			_corelPalette.Add(new NamedColor("Desert Blue", 51, 102, 153));
			_corelPalette.Add(new NamedColor("Sky Blue", 2, 204, 255));
			_corelPalette.Add(new NamedColor("Ice Blue", 153, 255, 255));
			_corelPalette.Add(new NamedColor("Light Blue Green", 153, 204, 204));
			_corelPalette.Add(new NamedColor("Ocean Green", 102, 153, 153));
			_corelPalette.Add(new NamedColor("Moss Green", 51, 102, 102));
			_corelPalette.Add(new NamedColor("Dark Green", 0, 51, 51));
			_corelPalette.Add(new NamedColor("Forest Green", 0, 102, 51));
			_corelPalette.Add(new NamedColor("Grass Green", 0, 153, 51));
			_corelPalette.Add(new NamedColor("Kentucky Green", 51, 153, 102));
			_corelPalette.Add(new NamedColor("Light Green", 51, 204, 102));
			_corelPalette.Add(new NamedColor("Spring Green", 51, 204, 51));
			_corelPalette.Add(new NamedColor("Turquoise", 102, 255, 204));
			_corelPalette.Add(new NamedColor("Sea Green", 51, 204, 153));
			_corelPalette.Add(new NamedColor("Faded Green", 153, 204, 153));
			_corelPalette.Add(new NamedColor("Ghost Green", 204, 255, 204));
			_corelPalette.Add(new NamedColor("Mint Green", 153, 255, 153));
			_corelPalette.Add(new NamedColor("Army Green", 102, 153, 102));
			_corelPalette.Add(new NamedColor("Avocado Green", 102, 153, 51));
			_corelPalette.Add(new NamedColor("Martian Green", 153, 204, 51));
			_corelPalette.Add(new NamedColor("Dull Green", 153, 204, 102));
			_corelPalette.Add(new NamedColor("Chartreuse", 153, 255, 0));
			_corelPalette.Add(new NamedColor("Moon Green", 204, 255, 102));
			_corelPalette.Add(new NamedColor("Murky Green", 51, 51, 0));
			_corelPalette.Add(new NamedColor("Olive Drab", 102, 102, 51));
			_corelPalette.Add(new NamedColor("Khaki", 153, 153, 102));
			_corelPalette.Add(new NamedColor("Olive", 153, 153, 51));
			_corelPalette.Add(new NamedColor("Banana Yellow", 204, 204, 51));
			_corelPalette.Add(new NamedColor("Light Yellow", 255, 255, 102));
			_corelPalette.Add(new NamedColor("Chalk", 255, 255, 153));
			_corelPalette.Add(new NamedColor("Pale Yellow", 255, 255, 204));
			_corelPalette.Add(new NamedColor("Brown", 153, 102, 51));
			_corelPalette.Add(new NamedColor("Red Brown", 204, 102, 51));
			_corelPalette.Add(new NamedColor("Gold", 204, 153, 51));
			_corelPalette.Add(new NamedColor("Autumn Orange", 255, 102, 51));
			_corelPalette.Add(new NamedColor("Light Orange", 255, 153, 51));
			_corelPalette.Add(new NamedColor("Peach", 255, 153, 102));
			_corelPalette.Add(new NamedColor("Deep Yellow", 255, 204, 0));
			_corelPalette.Add(new NamedColor("Sand", 255, 204, 153));
			_corelPalette.Add(new NamedColor("Walnut", 102, 51, 0));
			_corelPalette.Add(new NamedColor("Ruby Red", 153, 0, 0));
			_corelPalette.Add(new NamedColor("Brick Red", 204, 51, 0));
			_corelPalette.Add(new NamedColor("Tropical Pink", 255, 102, 102));
			_corelPalette.Add(new NamedColor("Soft Pink", 255, 153, 153));
			_corelPalette.Add(new NamedColor("Faded Pink", 255, 204, 204));
			_corelPalette.Add(new NamedColor("Crimson", 153, 51, 102));
			_corelPalette.Add(new NamedColor("Regal Red", 204, 51, 102));
			_corelPalette.Add(new NamedColor("Deep Rose", 204, 51, 153));
			_corelPalette.Add(new NamedColor("Neon Red", 255, 0, 102));
			_corelPalette.Add(new NamedColor("Deep Pink", 255, 102, 153));
			_corelPalette.Add(new NamedColor("Hot Pink", 255, 51, 153));
			_corelPalette.Add(new NamedColor("Dusty Rose", 204, 102, 153));
			_corelPalette.Add(new NamedColor("Plum", 102, 0, 102));
			_corelPalette.Add(new NamedColor("Deep Violet", 153, 0, 153));
			_corelPalette.Add(new NamedColor("Light Violet", 255, 153, 255));
			_corelPalette.Add(new NamedColor("Violet", 204, 102, 204));
			_corelPalette.Add(new NamedColor("Dusty Plum", 153, 102, 153));
			_corelPalette.Add(new NamedColor("Pale Purple", 204, 153, 204));
			_corelPalette.Add(new NamedColor("Majestic Purple", 153, 51, 204));
			_corelPalette.Add(new NamedColor("Neon Purple", 204, 51, 255));
			_corelPalette.Add(new NamedColor("Light Purple", 204, 102, 255));
			_corelPalette.Add(new NamedColor("Twilight Violet", 153, 102, 204));
			_corelPalette.Add(new NamedColor("Easter Purple", 204, 153, 255));
			_corelPalette.Add(new NamedColor("Deep Purple", 51, 0, 102));
			_corelPalette.Add(new NamedColor("Grape", 102, 51, 153));
			_corelPalette.Add(new NamedColor("Blue Violet", 153, 102, 255));
			_corelPalette.Add(new NamedColor("Blue Purple", 153, 0, 255));
			_corelPalette.Add(new NamedColor("Deep River", 102, 0, 204));
			_corelPalette.Add(new NamedColor("Deep Azure", 102, 51, 255));
			_corelPalette.Add(new NamedColor("Storm Blue", 51, 0, 153));
			_corelPalette.Add(new NamedColor("Deep Blue", 51, 0, 204));
			_corelPalette.Add(new NamedColor("Denim", 210, 70, 45));
			_corelPalette.Add(new NamedColor("Hazelnut", 78, 48, 18));
		}

		private void LoadStandardPalette()
		{
			_standardPalette = new Palette();
			_standardPalette.Add(new NamedColor("White", 255, 255, 255));
			_standardPalette.Add(new NamedColor("95% Black", 224, 224, 224));
			_standardPalette.Add(new NamedColor("Silver", 192, 192, 192));
			_standardPalette.Add(new NamedColor("Gray", 128, 128, 128));
			_standardPalette.Add(new NamedColor("25% Black", 64, 64, 64));
			_standardPalette.Add(new NamedColor("Black", 0, 0, 0));
			_standardPalette.Add(new NamedColor("Light Pink", 255, 192, 192));
			_standardPalette.Add(new NamedColor("Medium Pink", 255, 128, 128));
			_standardPalette.Add(new NamedColor("Red", 255, 0, 0));
			_standardPalette.Add(new NamedColor("Dusky Red", 192, 0, 0));
			_standardPalette.Add(new NamedColor("Maroon", 128, 0, 0));
			_standardPalette.Add(new NamedColor("Dark Red", 64, 0, 0));
			_standardPalette.Add(new NamedColor("Pumpkin Blush", 255, 224, 192));
			_standardPalette.Add(new NamedColor("Light Orange", 255, 192, 128));
			_standardPalette.Add(new NamedColor("Orange", 255, 165, 0));
			_standardPalette.Add(new NamedColor("Dark Pumpkin", 192, 64, 0));
			_standardPalette.Add(new NamedColor("Sienna", 128, 64, 0));
			_standardPalette.Add(new NamedColor("Bruised Orange", 128, 64, 64));
			_standardPalette.Add(new NamedColor("Lemonade", 255, 255, 192));
			_standardPalette.Add(new NamedColor("Light Yellow", 255, 255, 128));
			_standardPalette.Add(new NamedColor("Yellow", 255, 255, 0));
			_standardPalette.Add(new NamedColor("Dark Yellow", 192, 192, 0));
			_standardPalette.Add(new NamedColor("Olive", 128, 128, 0));
			_standardPalette.Add(new NamedColor("Scourged Yellow", 64, 64, 0));
			_standardPalette.Add(new NamedColor("Faded Green", 192, 255, 192));
			_standardPalette.Add(new NamedColor("Light Green", 128, 255, 128));
			_standardPalette.Add(new NamedColor("Lime", 0, 255, 0));
			_standardPalette.Add(new NamedColor("Kelly Green", 0, 192, 0));
			_standardPalette.Add(new NamedColor("Green", 0, 128, 0));
			_standardPalette.Add(new NamedColor("Forest Green", 0, 64, 0));
			_standardPalette.Add(new NamedColor("Fairy Dust", 192, 255, 255));
			_standardPalette.Add(new NamedColor("Light Cyan", 128, 255, 255));
			_standardPalette.Add(new NamedColor("Cyan", 0, 255, 255));
			_standardPalette.Add(new NamedColor("Dark Cyan", 0, 192, 192));
			_standardPalette.Add(new NamedColor("Teal", 0, 128, 128));
			_standardPalette.Add(new NamedColor("Dark Teal", 0, 64, 64));
			_standardPalette.Add(new NamedColor("Powder Blue", 192, 192, 255));
			_standardPalette.Add(new NamedColor("Light Blue", 128, 128, 255));
			_standardPalette.Add(new NamedColor("Blue", 0, 0, 255));
			_standardPalette.Add(new NamedColor("Dark Blue", 0, 0, 192));
			_standardPalette.Add(new NamedColor("Navy", 0, 0, 128));
			_standardPalette.Add(new NamedColor("Dark Navy", 0, 0, 64));
			_standardPalette.Add(new NamedColor("Aurora Pink", 255, 192, 255));
			_standardPalette.Add(new NamedColor("Light Magenta", 255, 128, 255));
			_standardPalette.Add(new NamedColor("Magenta", 255, 0, 255));
			_standardPalette.Add(new NamedColor("Dark Magenta", 192, 0, 192));
			_standardPalette.Add(new NamedColor("Purple", 128, 0, 128));
			_standardPalette.Add(new NamedColor("Dark Purple", 64, 0, 64));
		}

		#endregion [ Private Methods ]

		#region [ Public Methods ]

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point, oriented to the upper left corner of the cell.
		/// </summary>
		/// <param name="latticePoint">Point with values in cells</param>
		/// <returns>CanvasPoint with values in pixels</returns>
		public CanvasPoint CalcCanvasPoint(LatticePoint latticePoint)
		{
			return CalcCanvasPoint(latticePoint, Profile);
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point, oriented to the upper left corner of the cell.
		/// </summary>
		/// <param name="latticePoint">Point with values in cells</param>
		/// <param name="profile">Profile used to get scaling information.</param>
		/// <returns>CanvasPoint with values in pixels</returns>
		public CanvasPoint CalcCanvasPoint(LatticePoint latticePoint, BaseProfile profile)
		{
			int CellScale = profile.Scaling.CellScale;
			return new Point(latticePoint.X * CellScale, latticePoint.Y * CellScale);
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point, oriented to the center of the cell.
		/// </summary>
		/// <param name="latticePoint">Point with values in cells</param>
		/// <returns>CanvasPoint with values in pixels</returns>
		public CanvasPoint CalcCanvasPoint_OC(LatticePoint latticePoint)
		{
			return CalcCanvasPoint_OC(latticePoint, Profile);
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point, oriented to the center of the cell.
		/// </summary>
		/// <param name="latticePoint">Point with values in cells</param>
		/// <param name="profile">Profile used to get scaling information.</param>
		/// <returns>CanvasPoint with values in pixels</returns>
		public CanvasPoint CalcCanvasPoint_OC(LatticePoint latticePoint, BaseProfile profile)
		{
			int CellScale = profile.Scaling.CellScale;
			int OC = CellScale / 2;
			return new Point(latticePoint.X * CellScale + OC, latticePoint.Y * CellScale + OC);
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point, oriented to the upper left corner of the cell.
		/// </summary>
		/// <param name="latticePoint">Point with values in cells</param>
		/// <returns>CanvasPointF with values in pixels</returns>
		public CanvasPointF CalcCanvasPointF(LatticePointF latticePoint)
		{
			return CalcCanvasPointF(latticePoint, Profile);
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point, oriented to the upper left corner of the cell.
		/// </summary>
		/// <param name="latticePoint">Point with values in cells</param>
		/// <returns>CanvasPointF with values in pixels</returns>
		public CanvasPointF CalcCanvasPointF(LatticePointF latticePoint, BaseProfile profile)
		{
			float CellScaleF = profile.Scaling.CellScaleF;
			return new PointF(latticePoint.X * CellScaleF, latticePoint.Y * CellScaleF);
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point, oriented to the center of the cell.
		/// </summary>
		/// <param name="latticePoint">Point with values in cells</param>
		/// <returns>CanvasPointF with values in pixels</returns>
		public CanvasPointF CalcCanvasPointF_OC(LatticePointF latticePoint)
		{
			return CalcCanvasPointF_OC(latticePoint, Profile);
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point, oriented to the center of the cell.
		/// </summary>
		/// <param name="latticePoint">Point with values in cells</param>
		/// <returns>CanvasPointF with values in pixels</returns>
		public CanvasPointF CalcCanvasPointF_OC(LatticePointF latticePoint, BaseProfile profile)
		{
			float CellScaleF = profile.Scaling.CellScaleF;
			float OC = CellScaleF / 2f;
			return new PointF(latticePoint.X * CellScaleF + OC, latticePoint.Y * CellScaleF + OC);
		}

		/// <summary>
		/// Create a new point in Cell values from the Pixel-based point
		/// </summary>
		/// <param name="pixelPoint">Point with values in cells</param>
		/// <returns>Point with values in pixels</returns>
		public LatticePoint CalcLatticePoint(CanvasPoint pixelPoint)
		{
			if (Profile != null)
				return new Point(pixelPoint.X / Profile.Scaling.CellScale, pixelPoint.Y / Profile.Scaling.CellScale);
			else
				return pixelPoint;
		}

		/// <summary>
		/// Create a new point in Cell values from the Pixel-based point
		/// </summary>
		/// <param name="pixelPoint">Point with values in cells</param>
		/// <returns>Point with values in pixels</returns>
		public LatticePointF CalcLatticePointF(CanvasPointF pixelPoint)
		{
			if (Profile != null)
				return new PointF((int)(pixelPoint.X / Profile.Scaling.CellScaleF), (int)(pixelPoint.Y / Profile.Scaling.CellScaleF));
			else
				return pixelPoint;
		}

		/// <summary>
		/// Generate a bitmap of the Canvas, excluding the active Channel(s)
		/// </summary>
		/// <param name="canvasPane"></param>
		/// <returns></returns>
		public Bitmap CaptureCanvas()
		{
			return CaptureCanvas(false);
		}

		/// <summary>
		/// Generate a bitmap of the Canvas. If indicated, exclude the selected Channel(s)
		/// </summary>
		/// <param name="excludeSelectedChannels">Indicates whether selected Channels should be included in this screen grab</param>
		/// <returns>Bitmap of the current canvas</returns>
		public Bitmap CaptureCanvas(bool excludeSelectedChannels)
		{
			PictureBox Canvas = Profile.GetCanvas();
			if (Canvas == null)
				return null;

			int width = Profile.Scaling.CanvasSize.Width;
			int height = Profile.Scaling.CanvasSize.Height;

			Bitmap bm = new Bitmap(width, height, Profile.GetCanvasGraphics());

			if (excludeSelectedChannels)
			{
				Profile.Channels.AllowChannelDrawing(Profile.Channels.Selected, false);
				Canvas.DrawToBitmap(bm, new Rectangle(0, 0, width, height));
				Profile.Channels.AllowChannelDrawing(Profile.Channels.Selected, true);
				Profile.Refresh();
			}
			else
				Canvas.DrawToBitmap(bm, new Rectangle(0, 0, width, height));

			return bm;
		}

		/// <summary>
		/// Calculates the distance between 2 points
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public float ComputeDistance(Point pt1, Point pt2)
		{
			return (float)Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));
		}

		/// <summary>
		/// Returns a point that is constrained to the various 45 deg angles relative to the mouse down point, but only if the
		/// Control Key is pressed.
		/// </summary>
		/// <param name="currentPoint">Point to be constrained</param>
		/// <param name="referencePoint">Reference point for constrainment. Typically this is the mouse down point</param>
		public Point ConstrainLine(Point currentPoint, Point referencePoint)
		{
			// If the Control key is not pressed, then nothing to do.
			if ((Control.ModifierKeys == Keys.None) || ((Control.ModifierKeys | _uniformConstrainKeys) != _uniformConstrainKeys))
				return currentPoint;

			Point Constrained = currentPoint;
			int X = Math.Abs(currentPoint.X - referencePoint.X);
			int Y = Math.Abs(currentPoint.Y - referencePoint.Y);
			int D = Math.Max(X, Y);
			Point[] Snaps = new Point[8];
			float[] Distance = new float[8];

			float MinDistance = float.MaxValue;
			float ThisDistance = 0f;

			for (int i = 0; i < 8; i++)
				Snaps[i] = referencePoint;

			Snaps[0].Offset(D, 0);
			Snaps[1].Offset(D, -D);
			Snaps[2].Offset(0, -D);
			Snaps[3].Offset(-D, -D);
			Snaps[4].Offset(-D, 0);
			Snaps[5].Offset(-D, D);
			Snaps[6].Offset(0, D);
			Snaps[7].Offset(D, D);

			for (int i = 0; i < 8; i++)
			{
				ThisDistance = ComputeDistance(currentPoint, Snaps[i]);
				MinDistance = Math.Min(MinDistance, ThisDistance);
				if (MinDistance == ThisDistance)
					Constrained = Snaps[i];
			}
			return Constrained;
		}

		/// <summary>
		/// Calculates the position of the point where it should lie if contraining conditions are met.
		/// </summary>
		/// <param name="currentPoint">Point to be constrained</param>
		/// <param name="referencePoint">Reference point for constrainment. Typically this is the mouse down point</param>
		/// <param name="direction">Direction to force the constraint.</param>
		public Point ConstrainPaint(Point currentPoint, Point referencePoint, ref ConstrainDirection direction)
		{
			// If the Control key is not pressed, then nothing to do.
			if ((Control.ModifierKeys == Keys.None) || ((Control.ModifierKeys | _uniformConstrainKeys) != _uniformConstrainKeys))
			{
				direction = ConstrainDirection.NotSet;
				return currentPoint;
			}

			if (currentPoint.Equals(referencePoint))
				return currentPoint;

			if (direction == ConstrainDirection.NotSet)
			{ 
				// determine a constraining direction based on the current point and the reference point
				int X = Math.Abs(currentPoint.X - referencePoint.X);
				int Y = Math.Abs(currentPoint.Y - referencePoint.Y);
				int D = Math.Max(X, Y);
				Point[] Snaps = new Point[4];
				float[] Distance = new float[4];
				float MinDistance = float.MaxValue;
				float ThisDistance = 0f;
				int Index = -1;

				for (int i = 0; i < Snaps.Length; i++)
					Snaps[i] = referencePoint;

				// Horizontal
				Snaps[0].Offset(D, 0);
				Snaps[1].Offset(-D, 0);

				// Vertical
				Snaps[2].Offset(0, D);
				Snaps[3].Offset(0, D);

				for (int i = 0; i < Snaps.Length; i++)
				{
					ThisDistance = ComputeDistance(currentPoint, Snaps[i]);
					MinDistance = Math.Min(MinDistance, ThisDistance);
					if (MinDistance == ThisDistance)
						Index = i;
				}

				if (Index < 3)
					direction = ConstrainDirection.Horizontal;
				else
					direction = ConstrainDirection.Vertical;
			}

			//WriteTraceMessage("Direction: " + direction.ToString(), TraceLevel.Info);

			if (direction == ConstrainDirection.Horizontal)
				return new Point(currentPoint.X, referencePoint.Y);
			else if (direction == ConstrainDirection.Vertical)
				return new Point(referencePoint.X, currentPoint.Y);
			else
				return currentPoint;
		}

		/// <summary>
		/// Converts an angle from Degrees to one of Radians
		/// </summary>
		/// <param name="angle">Angle value in Degrees</param>
		/// <returns>Corresponding angle in Radians</returns>
		public float DegreeToRadian(float angle)
		{
			return (float)(Math.PI * angle / 180.0);
		}

		/// <summary>
		/// Determines the size of a span of cells in pixels
		/// d(c) = d(p) / (Cs + Glw)
		/// </summary>
		/// <param name="cellLength">Length of pixels</param>
		/// <returns>Count of Cells</returns>
		public int GetCellLength(int pixelDistance)
		{
			return GetCellLength(pixelDistance, Profile);
		}

		/// <summary>
		/// Determines the size of a span of cells in pixels
		/// d(c) = d(p) / (Cs + Glw)
		/// </summary>
		/// <param name="cellLength">Length of pixels</param>
		/// <returns>Count of Cells</returns>
		public int GetCellLength(int pixelDistance, BaseProfile profile)
		{
			return (int)(GetCellLength((float)pixelDistance, profile));
		}

		/// <summary>
		/// Determines the size of a span of cells in pixels
		/// d(c) = d(p) / (Cs + Glw)
		/// </summary>
		/// <param name="cellLength">Length of pixels</param>
		/// <returns>Count of Cells</returns>
		public float GetCellLength(float pixelDistance)
		{
			return GetCellLength(pixelDistance, Profile);
		}

		/// <summary>
		/// Determines the size of a span of cells in pixels
		/// d(c) = d(p) / (Cs + Glw)
		/// </summary>
		/// <param name="cellLength">Length of pixels</param>
		/// <returns>Count of Cells</returns>
		public float GetCellLength(float pixelDistance, BaseProfile profile)
		{
			return pixelDistance / profile.Scaling.CellGridF;
		}

		/// <summary>
		/// Calculates the width and height of the background image, in current Cell Size, factoring in GridWidth
		/// </summary>
		public Size GetSizeInCells(Size pixelSize)
		{
			return GetSizeInCells(pixelSize, Profile);
		}

		/// <summary>
		/// Calculates the width and height of the background image, in current Cell Size, factoring in GridWidth
		/// </summary>
		public Size GetSizeInCells(Size pixelSize, BaseProfile profile)
		{
			if (!pixelSize.IsEmpty)
				return new Size(GetCellLength(pixelSize.Width, profile), GetCellLength(pixelSize.Height, profile));
			else
				return profile.Scaling.LatticeSize;
		}

		/// <summary>
		/// Determines the size of a span of Cells in Pixels
		/// d(p) = d(c) * z * (Cs + Glw)
		/// </summary>
		/// <param name="pixelDistance">Length of Cells</param>
		/// <returns>Count of pixels</returns>
		public float GetPixelLength(float cellLength)
		{
			return cellLength * Profile.Scaling.CellScaleF;
		}

		/// <summary>
		/// Creates the Pen object used for marquee displays
		/// </summary>
		/// <returns></returns>
		public Pen GetMarqueePen()
		{
			Pen MarqueePen = new Pen(Color.White, 1);
			MarqueePen.DashStyle = DashStyle.Custom;
			MarqueePen.DashPattern = new float[2] { 5, 5 };
			MarqueePen.DashOffset = _marqueeDashedLineOffset;

			_marqueeDashedLineOffset += 2;
			if (_marqueeDashedLineOffset > 10)
				_marqueeDashedLineOffset = 2;

			return MarqueePen;
		}

		///// <summary>
		///// Creates the filter list for the Open|Save file dialogs, used for Profiles
		///// </summary>
		//public string GetProfileFilterList()
		//{ 
		//	string Filter = "All Files (*.*)|*.*";
			
		//	if (AvailableProfiles == null)
		//		return Filter;

		//	foreach (PlugInProfile pProfile in this.AvailableProfiles)
		//		Filter = pProfile.FileFilter() + Filter;

		//	return Filter;
		//}

		/// <summary>
		/// Called right after this object is created, spawns all the internal classes, such as UISettings, ChannelController, etc.
		/// We cannot do this in the constructor itself due to the fact that a number of the object use the Instance of 
		/// Workshop, which does not exist at the time of constructing.
		/// </summary>
		public void Initialize()
		{
			if (_initialized)
				return;

			_initialized = true;
			//_traceSwitch.Level = TraceLevel.Off;

			UI = new UISettings();
			UI.LoadSettings();
			//InitializeTraceLog();

			AvailableProfiles = new PlugInProfileList();
			ProfileController = new ProfileController();
			KeyboardController = new KeyboardController();
			UserDirectory = Environment.GetEnvironmentVariable("userprofile") + @"\";
			CropArea = Rectangle.Empty;

			//this.Clipboard = new Core.Clipboard();
			//ImageStampChannel = new ImageStampChannel();
			//MoveChannel = new Channel();
		}

		/// <summary>
		/// Normalizes 2 points, so that p1 represents the upper left corner and p2 represents the lower right corner
		/// </summary>
		/// <param name="p1">First defining point in the rectangle</param>
		/// <param name="p2">Second defining point in the rectangle</param>
		public Rectangle NormalizedRectangle(Point p1, Point p2)
		{
			Rectangle rc = new Rectangle();

			// Normalize the rectangle.
			rc.X = Math.Min(p1.X, p2.X);
			rc.Y = Math.Min(p1.Y, p2.Y);
			rc.Width = Math.Abs(p2.X - p1.X);
			rc.Height = Math.Abs(p2.Y - p1.Y);

			// Why are we doing this??
			//if ((p2.X - p1.X) < 0)
			//{
			//    rc.X += 1;
			//    rc.Width -= 2;
			//}

			//if ((p2.Y - p1.Y) < 0)
			//{
			//    rc.Y += 1;
			//    rc.Height -= 2;
			//}

			if (rc.Width <= 0)
				rc.Width = 1;
			if (rc.Height <= 0)
				rc.Height = 1;
			return rc;
		}
		
		/// <summary>
		/// Find a point along an ellipse based on the angle in degrees
		/// </summary>
		/// <param name="bounds">Rectangle bounding the ellipse</param>
		/// <param name="angle">Angle in degrees</param>
		public PointF PointFromEllipse(RectangleF bounds, float angle)
		{
			float a = bounds.Width / 2.0f;
			float b = bounds.Height / 2.0f;
			float rad = DegreeToRadian(angle);

			float xCenter = (bounds.Left + bounds.Right) / 2;
			float yCenter = (bounds.Top + bounds.Bottom) / 2;

			float x = xCenter + (a * (float)Math.Cos(rad));
			float y = yCenter + (b * (float)Math.Sin(rad));

			return new PointF(x, y);
		}

		/// <summary>
		/// Converts an angle from Radians to one of Degrees
		/// </summary>
		/// <param name="angle">Angle value in Radians</param>
		/// <returns>Corresponding angle in Degrees</returns>
		public float RadianToDegree(float angle)
		{
			return (float)(angle * (180.0 / Math.PI));
		}

		#region [ Mouse Handling Methods ]

		/// <summary>
		/// Canvas Click event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public void MouseClick(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if ((CurrentTool == null) || (Profile == null))
				return;
			CurrentTool.Tool.MouseClick(buttons, latticePoint, actualCanvasPoint);
		}

		/// <summary>
		/// Canvas DoubleClick event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public void MouseDoubleClick(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if ((CurrentTool == null) || (Profile == null))
				return;
			CurrentTool.Tool.MouseDoubleClick(buttons, latticePoint, actualCanvasPoint);
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if ((CurrentTool == null) || (Profile == null))
				return;
			CurrentTool.Tool.MouseDown(buttons, latticePoint, actualCanvasPoint);
		}

		/// <summary>
		/// Canvas MouseLeave event was fired
		/// </summary>
		public void MouseLeave()
		{
			if ((CurrentTool == null) || (Profile == null))
				return;
			CurrentTool.Tool.MouseLeave();
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public void MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if ((CurrentTool == null) || (Profile == null))
				return;
			CurrentTool.Tool.MouseMove(buttons, latticePoint, actualCanvasPoint);
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public void MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if ((CurrentTool == null) || (Profile == null))
				return;
			CurrentTool.Tool.MouseUp(buttons, latticePoint, actualCanvasPoint);
		}

		#endregion [ Mouse Handling Methods ]

		#endregion [ Public Methods ]

	}
}
