using Notification.Helpers;
using Notification.Strings;
using Notification.View.Dashboard;
using Notification.View.ForgetPassword;
using Notification.View.Signup;
using Notification.ViewModel.Signin;
using System;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Signin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SigninPage : ContentPage
    {
        public SigninPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new SigninViewModel(this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((SigninViewModel)BindingContext).Email = string.Empty;
            ((SigninViewModel)BindingContext).Password = string.Empty;
        }

        private void RoundedEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((SigninViewModel)BindingContext).EmailColor = Color.Default;
            ((SigninViewModel)BindingContext).PasswordColor = Color.Default;
        }
    }
}