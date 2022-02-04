using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using SDTS.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SDTS.Droid
{
    [Service]
    public class SensorsService : Service
    {
        Handler handler;
        Action runnable;
        ReadSensorsrData sensorsrData=null;
        System.Timers.Timer SensorsTimer = new System.Timers.Timer();
        public override void OnCreate()
        {
            base.OnCreate();
            if(sensorsrData==null)
            {
                sensorsrData = new ReadSensorsrData();
                sensorsrData.ToggleAccelerometer();
                DependencyService.RegisterSingleton<ReadSensorsrData>(sensorsrData);
                //sensorsrData.ToggleAccelerometer();
            }
            //SensorsTimer.Interval = 1000;
            //SensorsTimer.Elapsed += SendSensorsData;
            //SensorsTimer.AutoReset = true;
            //SensorsTimer.Enabled = true;
        }
        public async void SendSensorsData(Object source, System.Timers.ElapsedEventArgs e)
        {
            await SendSensorsDataToGuardian();
        }
        public async Task SendSensorsDataToGuardian()
        {
            Console.WriteLine(sensorsrData.dataAcc.Count);
            Console.WriteLine(sensorsrData.dataBar.Count);
            Console.WriteLine(sensorsrData.dataGyr.Count);
            Console.WriteLine(sensorsrData.dataMag.Count);
            Console.WriteLine(sensorsrData.dataOri.Count);
            sensorsrData.ClearData();
        }



        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action.Equals("ServicesDemo3.action.START_SERVICE"))
            {
                TRegisterForegroundService();
            }

			// This tells Android not to restart the service if it is killed to reclaim resources.
			return StartCommandResult.Sticky;
		}

        public override void OnDestroy()
        {

            Intent localIntent = new Intent();
            localIntent = new Intent(this, typeof(SensorsService));
            localIntent.SetAction("ServicesDemo3.action.START_SERVICE");

            this.StartForegroundService(localIntent);
        }


    void TRegisterForegroundService()
        {
            String NOTIFICATION_CHANNEL_ID = "no.jore.hajk";
            String channelName = "test app service";
            NotificationChannel chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName, NotificationImportance.None)
            {
                LockscreenVisibility = NotificationVisibility.Private
            };
            NotificationManager manager = (NotificationManager)GetSystemService(Context.NotificationService);
            manager.CreateNotificationChannel(chan);

            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
            //NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this);
            Notification notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_circle)
                    .SetContentTitle("233")
                    .SetContentText("332")
                    .SetContentIntent(BuildIntentToShowMainActivity())
                    .SetPriority(1)
                    .SetOngoing(true)
                    .SetCategory(Notification.CategoryService)
                    //.AddAction(BuildStopServiceAction())
                    .Build();

            // Enlist this instance of the service as a foreground service
            StartForeground(1, notification);
        }
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction("ServicesDemo3.action.MAIN_ACTIVITY");
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra("has_service_been_started", true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}