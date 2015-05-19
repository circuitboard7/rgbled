namespace Windows.Devices.Gpio.FluentApi
{
	/// <summary>
	/// Provides a set of extension methods to create a fluent API for
	/// the GPIO pin.
	/// </summary>
	public static class GpioFluentApiExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gpio">The GPIO Controller.</param>
		/// <param name="pinNumber">The pin number on the controller to assign.</param>
		/// <returns>Returns an instance of Windows.Devices.Gpio.IGpioPinConfiguration
		/// used to open the pin on the controller.</returns>
		public static IGpioPinConfiguration OnPin(this GpioController gpio, int pinNumber)
		{
			return new GpioPinConfiguration()
			{
				Gpio = gpio,
				PinNumber = pinNumber,
				SharingMode = GpioSharingMode.SharedReadOnly
			};
        }

		/// <summary>
		/// Specifies that the pin on the controller should be opened in exclusive mode.
		/// </summary>
		/// <param name="configuration">An instance of Windows.Devices.Gpio.IGpioPinConfiguration
		/// containing the current configuration used top open the pin on the controller.</param>
		/// <returns>Returns an instance of Windows.Devices.Gpio.IGpioPinConfiguration
		/// used to open the pin on the controller.</returns>
		public static IGpioPinConfiguration AsExclusive(this IGpioPinConfiguration configuration)
		{
			configuration.SharingMode = GpioSharingMode.Exclusive;
            return configuration;
        }

		/// <summary>
		/// Specifies that the pin on the controller should be opened in shared read-only mode.
		/// </summary>
		/// <param name="configuration">An instance of Windows.Devices.Gpio.IGpioPinConfiguration
		/// containing the current configuration used top open the pin on the controller.</param>
		/// <returns>Returns an instance of Windows.Devices.Gpio.IGpioPinConfiguration
		/// used to open the pin on the controller.</returns>
		public static IGpioPinConfiguration AsSharedReadOnly(this IGpioPinConfiguration configuration)
		{
			configuration.SharingMode = GpioSharingMode.SharedReadOnly;
			return configuration;
		}

		/// <summary>
		/// Opens the pin on the controller using the configuration provided.
		/// </summary>
		/// <param name="configuration">An instance of Windows.Devices.Gpio.IGpioPinConfiguration
		/// containing the current configuration used top open the pin on the controller.</param>
		/// <returns>Returns an instance of Windows.Devices.Gpio.GpioPin if it is successfully
		/// opened.</returns>
		public static GpioPin Open(this IGpioPinConfiguration configuration)
		{
			return configuration.Gpio.OpenPin(configuration.PinNumber, configuration.SharingMode);
		}
	}
}
