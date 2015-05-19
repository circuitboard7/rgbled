using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Gpio.SoftPwm;
using Windows.Devices.Gpio.FluentApi;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Circuitboard7.RgbLed
{
	public sealed partial class MainPage : Page
	{
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
				public const int Red = 11;
				public const int Green = 54;
				public const int Blue = 37;
				public const double PulseWidth = 25d;
			}

			public static class Setting
			{
				public const string Red = "Red";
				public const string Green = "Green";
				public const string Blue = "Blue";
				public const string PulseWdith = "PulseWdith";
			}
		}

		private ISoftPwm RedPwm { get; set; }
		private ISoftPwm GreenPwm { get; set; }
		private ISoftPwm BluePwm { get; set; }

		public MainPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			// ***
			// *** Get a reference to the GPIO Controller
			// ***
			GpioController gpio = this.GetGpioController();

			// ***
			// *** Make sure there is a GPIO Controller
			// ***
			if (gpio != null)
			{
				// ***
				// *** Get saved application settings
				// ***
				int red = ApplicationSettings.Get(Constants.Setting.Red, Constants.Default.Red);
				int green = ApplicationSettings.Get(Constants.Setting.Green, Constants.Default.Green);
				int blue = ApplicationSettings.Get(Constants.Setting.Blue, Constants.Default.Blue);
				double pulseWidth = ApplicationSettings.Get(Constants.Setting.PulseWdith, Constants.Default.PulseWidth);

				// ***
				// *** Setup the three pins on a Soft PWM
				// ***
				this.RedPwm = gpio.OnPin(Constants.Pin.Red)
										.AsExclusive()
										.Open()
										.AssignSoftPwm()
										.WithValue(red)
										.WithPulseWidth(pulseWidth)
										.Start();

				this.GreenPwm = gpio.OnPin(Constants.Pin.Green)
										.AsExclusive()
										.Open()
										.AssignSoftPwm()
										.WithValue(green)
										.WithPulseWidth(pulseWidth)
										.Start();

				this.BluePwm = gpio.OnPin(Constants.Pin.Blue)
										.AsExclusive()
										.Open()
										.AssignSoftPwm()
										.WithValue(blue)
										.WithPulseWidth(pulseWidth)
										.Start();

				// ***
				// *** Initialize the sliders
				// ***
				this.InitializeSliderControl(redSlider, this.RedPwm);
				this.InitializeSliderControl(greenSlider, this.GreenPwm);
				this.InitializeSliderControl(blueSlider, this.BluePwm);

				// ***
				// *** Set the initial value of the pulse width slider
				// ***
				pulseWidthSlider.Value = this.RedPwm.PulseWidth;
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			// ***
			// *** Stop and Dispose the SoftPwm instances
			// ***
			if (this.RedPwm != null) { this.RedPwm.Dispose(); }
			if (this.GreenPwm != null) { this.GreenPwm.Dispose(); }
			if (this.BluePwm != null) { this.BluePwm.Dispose(); }

			base.OnNavigatedFrom(e);
		}

		protected async override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			// ***
			// *** Initialize the screen color
			// ***
			await this.SetSelectedColor();
		}

		private async void RedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			try
			{
				if (this.RedPwm != null)
				{
					this.RedPwm.Value = (int)e.NewValue;
					ApplicationSettings.Save<int>(Constants.Setting.Red, this.RedPwm.Value);
				}
			}
			finally
			{
				await this.SetSelectedColor();
			}
		}

		private async void GreenSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			try
			{
				if (this.GreenPwm != null)
				{
					this.GreenPwm.Value = (int)e.NewValue;
					ApplicationSettings.Save<int>(Constants.Setting.Green, this.GreenPwm.Value);
				}
			}
			finally
			{
				await this.SetSelectedColor();
			}
		}

		private async void BlueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			try
			{
				if (this.BluePwm != null)
				{
					this.BluePwm.Value = (int)e.NewValue;
					ApplicationSettings.Save<int>(Constants.Setting.Blue, this.BluePwm.Value);
				}
			}
			finally
			{
				await this.SetSelectedColor();
			}
		}

		private void PulseWidth_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (this.RedPwm != null && this.GreenPwm != null && this.BluePwm != null)
			{
				this.RedPwm.PulseWidth = e.NewValue;
				this.GreenPwm.PulseWidth = e.NewValue;
				this.BluePwm.PulseWidth = e.NewValue;

				ApplicationSettings.Save<double>(Constants.Setting.Blue, e.NewValue);
			}
		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Exit();
		}

		private void InitializeSliderControl(Slider slider, ISoftPwm softPwm)
		{
			slider.Minimum = softPwm.MinimumValue;
			slider.Maximum = softPwm.MaximumValue;
			slider.Value = softPwm.Value;
		}

		private GpioController GetGpioController()
		{
			GpioController returnValue = null;

			try
			{
				returnValue = GpioController.GetDefault();
			}
			catch
			{
				returnValue = null;
			}

			return returnValue;
		}

		private async Task SetSelectedColor()
		{
			if (redSlider != null && greenSlider != null && blueSlider != null && selectedColor != null)
			{
				// ***
				// *** Calculate each of the three colors
				// ***
				byte r = (byte)(byte.MaxValue * (this.redSlider.Value / (this.redSlider.Maximum - this.redSlider.Minimum)));
				byte g = (byte)(byte.MaxValue * (this.greenSlider.Value / (this.greenSlider.Maximum - this.greenSlider.Minimum)));
				byte b = (byte)(byte.MaxValue * (this.blueSlider.Value / (this.blueSlider.Maximum - this.blueSlider.Minimum)));

				// ***
				// *** Create an instance of the color
				// ***
				Color color = Color.FromArgb((byte)(byte.MaxValue * .95), r, g, b);

				// ***
				// *** Create a textual representation of the color
				// ***
				string colorText = string.Format("#{0:x2}{1:x2}{2:x2}", r, g, b);

				// ***
				// *** Update the UI
				// ***
				await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
				{
					selectedColor.Background = null;
					selectedColor.Background = new SolidColorBrush(color);
					colorLabel.Text = colorText;
				}).AsTask();
			}
		}
	}
}
