using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System;
using Microsoft.Extensions.Hosting;
using RpiControl.Domain.Entities;
using Microsoft.Extensions.Options;
using Mciec.Drivers;
using Nito.AsyncEx.Interop;

namespace RpiControlApi.HostedServices
{
    public class BorderControl : BackgroundService
    {
        private CommunicationObject _communicationObject;
        private BorderControlConfig _borderControlConfig;

        public BorderControl(IOptions<BorderControlConfig> borderControlConfig, CommunicationObject communicationObject)
        {
            _borderControlConfig = borderControlConfig.Value;
            _communicationObject = communicationObject;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            int channel = _borderControlConfig.ServoConfig.ChannelNumber;
            var objectInside = _communicationObject.ObjectInside;
            var objectOutside = _communicationObject.ObjectOutside;
            int openingDelay = _borderControlConfig.DelayBeforeOpeningMiliseconds;

            using (var servoHat = new ServoHat(_borderControlConfig.ServoHatI2c))
            {
                servoHat.Init(_borderControlConfig.ServoConfig.ChannelNumber);
                servoHat[_borderControlConfig.ServoConfig.ChannelNumber].Calibrate(
                    _borderControlConfig.ServoConfig.MaximumAngle,
                    _borderControlConfig.ServoConfig.MinimumPulseWidthMicroseconds,
                    _borderControlConfig.ServoConfig.MaximumPulseWidthMicroseconds);
                servoHat[_borderControlConfig.ServoConfig.ChannelNumber].WriteAngle(90);
                while (!ct.IsCancellationRequested)
                {
                    Console.WriteLine("Open. Waiting for CLOSE signal...");
                    //wait for CLOSE signal periodically checking if exit requested
                    while (!await WaitHandleAsyncFactory.FromWaitHandle(objectInside, System.TimeSpan.FromSeconds(1), ct))
                    {
                        if (ct.IsCancellationRequested) return;
                    }

                    servoHat[channel].WriteAngle(0);
                    do
                    {
                        Console.WriteLine("Close. CLOSE signal received. Waiting for OPEN signal...");
                        //wait for OPEN signal periodically checking if exit requested
                        while (!await WaitHandleAsyncFactory.FromWaitHandle(objectOutside, System.TimeSpan.FromSeconds(1), ct))
                        {
                            if (ct.IsCancellationRequested) return;
                        }
                        Console.WriteLine("Close. OPEN signal received. Waiting 5sec...");
                    } while (await WaitHandleAsyncFactory.FromWaitHandle(objectInside, System.TimeSpan.FromMilliseconds(openingDelay)));
                    servoHat[channel].WriteAngle(90);
                }
            }
        }
    }
}