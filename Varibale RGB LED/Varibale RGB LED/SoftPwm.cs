using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using System.Threading;

namespace Circuitboard7.RgbLed
{
	public class SoftPwm : IDisposable
	{
		private int _counter = 0;

		public SoftPwm(GpioPin pin)
		{
			if (pin == null) throw new ArgumentNullException(nameof(pin));

			// ***
			// *** Set the pin up
			// ***
			this.Pin = pin;
			this.Pin.SetDriveMode(GpioPinDriveMode.Output);
			this.Pin.Write(GpioPinValue.Low);
		}

		public GpioPin Pin { get; set; } = null;
		public int MinimumValue { get; } = 0;
		public int MaximumValue { get; set; } = 100;

		/// <summary>
		/// Gets/set the width of the pulse in μs (micro-seconds).
		/// </summary>
		public double PulseWidth { get; set; } = 100d;

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private int _value = 0;

		public async void StartAsync()
		{
			this.CheckDisposed();

			while (!_cancellationTokenSource.IsCancellationRequested)
			{
				// ***
				// *** Pulse High
				// ***
				{
					if (_value != 0)
					{
						this.Pin.Write(GpioPinValue.High);
					}

					double delayMicroseconds = (double)_value * this.PulseWidth;
					await this.DelayMicroSeconds(delayMicroseconds);
				}

				// ***
				// *** Pulse Low
				// ***
				{
					int space = this.MaximumValue - _value;

					if (space != 0)
					{
						this.Pin.Write(GpioPinValue.Low);
					}

					double delayMicroseconds = (double)space * this.PulseWidth;
					await this.DelayMicroSeconds(delayMicroseconds);
				}
			}
		}

		public Task StopAsync()
		{
			this.CheckDisposed();
			_cancellationTokenSource.Cancel();
			return Task.FromResult(0);
		}

		public int Value
		{
			get
			{
				this.CheckDisposed();
				return _value;
			}
			set
			{
				this.CheckDisposed();
				_value = value;
				if (_value < this.MinimumValue)
				{
					_value = this.MinimumValue;
				}

				if (_value > this.MaximumValue)
				{
					_value = this.MaximumValue;
				}
			}
		}

		public void Dispose()
		{
			this.CheckDisposed();
			this.StopAsync();
			this.Pin.Dispose();
			this.Pin = null;
		}

		private void CheckDisposed()
		{
			if (this.Pin == null) throw new ObjectDisposedException(nameof(SoftPwm));
		}

		private async Task DelayMicroSeconds(double delayMicroseconds)
		{
			double delayInMiliseconds = delayMicroseconds / 1000d;
			await Task.Delay(TimeSpan.FromMilliseconds(delayInMiliseconds), _cancellationTokenSource.Token);
		}
	}

	public static class PwmExtensions
	{
		public static SoftPwm CreateSoftPwm(this GpioPin pin)
		{
			return new SoftPwm(pin);
		}

		public static SoftPwm StartSoftPwm(this SoftPwm pwm, int initialValue = 0, double pulseWidth = 100d)
		{
			pwm.Value = initialValue;
			pwm.PulseWidth = pulseWidth;
			pwm.StartAsync();
			return pwm;
		}
	}
}
