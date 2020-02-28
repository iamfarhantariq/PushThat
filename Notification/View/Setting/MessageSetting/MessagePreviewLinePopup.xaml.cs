using Notification.Strings;
using Notification.ViewModel.Setting;
using Rg.Plugins.Popup.Pages;
using System;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Setting.MessageSetting
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePreviewLinePopup : PopupPage
    {
        private CheckBox prevCB;
        private string massagePreviewInLine;
        public MessagePreviewLinePopup(string messagePreviewInLine)
        {
            InitializeComponent();
            this.massagePreviewInLine = messagePreviewInLine.ToString();
            CheckBoxChecked();
        }

        private void CheckBoxChecked()
        {
            switch (massagePreviewInLine.ToString())
            {
                case "All":
                    CBxAll.IsChecked = true;
                    break;
                case "1":
                    CBxOne.IsChecked = true;
                    break;
                case "2":
                    CBxTwo.IsChecked = true;
                    break;
                case "3":
                    CBxThree.IsChecked = true;
                    break;
                case "4":
                    CBxFour.IsChecked = true;
                    break;
                case "5":
                    CBxFive.IsChecked = true;
                    break;
                case "6":
                    CBxSix.IsChecked = true;
                    break;
                case "7":
                    CBxSeven.IsChecked = true;
                    break;
                case "8":
                    CBxEight.IsChecked = true;
                    break;
                case "9":
                    CBxNine.IsChecked = true;
                    break;
                case "10":
                    CBxTen.IsChecked = true;
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
                case "0":
                    massagePreviewInLine = "All";
                    break;
                case "1":
                    massagePreviewInLine = "1";
                    break;
                case "2":
                    massagePreviewInLine = "2";
                    break;
                case "3":
                    massagePreviewInLine = "3";
                    break;
                case "4":
                    massagePreviewInLine = "4";
                    break;
                case "5":
                    massagePreviewInLine = "5";
                    break;
                case "6":
                    massagePreviewInLine = "6";
                    break;
                case "7":
                    massagePreviewInLine = "7";
                    break;
                case "8":
                    massagePreviewInLine = "8";
                    break;
                case "9":
                    massagePreviewInLine = "9";
                    break;
                case "10":
                    massagePreviewInLine = "10";
                    break;
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(this, "MessagePreview");
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties[NotificationStrings.MessagePreviewInLine] = massagePreviewInLine;
            await Application.Current.SavePropertiesAsync();
            MessagingCenter.Send(this, "MessagePreview");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}