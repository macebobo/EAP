using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ElfCore.Util
{
	public static class Extends
	{
		///// <summary>
		///// Clone a generic list.
		///// http://stackoverflow.com/questions/222598/how-do-i-clone-a-generic-list-in-c
		///// </summary>
		///// <typeparam name="T">Type stored in the list</typeparam>
		///// <param name="listToClone">The original list to be cloned</param>
		///// <returns>A cloned copy of the list.</returns>
		//public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		//{
		//	return listToClone.Select(item => (T)item.Clone()).ToList();
		//}

		/// <summary>
		/// http://www.codeproject.com/Questions/110762/How-to-Serialize-Point-Pen-and-Brush
		/// </summary>
		public static string SerializeObjectToXml<T>(T objectToSerialize)
		{
			//Controllers.Workshop.Instance.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			//Controllers.Workshop.Instance.WriteTraceMessage("SerializeObjectToXml: " + objectToSerialize.GetType().FullName, TraceLevel.Info);

			StringWriter outStream = new StringWriter();
			XmlSerializer s = null;
			string value = string.Empty;

			try
			{
				s = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
				s.Serialize(outStream, objectToSerialize);
				value = outStream.ToString();
			}
			catch (Exception ex)
			{
				//Controllers.Workshop.Instance.WriteTraceMessage(ex.ToString(), TraceLevel.Error);
				Debug.WriteLine(ex.ToString());
				throw;
			}
			finally
			{
				outStream.Close();
				outStream = null;
				s = null;
			}
			//Controllers.Workshop.Instance.WriteTraceMessage("END", TraceLevel.Verbose);
			return value;
		}

		public static T DeserializeObjectFromXml<T>(string xml)
		{
			T result;
			XmlSerializer s = new XmlSerializer(typeof(T));
			using (TextReader tr = new StringReader(xml))
			{
				result = (T)s.Deserialize(tr);
			}
			s = null;
			return result;
		}

		
	}

	/// <summary>
	/// http://www.codeproject.com/KB/cs/DelegateFromEvent/DelegateFromSubscription.zip
	/// </summary>
	public static class EventReporter
	{

		//public static void Main(string[] args)
		//{
		//    // Event publisher
		//    var toolStripButton = new ToolStripButton();

		//    // We want to unsubsribe from this delegate
		//    toolStripButton.Click += delegate { MessageBox.Show("I was found!"); };

		//    // Search for the delegate and make sure that it's it
		//    var handler = (EventHandler)GetDelegate(toolStripButton, "EventClick");
		//    handler.Invoke(null, null);

		//    // Unsubscribe from the delegate
		//    toolStripButton.Click -= handler;

		//    // What other event keys can we use
		//    PrintEventKeys(toolStripButton);
		//    Console.ReadLine();
		//}

		///// <summary>
		///// Prints all event keys of a control
		///// </summary>
		//public static void PrintEventKeys(Component issuer)
		//{
		//	Debug.WriteLine("Allowed event keys:");
		//	foreach (var key in GetEventKeysList(issuer))
		//	{
		//		Debug.WriteLine("- " + key);
		//	}
		//}

		///// <summary>
		///// Gets list of all event keys for a control
		///// </summary>
		//public static IEnumerable<string> GetEventKeysList(Component issuer)
		//{
		//	return
		//		from key in issuer.GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
		//		where key.Name.StartsWith("Event")
		//		select key.Name;
		//}

		/// <summary>
		/// Get delegate that listens an event of a component
		/// </summary>
		/// <param name="issuer">Component with an event</param>
		/// <param name="keyName">Key of event(<seealso cref="GetEventKeysList"/>)</param>
		/// <example>
		/// toolStripButton.Click += delegate { MessageBox.Show( "I was found!" ); };
		/// toolStripButton.Click -= (EventHandler) GetDelegate( toolStripButton, "EventClick" );
		/// </example>
		public static object GetDelegate(Component issuer, string keyName)
		{
			// Get key value for a Click Event
			// (key = internal static readonly object ToolStripItem.EventClick)
			var key = issuer
				.GetType()
				.GetField(keyName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
				.GetValue(null);

			// Get events value to get access to subscribed delegates list
			// (events = protected EventHandlerList Component.events)
			var events = typeof(Component)
				.GetField("events", BindingFlags.Instance | BindingFlags.NonPublic)
				.GetValue(issuer);

			// Find the Find method and use it to search up listEntry for corresponding key
			// (listEntry = Events.Find( key ))
			var listEntry = typeof(EventHandlerList)
				.GetMethod("Find", BindingFlags.NonPublic | BindingFlags.Instance)
				.Invoke(events, new object[] { key });

			// Get handler value from listEntry 
			// (handler = listEntry.handler )
			var handler = listEntry
				.GetType()
				.GetField("handler", BindingFlags.Instance | BindingFlags.NonPublic)
				.GetValue(listEntry);

			return handler;
		}
	}
}
