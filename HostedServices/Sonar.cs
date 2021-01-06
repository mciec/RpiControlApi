using System.Device.Gpio;
using System.Threading.Tasks;
using Iot.Device.Hcsr04;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using RpiControl.Domain.Entities;
using Microsoft.Extensions.Options;

namespace RpiControlApi.HostedServices
{
    public class Sonar : BackgroundService
    {
        private readonly double _borderDistanceCentimeters;
        private readonly int _triggerPin, _echoPin;
        private readonly PinNumberingScheme _pinNumberingScheme;
        private Hcsr04 _hcsr04;
        private CommunicationObject _communicationObject;

        public double Distance { get; private set; }
        private double _prevDistance;
        public Sonar(IOptions<SonarConfig> sonarConfig, CommunicationObject communicationObject, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical)
        {
            _triggerPin = sonarConfig.Value.TriggerPin;
            _echoPin = sonarConfig.Value.EchoPin;
            _borderDistanceCentimeters = sonarConfig.Value.BorderDistanceCentimeters;
            _pinNumberingScheme = pinNumberingScheme;
            _communicationObject = communicationObject;
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
                    _communicationObject.Distance = Distance;
                }
                catch { }
                if (_prevDistance <= _borderDistanceCentimeters && Distance > _borderDistanceCentimeters)
                {
                    _communicationObject.ObjectInside?.Reset();
                    _communicationObject.ObjectOutside?.Set();
                    if (_communicationObject.BorderCrossedEvent != null)
                        _communicationObject.BorderCrossedEvent(this, BorderCrossed.Outside);
                }
                else
                if (_prevDistance > _borderDistanceCentimeters && Distance <= _borderDistanceCentimeters)
                {
                    _communicationObject.ObjectOutside?.Reset();
                    _communicationObject.ObjectInside?.Set();
                    if (_communicationObject.BorderCrossedEvent != null)
                        _communicationObject.BorderCrossedEvent(this, BorderCrossed.Inside);
                }
                _prevDistance = Distance;
                await Task.Delay(100);
            }
        }
    }
}