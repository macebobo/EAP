using System;
using System.Collections.Generic;
using System.Text;

namespace ElfCore.PlugIn
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AdjPrevTool : Attribute
	{
		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public AdjPrevTool(string name)
		{
			_name = name;
		}

	}
}
