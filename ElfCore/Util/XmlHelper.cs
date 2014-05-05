using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml;

using Microsoft.VisualBasic;

namespace ElfCore.Util
{
	/// <summary>
	/// This class helps in retrieving and storing information in Xml objects
	/// </summary>
	public sealed class XmlHelper : IDisposable
	{
		#region [ Private Variables ]

		private static readonly XmlHelper _instance = new XmlHelper();

		#endregion [ Private Variables ]

		#region [ Properties ]

		public static XmlHelper Instance
		{
			get { return _instance; }
		}

		#endregion [ Properties ]

		#region [ Contructors ]

		static XmlHelper()
		{ }

		private XmlHelper()
		{ }

		#endregion [ Contructors ]

		#region [ Destructors ]

		private bool _disposed;

		public void Dispose()
		{

			// Execute the code that does the cleanup.
			Dispose(true);

			// Let the common language runtime know that Finalize doesn't have to be called.
			GC.SuppressFinalize(this);

		}

		public void Dispose(bool disposing)
		{
			// Exit if we've already cleaned up this object.
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				//  ny General Cleanup goes here
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

		/// <summary>
		/// Gets the node from the XmlDocument from the XPath passed in. If that node does not exist, create it, then return it.
		/// </summary>
		public XmlNode GetNode(XmlDocument doc, string xPath)
		{
			if (doc == null)
				return null;

			XmlNode Node = null;

			if (xPath.Length == 0)
				Node = doc.DocumentElement;
			else
				Node = doc.SelectSingleNode(xPath);

			if (Node == null)
				Node = CreateNode(doc, xPath);
			
			return Node;
		}

		#region [ GetNodeValue ]

		/// <summary>
		/// Finds the node indicated by the XPath from the document. If the node was not found and the create 
		/// flag is set, then it will create the node in the document and return it.
		/// </summary>
		/// <param name="doc">XmlDocument to search for the node within</param>
		/// <param name="xPath">XPath of the node</param>
		/// <param name="create">Indicates if the node was not found, whether it should be created.</param>
		/// <returns>The XmlNode, either found or created.</returns>
		public XmlNode GetNode(XmlDocument doc, string xPath, bool create)
		{
			if ((doc == null) || ((xPath ?? string.Empty).Length == 0))
				return null;

			XmlNode Node = doc.SelectSingleNode(xPath);
			if (Node != null)
				return Node;

			if (!create)
				return null;

			// Create the node. First, let's get the node name.
			string[] Path = xPath.Split('/');
			string NodeName = Path[Path.Length - 1];
			Node = doc.CreateElement(NodeName);

			// Find the parent node. If it doesn't exist, create it.
			xPath = xPath.Substring(0, xPath.Length - (NodeName.Length + 1));
			if (xPath.Length == 0)
			{
				// Check to see if there is a root node on the document. if not, then set the node as the root.
				if (doc.ChildNodes.Count == 0)
				{
					doc.AppendChild(Node);
					return Node;
				}
				else
					return null;
			}

			XmlNode ParentNode = GetNode(doc, xPath, true);
			if (ParentNode == null)
				return null;
			ParentNode.AppendChild(Node);
			return Node;
		}

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
		public DateTime GetNodeValue(XmlNode parentNode, string xPath, DateTime defaultValue)
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
			return GetNodeValue(parentNode, xPath, DateTime.MinValue);
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
		/// Converts the string value saved in the Xml to a byte (8-bit unsigned int). 
		/// </summary>
		public byte GetByteValue(string savedValue, byte defaultValue)
		{
			decimal DecValue = 0;
			byte IntValue = 0;

			if (Byte.TryParse(savedValue, out IntValue))
				return IntValue;
			else
			{
				if (Decimal.TryParse(savedValue, out DecValue))
					return (byte)DecValue;
			}
			return defaultValue;
		}

		/// <summary>
		/// Converts the string value saved in the Xml to an unsigned 32-bit integer. 
		/// </summary>
		public UInt32 GetUInt32Value(string savedValue, UInt32 defaultValue)
		{
			decimal DecValue = 0;
			UInt32 IntValue = 0;

			if (UInt32.TryParse(savedValue, out IntValue))
				return IntValue;
			else
			{
				if (Decimal.TryParse(savedValue, out DecValue))
					return (UInt32)DecValue;
			}
			return defaultValue;
		}

		/// <summary>
		/// Converts the string value saved in the Xml to a long integer (64bit). 
		/// </summary>
		public Int64 GetInt64Value(string savedValue, Int64 defaultValue)
		{
			decimal DecValue = 0;
			Int64 IntValue = 0;

			if (Int64.TryParse(savedValue, out IntValue))
				return IntValue;
			else
			{
				if (Decimal.TryParse(savedValue, out DecValue))
					return (Int64)DecValue;
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
		public int GetNodeValue(XmlNode parentNode, string xPath, int defaultValue)
		{
			return GetIntValue(GetNodeValue(parentNode, xPath), defaultValue);
		}

		/// <summary>
		/// Returns the value of the node at the end of the XPath from the node passed in.
		/// If no node is found, or the returned value is an empty string, return the value
		/// passed in for the default Value.
		/// </summary>
		public byte GetNodeValue(XmlNode parentNode, string xPath, byte defaultValue)
		{
			return GetByteValue(GetNodeValue(parentNode, xPath), defaultValue);
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
			return GetNodeValue(parentNode, xPath, 0);
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
		public bool GetNodeValue(XmlNode parentNode, string xPath, bool defaultValue)
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
			return GetNodeValue(parentNode, xPath, false);
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
		public decimal GetNodeValue(XmlNode parentNode, string xPath, decimal defaultValue)
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
			return GetNodeValue(parentNode, xPath, 0M);
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
		public double GetNodeValue(XmlNode parentNode, string xPath, double defaultValue)
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
			return GetNodeValue(parentNode, xPath, 0);
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
		public float GetNodeValue(XmlNode parentNode, string xPath, float defaultValue)
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
			return GetNodeValue(parentNode, xPath, 0f);
		}

		#endregion

		public Color GetNodeValue(XmlNode parentNode, string xPath, Color defaultValue)
		{
			string Value = GetNodeValue(parentNode, xPath);
			if (Value.Length == 0)
				return defaultValue;
			if (IsNumeric(Value))
				return Color.FromArgb(Convert.ToInt32(Value));
			else
				return Color.FromName(Value);
		}

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

		#region [ Remove Value ]

		/// <summary>
		/// Removes the node
		/// </summary>
		/// <param name="doc">XmlDocument to edit</param>
		/// <param name="xpath">Path to the node to remove</param>
		public void RemoveNode(XmlDocument doc, string xpath)
		{
			XmlNode Node = doc.SelectSingleNode(xpath);
			if (Node == null)
				return;
			Node.ParentNode.RemoveChild(Node);
		}

		#endregion [ Remove Value ]

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
			if ((nodeValue ?? string.Empty).Length > 0)
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
		public XmlNode SetValue(XmlNode parentNode, string nodeName, byte nodeValue)
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
		/// Sets the value passed in to the text part of the xml node. If that node does not exist, create it, then populate it
		/// </summary>
		public XmlNode SetValue(XmlNode parentNode, string nodeName, Color nodeValue)
		{
			return SetValue(parentNode, nodeName, nodeValue.ToArgb());
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
		/// Forces the creation of a new node by that name, regardless if one already exists
		/// </summary>
		public XmlNode SetNewValue(XmlNode parentNode, string nodeName, byte nodeValue)
		{
			// forces the creation of a new node by that name, regardless if one already exists
			XmlDocument doc = (XmlDocument)(parentNode.OwnerDocument == null ? parentNode : parentNode.OwnerDocument);
			XmlNode node = doc.CreateElement(nodeName);
			parentNode.AppendChild(node);
			node.InnerText = nodeValue.ToString();
			return node;
		}

		/// <summary>
		/// Forces the creation of a new node by that name, regardless if one already exists
		/// </summary>
		public XmlNode SetNewValue(XmlNode parentNode, string nodeName, float nodeValue)
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
			XmlNode Node = GetNode(doc, xPath);
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
		public XmlNode SetNodeValue(XmlDocument doc, string xPath, byte value)
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
			return value ? "True" : "False";
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
		public XmlNode CreateNode(XmlDocument doc, string xPath)
		{
			return CreateNode(doc, doc as XmlNode, xPath);
		}

		/// <summary>
		/// Create a new node on the XmlDocument based on the path passed in
		/// </summary>
		/// <param name="parentNode">Node that will own the new child node</param>
		/// <param name="xPath">XPath that contains information on creating the node. The created node may not necessarily be the direct child of parentNode</param>
		public XmlNode CreateNode(XmlNode parentNode, string xPath)
		{
			return CreateNode(parentNode.OwnerDocument, parentNode, xPath);
		}

		/// <summary>
		/// Create a new node on the XmlDocument based on the path passed in
		/// </summary>
		public XmlNode CreateNode(XmlDocument doc, XmlNode parentNode, string xPath)
		{
			// grab the next node name in the xpath; or return parent if empty 
			string[] partsOfXPath = xPath.Trim('/').Split('/');
			string nextNodeInXPath = string.Empty;

			if (partsOfXPath.Length > 0)
				nextNodeInXPath = partsOfXPath[0];
			if (string.IsNullOrEmpty(nextNodeInXPath))
				return parentNode;

			// get or create the node from the name 
			XmlNode node = parentNode.SelectSingleNode(nextNodeInXPath);
			if (node == null)
				node = parentNode.AppendChild(doc.CreateElement(nextNodeInXPath));

			// rejoin the remainder of the array as an xpath expression and recurse 
			List<string> Arr = new List<string>(partsOfXPath);
			Arr.RemoveAt(0);
			string rest = String.Join("/", Arr.ToArray());

			return CreateNode(doc, node, rest);
		}

		/// <summary>
		/// Attaches an XmlNode created by a different XmlDocument to the parentNode
		/// </summary>
		/// <param name="doc">XmlDocument that is trying to import this alien node.</param>
		/// <param name="parentNode">XmlNode that is to be the parent of this alien node.</param>
		/// <param name="alienNode">The XmlNode from another XmlDocument to import.</param>
		public void ImportNode(XmlDocument doc, XmlNode parentNode, XmlNode alienNode)
		{
			parentNode.AppendChild(doc.ImportNode(alienNode, true));
		}

		/// <summary>
		/// Attaches an XmlNode created by a different XmlDocument to the parentNode
		/// </summary>
		/// <param name="doc">XmlDocument that is trying to import this alien node.</param>
		/// <param name="parentNode">XmlNode that is to be the parent of this alien node.</param>
		/// <param name="alienDoc">XmlDocument that originally owns the node.</param>
		/// <param name="xPath">XPath of the node to be imported.</param>
		public void ImportNode(XmlDocument doc, XmlNode parentNode, XmlDocument alienDoc, string xPath)
		{
			XmlNode CopiedNode = doc.ImportNode(alienDoc.SelectSingleNode(xPath), true);
			parentNode.AppendChild(CopiedNode);
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
		public string GetAttributeValue(XmlNode node, string attributeName)
		{
			return GetAttributeValue(node, attributeName, string.Empty);
		}

		/// <summary>
		/// Checks to see if the attribute attributeName exists.
		/// If so, returns the value within, else return an empty string.
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <param name="defaultValue">Value to return if the attribute is not found</param>
		/// <returns>Attribute value</returns>
		public string GetAttributeValue(XmlNode node, string attributeName, string defaultValue)
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
		/// If so, returns the value within, else return an empty string.
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <param name="defaultValue">Value to return if the attribute is not found</param>
		/// <returns>Attribute value</returns>
		public Color GetAttributeValue(XmlNode node, string attributeName, Color defaultValue)
		{
			try
			{
				XmlNode Attr = node.Attributes.GetNamedItem(attributeName);
				if (Attr == null)
					return defaultValue;

				string Value = Attr.Value;
				if (Value.Length == 0)
					return defaultValue;
				if (IsNumeric(Value))
					return Color.FromArgb(Convert.ToInt32(Value));
				else
					return Color.FromName(Value);
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
		/// If so, returns the value within, else return an empty string.
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <param name="defaultValue">Value to return if the attribute is not found</param>
		/// <returns>Attribute value</returns>
		public bool GetAttributeValue(XmlNode node, string attributeName, bool defaultValue)
		{
			try
			{
				if (node.Attributes.GetNamedItem(attributeName) != null)
					return StringToBool(node.Attributes.GetNamedItem(attributeName).Value);
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
		public int GetAttributeValue(XmlNode node, string attributeName, int defaultValue)
		{
			if (node.Attributes.GetNamedItem(attributeName) != null)
				return GetIntValue(node.Attributes.GetNamedItem(attributeName).Value, defaultValue);
			else
				return defaultValue;
		}

		/// <summary>
		/// Checks to see if the attribute attributeName exists.
		/// If so, returns the value within, else returns the default Value
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <param name="defaultValue">Value to return if the attribute is not found</param>
		/// <returns>Attribute value</returns>
		public UInt32 GetAttributeValue(XmlNode node, string attributeName, UInt32 defaultValue)
		{
			if (node.Attributes.GetNamedItem(attributeName) != null)
				return GetUInt32Value(node.Attributes.GetNamedItem(attributeName).Value, defaultValue);
			else
				return defaultValue;
		}

		/// <summary>
		/// Checks to see if the attribute attributeName exists.
		/// If so, returns the value within, else returns the default Value
		/// </summary>
		/// <param name="node">Node to check for the attribute</param>
		/// <param name="attributeName">Name of the attribute</param>
		/// <param name="defaultValue">Value to return if the attribute is not found</param>
		/// <returns>Attribute value</returns>
		public Int64 GetAttributeValue(XmlNode node, string attributeName, Int64 defaultValue)
		{
			if (node.Attributes.GetNamedItem(attributeName) != null)
				return GetInt64Value(node.Attributes.GetNamedItem(attributeName).Value, defaultValue);
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
		public void AddAttribute(XmlNode node, string name, bool value)
		{
			XmlAttribute Attr = node.OwnerDocument.CreateAttribute(name);
			Attr.Value = value.ToString();
			node.Attributes.Append(Attr);
			Attr = null;
		}

		/// <summary>
		/// Adds an attribute to an existing XmlNode
		/// </summary>
		/// <param name="node">XmlNode object to which to add the Attribute</param>
		/// <param name="name">Name of the Attribute</param>
		/// <param name="value">Value of the Attribute</param>
		public void AddAttribute(XmlNode node, string name, int value)
		{
			XmlAttribute Attr = node.OwnerDocument.CreateAttribute(name);
			Attr.Value = value.ToString();
			node.Attributes.Append(Attr);
			Attr = null;
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

		/// <summary>
		/// Sets the value of an Attribute to an existing XmlNode. If the Attribute does not exist, it will then add it.
		/// </summary>
		/// <param name="node">XmlNode object to which to add the Attribute</param>
		/// <param name="name">Name of the Attribute</param>
		/// <param name="value">Value of the Attribute</param>
		public void SetAttribute(XmlNode node, string name, int value)
		{
			SetAttribute(node, name, value.ToString());
		}

		/// <summary>
		/// Sets the value of an Attribute to an existing XmlNode. If the Attribute does not exist, it will then add it.
		/// </summary>
		/// <param name="node">XmlNode object to which to add the Attribute</param>
		/// <param name="name">Name of the Attribute</param>
		/// <param name="value">Value of the Attribute</param>
		public void SetAttribute(XmlNode node, string name, Color value)
		{
			string ColorValue = value.ToArgb().ToString();
			if (value.IsKnownColor)
				ColorValue = value.Name;
			SetAttribute(node, name, ColorValue);
		}

		/// <summary>
		/// Sets the value of an Attribute to an existing XmlNode. If the Attribute does not exist, it will then add it.
		/// </summary>
		/// <param name="node">XmlNode object to which to add the Attribute</param>
		/// <param name="name">Name of the Attribute</param>
		/// <param name="value">Value of the Attribute</param>
		public void SetAttribute(XmlNode node, string name, bool value)
		{
			SetAttribute(node, name, value.ToString());
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

		public void WriteElement(XmlWriter writer, string name, Color value)
		{
			if (writer == null)
				return;

			string ColorValue = value.ToArgb().ToString();
			if (value.IsKnownColor)
				ColorValue = value.Name;
			writer.WriteElementString(name, ColorValue);
		}

		#region [ Static Methods ]

		public static bool IsNumeric(object Object)
		{
			return Information.IsNumeric(Object);
		}

		public static bool IsDate(object Object)
		{
			return Information.IsDate(Object);
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