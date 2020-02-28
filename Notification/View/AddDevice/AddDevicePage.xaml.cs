using Notification.Helpers;
using Notification.Strings;
using Notification.ViewModel.AddDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.AddDevice
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDevicePage : ContentPage
    {
        public AddDevicePage()
        {
            InitializeComponent();
            BindingContext = new AddDeviceViewModel(this);
        }

        private void RoundedEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((AddDeviceViewModel)BindingContext).AddDeviceColor = Color.Default;
        }
    }
}