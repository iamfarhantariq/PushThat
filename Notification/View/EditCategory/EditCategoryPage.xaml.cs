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
    public partial class EditCategoryPage : ContentPage
    {
        public EditCategoryPage()
        {
            InitializeComponent();
            BindingContext = new EditCategoryViewModel(this);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var image = ((MessageClass)ListViewCategories.SelectedItem).ImageUrl;
            var from = ((MessageClass)ListViewCategories.SelectedItem).MessageFrom;
            var tone = ((MessageClass)ListViewCategories.SelectedItem).DefualtTone;
            ((EditCategoryViewModel)BindingContext).ItemTapped(image, from, tone);
        }
    }
}