using Notification.Strings;
using Notification.ViewModel.Base;
using Notification.ViewModel.Setting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Notification.Themes
{
    public class ChangeTheme
    {
        private static TimeSpan morningTime = new TimeSpan(7, 00, 00);
        private static TimeSpan nightTime = new TimeSpan(19, 00, 00);
        private static TimeSpan time = new TimeSpan();
        public static void Theming()
        {
            if ((bool)Application.Current.Properties[NotificationStrings.AutoTheme] &&
                !(bool)Application.Current.Properties[NotificationStrings.ManualTheme])
            {
                TimeSpan currentTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                if (currentTime.Hours >= 7 && currentTime.Hours < 19) // this is between 7am to 7pm
                {
                    ThemeSwitcher(false);
                    time = nightTime.Subtract(currentTime);
                    Device.StartTimer(time, () =>
                    {
                        ThemeSwitcher(true);
                        AutoInteralSwitcher();
                        return false; // runs again, or false to stop
                    });
                }

                if (currentTime.Hours >= 19) // this is between 7pm to 12am
                {
                    ThemeSwitcher(true);
                    var hours = 24 - currentTime.Hours;
                    var minuts = 60 - currentTime.Minutes;
                    var seconds = 60 - currentTime.Seconds;
                    time = morningTime.Add(new TimeSpan(hours, minuts, seconds));
                    Device.StartTimer(time, () =>
                    {
                        ThemeSwitcher(false);
                        AutoInteralSwitcher();
                        return false; // runs again, or false to stop
                    });
                }

                if (currentTime.Hours >= 0 && currentTime.Hours < 7) //this time is between 12am to 7am
                {
                    ThemeSwitcher(true);
                    time = morningTime.Subtract(currentTime);
                    Device.StartTimer(time, () =>
                    {
                        ThemeSwitcher(false);
                        AutoInteralSwitcher();
                        return false; // runs again, or false to stop
                    });
                }
            }
            else
            {
                ThemeSwitcher((bool)Application.Current.Properties[NotificationStrings.ManualTheme]);
            }
        }

        private static void AutoInteralSwitcher()
        {
            Device.StartTimer(new TimeSpan(12, 00, 00), () =>
            {
                ThemeSwitcher(!(bool)Application.Current.Properties[NotificationStrings.DarkTheme]);
                return true; // runs again, or false to stop
            });
        }

        public static async void ThemeSwitcher(bool v)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                if (v)
                {
                    mergedDictionaries.Add(new DarkTheme());
                    Application.Current.Properties[NotificationStrings.DarkTheme] = true;
                }
                else
                {
                    mergedDictionaries.Add(new LightTheme());
                    Application.Current.Properties[NotificationStrings.DarkTheme] = false;
                }
                await Application.Current.SavePropertiesAsync();
            }

        }
    }
}
