using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace ElfCore.PlugIn
{
	internal class ToolHostProvider
	{
		private static List<ToolHost> _toolHostList;

		internal static List<ToolHost> ToolHosts
		{
			get
			{
				if (_toolHostList == null)
					GetAllTools();

				return _toolHostList;
			}
		}

		internal static List<Assembly> LoadPlugInAssemblies()
		{
		//    // http://blogs.msdn.com/b/shawnfa/archive/2009/06/08/more-implicit-uses-of-cas-policy-loadfromremotesources.aspx

		//    // Since this application only trusts a handful of LoadFrom operations,
		//    // we'll put them all into the same AppDomain which is a simple sandbox
		//    // with a full trust grant set.  The application itself will not enable
		//    // loadFromRemoteSources, but instead Channel all of the trusted loads
		//    // into this domain.

		//    PermissionSet trustedLoadFromRemoteSourceGrantSet = new PermissionSet(PermissionState.Unrestricted);
		//    AppDomainSetup trustedLoadFromRemoteSourcesSetup = new AppDomainSetup();
		//    trustedLoadFromRemoteSourcesSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
		//    AppDomain trustedRemoteLoadDomain = AppDomain.CreateDomain("Trusted LoadFromRemoteSources Domain", null, trustedLoadFromRemoteSourcesSetup, trustedLoadFromRemoteSourceGrantSet);
		//    // Now all trusted remote LoadFroms can be done in the trustedRemoteLoadDomain,
		//    // and communicated with via a MarshalByRefObject.

		//    // Since this application trusts almost all of its assembly loads, it
		//    // is going to enable the process-wide loadFromRemoteSources switch.
		//    // However, the loads that it does not trust still need to be sandboxed. 
		//    // First figure out a grant set that the CLR considers safe to apply
		//    // to code from the Internet.
		//    Evidence sandboxEvidence = new Evidence();
		//    sandboxEvidence.AddHostEvidence(new Zone(SecurityZone.Internet));

		//    PermissionSet remoteLoadGrantSet = SecurityManager.GetStandardSandbox(sandboxEvidence);
		//    AppDomainSetup remoteLoadSetup = new AppDomainSetup();
		//    trustedLoadFromRemoteSourcesSetup.ApplicationBase = GetSandboxRoot();
		//    AppDomain remoteLoadSandbox = AppDomain.CreateDomain("Remote Load Sandbox", sandboxEvidence, remoteLoadSetup, remoteLoadGrantSet);
		//    // Now all trusted remote LoadFroms can be done in the default domain
		//    // with loadFromRemoteSources set, and untrusted loads can be done
		//    // in the sandbox that we just setup.


			FileInfo ThisDLL = new FileInfo(Assembly.GetExecutingAssembly().Location);

			DirectoryInfo dInfo = new DirectoryInfo(Environment.CurrentDirectory);
			FileInfo[] files = dInfo.GetFiles("*.dll");
			List<Assembly> plugInAssemblyList = new List<Assembly>();

			if (null != files)
			{
				foreach (FileInfo file in files)
				{
					try
					{
						if (file.FullName != ThisDLL.FullName)
							plugInAssemblyList.Add(Assembly.LoadFile(file.FullName));
					}
					catch (NotSupportedException)
					{ }
				}
			}

			dInfo = null;
			files = null;
			ThisDLL = null;

			return plugInAssemblyList;
		}

		internal static List<IToolGroup> GetLocalToolGroups()
		{
			List<Type> availableTypes = new List<Type>();

			availableTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());

			// Get a list of objects that implement the IToolGroup interface AND 
			// have the ToolPlugInAttribute
			List<Type> ToolList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] arr = t.GetCustomAttributes(typeof(AdjPrevTool), true);
				return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(IToolGroup));
			});

			availableTypes = null;

			// convert the list of Objects to an instantiated list of IToolGroups
			return ToolList.ConvertAll<IToolGroup>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as IToolGroup;
			});
		}

		internal static List<ITool> GetLocalTools()
		{
			List<Type> availableTypes = new List<Type>();

			availableTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());

			// Get a list of objects that implement the ITool interface AND 
			// have the ToolPlugInAttribute
			List<Type> ToolList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] arr = t.GetCustomAttributes(typeof(AdjPrevTool), true);
				return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(ITool));
			});

			availableTypes = null;

			// convert the list of Objects to an instantiated list of ITools
			return ToolList.ConvertAll<ITool>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as ITool;
			});
		}

		internal static List<ITool> GetPlugInTools(List<Assembly> assemblies)
		{
			List<Type> availableTypes = new List<Type>();

			foreach (Assembly currentAssembly in assemblies)
				availableTypes.AddRange(currentAssembly.GetTypes());

			// Get a list of objects that implement the ITool interface AND 
			// have the ToolPlugInAttribute
			List<Type> toolList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] arr = t.GetCustomAttributes(typeof(AdjPrevTool), true);
				return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(ITool));
			});

			// convert the list of Objects to an instantiated list of ITools
			return toolList.ConvertAll<ITool>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as ITool;
			});
		}

		internal static List<IToolGroup> GetPlugInToolGroups(List<Assembly> assemblies)
		{
			List<Type> availableTypes = new List<Type>();

			foreach (Assembly currentAssembly in assemblies)
				availableTypes.AddRange(currentAssembly.GetTypes());

			// Get a list of objects that implement the IToolGroup interface AND 
			// have the ToolPlugInAttribute
			List<Type> toolList = availableTypes.FindAll(delegate(Type t)
			{
				List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
				object[] arr = t.GetCustomAttributes(typeof(AdjPrevTool), true);
				return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(IToolGroup));
			});

			// convert the list of Objects to an instantiated list of IToolGroups
			return toolList.ConvertAll<IToolGroup>(delegate(Type t)
			{
				return Activator.CreateInstance(t) as IToolGroup;
			});
		}

		internal static void GetAllTools()
		{
			if (null == _toolHostList)
				_toolHostList = new List<ToolHost>();
			else
				_toolHostList.Clear();

			// Find all the ToolGroups in this Assembly
			foreach (IToolGroup toolGroup in GetLocalToolGroups())
			{
				_toolHostList.Add(new ToolHost(toolGroup, toolGroup.ID));
			}

			// Find all the Tools in this Assembly
			foreach (ITool tool in GetLocalTools())
			{
				_toolHostList.Add(new ToolHost(tool, tool.ID));
			}

			// Get a list of all the other Assemblies that live in the same folder as this Assembly
			List<Assembly> PlugInAssemblies = LoadPlugInAssemblies();

			// Find all the ToolGroups in the foreign Assemblies
			int ToolID = (int)Tool.PlugInToolGroup;
			foreach (ITool tool in GetPlugInToolGroups(PlugInAssemblies))
			{
				_toolHostList.Add(new ToolHost(tool, ToolID++));
			}

			// Find all the Tools in the foreign Assemblies
			ToolID = (int)Tool.PlugIn;
			foreach (ITool tool in GetPlugInTools(PlugInAssemblies))
			{
				_toolHostList.Add(new ToolHost(tool, ToolID++));
			}

			// Now sort the list in ID order.
			SortedList<int, ToolHost> Sorted = new SortedList<int, ToolHost>();
			foreach (ToolHost toolHost in _toolHostList)
			{
				Sorted.Add(toolHost.ID, toolHost);
			}
			
			_toolHostList.Clear();
			foreach (KeyValuePair<int, ToolHost> KVP in Sorted)
				_toolHostList.Add(KVP.Value);

		}

	}
}
