using Notification.Strings;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Setting.AlertSetting
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VibrateIterationPopup : PopupPage
    {
        private CheckBox prevCB;
        private int vibrateIterationPopup;
        private bool firstcall;
        public VibrateIterationPopup(int vibrateIterationPopup)
        {
            InitializeComponent();
            firstcall = true;
            this.vibrateIterationPopup = vibrateIterationPopup;
            CheckBoxChecked();
        }

        private void CheckBoxChecked()
        {
            switch (vibrateIterationPopup)
            {
                case 1:
                    CBxOne.IsChecked = true;
                    break;
                case 2:
                    CBxTwo.IsChecked = true;
                    break;
                case 3:
                    CBxThree.IsChecked = true;
                    break;
                case 4:
                    CBxFour.IsChecked = true;
                    break;
                case 5:
                    CBxFive.IsChecked = true;
                    break;
            }
        }
        private void CheckBoxes_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var id = ((CheckBox)sender).ClassId;
            ((CheckBox)sender).IsEnabled = false;

            if (prevCB != null && prevCB != (CheckBox)sender)
            {
                prevCB.IsChecked = false;
                prevCB.IsEnabled = true;
                prevCB = (CheckBox)sender;
            }
            if (prevCB == null)
            {
                prevCB = (CheckBox)sender;
            }

            switch (id)
            {
                case "1":
                    VibrationRounds((CheckBox)sender, 1);
                    vibrateIterationPopup = 1;
                    break;
                case "2":
                    VibrationRounds((CheckBox)sender, 2);
                    vibrateIterationPopup = 2;
                    break;
                case "3":
                    VibrationRounds((CheckBox)sender, 3);
                    vibrateIterationPopup = 3;
                    break;
                case "4":
                    VibrationRounds((CheckBox)sender, 4);
                    vibrateIterationPopup = 4;
                    break;
                case "5":
                    VibrationRounds((CheckBox)sender, 5);
                    vibrateIterationPopup = 5;
                    break;
            }
        }

        private async void VibrationRounds(CheckBox sender, int id)
        {
            if (!firstcall)
            {
                if (sender.IsChecked == true)
                {
                    for (int i = 0; i < id; i++)
                    {
                        Vibration.Vibrate(300);
                        await Task.Delay(400);
                    }
                }
            }
            firstcall = false;
        }

        protected override void OnDisappearing()
        {
            Vibration.Cancel();
            base.OnDisappearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties[NotificationStrings.VibrateIteration] = vibrateIterationPopup;
            await Application.Current.SavePropertiesAsync();
            MessagingCenter.Send(this, "VibrateIteration");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}