using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Circuitboard7.RgbLed
{
	/// <summary>
	/// Provides a software based Pulse Width Modulation capability for any GPIO pin on
	/// the device. PWM is used in a variety of circuits as a way to control analog 
	/// circuits through digital interfaces.
	/// </summary>
	public class SoftPwm : IDisposable
	{
		private int _counter = 0;
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private int _value = 0;

		/// <summary>
		/// Creates an instance of SoftPwm given an instance
		/// of Windows.Devices.Gpio.GpioPin.
		/// </summary>
		/// <param name="pin">An instance of Windows.Devices.Gpio.GpioPin to create the SoftPwm on.</param>
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

		/// <summary>
		/// Gets the underlying Windows.Devices.Gpio.GpioPin instance that this SoftPwm instance
		/// is controlling.
		/// </summary>
		public GpioPin Pin { get; protected set; } = null;

		/// <summary>
		/// Gets the minimum value that can be set.
		/// </summary>
		public int MinimumValue { get; } = 0;

		/// <summary>
		/// Gets/sets the maximum value allowed.
		/// </summary>
		public int MaximumValue { get; set; } = 100;

		/// <summary>
		/// Gets/set the width of the pulse in μs (micro-seconds).
		/// </summary>
		public double PulseWidth { get; set; } = 100d;

		/// <summary>
		/// Start the SoftPwm in the GPIO pin.
		/// </summary>
		public async void StartAsync()
		{
			this.CheckDisposed();

			while (!_cancellationTokenSource.IsCancellationRequested)
			{
				// ***
				// *** Pulse High
				// ***
				{
					if (_value != this.MinimumValue)
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

					if (space != this.MinimumValue)
					{
						this.Pin.Write(GpioPinValue.Low);
					}

					double delayMicroseconds = (double)space * this.PulseWidth;
					await this.DelayMicroSeconds(delayMicroseconds);
				}
			}
		}

		/// <summary>
		/// Stop the SoftPwm on the GPIO pin.
		/// </summary>
		/// <returns></returns>
		public Task StopAsync()
		{
			this.CheckDisposed();
			_cancellationTokenSource.Cancel();
			return Task.FromResult(0);
		}

		/// <summary>
		/// Gets/sets the current value.
		/// </summary>
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

		/// <summary>
		/// Stops the SoftPwm if active and calls Dispose on the GPIO pin.
		/// </summary>
		public void Dispose()
		{
			this.CheckDisposed();
			this.StopAsync();
			this.Pin.Dispose();
			this.Pin = null;
		}

		/// <summary>
		/// Checks if this instance has been disposed and 
		/// throws the ObjectDisposedException exception if it is.
		/// </summary>
		private void CheckDisposed()
		{
			if (this.Pin == null) throw new ObjectDisposedException(nameof(SoftPwm));
		}

		/// <summary>
		/// Delays the current thread by the given number of μs.
		/// </summary>
		/// <param name="delayMicroseconds">The number of μs to delay the thread.</param>
		/// <returns>Returns an awaitable Task instance.</returns>
		private async Task DelayMicroSeconds(double delayMicroseconds)
		{
			double delayInMiliseconds = delayMicroseconds / 1000d;
			await Task.Delay(TimeSpan.FromMilliseconds(delayInMiliseconds), _cancellationTokenSource.Token);
		}
	}
}
