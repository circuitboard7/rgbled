using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace Circuitboard7.RgbLed
{
	public sealed partial class MainPage : Page
	{
		public static class Constants
		{
			public static class Pin
			{
				public const int RedLed = 6;
				public const int GreenLed = 5;
				public const int BlueLed = 22;
			}

			public static class Default
			{
				public const int LedValue = 50;
			}
		}

		private SoftPwm RedPin { get; set; }
		private SoftPwm GreenPin { get; set; }
		private SoftPwm BluePin { get; set; }

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
			var gpio = GpioController.GetDefault();

			// ***
			// *** Setup the three pins
			// ***
			this.RedPin = gpio.OpenPin(Constants.Pin.RedLed, GpioSharingMode.Exclusive).CreateSoftPwm().StartSoftPwm(Constants.Default.LedValue);
			this.GreenPin = gpio.OpenPin(Constants.Pin.GreenLed, GpioSharingMode.Exclusive).CreateSoftPwm().StartSoftPwm(Constants.Default.LedValue);
			this.BluePin = gpio.OpenPin(Constants.Pin.BlueLed, GpioSharingMode.Exclusive).CreateSoftPwm().StartSoftPwm(Constants.Default.LedValue);

			// ***
			// *** Initialize the sliders
			// ***
			this.InitializeSliderControl(redSlider, this.RedPin);
			this.InitializeSliderControl(greenSlider, this.GreenPin);
			this.InitializeSliderControl(blueSlider, this.BluePin);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			// ***
			// *** Stop and Dispose the SoftPwm instances
			// ***
			if (this.RedPin != null) { this.RedPin.Dispose(); }
			if (this.GreenPin != null) { this.GreenPin.Dispose(); }
			if (this.BluePin != null) { this.BluePin.Dispose(); }

			base.OnNavigatedFrom(e);
		}

		private void RedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (this.RedPin != null)
			{
				this.RedPin.Value = (int)e.NewValue;
			}
		}

		private void GreenSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (this.GreenPin != null)
			{
				this.GreenPin.Value = (int)e.NewValue;
			}
		}

		private void BlueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (this.BluePin != null)
			{
				this.BluePin.Value = (int)e.NewValue;
			}
		}

		private void InitializeSliderControl(Slider slider, SoftPwm pin)
		{
			slider.Minimum = pin.MinimumValue;
			slider.Maximum = pin.MaximumValue;
			slider.Value = pin.Value;
		}
	}
}
