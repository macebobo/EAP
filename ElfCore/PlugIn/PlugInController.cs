using ElfCore.Controllers;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ElfCore.PlugIn
{
	internal class PlugInController
	{
		#region [ Private Static Variables ]

		/// <summary>
		/// List of all PlugInTool objects, holding Tools loaded from the found assemblies
		/// </summary>
		private static PlugInToolList _plugInToolList;

		/// <summary>
		/// List of all PlugInToolGroup objects, holding ToolGroups loaded from the found assemblies
		/// </summary>
		private static PlugInToolGroupList _plugInToolGroupList;

		/// <summary>
		/// List of all PlugInProfile objects, holding Profiles loaded from the found assemblies
		/// </summary>
		private static PlugInProfileList _plugInProfileList;

		#endregion [ Private Static Variables ]

		#region [ Internal Static Properties ]

		/// <summary>
		/// Returns the list of PlugInTools. If the list is empty, then retrieves the list of PlugInTools from found Assemblies
		/// </summary>
		internal static PlugInToolList PlugInToolList
		{
			get
			{
				if (_plugInToolList == null)
					GetAllPlugIns();
				return _plugInToolList;
			}
		}

		/// <summary>
		/// Returns the list of PlugInToolGroups. If the list is empty, then retrieves the list of PlugInToolGroups from found Assemblies
		/// </summary>
		internal static PlugInToolGroupList PlugInToolGroupList
		{
			get
			{
				if (_plugInToolGroupList == null)
					GetAllPlugIns();
				return _plugInToolGroupList;
			}
		}

		/// <summary>
		/// Returns the list of PlugInProfiles. If the list is empty, then retrieves the list of PlugInProfiles from found Assemblies
		/// </summary>
		internal static PlugInProfileList PlugInProfileList
		{
			get
			{
				if (_plugInProfileList == null)
					GetAllPlugIns();
				return _plugInProfileList;
			}
		}

		#endregion [ Internal Static Properties ]

		#region [ Private Static Methods ]

		/// <summary>
		/// Finds all the Tools and ToolGroups from this and other Assemblies.
		/// </summary>
		private static void GetAllPlugIns()
		{
			if (_plugInToolList == null)
				_plugInToolList = new PlugInToolList();
			else
				_plugInToolList.Clear();

			if (_plugInToolGroupList == null)
				_plugInToolGroupList = new PlugInToolGroupList();
			else
				_plugInToolGroupList.Clear();

			if (_plugInProfileList == null)
				_plugInProfileList = new PlugInProfileList();
			else
				_plugInProfileList.Clear();

			// Get a list of all the other Assemblies that live in the same folder as this Assembly
			List<Assembly> PlugInAssemblies = LoadPlugInAssemblies();

			// Find all the ToolGroups in the ElfTools assembly, the initial Assembly.
			foreach (IToolGroup toolGroup in GetLocalToolGroups(PlugInAssemblies))
			{
				_plugInToolGroupList.Add(new PlugInToolGroup(toolGroup, toolGroup.ID));
			}

			// Find all the Tools in the ElfTools assembly, the initial Assembly.
			foreach (ITool tool in GetLocalTools(PlugInAssemblies))
			{
				_plugInToolList.Add(new PlugInTool(tool, tool.ID));
			}
						
			// Find all the ToolGroups in the foreign Assemblies, ie Assemblies added later, for new versions, or by other developers
			int ToolGroupID = (int)Util.ToolID.PlugInToolGroup;
			foreach (IToolGroup toolGroup in GetForeignToolGroups(PlugInAssemblies))
			{
				_plugInToolGroupList.Add(new PlugInToolGroup(toolGroup, ToolGroupID++));
			}

			// Find all the Tools in the foreign Assemblies, ie Assemblies added later, for new versions, or by other developers
			int ToolID = (int)Util.ToolID.PlugIn;
			foreach (ITool tool in GetForeignTools(PlugInAssemblies))
			{
				_plugInToolList.Add(new PlugInTool(tool, ToolID++));
			}

			// Find all the Profiles in the foreign Assemblies, ie Assemblies added later, for new versions, or by other developers
			foreach (IProfile profile in GetProfiles(PlugInAssemblies))
			{
				_plugInProfileList.Add(new PlugInProfile(profile, profile.ID));
			}
			_plugInProfileList.Sort();

			// Now sort the list in ID order.
			SortedList<int, PlugInTool> Sorted = new SortedList<int, PlugInTool>();
			foreach (PlugInTool pTool in _plugInToolList)
			{
				Sorted.Add(pTool.ID, pTool);
			}

			_plugInToolList.Clear();
			foreach (KeyValuePair<int, PlugInTool> KVP in Sorted)
				_plugInToolList.Add(KVP.Value);

		}

		/// <summary>
		/// http://blogs.msdn.com/b/shawnfa/archive/2009/06/08/more-implicit-uses-of-cas-policy-loadfromremotesources.aspx
		/// Since this application only trusts a handful of LoadFrom operations,
		///	we'll put them all into the same AppDomain which is a simple sandbox
		///	with a full trust grant set.  The application itself will not enable
		///	loadFromRemoteSources, but instead Channel all of the trusted loads
		///	into this domain.
		/// </summary>
		/// <returns></returns>
		private static List<Assembly> LoadPlugInAssemblies()
		{
			string Directory = Assembly.GetEntryAssembly().ManifestModule.Assembly.Location;
			FileInfo ExecutingFile = new FileInfo(Directory);

			// File information of the currently executing assembly.
			FileInfo ThisDLL = new FileInfo(Assembly.GetExecutingAssembly().Location);

			// Directory information for the folder the executing assembly lives in.
			FileInfo[] files = ThisDLL.Directory.GetFiles("*.dll");
			List<Assembly> plugInAssemblyList = new List<Assembly>();
			Workshop Workshop = Workshop.Instance;

			if (null != files)
			{
				foreach (FileInfo file in files)
				{
					try
					{
						if (file.FullName != ThisDLL.FullName)
							plugInAssemblyList.Add(Assembly.LoadFile(file.FullName));
					}
					catch (NotSupportedException nsex)
					{
						//Workshop.WriteTraceMessage(nsex.ToString(), TraceLevel.Error);
						Debug.WriteLine(nsex.ToString());
					}
					catch (BadImageFormatException bifex)
					{
						//Workshop.WriteTraceMessage(bifex.ToString(), TraceLevel.Error);
						Debug.WriteLine(bifex.ToString());
					}
					catch (Exception ex)
					{
						//Workshop.WriteTraceMessage(ex.ToString(), TraceLevel.Error);
						Debug.WriteLine(ex.ToString());
					}
				}
			}

			ExecutingFile = null;
			files = null;
			ThisDLL = null;

			return plugInAssemblyList;
		}

		/// <summary>
		/// Retrieve the list of ToolGroups from the Assemblies that implement the ElfToolCoreAttr attribute.
		/// </summary>
		/// <param name="assemblies">List of Assemblies previously retrieved.</param>
		private static List<IToolGroup> GetLocalToolGroups(List<Assembly> assemblies)
		{
			List<Type> availableTypes = GetTypes(assemblies);

			// Get a list of objects that implement the IToolGroup interface AND 
			// have the ToolPlugInAttribute
			List<Type> ToolList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] ElfToolGroupAttr = t.GetCustomAttributes(typeof(ElfToolGroup), true);
				object[] ElfToolCoreAttr = t.GetCustomAttributes(typeof(ElfToolCore), true);

				return (!(ElfToolGroupAttr == null || ElfToolGroupAttr.Length == 0)) &&
					(!(ElfToolCoreAttr == null || ElfToolCoreAttr.Length == 0)) && 
					interfaceTypes.Contains(typeof(IToolGroup));
			});

			availableTypes = null;

			// Convert the list of Objects to an instantiated list of IToolGroups
			return ToolList.ConvertAll<IToolGroup>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as IToolGroup;
			});
		}

		/// <summary>
		/// Retrieve the list of Tools from the Assemblies that implement the ElfToolCoreAttr attribute.
		/// </summary>
		/// <param name="assemblies">List of Assemblies previously retrieved.</param>
		private static List<ITool> GetLocalTools(List<Assembly> assemblies)
		{
			List<Type> availableTypes = GetTypes(assemblies);

			// Get a list of objects that implement the ITool interface AND 
			// have the ToolPlugInAttribute
			List<Type> ToolList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] ElfToolAttr = t.GetCustomAttributes(typeof(ElfTool), true);
				object[] ElfToolCoreAttr = t.GetCustomAttributes(typeof(ElfToolCore), true);

				return (!(ElfToolAttr == null || ElfToolAttr.Length == 0)) &&
						(!(ElfToolCoreAttr == null || ElfToolCoreAttr.Length == 0)) &&
						interfaceTypes.Contains(typeof(ITool));
			});

			availableTypes = null;

			// convert the list of Objects to an instantiated list of ITools
			return ToolList.ConvertAll<ITool>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as ITool;
			});
		}

		/// <summary>
		/// Retrieve the list of Profiles from the Assemblies.
		/// </summary>
		/// <param name="assemblies">List of Assemblies previously retrieved.</param>
		private static List<IProfile> GetProfiles(List<Assembly> assemblies)
		{
			List<Type> availableTypes = GetTypes(assemblies);

			// Get a list of objects that implement the IProfile interface AND 
			// have the ElfProfile attribute.
			List<Type> ProfileList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] ElfProfileAttr = t.GetCustomAttributes(typeof(ElfProfile), true);

				return ((!(ElfProfileAttr == null || ElfProfileAttr.Length == 0)) && 
					interfaceTypes.Contains(typeof(IProfile)));
			});

			availableTypes = null;

			// convert the list of Objects to an instantiated list of IProfile
			return ProfileList.ConvertAll<IProfile>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as IProfile;
			});
		}

		/// <summary>
		/// Retrieves the list of ToolGroups from the list of Assemblies passed in.
		/// </summary>
		private static List<IToolGroup> GetForeignToolGroups(List<Assembly> assemblies)
		{
			List<Type> availableTypes = GetTypes(assemblies);

			// Get a list of objects that implement the IToolGroup interface AND 
			// have the ToolPlugInAttribute
			List<Type> toolGroupList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] ElfToolGroupAttr = t.GetCustomAttributes(typeof(ElfToolGroup), true);
				object[] ElfToolCoreAttr = t.GetCustomAttributes(typeof(ElfToolCore), true);

				return (!(ElfToolGroupAttr == null || ElfToolGroupAttr.Length == 0)) &&
						((ElfToolCoreAttr == null || ElfToolCoreAttr.Length == 0)) &&
					interfaceTypes.Contains(typeof(IToolGroup));
			});

			// Convert the list of Objects to an instantiated list of IToolGroups
			return toolGroupList.ConvertAll<IToolGroup>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as IToolGroup;
			});
		}

		/// <summary>
		/// Retrieves the list of tools from the list of Assemblies passed in.
		/// </summary>
		private static List<ITool> GetForeignTools(List<Assembly> assemblies)
		{
			List<Type> availableTypes = GetTypes(assemblies);

			// Get a list of objects that implement the ITool interface AND 
			// have the ToolPlugInAttribute
			List<Type> toolList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] ElfToolAttr = t.GetCustomAttributes(typeof(ElfTool), true);
				object[] ElfToolCoreAttr = t.GetCustomAttributes(typeof(ElfToolCore), true);
				
				return (!(ElfToolAttr == null || ElfToolAttr.Length == 0)) &&
						((ElfToolCoreAttr == null || ElfToolCoreAttr.Length == 0)) &&
						interfaceTypes.Contains(typeof(ITool));
			});

			// Convert the list of Objects to an instantiated list of ITools
			return toolList.ConvertAll<ITool>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as ITool;
			});
		}

		/// <summary>
		/// Retrieves a list of Types from all the Assemblies.
		/// </summary>
		/// <param name="assemblies">List of Assemblies.</param>
		private static List<Type> GetTypes(List<Assembly> assemblies)
		{
			List<Type> availableTypes = new List<Type>();

			foreach (Assembly item in assemblies)
			{
				try
				{
					availableTypes.AddRange(item.GetTypes());
				}
				catch (ReflectionTypeLoadException)
				{ }
				catch (Exception)
				{
					throw;
				}
			}
			return availableTypes;
		}

		#endregion [ Private Static Methods ]

		#region [ DEAD CODE ]

		///// <summary>
		///// Retrieve the list of all ToolGroups from this current Assembly
		///// </summary>
		//private static List<IToolGroup> GetLocalToolGroups()
		//{
		//	List<Type> availableTypes = new List<Type>();

		//	availableTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());

		//	// Get a list of objects that implement the IToolGroup interface AND 
		//	// have the ToolPlugInAttribute
		//	List<Type> ToolList = availableTypes.FindAll(delegate(Type t)
		//	{
		//		List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
		//		object[] arr = t.GetCustomAttributes(typeof(ElfToolGroup), true);
		//		return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(IToolGroup));
		//	});

		//	availableTypes = null;

		//	// Convert the list of Objects to an instantiated list of IToolGroups
		//	return ToolList.ConvertAll<IToolGroup>(delegate(Type t)
		//	{
		//		return Activator.CreateInstance(t) as IToolGroup;
		//	});
		//}

		///// <summary>
		///// Retrieve the list of all Tools from this current Assembly
		///// </summary>
		//private static List<ITool> GetLocalTools()
		//{
		//	List<Type> availableTypes = new List<Type>();

		//	availableTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());

		//	// Get a list of objects that implement the ITool interface AND 
		//	// have the ToolPlugInAttribute
		//	List<Type> ToolList = availableTypes.FindAll(delegate(Type t)
		//	{
		//		List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
		//		object[] arr = t.GetCustomAttributes(typeof(ElfTool), true);
		//		return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(ITool));
		//	});

		//	availableTypes = null;

		//	// convert the list of Objects to an instantiated list of ITools
		//	return ToolList.ConvertAll<ITool>(delegate(Type t)
		//	{
		//		return Activator.CreateInstance(t) as ITool;
		//	});
		//}

		#endregion [ DEAD CODE ]

	}
}
