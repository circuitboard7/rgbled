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
using Windows.UI.Xaml.Data;

namespace Circuitboard7.RgbLed.Converters
{
	public class FrequencyToTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			string returnValue = string.Empty;

			// ***
			// *** Divide the frequency by 1 to get the length pulse length
			// *** in seconds. Divide twice for readability. The first 1,000
			// *** converts seconds to milliseconds. The second 1,000 converts
			// *** millisecond to microseconds.
			// ***
			double microseconds = (1d / System.Convert.ToDouble(value)) * 1000d * 1000d;
			returnValue = string.Format("({0:#,##0.0} μs)", microseconds);

			return returnValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
