using Notification.Model;
using Notification.SQLite;
using Notification.Strings;
using Notification.View.Dashboard;
using Notification.ViewModel.Base;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using Notification.Services;
using Newtonsoft.Json;
using Notification.Helpers;

namespace Notification.ViewModel.Dashboard
{
    public class DashboardViewModel : BaseViewModel
    {
        public List<int> classIds = new List<int>();
        private GetMessagesModel data;
        private object status;
        private bool clicked = true;
        public DashboardPage dashboardPage;
#pragma warning disable IDE0052 // Remove unread private members
        readonly SQLiteDatabase database = new SQLiteDatabase();
#pragma warning restore IDE0052 // Remove unread private members
        private List<MessageClass> tableMessages;
        public ICommand DeleteTappedCommand { get; private set; }
        public ICommand CancelTappedCommand { get; private set; }
        public ICommand StackLayoutCellTappedCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public DashboardViewModel(DashboardPage dashboardPage)
        {
            this.dashboardPage = dashboardPage;
            DeleteTappedCommand = new Command(DeleteTapped);
            CancelTappedCommand = new Command(CancelTapped);
            RefreshCommand = new Command(Refresh);
            FetchMessages();
        }

        #region GetAllDataASync
        public async void GetAllDataASync(bool datasync, bool? loading)
        {
            SearchBarVisible = true;
            tableMessages = await SQLiteDatabase.GetAllTableDataAsync<MessageClass>();
            HeaderChange("AllMessages");

            if (datasync)
            {
                if (loading == true)
                    IsWorking = true;
                GetMessagesModel _obj = new GetMessagesModel()
                {
                    NotificationKey = (string)Application.Current.Properties[NotificationStrings.NotificationKey]
                };
                var status = await ApiServices.Post(WebConfig.AllMessagesAsync, _obj);
                if (status != null)
                {
                    data = JsonConvert.DeserializeObject<GetMessagesModel>((string)status);
                }
                if (data != null && data.Success == true)
                {
                    foreach (var item in data.MessageList)
                    {
                        MessageClass message = new MessageClass()
                        {
                            MessageId = item.Id,
                            MessageTitle = item.MessageTitle,
                            MessageFrom = item.From,
                            MessageBody = item.MessageBody,
                            ImageUrl = item.Image,
                            MessageTime = item.Date,
                        };
                        var alreadyFound = tableMessages.Where(x => x.MessageId == item.Id).FirstOrDefault();
                        if (alreadyFound == null)
                            await SQLiteDatabase.InsertAsync(message);
                    }
                    tableMessages = await SQLiteDatabase.GetAllTableDataAsync<MessageClass>();
                }
                if (loading == true)
                    IsWorking = false;
            }

            HeaderChange("AllMessages");
            CategoriesList = tableMessages
                .GroupBy(m => m.MessageFrom)
                .Select(g => new MessageClass()
                {
                    MessageFrom = g.Select(l => l.MessageFrom).Distinct().FirstOrDefault(),
                    ImageUrl = g.Select(l => l.ImageUrl).FirstOrDefault(),
                    Count = g.Select(l => l.MessageId).Distinct().Count()
                }).ToList();
        }
        #endregion

        #region CancelTapped
        private void CancelTapped(object obj)
        {
            IsPullToRefreshEnabled = true;
            NavStackIsVisible = false;
            SearchBarVisible = true;
            CounterFuntion();
            SelectAllCheckBox = false;
        }
        #endregion

        #region HeaderChange
        public async void HeaderChange(string classId)
        {
            if (classId != "AllMessages")
            {
                List<MessageClass> sorted = tableMessages.Where(x => x.MessageFrom == classId).ToList();
                CategoryHeader = sorted[0].MessageFrom;
                LblHeaderCount = sorted.Count;
                ItemSource = tableMessages.Where(x => x.MessageFrom == classId).ToList();
                await Task.Delay(200);
            }
            if (classId == "AllMessages")
            {
                CategoryHeader = "All Messages";
                LblHeaderCount = tableMessages.Count;
                AllMessagesCount = tableMessages.Count;
                ItemSource = tableMessages;
                await Task.Delay(200);
            }
            if (tableMessages.Count == 0)
            {
                MessageListViewIsVisible = false;
                NoMessageImage = true;
            }
            else
            {
                MessageListViewIsVisible = true;
                NoMessageImage = false;
            }
        }
        #endregion

        #region CounterFuntion
        public void CounterFuntion()
        {
            if (ItemSource != null)
            {
                foreach (var item in ItemSource)
                {
                    item.BGColor = Color.Transparent;
                }
            }
            OnPropertyChanged("ItemSource");
            Counter = 0;
        }
        #endregion

        #region IsRefreshing
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        #endregion

        #region SearchBarVisible
        public bool _SearchBarVisible;
        public bool SearchBarVisible { get { return _SearchBarVisible; } set { _SearchBarVisible = value; OnPropertyChanged("SearchBarVisible"); } }

        #endregion

        #region MessageListViewIsVisible
        public bool _MessageListViewIsVisible = false;
        public bool MessageListViewIsVisible { get { return _MessageListViewIsVisible; } set { _MessageListViewIsVisible = value; OnPropertyChanged("MessageListViewIsVisible"); } }

        #endregion

        #region NoMessageImage
        public bool _NoMessageImage = false;
        public bool NoMessageImage { get { return _NoMessageImage; } set { _NoMessageImage = value; OnPropertyChanged("NoMessageImage"); } }

        #endregion

        #region IsPullToRefreshEnabled
        public bool _IsPullToRefreshEnabled = true;
        public bool IsPullToRefreshEnabled { get { return _IsPullToRefreshEnabled; } set { _IsPullToRefreshEnabled = value; OnPropertyChanged("IsPullToRefreshEnabled"); } }

        #endregion

        #region NavStack
        public bool _NavStackIsVisible;
        public bool NavStackIsVisible { get { return _NavStackIsVisible; } set { _NavStackIsVisible = value; OnPropertyChanged("NavStackIsVisible"); } }
        #endregion

        #region SelectAllCheckBox
        public bool _SelectAllCheckBox;
        public bool SelectAllCheckBox
        {
            get { return _SelectAllCheckBox; }
            set
            {
                if (value)
                {
                    foreach (var item in ItemSource)
                    {
                        item.BGColor = Color.FromHex("#590968A2");
                    }
                    Counter = ItemSource.Count;
                }
                else
                {
                    foreach (var item in ItemSource)
                    {
                        item.BGColor = Color.Transparent;
                    }
                    Counter = 0;
                }
                _SelectAllCheckBox = value;
                OnPropertyChanged("SelectAllCheckBox");
                OnPropertyChanged("ItemSource");
            }
        }
        #endregion

        #region AllMessagesCount

        public int _AllMessagesCount;
        public int AllMessagesCount
        {
            get { return _AllMessagesCount; }
            set
            {
                _AllMessagesCount = value;
                OnPropertyChanged("AllMessagesCount");
            }
        }
        #endregion

        #region Counter
        public int _Counter;
        public int Counter
        {
            get { return _Counter; }
            set
            {
                _Counter = value;
                OnPropertyChanged("Counter");
            }
        }
        #endregion

        #region CategoryHeader
        public string _CategoryHeader;
        public string CategoryHeader
        {
            get
            {
                if (_CategoryHeader == null)
                {
                    _CategoryHeader = "All Messages";
                }
                return _CategoryHeader;
            }
            set
            {
                _CategoryHeader = value;
                OnPropertyChanged("CategoryHeader");
            }
        }
        #endregion

        #region LblHeaderCount
        public int _LblHeaderCount;
        public int LblHeaderCount
        {
            get { return _LblHeaderCount; }
            set
            {
                _LblHeaderCount = value;
                OnPropertyChanged("LblHeaderCount");
            }
        }
        #endregion

        #region CategoriesList
        public List<MessageClass> _CategoriesList;
        public List<MessageClass> CategoriesList
        {
            get
            {
                return _CategoriesList;
            }

            set
            {
                _CategoriesList = value;
                OnPropertyChanged("CategoriesList");
            }
        }
        #endregion

        #region ItemSource
        public List<MessageClass> _ItemSource;
        public List<MessageClass> ItemSource
        {
            get
            {
                return _ItemSource;
            }
            set
            {
                if (value != null)
                    _ItemSource = value.OrderByDescending(x => x.MessageId).ToList();
                OnPropertyChanged("ItemSource");
            }
        }
        #endregion

        #region ImageUrl
        public string _ImageUrl;
        public string ImageUrl { get { return _ImageUrl; } set { _ImageUrl = value; OnPropertyChanged("ImageUrl"); } }
        #endregion

        #region MessageFrom
        public string _MessageFrom;
        public string MessageFrom { get { return _MessageFrom; } set { _MessageFrom = value; OnPropertyChanged("MessageFrom"); } }
        #endregion

        #region Count
        public int _Count;
        public int Count { get { return _Count; } set { _Count = value; OnPropertyChanged("Count"); } }
        #endregion

        #region Refresh
        public string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set { _SearchText = value; OnPropertyChanged("SearchText"); }
        }
        private void Refresh(object obj)
        {
            if (clicked)
            {
                clicked = false;
                IsRefreshing = true;
                SearchText = string.Empty;
                GetAllDataASync(true, false);
                DependencyService.Get<IMessage>().ShortAlert("Synced");
                IsRefreshing = false;
                clicked = true;
            }
        }
        #endregion

        #region DeleteTapped


        private async void DeleteTapped(object obj)
        {
            var ans = await dashboardPage.DisplayAlert("Delete?", $"Do you want to delete {Counter} messages?", "Yes", "Cancel");
            {
                if (ans)
                {
                    IsWorking = true;

                    var _list = ItemSource.Where(x => x.BGColor == Color.FromHex("#590968A2")).ToList();
                    if (_list != null)
                    {
                        IsWorking = true;
                        foreach (var item in _list)
                        {
                            classIds.Add(item.MessageId);
                        }
                        GetMessagesModel _obj = new GetMessagesModel()
                        {
                            NotificationKey = (string)Application.Current.Properties[NotificationStrings.NotificationKey],
                            Ids = classIds
                        };

                        if (_list.Count == Counter)
                            status = await ApiServices.Post(WebConfig.DeleteMultiple, _obj);
                        else
                            status = await ApiServices.Post(WebConfig.DeleteAll, _obj);

                        if (status != null)
                            data = JsonConvert.DeserializeObject<GetMessagesModel>(status.ToString());
                        if (data != null && data.Success == true)
                        {
                            foreach (var item in _list)
                            {
                                await SQLiteDatabase.DeleteAsync(item);
                            }

                            DependencyService.Get<IMessage>().ShortAlert("Deleted");
                            NavStackIsVisible = false;
                            SearchBarVisible = true;
                            CounterFuntion();
                            SelectAllCheckBox = false;
                            GetAllDataASync(false, null);
                        }
                        IsWorking = false;
                    }
                }
            }
        }
        #endregion

        #region FetchMessages
        private void FetchMessages()
        {
            IsWorking = true;



            IsWorking = false;
        }
        #endregion

    }
}
