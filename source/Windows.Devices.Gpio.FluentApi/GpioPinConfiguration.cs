namespace Windows.Devices.Gpio.FluentApi
{
	internal class GpioPinConfiguration: IGpioPinConfiguration
	{
		public GpioController Gpio { get; set; }
		public int PinNumber { get; set; }
		public GpioSharingMode SharingMode { get; set; }
	}
}
