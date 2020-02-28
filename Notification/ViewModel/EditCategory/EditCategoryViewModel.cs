using Notification.Model;
using Notification.SQLite;
using Notification.View.EditCategory;
using Notification.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Notification.ViewModel.EditCategory
{
    public class EditCategoryViewModel : BaseViewModel
    {
        private EditCategoryPage editCategoryPage;
        private List<MessageClass> tableMessages;

        public EditCategoryViewModel(EditCategoryPage editCategoryPage)
        {
            this.editCategoryPage = editCategoryPage;
            GetCategories();

        }
        private async void GetCategories()
        {
            tableMessages = await SQLiteDatabase.GetAllTableDataAsync<MessageClass>();
            CategoryList = tableMessages
                .GroupBy(m => m.MessageFrom)
                .Select(g => new MessageClass()
                {
                    MessageFrom = g.Select(l => l.MessageFrom).Distinct().FirstOrDefault(),
                    ImageUrl = g.Select(l => l.ImageUrl).FirstOrDefault()
                }).ToList();
        }

        public void ItemTapped(string image, string from, string tone)
        {
            if (isTapped)
            {
                isTapped = false;
                editCategoryPage.Navigation.PushAsync(new ItemSelectedPage(image, from, tone));
                isTapped = true;
            }
        }

        public List<MessageClass> _CategoryList;
        private bool isTapped = true;

        public List<MessageClass> CategoryList
        {
            get
            {
                return _CategoryList;
            }

            set
            {
                _CategoryList = value;
                OnPropertyChanged("CategoryList");
            }
        }

    }
}
