using Notification.ViewModel.ForgetPassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.ForgetPassword
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgetPasswordPage : ContentPage
    {
        public ForgetPasswordPage()
        {
            InitializeComponent();
            BindingContext = new ForgetPasswordViewModel(this);
        }

        private void RoundedEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((ForgetPasswordViewModel)BindingContext).EmailColor = Color.Default;
        }
    }
}