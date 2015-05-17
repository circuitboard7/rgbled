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
				public const int LedValue = 75;
			}
		}

		private SoftPwm RedPwm { get; set; }
		private SoftPwm GreenPwm { get; set; }
		private SoftPwm BluePwm { get; set; }

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
			// *** Setup the three pins on a Soft PWM
			// ***
			this.RedPwm = gpio.OpenPin(Constants.Pin.RedLed, GpioSharingMode.Exclusive).CreateSoftPwm().StartSoftPwm(initialValue: Constants.Default.LedValue, pulseWidth: 25d);
			this.GreenPwm = gpio.OpenPin(Constants.Pin.GreenLed, GpioSharingMode.Exclusive).CreateSoftPwm().StartSoftPwm(initialValue: Constants.Default.LedValue, pulseWidth: 25d);
			this.BluePwm = gpio.OpenPin(Constants.Pin.BlueLed, GpioSharingMode.Exclusive).CreateSoftPwm().StartSoftPwm(initialValue: Constants.Default.LedValue, pulseWidth: 25d);

			// ***
			// *** Initialize the sliders
			// ***
			this.InitializeSliderControl(redSlider, this.RedPwm);
			this.InitializeSliderControl(greenSlider, this.GreenPwm);
			this.InitializeSliderControl(blueSlider, this.BluePwm);
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

		private void RedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (this.RedPwm != null)
			{
				this.RedPwm.Value = (int)e.NewValue;
			}
		}

		private void GreenSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (this.GreenPwm != null)
			{
				this.GreenPwm.Value = (int)e.NewValue;
			}
		}

		private void BlueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (this.BluePwm != null)
			{
				this.BluePwm.Value = (int)e.NewValue;
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
