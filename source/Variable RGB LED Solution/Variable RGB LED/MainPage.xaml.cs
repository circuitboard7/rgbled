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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel;
using Windows.Devices.Gpio;
using Windows.Devices.Gpio.FluentApi;
using Windows.Devices.Gpio.SoftPwmSharp;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Porrey.RgbLed
{
	public sealed partial class MainPage : Page, INotifyPropertyChanged
	{
		/// <summary>
		/// Part of the INotifyPropertyChanged interface. Bindings use this
		/// event to monitor for changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = null;

		/// <summary>
		/// Constants used within the class.
		/// </summary>
		public static class Constants
		{
			public static class Pin
			{
				public const int Red = 6;
				public const int Green = 5;
				public const int Blue = 22;
			}

			public static class Default
			{
				public const double RedValue = 0d;
				public const double GreenValue = 0d;
				public const double BlueValue = 0d;
				public const double RedFrequency = 700d;
				public const double GreenFrequency = 1000d;
				public const double BlueFrequency = 1500d;
			}

			public static class Limit
			{
				public const double MinimumValue = 0d;
				public const double MaximumValue = 255d;
				public const double MinimumFrequency = 100d;
				public const double MaximumFrequency = 2000d;
			}

			public static class Setting
			{
				public const string RedValue = "RedValue";
				public const string GreenValue = "GreenValue";
				public const string BlueValue = "BlueValue";
				public const string RedFrequency = "RedFrequency";
				public const string GreenFrequency = "GreenFrequency";
				public const string BlueFrequency = "BlueFrequency";
			}

			public static class ColorName
			{
				public const string Red = "Red";
				public const string Green = "Green";
				public const string Blue = "Blue";
			}

			public static class Pulse
			{
				public const string Low = "LowPulseWidth";
				public const string High = "HighPulseWidth";
			}

			public static class DialogTitle
			{
				public const string Exception = "Exception";
			}
		}

		/// <summary>
		/// Gets/sets the red PWM. This will be null when there is no GPIO Controller
		/// present.
		/// </summary>
		private ISoftPwm RedPwm { get; set; }

		/// <summary>
		/// Gets/sets the green PWM. This will be null when there is no GPIO Controller
		/// present.
		/// </summary>
		private ISoftPwm GreenPwm { get; set; }

		/// <summary>
		/// Gets/sets the blue PWM. This will be null when there is no GPIO Controller
		/// present.
		/// </summary>
		private ISoftPwm BluePwm { get; set; }

		/// <summary>
		/// (Constructor) Initializes the page
		/// </summary>
		public MainPage()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Occurs when the page is navigated to
		/// </summary>
		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			try
			{
				// ***
				// *** Load the crayon colors for the combo box to read 
				// *** from (via Xaml binding).
				// ***
				await this.LoadCrayonColors();

				// ***
				// *** Check if there is a GPIO Controller
				// ***
				if (ApiInformation.IsTypePresent(typeof(GpioController).FullName))
				{
					// ***
					// *** Get a reference to the GPIO Controller
					// ***
					GpioController gpio = GpioController.GetDefault();

					// ***
					// *** Setup the three pins on a Soft PWM
					// ***
					this.RedPwm = gpio.OnPin(Constants.Pin.Red)
											.AsExclusive()
											.Open()
											.AssignSoftPwm()
											.WithValue(ApplicationSettings.Get(Constants.Setting.RedValue, Constants.Default.RedValue))
											.WithPulseFrequency(ApplicationSettings.Get(Constants.Setting.RedFrequency, Constants.Default.RedFrequency))
											.WatchPulseWidthChanges((s, args) => { this.OnFrequencyChanged(Constants.ColorName.Red); })
											.Start();

					this.GreenPwm = gpio.OnPin(Constants.Pin.Green)
											.AsExclusive()
											.Open()
											.AssignSoftPwm()
											.WithValue(ApplicationSettings.Get(Constants.Setting.GreenValue, Constants.Default.GreenValue))
											.WithPulseFrequency(ApplicationSettings.Get(Constants.Setting.GreenFrequency, Constants.Default.GreenFrequency))
											.WatchPulseWidthChanges((s, args) => { this.OnFrequencyChanged(Constants.ColorName.Green); })
											.Start();

					this.BluePwm = gpio.OnPin(Constants.Pin.Blue)
											.AsExclusive()
											.Open()
											.AssignSoftPwm()
											.WithValue(ApplicationSettings.Get(Constants.Setting.BlueValue, Constants.Default.BlueValue))
											.WithPulseFrequency(ApplicationSettings.Get(Constants.Setting.BlueFrequency, Constants.Default.BlueFrequency))
											.WatchPulseWidthChanges((s, args) => { this.OnFrequencyChanged(Constants.ColorName.Blue); })
											.Start();

					// ***
					// *** Initialize the values
					// ***
					this.RedValue = this.RedPwm.Value;
					this.GreenValue = this.GreenPwm.Value;
					this.BlueValue = this.BluePwm.Value;

					// ***
					// *** Initialize the pulse frequencies
					// ***
					this.RedPulseFrequency = this.RedPwm.PulseFrequency;
					this.GreenPulseFrequency = this.GreenPwm.PulseFrequency;
					this.BluePulseFrequency = this.BluePwm.PulseFrequency;
				}
				else
				{
					// ***
					// *** Initialize these so the user can interact with the application
					// *** even though there is no GPIO
					// ***

					// ***
					// *** Initialize the values
					// ***
					this.RedValue = ApplicationSettings.Get(Constants.Setting.RedValue, Constants.Default.RedValue);
					this.GreenValue = ApplicationSettings.Get(Constants.Setting.GreenValue, Constants.Default.GreenValue);
					this.BlueValue = ApplicationSettings.Get(Constants.Setting.BlueValue, Constants.Default.BlueValue);

					// ***
					// *** Initialize the values
					// ***
					this.RedPulseFrequency = ApplicationSettings.Get(Constants.Setting.RedFrequency, Constants.Default.RedFrequency);
					this.GreenPulseFrequency = ApplicationSettings.Get(Constants.Setting.GreenFrequency, Constants.Default.GreenFrequency);
					this.BluePulseFrequency = ApplicationSettings.Get(Constants.Setting.BlueFrequency, Constants.Default.BlueFrequency);
				}
			}
			catch (Exception ex)
			{
				MessageDialog md = new MessageDialog(ex.Message, Constants.DialogTitle.Exception);
			}
			finally
			{
				base.OnNavigatedTo(e);
			}
		}

		/// <summary>
		/// Occurs when the page is navigated away
		/// </summary>
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			try
			{
				// ***
				// *** Stop and Dispose the SoftPwm instances
				// ***
				if (this.RedPwm != null) { this.RedPwm.Dispose(); }
				if (this.GreenPwm != null) { this.GreenPwm.Dispose(); }
				if (this.BluePwm != null) { this.BluePwm.Dispose(); }
			}
			catch (Exception ex)
			{
				MessageDialog md = new MessageDialog(ex.Message, Constants.DialogTitle.Exception);
			}
			finally
			{
				base.OnNavigatedFrom(e);
			}
		}

		/// <summary>
		/// Called when the user clicks the 'Defaults' button
		/// </summary>
		private void Defaults_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				this.RedValue = Constants.Default.RedValue;
				this.RedPulseFrequency = Constants.Default.RedFrequency;

				this.GreenValue = Constants.Default.GreenValue;
				this.GreenPulseFrequency = Constants.Default.GreenFrequency;

				this.BlueValue = Constants.Default.BlueValue;
				this.BluePulseFrequency = Constants.Default.BlueFrequency;
			}
			catch (Exception ex)
			{
				MessageDialog md = new MessageDialog(ex.Message, Constants.DialogTitle.Exception);
			}
		}

		/// <summary>
		/// Called when the user clicks the 'Exit' button
		/// </summary>
		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Exit();
		}

		/// <summary>
		/// Loads the Crayon Colors into an ObservableCollection to be
		/// loaded into the ComboBox via Xaml binding.
		/// </summary>
		private async Task LoadCrayonColors()
		{
			try
			{
				// ***
				// *** Get the application installed folder
				// ***
				StorageFolder storageFolder = Package.Current.InstalledLocation;

				// ***
				// *** Get the json file
				// ***
				StorageFile storageFile = await storageFolder.GetFileAsync(@"Data\CrayonColors.json");

				// ***
				// *** Read the json text
				// ***
				string json = await FileIO.ReadTextAsync(storageFile);

				// ***
				// *** Deserialize the json
				// ***
				JArray items = (JArray)JsonConvert.DeserializeObject(json);

				// ***
				// *** Convert to IEnumerable of CrayColor objects
				// ***
				var colors = from tbl in items
							 select new CrayonColor()
							 {
								 Name = tbl["Name"].Value<string>(),
								 Hex = tbl["Hex"].Value<string>(),
								 Rgb = tbl["Rgb"].Value<string>()
							 };

				// ***
				// *** Clear the collection before loading it.
				// ***
				this.CrayonColors.Clear();

				// ***
				// *** There is no AddRange on ObservableCollection and since
				// *** there is a binding, let's not create a new one.
				// ***
				foreach (var color in colors)
				{
					this.CrayonColors.Add(color);
				}
			}
			catch (Exception ex)
			{
				MessageDialog md = new MessageDialog(ex.Message, Constants.DialogTitle.Exception);
			}
		}

		/// <summary>
		/// Called to update bindings for the current Color
		/// </summary>
		private void OnColorChanged()
		{
			this.OnPropertyChanged("SelectedColorText");
			this.OnPropertyChanged("SelectedColor");
		}

		/// <summary>
		/// Called to notify bindings that the High and Low frequency values
		/// have been updated for a specific call.
		/// </summary>
		/// <param name="colorName">The name of the color that had values change.</param>
		private void OnFrequencyChanged(string colorName)
		{
			this.OnPropertyChanged(string.Format("{0}{1}", colorName, Constants.Pulse.Low));
			this.OnPropertyChanged(string.Format("{0}{1}", colorName, Constants.Pulse.High));
		}

		#region Bindings
		/// <summary>
		/// The ComboBox uses this collection to get it's list from vis Xaml binding
		/// </summary>
		private readonly ObservableCollection<CrayonColor> _crayonColors = new ObservableCollection<CrayonColor>();
		public ObservableCollection<CrayonColor> CrayonColors
		{
			get
			{
				return _crayonColors;
			}
		}

		/// <summary>
		/// The ComboBox uses this collection to get or set the selected item in the list. This uses
		/// a two way binding so the set is called when the user selects a value from the list.
		/// </summary>
		private CrayonColor _selectedCrayonColor = null;
		public CrayonColor SelectedCrayonColor
		{
			get
			{
				return _selectedCrayonColor;
			}
			set
			{
				this.SetProperty(ref _selectedCrayonColor, value);

				this.RedValue = _selectedCrayonColor.R;
				this.GreenValue = _selectedCrayonColor.G;
				this.BlueValue = _selectedCrayonColor.B;

				this.OnColorChanged();
			}
		}

		/// <summary>
		/// Gets the hex value of the selected color.
		/// </summary>
		public string SelectedColorText
		{
			get
			{
				return string.Format("#{0:x2}{1:x2}{2:x2}", this.SelectedColor.R, this.SelectedColor.G, this.SelectedColor.B);
			}
		}

		/// <summary>
		/// Gets a Color object of the selected color.
		/// </summary>
		public Color SelectedColor
		{
			get
			{
				// ***
				// *** Create an instance of the color with
				// *** alpha at 95%
				// ***
				return Color.FromArgb((byte)(byte.MaxValue * .95),
									  (byte)this.RedValue,
									  (byte)this.GreenValue,
									  (byte)this.BlueValue);
			}
		}

		/// <summary>
		/// Gets the Minimum value used by the value sliders
		/// </summary>
		public double MinimumValue { get; } = Constants.Limit.MinimumValue;

		/// <summary>
		/// Gets the Maximum value used by the value sliders
		/// </summary>
		public double MaximumValue { get; } = Constants.Limit.MaximumValue;

		/// <summary>
		/// Gets the Minimum frequency used by the pulse sliders
		/// </summary>
		public double MinimumFrequency { get; } = Constants.Limit.MinimumFrequency;

		/// <summary>
		/// Gets the Maximum frequency used by the pulse sliders
		/// </summary>
		public double MaximumFrequency { get; } = Constants.Limit.MaximumFrequency;

		/// <summary>
		/// Gets the currently selected red color value. This value uses a two way
		/// binding to allow changes in this code to be reflected to the UI and changes
		/// in the UI to be updated here.
		/// </summary>
		private double _redValue = 0d;
		public double RedValue
		{
			get
			{
				// ***
				// *** If this is not running on a device with a GPIO then
				// *** use a local cached value instead of the actual value
				// *** from the device.
				// ***			
				return this.RedPwm == null ? _redValue : this.RedPwm.Value;
			}
			set
			{
				if (this.RedPwm != null) { this.RedPwm.Value = value; };
				ApplicationSettings.Save(Constants.Setting.RedValue, value);
				this.SetProperty(ref _redValue, value);
				this.OnColorChanged();
			}
		}

		/// <summary>
		/// Gets the currently selected red pulse frequency value. This value uses a two way
		/// binding to allow changes in this code to be reflected to the UI and changes
		/// in the UI to be updated here.
		/// </summary>
		private double _redPulseFrequency = 0d;
		public double RedPulseFrequency
		{
			get
			{
				// ***
				// *** If this is not running on a device with a GPIO then
				// *** use a local cached value instead of the actual value
				// *** from the device.
				// ***			
				return this.RedPwm == null ? _redPulseFrequency : this.RedPwm.PulseFrequency;
			}
			set
			{
				if (this.RedPwm != null) { this.RedPwm.PulseFrequency = value; };
				ApplicationSettings.Save(Constants.Setting.RedFrequency, value);
				this.SetProperty(ref _redPulseFrequency, value);
				this.OnFrequencyChanged(Constants.ColorName.Red);
			}
		}

		/// <summary>
		/// Gets the currently selected green color value. This value uses a two way
		/// binding to allow changes in this code to be reflected to the UI and changes
		/// in the UI to be updated here.
		/// </summary>
		private double _greenValue = 0d;
		public double GreenValue
		{
			get
			{
				// ***
				// *** If this is not running on a device with a GPIO then
				// *** use a local cached value instead of the actual value
				// *** from the device.
				// ***			
				return this.GreenPwm == null ? _greenValue : this.GreenPwm.Value;
			}
			set
			{
				if (this.GreenPwm != null) { this.GreenPwm.Value = value; };
				ApplicationSettings.Save(Constants.Setting.GreenValue, value);
				this.SetProperty(ref _greenValue, value);
				this.OnColorChanged();
			}
		}

		/// <summary>
		/// Gets the currently selected green pulse frequency value. This value uses a two way
		/// binding to allow changes in this code to be reflected to the UI and changes
		/// in the UI to be updated here.
		/// </summary>
		private double _greenPulseFrequency = 0d;
		public double GreenPulseFrequency
		{
			get
			{
				// ***
				// *** If this is not running on a device with a GPIO then
				// *** use a local cached value instead of the actual value
				// *** from the device.
				// ***			
				return this.GreenPwm == null ? _greenPulseFrequency : this.GreenPwm.PulseFrequency;
			}
			set
			{
				if (this.GreenPwm != null) { this.GreenPwm.PulseFrequency = value; };
				ApplicationSettings.Save(Constants.Setting.GreenFrequency, value);
				this.SetProperty(ref _greenPulseFrequency, value);
				this.OnFrequencyChanged(Constants.ColorName.Green);
			}
		}

		/// <summary>
		/// Gets the currently selected blue color value. This value uses a two way
		/// binding to allow changes in this code to be reflected to the UI and changes
		/// in the UI to be updated here.
		/// </summary>
		private double _blueValue = 0d;
		public double BlueValue
		{
			get
			{
				// ***
				// *** If this is not running on a device with a GPIO then
				// *** use a local cached value instead of the actual value
				// *** from the device.
				// ***			
				return this.BluePwm == null ? _blueValue : this.BluePwm.Value;
			}
			set
			{
				if (this.BluePwm != null) { this.BluePwm.Value = value; };
				ApplicationSettings.Save(Constants.Setting.BlueValue, value);
				this.SetProperty(ref _blueValue, value);
				this.OnColorChanged();
			}
		}

		/// <summary>
		/// Gets the currently selected blue pulse frequency value. This value uses a two way
		/// binding to allow changes in this code to be reflected to the UI and changes
		/// in the UI to be updated here.
		/// </summary>
		private double _bluePulseFrequency = 0d;
		public double BluePulseFrequency
		{
			get
			{
				// ***
				// *** If this is not running on a device with a GPIO then
				// *** use a local cached value instead of the actual value
				// *** from the device.
				// ***			
				return this.BluePwm == null ? _bluePulseFrequency : this.BluePwm.PulseFrequency;
			}
			set
			{
				if (this.BluePwm != null) { this.BluePwm.PulseFrequency = value; };
				ApplicationSettings.Save(Constants.Setting.BlueFrequency, value);
				this.SetProperty(ref _bluePulseFrequency, value);
				this.OnFrequencyChanged(Constants.ColorName.Blue);
			}
		}

		/// <summary>
		/// Gets low pulse width in microseconds for the currently selected red color.
		/// </summary>
		public double RedLowPulseWidth
		{
			get
			{
				return this.RedPwm != null ? this.RedPwm.LowPulseWidth : 0d;
			}
		}

		/// <summary>
		/// Gets high pulse width in microseconds for the currently selected red color.
		/// </summary>
		public double RedHighPulseWidth
		{
			get
			{
				return this.RedPwm != null ? this.RedPwm.HighPulseWidth : 0d;
			}
		}

		/// <summary>
		/// Gets low pulse width in microseconds for the currently selected green color.
		/// </summary>
		public double GreenLowPulseWidth
		{
			get
			{
				return this.GreenPwm != null ? this.GreenPwm.LowPulseWidth : 0d;
			}
		}

		/// <summary>
		/// Gets high pulse width in microseconds for the currently selected green color.
		/// </summary>
		public double GreenHighPulseWidth
		{
			get
			{
				return this.GreenPwm != null ? this.GreenPwm.HighPulseWidth : 0d;
			}
		}

		/// <summary>
		/// Gets low pulse width in microseconds for the currently selected blue color.
		/// </summary>
		public double BlueLowPulseWidth
		{
			get
			{
				return this.BluePwm != null ? this.BluePwm.LowPulseWidth : 0d;
			}
		}

		/// <summary>
		/// Gets high pulse width in microseconds for the currently selected blue color.
		/// </summary>
		public double BlueHighPulseWidth
		{
			get
			{
				return this.BluePwm != null ? this.BluePwm.HighPulseWidth : 0d;
			}
		}
		#endregion

		#region For INotifyPropertyChanged
		/// <summary>
		/// Checks if a property already matches a desired value. Sets the property and
		/// notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
		private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (value != null && value.Equals(storage)) return false;
			if (object.Equals(storage, value)) return false;

			storage = value;
			this.OnPropertyChanged(propertyName);

			return true;
		}

		/// <summary>
		/// Notifies listeners that a property value has changed.
		/// </summary>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers
		/// that support <see cref="CallerMemberNameAttribute"/>.</param>
		private async void OnPropertyChanged(string propertyName)
		{
			var eventHandler = this.PropertyChanged;

			if (eventHandler != null)
			{
				await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					eventHandler(this, new PropertyChangedEventArgs(propertyName));
				});
			}
		}
		#endregion
	}
}
