using System.Device.Gpio;
using System.Threading.Tasks;
using Iot.Device.Hcsr04;
using System.Threading;
using System.Diagnostics;
using System;
using Microsoft.Extensions.Hosting;

namespace RpiControlApi.HostedServices
{
    public enum BorderCrossed
    {
        Outside,
        Inside
    }

    public class Sonar : BackgroundService
    {
        private readonly double _borderDistanceCentimeters;
        private readonly int _triggerPin, _echoPin;
        private readonly PinNumberingScheme _pinNumberingScheme;
        private Hcsr04 _hcsr04;

        private EventWaitHandle _objectInside;
        private EventWaitHandle _objectOutside;

        public EventHandler<BorderCrossed> BorderCrossedEvent;

        public double Distance { get; private set; }
        private double _prevDistance;
        private Task _measurementTask;

        public Sonar(int triggerPin, int echoPin, EventWaitHandle objectInside, EventWaitHandle objectOutside, double borderDistanceCentimeters = 10.0, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical)
        {
            _triggerPin = triggerPin;
            _echoPin = echoPin;
            _pinNumberingScheme = pinNumberingScheme;
            _borderDistanceCentimeters = borderDistanceCentimeters;
            _objectInside = objectInside;
            _objectOutside = objectOutside;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _hcsr04 = new Hcsr04(_triggerPin, _echoPin, _pinNumberingScheme);
            _prevDistance = Distance;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Distance = _hcsr04.Distance.Centimeters;
                }
                catch { }
                //Console.WriteLine($"Distance: {Distance}");
                if (_prevDistance <= _borderDistanceCentimeters && Distance > _borderDistanceCentimeters)
                {
                    _objectInside?.Reset();
                    _objectOutside?.Set();
                    BorderCrossedEvent(this, BorderCrossed.Outside);
                }
                else
                if (_prevDistance > _borderDistanceCentimeters && Distance <= _borderDistanceCentimeters)
                {
                    _objectOutside?.Reset();
                    _objectInside?.Set();
                    BorderCrossedEvent(this, BorderCrossed.Inside);
                }
                _prevDistance = Distance;
                await Task.Delay(100);
            }
        }
    }
}