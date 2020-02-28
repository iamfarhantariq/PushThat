using Notification.Model;
using Notification.SQLite;
using Notification.ViewModel.Dashboard;
using Notification.ViewModel.Dashboard.SingleMessageViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Dashboard.SingleMessageViewer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPage : ContentPage
    {
        public NotificationPage(string classId)
        {
            InitializeComponent();
            BindingContext = new NotificationViewModel(this, classId);
            ShowData(classId);
        }

        public NotificationPage(MessageClass msg)
        {
            InitializeComponent();
        }

        private async void ShowData(string classId)
        {
            if (classId != null)
            {
                var _data = await SQLiteDatabase.GetAllTableDataAsync<MessageClass>();
                var viewCellData = _data.Where(x => x.MessageId == Convert.ToInt32(classId)).FirstOrDefault();
                if (viewCellData != null)
                {
                    ((NotificationViewModel)BindingContext).From = viewCellData.MessageFrom;
                    ((NotificationViewModel)BindingContext).MessageTitle = viewCellData.MessageTitle;
                    ((NotificationViewModel)BindingContext).Date = viewCellData.MessageTime.ToString("dd MMMM yyyy");
                    ((NotificationViewModel)BindingContext).Time = viewCellData.MessageTime.ToString("hh:mm");
                    ((NotificationViewModel)BindingContext).MessageBody = viewCellData.MessageBody;
                    ((NotificationViewModel)BindingContext).ImageUrl = viewCellData.ImageUrl;
                }
            }
        }
    }
}