using System;

namespace ElfCore.Interfaces
{
	internal interface ISettings : IDisposable
	{
		#region [ Get Methods ]

		/// <summary>
		/// Retrieves the value using the path indicated. If the value is not present, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Value found on the path, or the defaultValue</returns>
		bool GetValue(string path, bool defaultValue);

		/// <summary>
		/// Retrieves the value using the path indicated. If the value is not present, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Value found on the path, or the defaultValue</returns>
		int GetValue(string path, int defaultValue);

		/// <summary>
		/// Retrieves the value using the path indicated. If the value is not present, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Value found on the path, or the defaultValue</returns>
		byte GetValue(string path, byte defaultValue);

		/// <summary>
		/// Retrieves the value using the path indicated. If the value is not present, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Value found on the path, or the defaultValue</returns>
		float GetValue(string path, float defaultValue);

		/// <summary>
		/// Retrieves the value using the path indicated. If the value is not present, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Value found on the path, or the defaultValue</returns>
		double GetValue(string path, double defaultValue);

		/// <summary>
		/// Retrieves the value using the path indicated. If the value is not present, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Value found on the path, or the defaultValue</returns>
		string GetValue(string path, string defaultValue);

		#endregion [ Get Methods ]

		#region [ Remove Methods ]

		/// <summary>
		/// Removes the value at the path indicated
		/// </summary>
		/// <param name="path">Path to the entry to remove</param>
		void RemoveValue(string path);

		#endregion [ Remove Methods ]

		#region [ Save Methods ]

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path to use, indicating where to save the value.</param>
		/// <param name="value">Value to save</param>
		void SetValue(string path, string value);

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path to use, indicating where to save the value.</param>
		/// <param name="value">Value to save</param>
		void SetValue(string path, int value);

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path to use, indicating where to save the value.</param>
		/// <param name="value">Value to save</param>
		void SetValue(string path, float value);

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path to use, indicating where to save the value.</param>
		/// <param name="value">Value to save</param>
		void SetValue(string path, double value);

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path to use, indicating where to save the value.</param>
		/// <param name="value">Value to save</param>
		void SetValue(string path, bool value);

		#endregion [ Save Methods ]

		void Save();
	}
}
