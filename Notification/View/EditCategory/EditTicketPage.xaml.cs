using Notification.Model;
using Notification.ViewModel.EditCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.EditCategory
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTicketPage : ContentPage
    {
        public EditTicketPage()
        {
            InitializeComponent();
            BindingContext = new EditTicketViewModel(this);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var from = ((MessageClass)ListViewCategories.SelectedItem).MessageFrom;
            var tone = ((MessageClass)ListViewCategories.SelectedItem).DefualtTone;
            var image = ((MessageClass)ListViewCategories.SelectedItem).ImageUrl;
            ((EditTicketViewModel)BindingContext).ItemTapped(image, tone, from);
        }
    }
}