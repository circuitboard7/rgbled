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

namespace Porrey.RgbLed
{
	/// <summary>
	/// Represents a predefine crayon color.
	/// </summary>
	public class CrayonColor
	{
		/// <summary>
		/// Gets/sets the name of the color.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the hexadecimal color value.
		/// </summary>
		public string Hex { get; set; }

		/// <summary>
		/// Gets/sets the RGB color in string form.
		/// </summary>
		private string _rgb = string.Empty;
		public string Rgb
		{
			get
			{
				return _rgb;
			}
			set
			{
				_rgb = value;

				if (_rgb.Contains(","))
				{
					this.Initialize(_rgb);
				}
			}
		}

		/// <summary>
		/// Gets the red color value.
		/// </summary>
		public byte R { get; private set; }

		/// <summary>
		/// Gets the green color value.
		/// </summary>
		public byte G { get; private set; }

		/// <summary>
		/// Gets the blue color value.
		/// </summary>
		public byte B { get; private set; }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="rgb"></param>
		private void Initialize(string rgb)
		{
			string[] colorParts = rgb.Split(new char[] { ',' });
			this.R = Convert.ToByte(colorParts[0]);
			this.G = Convert.ToByte(colorParts[1]);
			this.B = Convert.ToByte(colorParts[2]);
		}

		/// <summary>
		/// Gets a string representation of this instance suitable for display.
		/// </summary>
		public string NameDisplay
		{
			get
			{
				return string.Format("{0} ({1})", this.Name, this.Hex);
			}
		}

		/// <summary>
		/// Gets the default string shown for this instance.
		/// </summary>
		public override string ToString()
		{
			return this.NameDisplay;
		}
	}
}
