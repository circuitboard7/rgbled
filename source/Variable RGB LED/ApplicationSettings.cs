using System;
using Windows.Storage;

namespace Circuitboard7.RgbLed
{
	/// <summary>
	/// Provides an interface to get and save application settings.
	/// </summary>
	public static class ApplicationSettings
	{
		/// <summary>
		/// Gets an application setting with name and type T.
		/// </summary>
		/// <typeparam name="T">The type of the application setting.</typeparam>
		/// <param name="name">The unique name or key of the application setting.</param>
		/// <param name="defaultValue">The default value to return if
		/// the setting does not have a current value.</param>
		/// <returns>Returns the application setting value or the default
		/// value if the setting does not exist.</returns>
		public static T Get<T>(string name, T defaultValue)
		{
			T returnValue = default(T);

			if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(name))
			{
				returnValue = ApplicationData.Current.RoamingSettings.Values[name].ConvertTo<T>();
			}
			else
			{
				returnValue = defaultValue;
			}

			return returnValue;
		}

		/// <summary>
		/// Saves (or creates) an application setting with name and of type T.
		/// </summary>
		/// <typeparam name="T">The type of the application setting.</typeparam>
		/// <param name="name">The unique name or key of the application setting.</param>
		/// <param name="value">The value that is saved.</param>
		public static void Save<T>(string name, T value)
		{
			ApplicationData.Current.RoamingSettings.Values[name] = value;
		}
	}
}
