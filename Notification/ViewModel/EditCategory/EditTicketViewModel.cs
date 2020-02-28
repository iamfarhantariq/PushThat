using Notification.Model;
using Notification.SQLite;
using Notification.View.EditCategory;
using Notification.View.Setting.AlertSetting;
using Notification.ViewModel.Base;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notification.ViewModel.EditCategory
{
    public class EditTicketViewModel : BaseViewModel
    {
        private EditTicketPage editTicketPage;
        private bool alreadyClicked = true;
        private List<MessageClass> tableMessages;

        public EditTicketViewModel(EditTicketPage editTicketPage)
        {
            this.editTicketPage = editTicketPage;
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

        public async void ItemTapped(string image, string tone, string from)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                await editTicketPage.Navigation.PushAsync(new ItemSelectedPage(image, from, tone));
                alreadyClicked = true;
            }
        }

        public List<MessageClass> _CategoryList;
        public List<MessageClass> CategoryList
        {
            get
            {
                return _CategoryList;
            }

            set
            {
                _CategoryList = value.Where(x => x.MessageType == "Ticket").ToList();
                OnPropertyChanged("CategoryList");
            }
        }
    }
}
