using ElfCore.Controllers;
using ElfCore.Forms;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace ElfPreview
{
	/// <summary>
	/// Elf Preview Output PlugIn For Vixen 2.5.x
	/// </summary>
	public class PlugIn : Vixen.IEventDrivenOutputPlugIn
	{
		#region [ Private Variables ]

		private PreviewDialog _previewDialog;
		private List<Vixen.Channel> _channels;
		private Vixen.SetupData _setupData;
		private XmlNode _setupNode;
		private int _startChannel; // index of first channel of data range sent
		private string _profileFileName = string.Empty;
		Vixen.IExecutable _executable = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Does not use any hardware
		/// </summary>
		public Vixen.HardwareMap[] HardwareMap
		{
			get { return new Vixen.HardwareMap[] { }; }
		}

		/// <summary>
		/// Name of the PlugIn Author(s)
		/// </summary>
		public string Author
		{
			get { return "Rob Anderson"; }
		}

		/// <summary>
		/// Description of the PlugIn
		/// </summary>
		public string Description
		{
			get { return "Elf Preview Output PlugIn for Vixen 2.5.x"; }
		}

		/// <summary>
		/// Public Name of the PlugIn
		/// </summary>
		public string Name
		{
			get { return "Elf Preview"; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public PlugIn()
		{
			_channels = new List<Vixen.Channel>();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Called when the plug-in needs to update the hardware state during execution
		/// </summary>
		/// <param name="channelValues">Event values in channel order, 1 byte per channel.</param>
		public void Event(byte[] channelValues)
		{
			if (_previewDialog == null || _previewDialog.Disposing || _previewDialog.IsDisposed)
				return;

			_previewDialog.UpdateWith(channelValues, _executable.Name);
		}

		/// <summary>
		/// Called anytime Vixen needs to make sure the plug-in's setup or other initialization is up to date.
		/// Initialize is called before the plug-in is setup, before sequence execution, and other times.
		/// It's called from multiple places at any time, therefore the plug-in can make no assumptions
		/// about the state of the program or sequence due to a call to Initialize.
		/// </summary>
		/// <param name="executableObject">An object which provides methods and fields which represent the executable object (typically a sequence) which is executing.</param>
		/// <param name="setupData">A SetupData reference that provides some plug-incentric convenience methods. It can be safely ignored.</param>
		/// <param name="setupNode">An XmlNode representing the root of the plug-in's setup data in the sequence.</param>
		public void Initialize(Vixen.IExecutable executableObject, Vixen.SetupData setupData, XmlNode setupNode)
		{
			_executable = executableObject;
			_channels.Clear();
			_channels.AddRange(executableObject.Channels);

			_setupData = setupData;
			_setupNode = setupNode;

			try
			{
				_profileFileName = ((Vixen.Profile)executableObject).FileName as string ?? string.Empty;
			}
			catch
			{
				try
				{
					_profileFileName = ((Vixen.EventSequence)executableObject).Profile.FileName as string ?? string.Empty;
					if (_profileFileName.Length == 0)
						_profileFileName = ((Vixen.EventSequence)executableObject).Profile.Name + ".pro";
				}
				catch { }
			}

			// They specified in 1-base, we use 0-base
			_startChannel = Convert.ToInt32(_setupNode.Attributes["from"].Value) - 1;
		}

		/// <summary>
		/// Called when the user has requested to setup the plug-in instance. (Select COM Ports, etc)
		/// </summary>
		public void Setup()
		{
			Settings MySettings = Settings.Instance;
			MySettings.Style = Settings.SettingsStyle.Xml;

			Workshop MyWorkshop = Workshop.Instance;
			BaseProfile LoadingProfile = null;
			List<ElfCore.Channels.RawChannel> RawChannelList = null;

			try
			{
				if (_channels.Count == 0)
				{
					MessageBox.Show("The Profile you are trying to edit has no channels.", "Elf Preview", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return;
				}

				MyWorkshop.RunMode = RunMode.PlugIn;
				MyWorkshop.Initialize();

				// Gather up all the Vixen channel data and send it along to the Load method, in case this is a brand
				// new profile and will need this.
				RawChannelList = new List<ElfCore.Channels.RawChannel>();
				foreach (Vixen.Channel vChannel in _channels)
				{
					RawChannelList.Add(new ElfCore.Channels.RawChannel()
					{
						Name = vChannel.Name,
						SequencerColor = vChannel.Color,
						Enabled = vChannel.Enabled,
						ID = vChannel.OutputChannel
					});
				}

				LoadingProfile = new BaseProfile(typeof(ElfProfiles.Vixen.Vixen25x));
				LoadingProfile.Load(_setupNode, RawChannelList);
				LoadingProfile.Name = _executable.Name;
				LoadingProfile.Filename = _profileFileName;
				MyWorkshop.ProfileController.Add(LoadingProfile);

				using (Editor Editor = new Editor())
					Editor.ShowDialog();

				MyWorkshop.ProfileController.Remove(LoadingProfile);
			}
			catch (Exception ex)
			{
				Workshop.CrashLog(ex);
			}
			finally
			{
				RawChannelList = null;
				LoadingProfile = null;
				if (MySettings != null)
				{
					MySettings.Save();
					MySettings = null;
				}
				MyWorkshop = null;
			}
		}

		/// <summary>
		/// Called when a sequence is executed. MDI forms are now handled differently.
		/// </summary>
		public void Startup()
		{
			try
			{
				if ((_channels == null) || (_channels.Count == 0))
				{
					return;
				}

				Vixen.ISystem SystemInterface = (Vixen.ISystem)Vixen.Interfaces.Available["ISystem"];
				System.Reflection.ConstructorInfo ci = typeof(PreviewDialog).GetConstructor(new Type[] { typeof(XmlNode), typeof(List<Vixen.Channel>), typeof(int) });
				_previewDialog = (PreviewDialog)SystemInterface.InstantiateForm(ci, _setupNode, _channels, _startChannel);
			}
			catch (Exception ex)
			{
				Workshop.CrashLog(ex);
			}
		}

		/// <summary>
		/// Called when execution is stopped or the plug-in instance is no longer going to be referenced.
		/// </summary>
		public void Shutdown()
		{
			if (_previewDialog != null)
			{
				if (_previewDialog.InvokeRequired)
				{
					_previewDialog.Invoke((MethodInvoker)delegate()
					{
						_previewDialog.Close();
						_previewDialog.Dispose();
					});
				}
				else
				{
					_previewDialog.Close();
					_previewDialog.Dispose();
				}
				_previewDialog = null;
				GC.Collect();
			}
			_channels.Clear();
			_setupData = null;
			_setupNode = null;
		}

		override public string ToString()
		{
			return Name;
		}

		#endregion [ Methods ]
	}
}
