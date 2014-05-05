using System.Collections.Generic;


#pragma warning disable 1591


/// <summary>
/// http://msdn.microsoft.com/en-us/magazine/cc164113.aspx
/// </summary>
public class ChannelEventArgs
{

	#region [ Private Variables ]

	#endregion [ Private Variables ]

	#region [ Properties ]

	public List<int> Channels { get; set; }

	#endregion [ Properties ]

	#region [ Constructors ]

	public ChannelEventArgs(List<int>channels)
	{
		this.Channels = channels;
	}

	#endregion [ Constructors ]
}

