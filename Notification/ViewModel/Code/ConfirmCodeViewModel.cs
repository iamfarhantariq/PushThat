using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.Strings;
using Notification.View.CodePage;
using Notification.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notification.ViewModel.Code
{
    public class ConfirmCode : BaseViewModel
    {
        public bool _IsEnableCouter;
        private object status;
        private UserClass data;
        private bool firstcall = true;
        private int time;
        private readonly ConfirmCodePage confirmCodePage;
        public string _Email;
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
                OnPropertyChanged("Email");
            }
        }
        public ICommand ResendCodeCommand { get; private set; }
        public ConfirmCode(ConfirmCodePage confirmCodePage, string email)
        {
            this.confirmCodePage = confirmCodePage;
            Email = email;
            ResendCodeCommand = new Command(ResendCode);
            _ = Timer();
        }

        private async void ResendCode()
        {
            if (firstcall)
            {
                firstcall = false;
                IsWorking = true;
                UserClass _obj = new UserClass()
                {
                    Email = Email,
                };
                status = await ApiServices.Post(WebConfig.ForgetPassword, _obj);
                if (status != null)
                {
                    data = JsonConvert.DeserializeObject<UserClass>(status.ToString());
                }
                if (data != null && data.Success == true)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Sent");
                    IsEnableResendButton = false;
                    IsWorking = false;
                    await Timer();
                }
                else if (data != null)
                {
                    DependencyService.Get<IMessage>().ShortAlert(data.Message);
                }
                IsWorking = false;
                firstcall = true;
            }
        }

        private async Task Timer()
        {
            await Task.Delay(250);
            if (!IsEnableResendButton)
            {
                for (time = 30; time > -1; time--)
                {
                    OnPropertyChanged("CountingBack");
                    await Task.Delay(900);
                }
            }
            IsEnableResendButton = true;
            OnPropertyChanged("CountingBack");
        }


        public string _CountingBack;
        public string CountingBack
        {
            get
            {
                if (!IsEnableResendButton)
                {
                    _CountingBack = $"00:{time.ToString("D2")}";
                }
                else
                    _CountingBack = "Resend Code";
                return _CountingBack;
            }
        }

        public bool _IsEnableResendButton = false;

        public bool IsEnableResendButton
        {
            get
            {
                return _IsEnableResendButton;
            }
            set
            {
                _IsEnableResendButton = value;
                OnPropertyChanged("IsEnableResendButton");
            }
        }
    }
}
