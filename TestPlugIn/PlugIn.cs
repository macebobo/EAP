using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Vixen;
using System.Xml;
using ElfCore.Controllers;
using ElfCore.Util;

namespace TestPlugIn
{
	public class PlugIn : IEventDrivenOutputPlugIn
    {
		private PreviewDialog m_previewDialog;
		private List<Channel> _channels;
		private SetupData _setupData;
		private XmlNode _setupNode;
		private List<Form> m_dialogList;
		private int _startChannel; // index of first channel of data range sent
		Workshop _workshop = Workshop.Instance;
		Settings _settings = Settings.Instance;
		private int _numChannels = 0;

		public PlugIn()
		{
			_settings.Style = Settings.SettingsStyle.Xml;
			_workshop.RunMode = RunMode.Standalone;
			_workshop.Initialize();

			_channels = new List<Channel>();
			m_dialogList = new List<Form>();
		}

		#region PlugIn Members

		public string Name
		{
			get { return "TestPlugIn"; }
		}

		public string Author
		{
			get { return "Rob Anderson"; }
		}

		public string Description
		{
			get { return "Testing an Output plugin for Vixen"; }
		}

		public void Setup()
		{
			if (_channels.Count == 0)
			{
				MessageBox.Show("The item you are trying to create a preview for has no channels.", "Preview", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}
			ElfCore.Forms.Editor Editor = new ElfCore.Forms.Editor(_setupNode, ElfCore.Util.ProfileType.Vixen21x);
			Editor.ShowDialog();
			Editor = null;
		}

		public void Initialize(IExecutable executableObject, SetupData setupData, XmlNode setupNode)
		{

			_channels.Clear();
			_channels.AddRange(executableObject.Channels);
			_numChannels = _channels.Count;

			_setupData = setupData;
			_setupNode = setupNode;

			// They specified in 1-base, we use 0-base
			_startChannel = Convert.ToInt32(_setupNode.Attributes["from"].Value) - 1;		
		}

		// 2.1
		public List<Form> Startup()
		{
			// 2.1
			if (_channels.Count == 0)
				return m_dialogList;

			ISystem systemInterface = (ISystem)Interfaces.Available["ISystem"];
			System.Reflection.ConstructorInfo ci = typeof(PreviewDialog).GetConstructor(new Type[] { typeof(XmlNode), typeof(List<Channel>), typeof(int) });
			m_previewDialog = (PreviewDialog)systemInterface.InstantiateForm(ci, _setupNode, _channels, _startChannel);

			// 2.1
			return m_dialogList;
		}

		//public void Startup()
		//{
		//	if (m_channels.Count == 0)
		//		return;

		//	ISystem systemInterface = (ISystem)Interfaces.Available["ISystem"];
		//	System.Reflection.ConstructorInfo ci = typeof(PreviewDialog).GetConstructor(new Type[] { typeof(XmlNode), typeof(List<Channel>), typeof(int) });
		//	m_previewDialog = (PreviewDialog)systemInterface.InstantiateForm(ci, m_setupNode, m_channels, m_startChannel);

		//}

		public void Shutdown()
		{
			if (m_previewDialog != null)
			{
				if (m_previewDialog.InvokeRequired)
				{
					m_previewDialog.Invoke((MethodInvoker)delegate()
					{
						m_previewDialog.Close();
						m_previewDialog.Dispose();
					});
				}
				else
				{
					m_previewDialog.Close();
					m_previewDialog.Dispose();
				}
				m_previewDialog = null;
				GC.Collect();
			}
			_channels.Clear();
			_setupData = null;
			_setupNode = null;
		}

		public void Event(byte[] channelValues)
		{
			if (m_previewDialog == null || m_previewDialog.Disposing || m_previewDialog.IsDisposed)
				return;

			m_previewDialog.UpdateWith(channelValues);
		}

		override public string ToString()
		{
			return Name;
		}

		#endregion

		public HardwareMap[] HardwareMap
		{
			get
			{
				return new HardwareMap[] { };   // does not use any hardware
			}
		}
    }
}
