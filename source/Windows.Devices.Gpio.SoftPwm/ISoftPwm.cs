using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Windows.Devices.Gpio.SoftPwmSharp
{
	public interface ISoftPwm : IDisposable
	{
		GpioPin Pin { get; }
		double MaximumValue { get; set; }
		double MinimumValue { get; }
		double PulseFrequency { get; set; }
		double HighPulseWidth { get; }
		double LowPulseWidth { get; }
		double Value { get; set; }
		void StartAsync();
		Task StopAsync();
    }
}