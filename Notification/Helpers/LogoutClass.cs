using Notification.Model;
using Notification.SQLite;
using Notification.Strings;
using Notification.Themes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Notification.Helpers
{
    public class LogoutClass
    {
        private static bool currentTheme;
        private static bool autoSwitcherTheme;
        private static bool manualSwitcherTheme;

        public async static void LogoutCalled()
        {
            new SQLiteDatabase();
            var EQdata = await SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>();
            var Mdata = await SQLiteDatabase.GetAllTableDataAsync<MessageClass>();
            if (EQdata != null)
                await SQLiteDatabase.DeleteAllAsync<EditQuitHoursModel>();
            if (Mdata != null)
                await SQLiteDatabase.DeleteAllAsync<MessageClass>();

            currentTheme = (bool)Application.Current.Properties[NotificationStrings.DarkTheme];
            autoSwitcherTheme = (bool)Application.Current.Properties[NotificationStrings.AutoTheme];
            manualSwitcherTheme = (bool)Application.Current.Properties[NotificationStrings.ManualTheme];
            Application.Current.Properties.Clear();
            await Task.Delay(250);
            Application.Current.Properties[NotificationStrings.DarkTheme] = currentTheme;
            Application.Current.Properties[NotificationStrings.AutoTheme] = autoSwitcherTheme;
            Application.Current.Properties[NotificationStrings.ManualTheme] = manualSwitcherTheme;
            await Application.Current.SavePropertiesAsync();
        }
    }
}
