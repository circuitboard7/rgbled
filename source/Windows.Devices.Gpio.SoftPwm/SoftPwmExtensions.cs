using Windows.Devices.Gpio;

namespace Windows.Devices.Gpio.SoftPwm
{
	/// <summary>
	/// Fluent API Extension for SoftPwm.
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
		public static SoftPwm AssignSoftPwm(this GpioPin pin)
		{
			return new SoftPwm(pin);
		}

		/// <summary>
		/// Starts the given SoftPwm instance initializing it with THE initial value
		/// and a pulse width.
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <param name="initialValue">The initial value to set the SoftPwm instance to.</param>
		/// <param name="pulseWidth">The pulse width to use given in μs (micro-seconds).</param>
		/// <returns></returns>
		public static SoftPwm Start(this SoftPwm pwm, int initialValue = 0, double pulseWidth = 100d)
		{
			pwm.Value = initialValue;
			pwm.PulseWidth = pulseWidth;
			pwm.StartAsync();
			return pwm;
		}

		/// <summary>
		/// Sets the value of a SoftPwm instance with the given value.
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <param name="value">The value to set the SoftPwm instance to.</param>
		/// <returns></returns>
		public static SoftPwm WithValue(this SoftPwm pwm, int value)
		{
			pwm.Value = value;
			return pwm;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <param name="pulseWidth">The pulse width to use given in μs (micro-seconds).</param>
		/// <returns></returns>
		public static SoftPwm WithPulseWidth(this SoftPwm pwm, double pulseWidth)
		{
			pwm.PulseWidth = pulseWidth;
			return pwm;
		}
	}
}
