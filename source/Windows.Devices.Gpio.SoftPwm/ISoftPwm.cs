using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Windows.Devices.Gpio.SoftPwm
{
	public interface ISoftPwm : IDisposable
	{
		GpioPin Pin { get; }
		int MaximumValue { get; set; }
		int MinimumValue { get; }
		double PulseWidth { get; set; }
		int Value { get; set; }
		void StartAsync();
		Task StopAsync();
	}
}