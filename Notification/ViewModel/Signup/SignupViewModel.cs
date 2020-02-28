using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.Strings;
using Notification.View.AddDevice;
using Notification.View.CodePage;
using Notification.View.Signup;
using Notification.ViewModel.Base;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notification.ViewModel.Signup
{
    public class SignupViewModel : BaseViewModel
    {
        private bool alreadyClicked = true;
        private string _email = string.Empty;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _confirmpassword = string.Empty;
        public string ConfirmPassword
        {
            get
            {
                return _confirmpassword;
            }
            set
            {
                _confirmpassword = value;
                OnPropertyChanged();
            }
        }

        private SignupPage signupPage;
        private object status;
        private UserClass data;
        private UserClass _data;

        public ICommand SignupCommand { get; private set; }

        public SignupViewModel(SignupPage signupPage)
        {
            this.signupPage = signupPage;
            SignupCommand = new Command(SignupClicked);
        }

        public Color _EmailColor = Color.Default;
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

        public Color _ConfirmPasswordColor = Color.Default;
        public Color ConfirmPasswordColor
        {
            get
            {
                return _ConfirmPasswordColor;
            }
            set
            {
                _ConfirmPasswordColor = value;
                OnPropertyChanged("ConfirmPasswordColor");
            }
        }

        private async void SignupClicked(object obj)
        {
            if (alreadyClicked)
            {
                string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
                alreadyClicked = false;
                IsWorking = true;
                if (string.IsNullOrEmpty(Email))
                {
                    EmailColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    DependencyService.Get<IMessage>().ShortAlert("Email is Empty");
                }
                else if (!Regex.IsMatch(Email, emailPattern))
                {
                    EmailColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    DependencyService.Get<IMessage>().ShortAlert("Invalid Email");
                }
                else if (string.IsNullOrEmpty(Password))
                {
                    PasswordColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    DependencyService.Get<IMessage>().ShortAlert("Password is Empty");
                }
                else if (string.IsNullOrEmpty(ConfirmPassword))
                {
                    ConfirmPasswordColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    DependencyService.Get<IMessage>().ShortAlert("Confirm Password is Empty");
                }
                else if (Password != ConfirmPassword)
                {
                    PasswordColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    ConfirmPasswordColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    DependencyService.Get<IMessage>().ShortAlert($"Password Did'nt Match");
                }
                else
                {
                    UserClass _obj = new UserClass()
                    {
                        Email = Email,
                        Password = Password
                    };

                    status = await ApiServices.Post(WebConfig.CreateAccount, _obj);
                    if (status != null)
                        data = JsonConvert.DeserializeObject<UserClass>(status.ToString());
                    if (data != null && data.Success == true)
                    {
                        var _status = await ApiServices.Post(WebConfig.ForgetPassword, _obj);
                        if (_status != null)
                        {
                            _data = JsonConvert.DeserializeObject<UserClass>(_status.ToString());
                        }
                        if (_data != null && _data.Success == true)
                        {
                            UserClass user = new UserClass()
                            {
                                Email = Email,
                                Password = Password,
                                NotificationKey = data.NotificationKey,
                                Token = data.Token,
                                License = data.License
                            };
                            await signupPage.DisplayAlert("Confirm Your Email", $"A security code has been sent to your email {Email}.", "Ok");
                            await signupPage.Navigation.PushAsync(new ConfirmCodePage(Email));
                        }
                    }
                    else if (data != null)
                        DependencyService.Get<IMessage>().ShortAlert(data.Message);
                }
                IsWorking = false;
                alreadyClicked = true;
            }
            IsWorking = false;
        }
    }
}