using Microsoft.AspNetCore.SignalR.Client;
using Models;
using SDTS.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SDTS.Sensors
{
    public class ReadAccelerometerData
    {
        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.Fastest;
        HubServices hubServices;

        public ReadAccelerometerData()
        {
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            

        }

        public List<Tuple<double,double,double>> data = new List<Tuple<double, double, double>>();

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            data.Add(new Tuple<double, double, double>(e.Reading.Acceleration.X, e.Reading.Acceleration.Y, e.Reading.Acceleration.Z));
        }

        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                {
                    Accelerometer.Start(speed);
                }
                    
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }
}
