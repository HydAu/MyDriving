﻿
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using MyTrips.ViewModel;
using MyTrips.Utils;
using Android.Support.V4.Content;
using Android.Graphics;

namespace MyTrips.Droid.Activities
{
    [Activity(Label = "Login", Theme="@style/MyThemeDark", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]            
    public class LoginActivity : BaseActivity
    {
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.activity_login;
            }
        }

        LoginViewModel viewModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((int)Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }

            viewModel = new LoginViewModel();
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            var twitter = FindViewById<Button>(Resource.Id.button_twitter);
            var microsoft = FindViewById<Button>(Resource.Id.button_microsoft);
            var facebook = FindViewById<Button>(Resource.Id.button_facebook);

            twitter.Click += (sender, e) => Login(LoginAccount.Twitter);
            microsoft.Click += (sender, e) => Login(LoginAccount.Microsoft);
            facebook.Click += (sender, e) => Login(LoginAccount.Facebook);

            FindViewById<Button>(Resource.Id.button_skip).Click += (sender, e) => 
            {
                viewModel.InitFakeUser();
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
                Finish();
            };

            //When the first screen of the app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub
            MyTrips.Services.OBDDataProcessor.GetProcessor().Initialize(viewModel.StoreManager);
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!viewModel.IsLoggedIn)
                return;

            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
        }


        void Login(LoginAccount account)
        {

            switch (account)
            {
                case LoginAccount.Facebook:
                    viewModel.LoginFacebookCommand.Execute(null);
                    break;
                case LoginAccount.Microsoft:
                    viewModel.LoginMicrosoftCommand.Execute(null);
                    break;
                case LoginAccount.Twitter:
                    viewModel.LoginTwitterCommand.Execute(null);
                    break;
            }
        }
    }
}

