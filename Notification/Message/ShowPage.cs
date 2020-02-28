using Notification.Model;
using Notification.View.Dashboard.SingleMessageViewer;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Notification.Message
{
    public class ShowPage
    {
        public static void PushMessage(MessageClass pushMessage)
        {
            Application.Current.MainPage.Navigation.PushAsync(new NotificationPage(pushMessage));
        }
    }
}
