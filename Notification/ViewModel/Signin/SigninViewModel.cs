using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.Strings;
using Notification.View.AddDevice;
using Notification.View.ForgetPassword;
using Notification.View.Signin;
using Notification.View.Signup;
using Notification.ViewModel.Base;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notification.ViewModel.Signin
{
    public class SigninViewModel : BaseViewModel
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

        private SigninPage SigninPage;
        private object status;
        private UserClass data;

        public ICommand ForgetPasswordCommand { get; private set; }
        public ICommand SigninCommand { get; private set; }
        public ICommand CreateAccountCommand { get; private set; }

        public SigninViewModel(SigninPage signinPage)
        {
            SigninPage = signinPage;
            ForgetPasswordCommand = new Command(ForgetPassword);
            SigninCommand = new Command(Signin);
            CreateAccountCommand = new Command(CreateAccount);
        }

        private async void ForgetPassword(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                await SigninPage.Navigation.PushAsync(new ForgetPasswordPage(), true);
                alreadyClicked = true;
            }
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

        private async void Signin(object obj)
        {
            if (alreadyClicked)
            {
                string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
                alreadyClicked = false;

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
                else
                {
                    IsWorking = true;
                    var _obj = new UserClass()
                    {
                        Email = Email,
                        Password = Password
                    };
                    status = await ApiServices.Post(WebConfig.Login, _obj);
                    if (status != null)
                        data = JsonConvert.DeserializeObject<UserClass>(status.ToString());
                    if (data != null && data.Success == true)
                    {
                        Application.Current.Properties[NotificationStrings.Email] = Email;
                        Application.Current.Properties[NotificationStrings.Password] = Password;
                        Application.Current.Properties[NotificationStrings.NotificationKey] = data.NotificationKey;
                        Application.Current.Properties[NotificationStrings.Token] = data.Token;
                        Application.Current.Properties[NotificationStrings.DeviceLicenseStatus] = data.License;
                        await Application.Current.SavePropertiesAsync();

                        DependencyService.Get<IMessage>().ShortAlert("Success");
                        Application.Current.MainPage = new NavigationPage(new AddDevicePage());
                        await Application.Current.MainPage.Navigation.PopToRootAsync();
                    }
                    else if (data != null)
                    {
                        if (data.MessageCode == "PA104")
                        {
                            EmailColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                            PasswordColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                            DependencyService.Get<IMessage>().ShortAlert("Invalid Credentials");
                        }
                    }
                    IsWorking = false;
                }
                alreadyClicked = true;
            }
        }
        private async void CreateAccount(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                await SigninPage.Navigation.PushAsync(new SignupPage());
                alreadyClicked = true;
            }
        }
    }
}

