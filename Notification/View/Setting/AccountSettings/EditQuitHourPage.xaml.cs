using Notification.Model;
using Notification.SQLite;
using Notification.ViewModel.Setting.AccountSetting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Setting.AccountSettings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditQuitHourPage : ContentPage
    {
        private List<EditQuitHoursModel> quitHoursModels;
        private string day = string.Empty;
        private string Stext;
        private string Etext;

        public EditQuitHourPage()
        {
            InitializeComponent();
            BindingContext = new EditQuitHourViewModel(this);
        }

        private void StartTimeSTapped(object sender, MR.Gestures.TapEventArgs e)
        {
            STimePicker.Focus();
            day = ((Label)sender).ClassId;
            Stext = ((Label)sender).Text;
            if (day == string.Empty)
                return;
            STimePicker.PropertyChanged += StartTimePicker_PropertyChanged;
            STimePicker.Unfocus();
        }

        private void StartTimePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            quitHoursModels = SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>().Result;
            var target = quitHoursModels.Where(x => x.Category == "CustomDaysHour" && x.Day == day).FirstOrDefault();

            if (Stext == "Set Start Time")
            {
                STimePicker.Time = new TimeSpan(12, 00, 00);
                Stext = STimePicker.Time.ToString();
            }

            if (target != null && target.StartTime != STimePicker.Time)
            {
                ((EditQuitHourViewModel)BindingContext).UpdateToDatabase(
                day, target.Image, target.Active, target.Category, $"{STimePicker.Time} to {target.EndTime}", STimePicker.Time, target.EndTime);
                Task.Delay(200);
            }
        }

        private void EndTimeSTapped(object sender, MR.Gestures.TapEventArgs e)
        {
            ETimePicker.Focus();
            day = ((Label)sender).ClassId;
            Etext = ((Label)sender).Text;
            if (day == string.Empty)
                return;
            ETimePicker.PropertyChanged += EndTimePicker_PropertyChanged;
            ETimePicker.Unfocus();
        }

        private void EndTimePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            quitHoursModels = SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>().Result;
            var target = quitHoursModels.Where(x => x.Category == "CustomDaysHour" && x.Day == day).FirstOrDefault();

            if (Etext == "Set End Time")
            {
                ETimePicker.Time = new TimeSpan(12, 00, 00);
                Etext = ETimePicker.Time.ToString();
            }

            if (target != null && target.EndTime != ETimePicker.Time)
            {
                ((EditQuitHourViewModel)BindingContext).UpdateToDatabase(
                     day, target.Image, target.Active, target.Category, $"{target.StartTime} to {ETimePicker.Time}", target.StartTime, ETimePicker.Time);
                Task.Delay(200);
            }
        }
    }
}