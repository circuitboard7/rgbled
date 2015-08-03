// Copyright © 2015 Daniel Porrey
//
// This file is part of Variable RGB LED.
// 
// Variable RGB LED is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Variable RGB LED is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Variable RGB LED.  If not, see http://www.gnu.org/licenses/.
//
using System;
using Windows.Storage;

namespace Porrey.RgbLed
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
