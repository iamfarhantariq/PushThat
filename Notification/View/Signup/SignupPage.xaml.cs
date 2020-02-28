using Notification.ViewModel.Signup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Signup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {
        public SignupPage()
        {
            InitializeComponent();
            BindingContext = new SignupViewModel(this);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((SignupViewModel)BindingContext).Email = string.Empty;
            ((SignupViewModel)BindingContext).Password = string.Empty;
            ((SignupViewModel)BindingContext).ConfirmPassword = string.Empty;
        }

        private void RoundedEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((SignupViewModel)BindingContext).EmailColor = Color.Default;
            ((SignupViewModel)BindingContext).PasswordColor = Color.Default;
            ((SignupViewModel)BindingContext).ConfirmPasswordColor = Color.Default;
        }
    }
}