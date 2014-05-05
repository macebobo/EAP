using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ElfCore
{
	public class EnumHelper
	{

		/// <summary>
		/// Returns an enumeration from the Description attribute, or the fieldname
		/// </summary>
		/// <typeparam name="T">Type to be returned</typeparam>
		/// <param name="description">Text to search for</param>
		/// <returns>Returns the Enum with this description or text, or the default value if not found</returns>
		public static T GetValueFromDescription<T>(string description)
		{
			var type = typeof(T);


			if (!type.IsEnum)
				throw new InvalidOperationException();

			foreach (var field in type.GetFields())
			{
				var attribute = Attribute.GetCustomAttribute(field,
					typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attribute != null)
				{
					if (attribute.Description == description)
						return (T)field.GetValue(null);
				}
				else
				{
					if (field.Name == description)
						return (T)field.GetValue(null);
				}
			}
			return default(T);

		}

		/// <summary>
		/// Finds the enumeration based on the numeric value
		/// </summary>
		/// <typeparam name="T">Type to be returned</typeparam>
		/// <param name="value">Numeric value</param>
		/// <returns></returns>
		public static T GetEnumFromValue<T>(int value)
		{
			try
			{
				if (System.Enum.IsDefined(typeof(T), value))
					return (T)System.Enum.ToObject(typeof(T), value);
				else
					return default(T);
			}
			catch
			{
				return default(T);
			}
		}

		/// <summary>
		/// Returns either the Description attribute from the enumeration, or the string equivalent it
		/// </summary>
		public static string GetEnumDescription(System.Enum value)
		{
			FieldInfo fi;
			DescriptionAttribute[] attributes;
			try
			{
				fi = value.GetType().GetField(value.ToString());

				attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attributes != null &&
					attributes.Length > 0)
					return attributes[0].Description;
				else
					return value.ToString();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return string.Empty;
			}
		}

		/// <summary>
		/// Returns an enumerable list of value from an enum
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetValues<T>()
		{
			return System.Enum.GetValues(typeof(T)).Cast<T>();
		} 

		///// <summary>
		///// Returns an enumerable list of value from an enum
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <returns></returns>
		//public static IEnumerable<T> GetValues<T>()
		//{
		//    return System.Enum.GetValues(typeof(T)).Cast<T>();
		//}

	}
}
