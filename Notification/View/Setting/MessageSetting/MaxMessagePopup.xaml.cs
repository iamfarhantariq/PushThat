using Notification.Strings;
using Notification.ViewModel.Setting;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Setting.MessageSetting
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MaxMessagePopup : PopupPage
    {
        private CheckBox prevCB;
        private int maxMessageToKeep;
        public MaxMessagePopup(int maxMessageToKeep)
        {
            InitializeComponent();
            this.maxMessageToKeep = maxMessageToKeep;
            CheckBoxChecked();
        }

        private void CheckBoxChecked()
        {
            switch (maxMessageToKeep)
            {
                case 25:
                    CBxTwoFive.IsChecked = true;
                    break;
                case 50:
                    CBxFiveZero.IsChecked = true;
                    break;
                case 100:
                    CBxHundrede.IsChecked = true;
                    break;
                case 150:
                    CBxOneFifty.IsChecked = true;
                    break;
                case 200:
                    CBxTwoHundred.IsChecked = true;
                    break;
                case 250:
                    CBxTwoFifty.IsChecked = true;
                    break;
                case 500:
                    CBxFiveHundred.IsChecked = true;
                    break;
                case 1000:
                    CBxThousand.IsChecked = true;
                    break;
                default:
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
                    maxMessageToKeep = 25;
                    break;
                case "2":
                    maxMessageToKeep = 50;
                    break;
                case "3":
                    maxMessageToKeep = 100;
                    break;
                case "4":
                    maxMessageToKeep = 150;
                    break;
                case "5":
                    maxMessageToKeep = 200;
                    break;
                case "6":
                    maxMessageToKeep = 250;
                    break;
                case "7":
                    maxMessageToKeep = 500;
                    break;
                case "8":
                    maxMessageToKeep = 1000;
                    break;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties[NotificationStrings.MaxMessageToKeep] = maxMessageToKeep;
            await Application.Current.SavePropertiesAsync();
            MessagingCenter.Send(this, "MaxMessage");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
