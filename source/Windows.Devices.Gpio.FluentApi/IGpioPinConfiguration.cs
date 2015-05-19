namespace Windows.Devices.Gpio.FluentApi
{
	public interface IGpioPinConfiguration
	{
		GpioController Gpio { get; set; }
		int PinNumber { get; set; }
		GpioSharingMode SharingMode { get; set; }
	}
}
