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

		public GpioPin Pin { get; set; }
		public int MinimumValue { get; } = 0;
		public int MaximumValue { get; } = 100;

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private int _value = 0;
		private const double _frequencyMultiplier = 3d;

		public async void StartAsync()
		{
			this.CheckDisposed();

			while (true)
			{
				if (_cancellationTokenSource.IsCancellationRequested)
				{
					break;
				}

				int space = this.MaximumValue - _value;

				if (_value != 0)
				{
					this.Pin.Write(GpioPinValue.High);
				}

				int delayMicroseconds = (int)((double)_value * 100d);
				await this.DelayMicroSeconds(delayMicroseconds);

				if (space != 0)
				{
					this.Pin.Write(GpioPinValue.Low);
				}

				delayMicroseconds = (int)((double)space * 100d);
				await this.DelayMicroSeconds(delayMicroseconds);
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

		private async Task DelayMicroSeconds(int delayMicroseconds)
		{
			double delayInMiliseconds = (int)((double)delayMicroseconds / 1000d);
			int multipliedDelay = (int)(delayInMiliseconds / _frequencyMultiplier);
			await Task.Delay(multipliedDelay, _cancellationTokenSource.Token);
		}
	}

	public static class PwmExtensions
	{
		public static SoftPwm CreateSoftPwm(this GpioPin pin)
		{
			return new SoftPwm(pin);
		}

		public static SoftPwm StartSoftPwm(this SoftPwm pwm, int initialValue = 0)
		{
			pwm.Value = initialValue;
			pwm.StartAsync();
			return pwm;
		}
	}
}
