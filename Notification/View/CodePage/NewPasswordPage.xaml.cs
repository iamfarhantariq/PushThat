using Notification.ViewModel.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.CodePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPasswordPage : ContentPage
    {
        public NewPasswordPage(string _Email)
        {
            InitializeComponent();
            BindingContext = new NewPasswordViewModel(this, _Email);
        }

        private void RoundedEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((NewPasswordViewModel)BindingContext).PasswordColor = Color.Default;
        }
    }
}