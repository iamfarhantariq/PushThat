using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.SQLite;
using Notification.Strings;
using Notification.View.Dashboard.SingleMessageViewer;
using Notification.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Notification.ViewModel.Dashboard.SingleMessageViewer
{
    public class NotificationViewModel : BaseViewModel
    {
        private NotificationPage notificationPage;
        private string classId;

        public ICommand CopyCommand { get; private set; }
        public ICommand ShareCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public NotificationViewModel(NotificationPage notificationPage, string classId)
        {
            this.notificationPage = notificationPage;
            this.classId = classId;
            CopyCommand = new Command(CopyButtonClicked);
            ShareCommand = new Command(ShareButtonClicked);
            DeleteCommand = new Command(DeleteButtonClicked);
        }


        public string _ImageUrl;
        public string ImageUrl
        {
            get
            {
                return _ImageUrl;
            }
            set
            {
                _ImageUrl = value;
                OnPropertyChanged("ImageUrl");
            }
        }
        public string _From;
        public string From
        {
            get
            {
                return _From;
            }
            set
            {
                _From = value;
                OnPropertyChanged("From");
            }
        }
        public string _Date;
        public string Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
                OnPropertyChanged("Date");
            }
        }

        public string _MessageTitle;
        public string MessageTitle
        {
            get
            {
                return _MessageTitle;
            }
            set
            {
                _MessageTitle = value;
                OnPropertyChanged("MessageTitle");
            }
        }

        public string _Time;
        public string Time
        {
            get
            {
                return _Time;
            }
            set
            {
                _Time = value;
                OnPropertyChanged("Time");
            }
        }

        public string _MessageBody;
        private object status;
        private GetMessagesModel data;

        public string MessageBody
        {
            get
            {
                return _MessageBody;
            }
            set
            {
                _MessageBody = value;
                OnPropertyChanged("MessageBody");
            }
        }
        private async void ShareButtonClicked(object obj)
        {
            //await Share.RequestAsync( )

            await CrossShare.Current.Share(new ShareMessage
            {
                Text = $"{From}: {MessageBody} \nat {Date} {Time}"
            });
        }

        private async void DeleteButtonClicked(object obj)
        {
            var ans = await notificationPage.DisplayAlert("Delete?", "Do you want to delete this message?", "Yes", "Cancel");
            {
                if (ans)
                {
                    IsWorking = true;

                    var _data = await SQLiteDatabase.GetAllTableDataAsync<MessageClass>();
                    var viewCellData = _data.Where(x => x.MessageId == Convert.ToInt32(classId)).FirstOrDefault();

                    GetMessagesModel _obj = new GetMessagesModel()
                    {
                        NotificationKey = (string)Application.Current.Properties[NotificationStrings.NotificationKey],
                        Id = Convert.ToInt16(classId)
                    };
                    status = await ApiServices.Post(WebConfig.DeleteOne, _obj);
                    if (status != null)
                        data = JsonConvert.DeserializeObject<GetMessagesModel>(status.ToString());

                    if (viewCellData != null)
                    {
                        if (data != null && data.Success == true)
                        {
                            await SQLiteDatabase.DeleteAsync(viewCellData);
                            DependencyService.Get<IMessage>().ShortAlert("Deleted");
                            await notificationPage.Navigation.PopAsync();
                        }
                    }

                    IsWorking = false;
                }
            }
        }
        private async void CopyButtonClicked(object obj)
        {
            if ((bool)Application.Current.Properties[NotificationStrings.CopyMessagesTitle])
            {
                await Clipboard.SetTextAsync(MessageTitle + "\n" + MessageBody);
                DependencyService.Get<IMessage>().ShortAlert("Copied with Message Title");
            }
            else
            {
                await Clipboard.SetTextAsync(MessageBody);
                DependencyService.Get<IMessage>().ShortAlert("Copied");
            }
        }
    }
}
