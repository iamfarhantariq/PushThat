using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.Strings;
using Notification.View.CodePage;
using Notification.View.ForgetPassword;
using Notification.ViewModel.Base;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Notification.ViewModel.ForgetPassword
{
    public class ForgetPasswordViewModel : BaseViewModel
    {
        private ForgetPasswordPage forgetPasswordPage;

        public ICommand SendConfirmationCodeCommand { get; private set; }
        public ForgetPasswordViewModel(ForgetPasswordPage forgetPasswordPage)
        {
            this.forgetPasswordPage = forgetPasswordPage;
            SendConfirmationCodeCommand = new Command(SendConfirmationCode);
        }

        private object status;
        private UserClass data;

        public string _ForgetPassword;
        public string ForgetPassword
        {
            get { return _ForgetPassword; }
            set
            {
                _ForgetPassword = value;
                OnPropertyChanged("ForgetPassword");
            }
        }

        public Color _EmailColor = Color.Default;
        private bool alreadyClicked = true;

        public Color EmailColor
        {
            get
            {
                return _EmailColor;
            }
            set
            {
                _EmailColor = value;
                OnPropertyChanged("EmailColor");
            }
        }

        private async void SendConfirmationCode(object obj)
        {
            if (alreadyClicked)
            {
                string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
                alreadyClicked = false;
                if (string.IsNullOrEmpty(ForgetPassword))
                {
                    EmailColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    DependencyService.Get<IMessage>().ShortAlert("Email is Empty");
                }
                else if (!Regex.IsMatch(ForgetPassword, emailPattern))
                {
                    EmailColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    DependencyService.Get<IMessage>().ShortAlert("Invalid Email");
                }
                else
                {
                    IsWorking = true;
                    UserClass _obj = new UserClass()
                    {
                        Email = ForgetPassword,
                    };
                    status = await ApiServices.Post(WebConfig.ForgetPassword, _obj);
                    if (status != null)
                    {
                        data = JsonConvert.DeserializeObject<UserClass>(status.ToString());
                    }
                    if (data != null && data.Success == true)
                    {
                        DependencyService.Get<IMessage>().ShortAlert("Check Your Email");
                        await forgetPasswordPage.Navigation.PushAsync(new ConfirmCodePage(ForgetPassword));
                    }
                    else if (data != null)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(data.Message);
                    }
                    IsWorking = false;
                }
                alreadyClicked = true;
            }
        }
    }
}
