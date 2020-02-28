using Notification.Model;
using Notification.SQLite;
using Notification.Strings;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Setting.AlertSetting
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseDefaultTonePopup : PopupPage
    {
        private CheckBox prevCB;
        private bool firstcall;
        private int call;
        private string catType;
        public string chooseDefaultTonePopup;

        public ChooseDefaultTonePopup(string chooseDefaultTonePopup, int call, string catType)
        {
            InitializeComponent();
            firstcall = true;
            this.call = call;
            this.catType = catType;
            this.chooseDefaultTonePopup = chooseDefaultTonePopup;
            CheckBoxChecked();
        }

        private void CheckBoxChecked()
        {
            switch (chooseDefaultTonePopup)
            {
                case "Notification":
                    CBxNotification.IsChecked = true;
                    break;
                case "Ringtone 2":
                    CBxRingtoneTwo.IsChecked = true;
                    break;
                case "Ringtone 3":
                    CBxRingtoneThree.IsChecked = true;
                    break;
                case "Ringtone 4":
                    CBxRingtoneFour.IsChecked = true;
                    break;
                case "Ringtone 5":
                    CBxRingtoneFive.IsChecked = true;
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

            if (((CheckBox)sender).IsChecked == false)
            {
                Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current.Pause();
                Task.Delay(100);

                Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current.Stop();
                Task.Delay(100);
            }

            var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

            if (((CheckBox)sender).IsChecked == true)
            {
                if (!firstcall)
                {
                    switch (id)
                    {
                        case "1":
                            player.Load("RingtoneOne.MP3");
                            player.Play();
                            chooseDefaultTonePopup = "Notification";
                            break;
                        case "2":
                            player.Load("RingtoneTwo.MP3");
                            player.Play();
                            chooseDefaultTonePopup = "Ringtone 2";
                            break;
                        case "3":
                            player.Load("RingtoneThree.MP3");
                            player.Play();
                            chooseDefaultTonePopup = "Ringtone 3";
                            break;
                        case "4":
                            player.Load("RingtoneFour.MP3");
                            player.Play();
                            chooseDefaultTonePopup = "Ringtone 4";
                            break;
                        case "5":
                            player.Load("RingtoneThree.MP3");
                            player.Play();
                            chooseDefaultTonePopup = "Ringtone 5";
                            break;
                    }
                }
                firstcall = false;
            }
        }

        protected override async void OnDisappearing()
        {
            Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current.Pause();
            await Task.Delay(100);

            Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current.Stop();
            await Task.Delay(100);

            base.OnDisappearing();
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (call == 1)
            {
                Application.Current.Properties[NotificationStrings.ChooseDefaultTone] = chooseDefaultTonePopup;
                MessagingCenter.Send(this, "ChooseDefaultTone");
            }
            if (call == 2)
            {
                //UpdateToneClass.Update(chooseDefaultTonePopup, catType);
                MessagingCenter.Send(this, "CategoryTypeTone");
                //Application.Current.Properties[NotificationStrings.CategoryTypeTone] = chooseDefaultTonePopup;
            }

            await Application.Current.SavePropertiesAsync();
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
