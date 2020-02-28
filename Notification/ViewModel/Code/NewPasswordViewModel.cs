using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.Strings;
using Notification.View.AddDevice;
using Notification.View.CodePage;
using Notification.ViewModel.Base;
using Xamarin.Forms;

namespace Notification.ViewModel.Code
{
    public class NewPasswordViewModel : BaseViewModel
    {
        private NewPasswordPage newPasswordPage;
        private string email;
        private bool firstcall = true;

        public ICommand ResetPasswordCommand { get; private set; }
        public NewPasswordViewModel(NewPasswordPage newPasswordPage, string email)
        {
            this.newPasswordPage = newPasswordPage;
            this.email = email;
            ResetPasswordCommand = new Command(ResetPassword);
        }

        public Color _PasswordColor = Color.Default;
        public Color PasswordColor
        {
            get
            {
                return _PasswordColor;
            }
            set
            {
                _PasswordColor = value;
                OnPropertyChanged("PasswordColor");
            }
        }

        public string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }

        public string _ConfirmPassword;
        private object status;
        private UserClass data;

        public string ConfirmPassword
        {
            get { return _ConfirmPassword; }
            set
            {
                _ConfirmPassword = value;
                OnPropertyChanged("ConfirmPassword");
            }
        }

        private async void ResetPassword(object obj)
        {
            if (firstcall)
            {
                firstcall = false;
                if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    DependencyService.Get<IMessage>().ShortAlert("Empty");
                }
                else
                {
                    if (Password != ConfirmPassword)
                    {
                        PasswordColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                        DependencyService.Get<IMessage>().ShortAlert("Password Mismatched");
                    }
                    else
                    {
                        IsWorking = true;
                        UserClass _obj = new UserClass()
                        {
                            Email = email,
                            Password = ConfirmPassword
                        };
                        status = await ApiServices.Post(WebConfig.CreateNewPassword, _obj);
                        if (status != null)
                        {
                            data = JsonConvert.DeserializeObject<UserClass>(status.ToString());
                        }
                        if (data != null && data.Success == true)
                        {
                            Application.Current.Properties[NotificationStrings.Email] = email;
                            Application.Current.Properties[NotificationStrings.Password] = ConfirmPassword;
                            Application.Current.Properties[NotificationStrings.NotificationKey] = data.NotificationKey;
                            Application.Current.Properties[NotificationStrings.Token] = data.Token;
                            Application.Current.Properties[NotificationStrings.DeviceLicenseStatus] = data.Licensing;
                            await Application.Current.SavePropertiesAsync();

                            DependencyService.Get<IMessage>().ShortAlert("Password Changed");
                            Application.Current.MainPage = new NavigationPage(new AddDevicePage());
                            await Application.Current.MainPage.Navigation.PopToRootAsync();
                        }
                        IsWorking = false;
                    }
                }
                firstcall = true;
            }
        }
    }
}
