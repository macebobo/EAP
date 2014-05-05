using System;

namespace ElfCore.Util
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ElfTool : Attribute
	{
		#region [ Private Variables ]

		private string _name;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ElfTool(string name)
		{
			_name = name;
		}

		#endregion [ Constructors ]
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ElfToolGroup : Attribute
	{
		#region [ Private Variables ]

		private string _name;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ElfToolGroup(string name)
		{
			_name = name + "_ToolGroup";
		}

		#endregion [ Constructors ]
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ElfToolCore : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Class)]
	public class ElfProfile : Attribute
	{ }


}
