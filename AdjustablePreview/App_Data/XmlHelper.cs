using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace ElfCore
{
	/// <summary>
	/// This class helps in retrieving and storing information in Xml objects
	/// </summary>
	public class XmlHelper : IDisposable
	{
		#region [ Contructors ]

		[DebuggerHidden()]
		public XmlHelper()
		{
			if (_disposed)
			{
				GC.ReRegisterForFinalize(true);
			}
			_disposed = false;

			// ---------------------------------------------------------------
			// Do Not put Initialize(true) here.  That causes major problems!
			// ---------------------------------------------------------------
		}

		/// <summary>
		/// Initializes or destroys the internal objects.
		/// </summary>
		/// <param name="setObjects"></param>
		protected virtual void Initialize(bool setObjects)
		{
			if (setObjects)
			{ }
			else
			{ }
		}
		
		#endregion [ Contructors ]

		#region [ Destructors ]

		protected bool _disposed;

		public void Dispose()
		{

			// Execute the code that does the cleanup.
			Dispose(true);

			// Let the common language runtime know that Finalize doesn't have to be called.
			GC.SuppressFinalize(this);

		}

		protected virtual void Dispose(bool disposing)
		{
			// Exit if we've already cleaned up this object.
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				//  ny General Cleanup goes here
				Initialize(false);
			}
			// Remember that we've executed this code
			_disposed = true;

		}

		~XmlHelper()
		{
			//Execute the code that does the cleanup.
			Dispose(false);
		}

		#endregion [ Destructors ]

		#region [ Get Values ]

		#region [ GetNodeValue ]

		/// <summary>
		/// Returns the value of the node at the end of the Xpath from the node passed in. 
		/// If no node is found, then returns the default value
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public string GetNodeValue(XmlNode parentNode, string xPath, string defaultValue)
		{
			if (parentNode == null)
				return defaultValue;

			XmlNode Node = null;
			if (xPath.Length == 0)
				Node = parentNode;
			else
			{
				try
				{
					Node = parentNode.SelectSingleNode(xPath);
				}
				catch { }
			}
			if (Node != null)
				return Node.InnerText;
			else
				return defaultValue;
		}

		/// <summary>
		/// Returns the value of the node at the end of the Xpath from the node passed in. 
		/// If no node is found, then returns an empty string.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <returns></returns>
		public string GetNodeValue(XmlNode parentNode, string xPath)
		{
			return GetNodeValue(parentNode, xPath, string.Empty);
		}

		#endregion

		#region [ GetNodeDateTimeValue ]

		/// <summary>
		/// Returns the DateTime value of the node found by the XPath the passed in parent Node.
		/// If not found, or not a valid DateTime object, then returns the default value.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public DateTime GetNodeDateTimeValue(XmlNode parentNode, string xPath, DateTime defaultValue)
		{
			string Value = GetNodeValue(parentNode, xPath);

			if (Value.Length == 7)
			{
				// we have a month/year value
				if (Value.Length == 0)
					return DateTime.MinValue;
				return new DateTime(Convert.ToInt32(Value.Substring(0, 4)), Convert.ToInt32(Value.Substring(5, 2)), 1);
			}
			DateTime Return;
			if (DateTime.TryParse(Value, out Return))
				return Return;
			else
				return defaultValue;
		}

		/// <summary>
		/// Returns the DateTime value of the node found by the XPath the passed in parent Node.
		/// If not found, or not a valid DateTime object, then returns DateTime.MinValue.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <returns></returns>
		public DateTime GetNodeDateTimeValue(XmlNode parentNode, string xPath)
		{
			return GetNodeDateTimeValue(parentNode, xPath, DateTime.MinValue);
		}

		#endregion

		#region [ GetNodeIntValue ]

		/// <summary>
		/// Converts the string value saved in the Xml to an integer. 
		/// </summary>
		public int GetIntValue(string savedValue, int defaultValue)
		{
			decimal DecValue = 0;
			int IntValue = 0;

			if (Int32.TryParse(savedValue, out IntValue))
				return IntValue;
			else
			{
				if (Decimal.TryParse(savedValue, out DecValue))
					return (int)DecValue;
			}
			return defaultValue;
		}

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return the value
		/// passed in for the default Value.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public int GetNodeIntValue(XmlNode parentNode, string xPath, int defaultValue)
		{
			return GetIntValue(GetNodeValue(parentNode, xPath), defaultValue);
		}

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, returns
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <returns></returns>
		public int GetNodeIntValue(XmlNode parentNode, string xPath)
		{
			return GetNodeIntValue(parentNode, xPath, 0);
		}

		#endregion

		#region [ GetNodeBoolValue ]

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return the value
		/// passed in for the default Value.
		/// Returns true if the value is "Yes", or is numeric and is non-zero. Returns false
		/// if the value is "No", or is numeric and is 0, otherwise will return the value
		/// passed in as the default
		/// </summary>
		/// <param name="parentNode">XmlNode to start searching</param>
		/// <param name="xPath">XPath to use to find the node</param>
		/// <param name="defaultValue">Value to return if the node cannot be found</param>
		/// <returns></returns>
		public bool GetNodeBoolValue(XmlNode parentNode, string xPath, bool defaultValue)
		{
			string Value = GetNodeValue(parentNode, xPath);

			if (Value.Length == 0)
				return defaultValue;
			else
				return StringToBool(Value);
		}

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, returns
		/// </summary>
		/// <param name="parentNode">XmlNode to start searching</param>
		/// <param name="xPath">XPath to use to find the node</param>
		/// <returns></returns>
		public bool GetNodeBoolValue(XmlNode parentNode, string xPath)
		{
			return GetNodeBoolValue(parentNode, xPath, false);
		}

		#endregion

		#region [ GetNodeDecimalValue ]

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return the value
		/// passed in for the default Value.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public decimal GetNodeDecimalValue(XmlNode parentNode, string xPath, decimal defaultValue)
		{
			string Value = GetNodeValue(parentNode, xPath);
			if (IsNumeric(Value))
				return Convert.ToDecimal(Value);
			else
				return defaultValue;
		}

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return 0M
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <returns></returns>
		public decimal GetNodeDecimalValue(XmlNode parentNode, string xPath)
		{
			return GetNodeDecimalValue(parentNode, xPath, 0M);
		}

		#endregion

		#region [ GetNodeDoubleValue ]

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return the value
		/// passed in for the default Value.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public double GetNodeDoubleValue(XmlNode parentNode, string xPath, double defaultValue)
		{
			string Value = GetNodeValue(parentNode, xPath);
			if (IsNumeric(Value))
				return Convert.ToDouble(Value);
			else
				return defaultValue;
		}

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return 0M
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <returns></returns>
		public double GetNodeDoubleValue(XmlNode parentNode, string xPath)
		{
			return GetNodeDoubleValue(parentNode, xPath, 0);
		}

		#endregion

		#region [ GetNodeFloatValue ]

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return the value
		/// passed in for the default Value.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public float GetNodeFloatValue(XmlNode parentNode, string xPath, float defaultValue)
		{
			string Value = GetNodeValue(parentNode, xPath);
			if (IsNumeric(Value))
				return (float)Convert.ToDouble(Value);
			else
				return defaultValue;
		}

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return 0M
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xPath"></param>
		/// <returns></returns>
		public float GetNodeFloatValue(XmlNode parentNode, string xPath)
		{
			return GetNodeFloatValue(parentNode, xPath, 0f);
		}

		#endregion

		/// <summary>
		/// If the data is missing, return -1
		/// If verify is true check that the value is numeric
		/// </summary>
		/// <param name="node"></param>
		/// <param name="verify"></param>
		/// <returns></returns>
		public int GetDateNumericValue(XmlNode node, bool verify)
		{
			string Value;
			if (node != null)
			{
				if (node.InnerText.Length > 0)
				{
					Value = node.InnerText.Replace("-", string.Empty);
					if (verify)
					{
						if (IsNumeric(Value))
							return Convert.ToInt32(Value);
					}
					else
						return Convert.ToInt32(Value);
				}
			}
			return -1;
		}

		#endregion [ Get Values ]

		#region [ Set Values ]

		/// <summary>
		/// Sets the value passed in to the text part of the xml node. If that node does not exist, create it, then populate it
		/// </summary>
		public XmlNode SetValue(XmlNode parentNode, string nodeName, string nodeValue)
		{
			XmlNode node = parentNode.SelectSingleNode(nodeName);
			if (node == null)
			{
				XmlDocument doc = (XmlDocument)(parentNode.OwnerDocument == null ? parentNode : parentNode.OwnerDocument);
				node = doc.CreateElement(nodeName);
				parentNode.AppendChild(node);
			}
			node.InnerText = nodeValue;
			return node;
		}

		/// <summary>
		/// Sets the value passed in to the text part of the xml node. If that node does not exist, create it, then populate it
		/// </summary>
		public XmlNode SetValue(XmlNode parentNode, string nodeName, int nodeValue)
		{
			XmlNode node = parentNode.SelectSingleNode(nodeName);
			if (node == null)
			{
				XmlDocument doc = (XmlDocument)(parentNode.OwnerDocument == null ? parentNode : parentNode.OwnerDocument);
				node = doc.CreateElement(nodeName);
				parentNode.AppendChild(node);
			}
			node.InnerText = nodeValue.ToString();
			return node;
		}

		/// <summary>
		/// Sets the value passed in to the text part of the xml node. If that node does not exist, create it, then populate it
		/// </summary>
		public XmlNode SetValue(XmlNode parentNode, string nodeName, float nodeValue)
		{
			XmlNode node = parentNode.SelectSingleNode(nodeName);
			if (node == null)
			{
				XmlDocument doc = (XmlDocument)(parentNode.OwnerDocument == null ? parentNode : parentNode.OwnerDocument);
				node = doc.CreateElement(nodeName);
				parentNode.AppendChild(node);
			}
			node.InnerText = nodeValue.ToString();
			return node;
		}

		/// <summary>
		/// Sets the value passed in to the text part of the xml node. If that node does not exist, create it, then populate it
		/// </summary>
		public XmlNode SetValue(XmlNode parentNode, string nodeName, bool nodeValue)
		{
			XmlNode node = parentNode.SelectSingleNode(nodeName);
			if (node == null)
			{
				XmlDocument doc = (XmlDocument)(parentNode.OwnerDocument == null ? parentNode : parentNode.OwnerDocument);
				node = doc.CreateElement(nodeName);
				parentNode.AppendChild(node);
			}
			node.InnerText = nodeValue.ToString();
			return node;
		}

		/// <summary>
		/// Forces the creation of a new node by that name, regardless if one already exists
		/// </summary>
		public XmlNode SetNewValue(XmlNode parentNode, string nodeName, string nodeValue)
		{
			XmlDocument doc = (XmlDocument)(parentNode.OwnerDocument == null ? parentNode : parentNode.OwnerDocument);
			XmlNode node = doc.CreateElement(nodeName);
			parentNode.AppendChild(node);
			node.InnerText = nodeValue;
			return node;
		}

		/// <summary>
		/// Forces the creation of a new node by that name, regardless if one already exists
		/// </summary>
		public XmlNode SetNewValue(XmlNode parentNode, string nodeName, int nodeValue)
		{
			// forces the creation of a new node by that name, regardless if one already exists
			XmlDocument doc = (XmlDocument)(parentNode.OwnerDocument == null ? parentNode : parentNode.OwnerDocument);
			XmlNode node = doc.CreateElement(nodeName);
			parentNode.AppendChild(node);
			node.InnerText = nodeValue.ToString();
			return node;
		}

		/// <summary>
		/// Sets the value into a xmlNode. If the node does not exist on that path, then it will
		/// create it and populate it
		/// </summary>
		/// <param name="doc">XmlDocument to modify</param>
		/// <param name="xPath">XPath to the node</param>
		/// <param name="value">Value to write into the node</param>
		public XmlNode SetNodeValue(XmlDocument doc, string xPath, string value)
		{
			if (doc == null)
				return null;

			XmlNode Node = null;

			if (xPath.Length == 0)
				Node = doc.DocumentElement;
			else
				Node = doc.SelectSingleNode(xPath);

			if (Node == null)
				Node = MakeXPath(doc, xPath);

			Node.InnerText = value;
			return Node;
		}

		/// <summary>
		/// Sets the value into a xmlNode. If the node does not exist on that path, then it will
		/// create it and populate it
		/// </summary>
		/// <param name="doc">XmlDocument to modify</param>
		/// <param name="xPath">XPath to the node</param>
		/// <param name="value">Value to write into the node</param>
		public XmlNode SetNodeValue(XmlDocument doc, string xPath, int value)
		{
			return SetNodeValue(doc, xPath, value.ToString());
		}

		/// <summary>
		/// Sets the value into a xmlNode. If the node does not exist on that path, then it will
		/// create it and populate it
		/// </summary>
		/// <param name="doc">XmlDocument to modify</param>
		/// <param name="xPath">XPath to the node</param>
		/// <param name="value">Value to write into the node</param>
		public XmlNode SetNodeValue(XmlDocument doc, string xPath, DateTime value)
		{
			return SetNodeValue(doc, xPath, value.ToString());
		}

		/// <summary>
		/// Sets the value into a xmlNode. If the node does not exist on that path, then it will
		/// create it and populate it
		/// </summary>
		/// <param name="doc">XmlDocument to modify</param>
		/// <param name="xPath">XPath to the node</param>
		/// <param name="value">Value to write into the node</param>
		public XmlNode SetNodeValue(XmlDocument doc, string xPath, bool value)
		{
			return SetNodeValue(doc, xPath, BoolToString(value));
		}

		#endregion [ Set Values ]

		/// <summary>
		/// Converts a boolean value to a string one to be saved to the Xml
		/// </summary>
		private string BoolToString(bool value)
		{
			return value ? "1" : "0";
		}

		/// <summary>
		/// Converts a string value to its boolean equivalent. Returns true if the value is set to "1", "YES", or "TRUE"
		/// </summary>
		private bool StringToBool(string value)
		{
			return (value == "1") || (string.Compare(value, "Yes", true) == 0) || (string.Compare(value, "True", true) == 0);
		}

		/// <summary>
		/// Create a new node on the XmlDocument based on the path passed in
		/// </summary>
		public XmlNode MakeXPath(XmlDocument doc, string xPath)
		{
			return MakeXPath(doc, doc as XmlNode, xPath);
		}

		/// <summary>
		/// Create a new node on the XmlDocument based on the path passed in
		/// </summary>
		public XmlNode MakeXPath(XmlDocument doc, XmlNode parent, string xPath)
		{
			// grab the next node name in the xpath; or return parent if empty 
			string[] partsOfXPath = xPath.Trim('/').Split('/');
			string nextNodeInXPath = string.Empty;

			if (partsOfXPath.Length > 0)
				nextNodeInXPath = partsOfXPath[0];
			if (string.IsNullOrEmpty(nextNodeInXPath))
				return parent;

			// get or create the node from the name 
			XmlNode node = parent.SelectSingleNode(nextNodeInXPath);
			if (node == null)
				node = parent.AppendChild(doc.CreateElement(nextNodeInXPath));

			// rejoin the remainder of the array as an xpath expression and recurse 
			List<string> Arr = new List<string>(partsOfXPath);
			Arr.RemoveAt(0);
			string rest = String.Join("/", Arr.ToArray());

			return MakeXPath(doc, node, rest);
		}

		/// <summary>
		/// Attaches an XmlNode created by a different XmlDocument to the parentNode
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parentNode"></param>
		/// <param name="alienNode"></param>
		public void ImportNode(XmlDocument doc, XmlNode parentNode, XmlNode alienNode)
		{
			parentNode.AppendChild(doc.ImportNode(alienNode, true));
		}

		/// <summary>
		/// Attaches an XmlNode created by a different XmlDocument to the parentNode
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="xPathToParentNode">XPath to the node the alien node is to be adopted to</param>
		/// <param name="alienNode"></param>
		public void ImportNode(XmlDocument doc, string xPathToParentNode, XmlNode alienNode)
		{
			doc.SelectSingleNode(xPathToParentNode).AppendChild(doc.ImportNode(alienNode, true));
		}

		/// <summary>
		/// Attaches an XmlNode created by a different XmlDocument to the document element
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="alienNode"></param>
		public void ImportNode(XmlDocument doc, XmlNode alienNode)
		{
			ImportNode(doc, doc.DocumentElement, alienNode);
		}

		#region [ Attributes ]

		/// <summary>
		/// Checks to see if the attribute attributeName exists.
		/// If so, returns the value within, else return an empty string.
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <returns>Attribute value</returns>
		public string GetTheAttribute(XmlNode node, string attributeName)
		{
			return GetTheAttribute(node, attributeName, string.Empty);
		}

		/// <summary>
		/// Checks to see if the attribute attributeName exists.
		/// If so, returns the value within, else return an empty string.
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <param name="defaultValue">Value to return if the attribute is not found</param>
		/// <returns>Attribute value</returns>
		public string GetTheAttribute(XmlNode node, string attributeName, string defaultValue)
		{
			try
			{
				if (node.Attributes.GetNamedItem(attributeName) != null)
					return node.Attributes.GetNamedItem(attributeName).Value;
				else
					return defaultValue;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				if (node != null)
					Debug.WriteLine(node.InnerXml);
				Debug.WriteLine("Attribute Name: " + attributeName);
				return defaultValue;
			}
		}

		/// <summary>
		/// Checks to see if the attribute attributeName exists.
		/// If so, returns the value within, else returns the default Value
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <param name="defaultValue">Value to return if the attribute is not found</param>
		/// <returns>Attribute value</returns>
		public int GetTheAttribute(XmlNode node, string attributeName, int defaultValue)
		{
			if (node.Attributes.GetNamedItem(attributeName) != null)
				return GetIntValue(node.Attributes.GetNamedItem(attributeName).Value, defaultValue);
			else
				return defaultValue;
		}

		/// <summary>
		/// Checks to see if the attribute attributeName exists.
		/// If so, returns the value within, else return an empty string.
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <returns>Attribute value</returns>
		public static string GetAttribute(XmlNode node, string attributeName)
		{
			return GetAttribute(node, attributeName, string.Empty);
		}

		/// <summary>
		/// Checks to see if the attribute attributeName exists.
		/// If so, returns the value within, else return an empty string.
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <param name="defaultValue">Value to return if the attribute is not found</param>
		/// <returns>Attribute value</returns>
		public static string GetAttribute(XmlNode node, string attributeName, string defaultValue)
		{
			try
			{
				if (node.Attributes.GetNamedItem(attributeName) != null)
					return node.Attributes.GetNamedItem(attributeName).Value;
				else
					return defaultValue;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return defaultValue;
			}
		}

		/// <summary>
		/// Removes the indicated attribute from a node, if present
		/// </summary>
		/// <param name="node">XmlNode to work on</param>
		/// <param name="attributeName">Name of the attribute to remove</param>
		public void RemoveAttribute(XmlNode node, string attributeName)
		{
			try
			{
				if (node.Attributes.GetNamedItem(attributeName) != null)
					node.Attributes.RemoveNamedItem(attributeName);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// Writes the attribute to the Xml if the string is not blank
		/// </summary>
		/// <param name="writer">XmlTextWriter being employed</param>
		/// <param name="name">Name of the attribute to write</param>
		/// <param name="value">Value of the attribute to record</param>
		public void WriteAttr(XmlTextWriter writer, string name, string value)
		{
			if (writer != null)
				if (value.Length > 0)
					writer.WriteAttributeString(name, value);
		}

		/// <summary>
		/// Writes the attribute to the Xml as a numeric value
		/// </summary>
		/// <param name="writer">XmlTextWriter being employed</param>
		/// <param name="name">Name of the attribute to write</param>
		/// <param name="value">Value of the attribute to record</param>
		public void WriteAttr(XmlTextWriter writer, string name, bool value)
		{
			if (writer != null)
				writer.WriteAttributeString(name, BoolToString(value));
		}

		/// <summary>
		/// Writes the attribute to the Xml if it's a number
		/// </summary>
		/// <param name="writer">XmlTextWriter being employed</param>
		/// <param name="name">Name of the attribute to write</param>
		/// <param name="value">Value of the attribute to record</param>
		public void WriteAttr(XmlTextWriter writer, string name, double value)
		{
			if (writer != null)
				if (!double.IsNaN(value))
					writer.WriteAttributeString(name, value.ToString());
		}

		/// <summary>
		/// Writes the attribute to the Xml
		/// </summary>
		/// <param name="writer">XmlTextWriter being employed</param>
		/// <param name="name">Name of the attribute to write</param>
		/// <param name="value">Value of the attribute to record</param>
		public void WriteAttr(XmlTextWriter writer, string name, int value)
		{
			WriteAttr(writer, name, value, false);
		}

		/// <summary>
		/// Writes the attribute to the Xml
		/// </summary>
		/// <param name="writer">XmlTextWriter being employed</param>
		/// <param name="name">Name of the attribute to write</param>
		/// <param name="value">Value of the attribute to record</param>
		public void WriteAttr(XmlTextWriter writer, string name, int value, bool checkValue)
		{
			if (writer != null)
				if (!checkValue || (checkValue && (value != 0)))
					writer.WriteAttributeString(name, value.ToString());
		}

		/// <summary>
		/// Adds an attribute to an existing XmlNode
		/// </summary>
		/// <param name="node">XmlNode object to which to add the Attribute</param>
		/// <param name="name">Name of the Attribute</param>
		/// <param name="value">Value of the Attribute</param>
		public void AddAttribute(XmlNode node, string name, string value)
		{
			XmlAttribute Attr = node.OwnerDocument.CreateAttribute(name);
			Attr.Value = value;
			node.Attributes.Append(Attr);
			Attr = null;
		}

		/// <summary>
		/// Sets the value of an Attribute to an existing XmlNode. If the Attribute does not exist, it will then add it.
		/// </summary>
		/// <param name="node">XmlNode object to which to add the Attribute</param>
		/// <param name="name">Name of the Attribute</param>
		/// <param name="value">Value of the Attribute</param>
		public void SetAttribute(XmlNode node, string name, string value)
		{
			XmlAttribute Attr = node.Attributes[name];
			if (Attr == null)
				AddAttribute(node, name, value);
			else
				Attr.Value = value;

			Attr = null;
		}

		#endregion [ Attributes ]

		public void StartElement(XmlTextWriter writer, string elementName)
		{
			if (writer != null)
				writer.WriteStartElement(elementName);
		}

		public void EndElement(XmlTextWriter writer)
		{
			if (writer != null)
				writer.WriteEndElement();
		}

		#region [ Static Methods ]

		public static bool IsNumeric(object Object)
		{
			return Microsoft.VisualBasic.Information.IsNumeric(Object);
		}

		public static bool IsDate(object Object)
		{
			return Microsoft.VisualBasic.Information.IsDate(Object);
		}

		public static bool IsValidXml(string xmlFile)
		{	
			using (XmlTextReader xmlTextReader = new XmlTextReader(xmlFile))	
			{		
				try		
				{			
					while (xmlTextReader.Read());
				}		
				catch		
				{			
					return false;		
				}	
			}	
			return true;
		}

		#endregion [ Static Methods ]

	}
}