
namespace ElfCore.Util
{
	/*
	public static class ProfileTypeExtensions
	{
		public static string GetName(ProfileType value)
		{
			//var attr = value.GetAttribute<ElfCore.Util.Profile>();
			//return attr.Name;
			
			FieldInfo fi;
			Profile[] attributes;
			try
			{
				fi = value.GetType().GetField(value.ToString());
				attributes = (Profile[])fi.GetCustomAttributes(typeof(Profile), false);

				if (attributes != null &&
					attributes.Length > 0)
					return attributes[0].Name;
				else
					return string.Empty;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return string.Empty;
			}
			
		}

		public static string GetExtension(ProfileType value)
		{
			//var attr = value.GetAttribute<ElfCore.Util.Profile>();
			//return attr.Extension;
			FieldInfo fi;
			Profile[] attributes;
			try
			{
				fi = value.GetType().GetField(value.ToString());
				attributes = (Profile[])fi.GetCustomAttributes(typeof(Profile), false);

				if (attributes != null &&
					attributes.Length > 0)
					return attributes[0].Extension;
				else
					return string.Empty;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return string.Empty;
			}
		}

		public static bool GetEnabled(ProfileType value)
		{
			//var attr = value.GetAttribute<ElfCore.Util.Profile>();
			//return attr.Enabled;
			FieldInfo fi;
			Profile[] attributes;
			try
			{
				fi = value.GetType().GetField(value.ToString());
				attributes = (Profile[])fi.GetCustomAttributes(typeof(Profile), false);

				if (attributes != null &&
					attributes.Length > 0)
					return attributes[0].Enabled;
				else
					return false;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return false;
			}
		}
	}
	 */ 
}
