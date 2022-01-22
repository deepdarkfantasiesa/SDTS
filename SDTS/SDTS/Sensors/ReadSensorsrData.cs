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
    public class ReadSensorsrData
    {
        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.Fastest;
        HubServices hubServices;

        public ReadSensorsrData()
        {
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;//加速度计
            Barometer.ReadingChanged += Barometer_ReadingChanged;//气压计
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;//陀螺仪
            Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;//磁力计
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;//方向传感器

        }

        public List<Tuple<double,double,double>> dataAcc = new List<Tuple<double, double, double>>();
        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            dataAcc.Add(new Tuple<double, double, double>(e.Reading.Acceleration.X, e.Reading.Acceleration.Y, e.Reading.Acceleration.Z));
        }
        public List<double> dataBar = new List<double>();
        void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            dataBar.Add(e.Reading.PressureInHectopascals);
        }
        public List<Tuple<double, double, double>> dataGyr = new List<Tuple<double, double, double>>();
        void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            dataGyr.Add(new Tuple<double, double, double>(e.Reading.AngularVelocity.X, e.Reading.AngularVelocity.Y, e.Reading.AngularVelocity.Z));
        }
        public List<Tuple<double, double, double>> dataMag = new List<Tuple<double, double, double>>();
        void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            dataMag.Add(new Tuple<double, double, double>(e.Reading.MagneticField.X, e.Reading.MagneticField.Y, e.Reading.MagneticField.Z));
        }
        public List<Tuple<double, double, double>> dataOri = new List<Tuple<double, double, double>>();
        void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            dataOri.Add(new Tuple<double, double, double>(e.Reading.Orientation.X, e.Reading.Orientation.Y, e.Reading.Orientation.Z));
        }

        public void ClearData()
        {
            dataAcc.Clear();
            dataBar.Clear();
            dataGyr.Clear();
            dataMag.Clear();
            dataOri.Clear();

        }

        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                {
                    Accelerometer.Stop();
                    Barometer.Stop();
                    Gyroscope.Stop();
                    Magnetometer.Stop();
                    OrientationSensor.Stop();
                }
                else
                {
                    Accelerometer.Start(speed);
                    Barometer.Start(speed);
                    Gyroscope.Start(speed);
                    Magnetometer.Start(speed);
                    OrientationSensor.Start(speed);
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
