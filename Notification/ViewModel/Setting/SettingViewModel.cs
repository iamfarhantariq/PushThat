using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.Strings;
using Notification.Themes;
using Notification.View.EditCategory;
using Notification.View.Setting;
using Notification.View.Setting.AccountSettings;
using Notification.View.Setting.AlertSetting;
using Notification.View.Setting.MessageSetting;
using Notification.View.Signin;
using Notification.ViewModel.Base;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Notification.ViewModel.Setting
{
    public class SettingViewModel : BaseViewModel
    {
        private readonly SettingPage settingPage;
        private bool alreadyClicked = true;
        public ICommand UserKeyTappedToCopyCommand { get; private set; }
        public ICommand EditCategoryTappedCommand { get; private set; }
        public ICommand EditTicketTappedCommand { get; private set; }
        public ICommand MaxMessagesToKeepTappedCommand { get; private set; }
        public ICommand MessagePreviewInLineTappedCommand { get; private set; }
        public ICommand ChooseDefaultToneTappedCommand { get; private set; }
        public ICommand VibrateIterationTappedCommand { get; private set; }
        public ICommand EditQuitHoursTappedCommand { get; private set; }
        public ICommand LogoutTappedCommand { get; private set; }

        public SettingViewModel(SettingPage settingPage)
        {
            this.settingPage = settingPage;
            UserKeyTappedToCopyCommand = new Command(UserKeyTappedToCopyAsync);
            EditCategoryTappedCommand = new Command(EditCategoryTapped);
            EditTicketTappedCommand = new Command(EditTicketTapped);
            MaxMessagesToKeepTappedCommand = new Command(MaxMessagesToKeepTapped);
            MessagePreviewInLineTappedCommand = new Command(MessagePreviewInLineTapped);
            ChooseDefaultToneTappedCommand = new Command(ChooseDefaultToneTapped);
            VibrateIterationTappedCommand = new Command(VibrateIterationTapped);
            EditQuitHoursTappedCommand = new Command(EditQuitHoursTapped);
            LogoutTappedCommand = new Command(LogoutTapped);
        }
        public string Notification { get { return NotificationStrings.ProjectName; } }


        #region EditCategoryTapped
        private void EditCategoryTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                settingPage.Navigation.PushAsync(new EditCategoryPage());
                alreadyClicked = true;
            }
        }
        #endregion


        #region EditTicketTapped
        private void EditTicketTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                settingPage.Navigation.PushAsync(new EditTicketPage());
                alreadyClicked = true;
            }
        } 
        #endregion


        #region NotificationKey
        public string NotificationKey
        {
            get
            {
                return (string)Application.Current.Properties[NotificationStrings.NotificationKey];
            }
        }
        private async void UserKeyTappedToCopyAsync(object obj)
        {
            await Clipboard.SetTextAsync(NotificationKey);
            DependencyService.Get<IMessage>().ShortAlert("Copied!");
        }
        #endregion

        #region MaxMessageToKeep
        public int MaxMessageToKeep
        {
            get
            {
                return (int)Application.Current.Properties[NotificationStrings.MaxMessageToKeep];
            }
            set
            {
                OnPropertyChanged("MaxMessageToKeep");
            }
        }
        private async void MaxMessagesToKeepTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                await PopupNavigation.Instance.PushAsync(new MaxMessagePopup(MaxMessageToKeep));
                alreadyClicked = true;
            }
        }
        #endregion

        #region MessagePreviewInLine
        public string MessagePreviewInLine
        {
            get
            {
                return (string)Application.Current.Properties[NotificationStrings.MessagePreviewInLine];
            }
            set
            {
                OnPropertyChanged("MessagePreviewInLine");
            }
        }
        private async void MessagePreviewInLineTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                await PopupNavigation.Instance.PushAsync(new MessagePreviewLinePopup(MessagePreviewInLine));
                alreadyClicked = true;
            }
        }

        #endregion

        #region Theming

        public bool _autoThemeSwitchIsToggled;
        public bool AutoThemeSwitchIsToggled
        {
            get { return (bool)Application.Current.Properties[NotificationStrings.AutoTheme]; }
            set
            {
                Application.Current.Properties[NotificationStrings.AutoTheme] = value;
                if (value)
                    Application.Current.Properties[NotificationStrings.ManualTheme] = !value;
                ChangeTheme.Theming();
                OnPropertyChanged("ManualThemeSwitchIsToggled");
                OnPropertyChanged("AutoThemeSwitchIsToggled");
            }
        }

        public bool _themeSwitchIsToggled;
        public bool ManualThemeSwitchIsToggled
        {
            get { return (bool)Application.Current.Properties[NotificationStrings.ManualTheme]; }
            set
            {
                Application.Current.Properties[NotificationStrings.ManualTheme] = value;
                if (value)
                    Application.Current.Properties[NotificationStrings.AutoTheme] = !value;
                ChangeTheme.Theming();
                OnPropertyChanged("AutoThemeSwitchIsToggled");
                OnPropertyChanged("ManualThemeSwitchIsToggled");
            }
        }

        private void Switcher()
        {
            if (AutoThemeSwitchIsToggled)
                ManualThemeSwitchIsToggled = false;
            if (ManualThemeSwitchIsToggled)
                AutoThemeSwitchIsToggled = false;
        }
        #endregion

        #region CopyMessagesTitle
        public bool CopyMessagesTitle
        {
            get
            {
                return (bool)Application.Current.Properties[NotificationStrings.CopyMessagesTitle];
            }
            set
            {
                Application.Current.Properties[NotificationStrings.CopyMessagesTitle] = value;
                OnPropertyChanged("CopyMessagesTitle");
            }
        }
        #endregion

        #region ChooseDefaultTone
        public string ChooseDefaultTone
        {
            get
            {
                return (string)Application.Current.Properties[NotificationStrings.ChooseDefaultTone];
            }
            set
            {
                OnPropertyChanged("ChooseDefaultTone");
            }
        }
        private async void ChooseDefaultToneTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                await PopupNavigation.Instance.PushAsync(new ChooseDefaultTonePopup(ChooseDefaultTone, 1, null));
                alreadyClicked = true;
            }
        }

        #endregion

        #region VibrateIteration
        public int VibrateIteration
        {
            get
            {
                return (int)Application.Current.Properties[NotificationStrings.VibrateIteration];
            }
            set
            {
                OnPropertyChanged("VibrateIteration");
            }
        }
        private async void VibrateIterationTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                await PopupNavigation.Instance.PushAsync(new VibrateIterationPopup(VibrateIteration));
                alreadyClicked = true;
            }
        }

        #endregion

        #region Email
        public string Email
        {
            get
            {
                return (string)Application.Current.Properties[NotificationStrings.Email];
            }
        }
        #endregion

        #region EditQuitHours
        private async void EditQuitHoursTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                if ((bool)Application.Current.Properties[NotificationStrings.NotificationDismissalSync])
                    await settingPage.DisplayAlert("NotificationDismissalSync", "NotificationDismissalSync is on, turn it off to Edit Quit Hour.", "Ok");
                else
                    await Application.Current.MainPage.Navigation.PushAsync(new EditQuitHourPage());
                //await Application.Current.MainPage.Navigation.PushAsync(new EditQuitHoursPage());
                alreadyClicked = true;
            }
        }
        #endregion

        #region DeviceName
        public string DeviceName
        {
            get
            {
                return (string)Application.Current.Properties[NotificationStrings.DeviceName];
            }
        }
        #endregion

        #region NotificationDismissalASyncToggled

        private DeviceModel data;
        private object status;
        private bool go = true;

        public bool NotificationDismissalASyncToggled
        {
            get
            {
                return (bool)Application.Current.Properties[NotificationStrings.NotificationDismissalSync];
            }
            set
            {
                if (alreadyClicked)
                {
                    alreadyClicked = false;
                    //Application.Current.Properties[NotificationStrings.NotificationDismissalSync] = value;
                    ASyncMethod(value).ContinueWith(x => { });
                    alreadyClicked = true;
                }
            }
        }

        private async Task ASyncMethod(bool _NotificationDismissalASyncToggled)
        {
            if (go)
            {
                go = false;
                DeviceModel _obj = new DeviceModel()
                {
                    NotificationKey = (string)Application.Current.Properties[NotificationStrings.NotificationKey],
                    NotificationDismissalSync = _NotificationDismissalASyncToggled.ToString().ToLower()
                };

                status = await ApiServices.Post(WebConfig.NotificationDismissalSync, _obj);

                //if (status == null)
                //{
                //    OnPropertyChanged("NotificationDismissalASyncToggled");
                //    return;
                //}

                if (status != null)
                {
                    data = JsonConvert.DeserializeObject<DeviceModel>(status.ToString());
                }
                if (data != null && data.Success == true)
                {
                    if (_NotificationDismissalASyncToggled)
                        DependencyService.Get<IMessage>().ShortAlert("Notification is Disabled on all devices!");
                    else
                        DependencyService.Get<IMessage>().ShortAlert("Notification is Enabled on all devices!");

                    Application.Current.Properties[NotificationStrings.NotificationDismissalSync] = _NotificationDismissalASyncToggled;
                }

                OnPropertyChanged("NotificationDismissalASyncToggled");
                await Task.Delay(250);
            }
            go = true;
        }
        #endregion

        #region DeviceLicenseStatus
        public string DeviceLicenseStatus
        {
            get
            {
                return (string)Application.Current.Properties[NotificationStrings.DeviceLicenseStatus];
            }
        }
        #endregion

        #region Logout
        private async void LogoutTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                var ans = await settingPage.DisplayAlert("Logout", "Do you want to logout?", "Yes", "Cancel");
                {
                    if (ans)
                    {
                        IsWorking = true;
                        DeviceModel _obj = new DeviceModel()
                        {
                            DeviceKey = (string)Application.Current.Properties[NotificationStrings.DeviceKey]
                        };
                        var status = await ApiServices.Post(WebConfig.Logout, _obj);
                        if (status != null)
                            data = JsonConvert.DeserializeObject<DeviceModel>(status.ToString());
                        if (data != null && data.Success == true)
                        {
                            LogoutClass.LogoutCalled();
                            Application.Current.MainPage = new NavigationPage(new SigninPage());
                            await settingPage.Navigation.PopToRootAsync();
                        }
                        IsWorking = false;
                    }
                }
                alreadyClicked = true;
            }
            #endregion

        }
    }
}
