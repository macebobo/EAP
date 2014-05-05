using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace ElfCore.Util
{
	public static class EnumHelper
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
				var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
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

		///// <summary>
		///// Returns the value from the enumeration attibute ProfileEnabled for the enumeration passed in.
		///// </summary>
		///// <param name="profileType">Enumeration value to examine.</param>
		//public static bool GetProfileEnabled(ProfileType profileType)
		//{
		//	foreach (var field in typeof(ProfileType).GetFields())
		//	{
		//		var attribute = Attribute.GetCustomAttribute(field, typeof(ProfileEnabled)) as ProfileEnabled;
		//		if (attribute != null)
		//			return attribute.Enabled;
		//	}
		//	return false;
		//}

		///// <summary>
		///// Returns the value from the enumeration attibute ProfileExtension for the enumeration passed in.
		///// </summary>
		///// <param name="profileType">Enumeration value to examine.</param>
		//public static string GetProfileExtension(ProfileType profileType)
		//{
		//	foreach (var field in typeof(ProfileType).GetFields())
		//	{
		//		var attribute = Attribute.GetCustomAttribute(field, typeof(ProfileExtension)) as ProfileExtension;
		//		if (attribute != null)
		//			return attribute.Extension;
		//	}
		//	return string.Empty;
		//}

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
				if (Enum.IsDefined(typeof(T), value))
					return (T)Enum.ToObject(typeof(T), value);
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
		public static string GetEnumDescription(Enum value)
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

		///// <summary>
		///// Returns an enumerable list of value from an enum
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <returns></returns>
		//public static IEnumerable<T> GetValues<T>()
		//{
		//	return System.Enum.GetValues(typeof(T)).Cast<T>();
		//}

		///// <summary>
		///// http://codereview.stackexchange.com/questions/5352/getting-the-value-of-a-custom-attribute-from-an-enum
		///// </summary>
		///// <typeparam name="TAttribute"></typeparam>
		///// <param name="value"></param>
		///// <returns></returns>
		//public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
		//{
		//	var type = value.GetType();
		//	var name = Enum.GetName(type, value);
		//	return type.GetField(name)
		//		.GetCustomAttributes(false);
		//		.OfType<TAttribute>()
		//		.SingleOrDefault();
		//}

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
