using System;
using System.Collections.Generic;

namespace Editor
{
	public class FakeVixen : Vixen.IExecutable
	{

		private List<Vixen.Channel> _channels = new List<Vixen.Channel>();
		private string _filename = @"C:\Program Files (x86)\Vixen 2.1.1.0\Profiles\Sequencing.pro";

		#region IExecutable Members

		int Vixen.IExecutable.AudioDeviceIndex
		{
			get { return 0; }
		}

		int Vixen.IExecutable.AudioDeviceVolume
		{
			get { return 0; }
		}

		bool Vixen.IExecutable.CanBePlayed
		{
			get { return true; }
		}

		List<Vixen.Channel> Vixen.IExecutable.Channels
		{
			get { return _channels; }
		}

		public List<Vixen.Channel> Channels
		{
			set { _channels = value; }
		}

		string Vixen.IExecutable.FileName
		{
			get { return _filename; }
		}

		int Vixen.IExecutable.Key
		{
			get { return 0; }
		}

		string Vixen.IExecutable.Name
		{
			get { return string.Empty; }
		}

		List<Vixen.Channel> Vixen.IExecutable.OutputChannels
		{
			get { return new List<Vixen.Channel>(); }
		}

		Vixen.SetupData Vixen.IExecutable.PlugInData
		{
			get { throw new NotImplementedException(); }
		}

		bool Vixen.IExecutable.TreatAsLocal
		{
			get
			{
				return true;
			}
			set
			{
				
			}
		}

		object Vixen.IExecutable.UserData
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IMaskable Members

		byte[][] Vixen.IMaskable.Mask
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
