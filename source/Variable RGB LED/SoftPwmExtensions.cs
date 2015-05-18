using Windows.Devices.Gpio;

namespace Circuitboard7.RgbLed
{
	/// <summary>
	/// Extension to help create new SoftPwm instances and start them from a GPIO pin.
	/// </summary>
	public static class SoftPwmExtensions
	{
		/// <summary>
		/// Creates an instance of a SoftPwm object from the given 
		/// Windows.Devices.Gpio.GpioPin instance.
		/// </summary>
		/// <param name="pin">An instance of Windows.Devices.Gpio.GpioPin to 
		/// create the SoftPwm on.</param>
		/// <returns>Returns a new SOftPwm instance.</returns>
		public static SoftPwm CreateSoftPwm(this GpioPin pin)
		{
			return new SoftPwm(pin);
		}

		/// <summary>
		/// Starts the given SoftPwm instance initializing it with the given initial value
		/// and a pulse width.
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <param name="initialValue">The initial value to set the SoftPwm instance to.</param>
		/// <param name="pulseWidth">The pulse width to use given in μs (micro-seconds).</param>
		/// <returns></returns>
		public static SoftPwm StartSoftPwm(this SoftPwm pwm, int initialValue = 0, double pulseWidth = 100d)
		{
			pwm.Value = initialValue;
			pwm.PulseWidth = pulseWidth;
			pwm.StartAsync();
			return pwm;
		}
	}
}
