using Notification.Services;
using Notification.Strings;
using Notification.View.Dashboard;
using Notification.View.AddDevice;
using Notification.View.Signin;
using Xamarin.Forms;
using Notification.Model;
using Notification.Themes;
using Plugin.FirebasePushNotification;
using System;

namespace Notification
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            if (!Current.Properties.ContainsKey(NotificationStrings.Token))
                MainPage = new NavigationPage(new SigninPage());
            else if (Current.Properties.ContainsKey(NotificationStrings.Token) && !Current.Properties.ContainsKey(NotificationStrings.DeviceKey))
                MainPage = new NavigationPage(new AddDevicePage());
            else if (Current.Properties.ContainsKey(NotificationStrings.Token) && Current.Properties.ContainsKey(NotificationStrings.DeviceKey))
                MainPage = new NavigationPage(new DashboardPage());
        }

        protected override void OnStart()
        {
            if (!Current.Properties.ContainsKey(NotificationStrings.DarkTheme))
            {
                Current.Properties[NotificationStrings.DarkTheme] = false;
            }
            if (!Current.Properties.ContainsKey(NotificationStrings.AutoTheme))
            {
                Current.Properties[NotificationStrings.AutoTheme] = false;
            }

            if (!Current.Properties.ContainsKey(NotificationStrings.ManualTheme))
            {
                Current.Properties[NotificationStrings.ManualTheme] = false;
            }

            ChangeTheme.Theming();
            //ChangeTheme.ChooseTheme((bool)Current.Properties[NotificationStrings.DarkTheme]);

            CrossFirebasePushNotification.Current.Subscribe("general");
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN REC: {p.Token}");
            };
            System.Diagnostics.Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");
            

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
                    {
                //try
                //{
                //    System.Diagnostics.Debug.WriteLine("Received");
                //    if (p.Data.ContainsKey("body"))
                //    {
                //        //Device.BeginInvokeOnMainThread(() =>
                //        //{
                //        //    mPage.Message = $"{p.Data["body"]}";
                //        //});

                //    }
                //}
                //catch (Exception ex)
                //{

                //}
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
                        {
                //System.Diagnostics.Debug.WriteLine(p.Identifier);

                System.Diagnostics.Debug.WriteLine("Opened");
                            foreach (var data in p.Data)
                            {
                                System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                            }

                            if (!string.IsNullOrEmpty(p.Identifier))
                            {
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    mPage.Message = p.Identifier;
                    //});
                }
                            else if (p.Data.ContainsKey("color"))
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                        //mPage.Navigation.PushAsync(new ContentPage()
                        //{
                        //    BackgroundColor = Color.FromHex($"{p.Data["color"]}")

                        //});
                    });

                            }
                            else if (p.Data.ContainsKey("aps.alert.title"))
                            {
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    mPage.Message = $"{p.Data["aps.alert.title"]}";
                    //});

                }
                        };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Action");

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    //System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                    //foreach (var data in p.Data)
                    //{
                    //    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                    //}

                }

            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                //System.Diagnostics.Debug.WriteLine("Dismissed");
            };
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            if (!Current.Properties.ContainsKey(NotificationStrings.GalleryOpened))
                Current.Properties[NotificationStrings.GalleryOpened] = false;

            if (Current.Properties.ContainsKey(NotificationStrings.DeviceKey) && !(bool)Current.Properties[NotificationStrings.GalleryOpened])
            {
                DeviceModel _obj = new DeviceModel()
                {
                    DeviceKey = (string)Current.Properties[NotificationStrings.DeviceKey]
                };
                _ = ApiServices.Post(WebConfig.LastSync, _obj);
            }
            Current.SavePropertiesAsync();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            //if (Current.Properties.ContainsKey(NotificationStrings.DeviceKey))
            //{
            //    DeviceModel _obj = new DeviceModel()
            //    {
            //        DeviceKey = (string)Current.Properties[NotificationStrings.DeviceKey]
            //    };
            //    _ = ApiServices.Post(WebConfig.LastSync, _obj);
            //}
            //Current.SavePropertiesAsync();
        }
    }
}
