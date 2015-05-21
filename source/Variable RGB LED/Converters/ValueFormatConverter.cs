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
	public class ValueFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			string format = (parameter as string).Replace("[", "{").Replace("]", "}");
            return string.Format(format, value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
