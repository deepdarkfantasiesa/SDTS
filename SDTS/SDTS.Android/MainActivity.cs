using System;
using Xamarin.Forms.GoogleMaps.Android;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms;
using SDTS.Services;
using Android.Content;
using SDTS.Droid.ENotification;
using SDTS.ENotification;

namespace SDTS.Droid
{
    //[Register("MainActivity")]
    [Activity(Label = "SDTS", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
        LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        Intent startServiceIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            startServiceIntent = new Intent(this, typeof(SensorsService));
            startServiceIntent.SetAction("ServicesDemo3.action.START_SERVICE");
            StartService(startServiceIntent);


            //github的
            Xamarin.FormsGoogleMapsBindings.Init();
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);
            //官方包
            //Xamarin.FormsMaps.Init(this, savedInstanceState);
            var hubservices = new HubServices();
            DependencyService.RegisterSingleton<HubServices>(hubservices);
            LoadApplication(new App());

            CreateNotificationFromIntent(Intent);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            CreateNotificationFromIntent(intent);
        }

        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(AndroidNotificationManager.TitleKey);
                string message = intent.GetStringExtra(AndroidNotificationManager.MessageKey);
                DependencyService.Get<INotificationManager>().ReceiveNotification(title, message);
            }
        }
    }
}