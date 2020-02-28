using Notification.Model;
using Notification.SQLite;
using Notification.Strings;
using Notification.View.Dashboard.SingleMessageViewer;
using Notification.View.Setting;
using Notification.ViewModel.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        private double _height;
        private bool alreadyClicked = true;
        public List<string> ListCheckBox;
        public int counter = 1;
        private List<MessageClass> _itemSource;
        private bool swiped;

        public DashboardPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new DashboardViewModel(this);
            ((DashboardViewModel)BindingContext).GetAllDataASync(true, true);
            firstcall = false;
        }

        #region DefualtCall

        private uint _time = 200;
        private int minheight = 93;
        private int maxheight = 120;
        private double rotationangle = 180;
        private bool firstcall;
        private bool _firstcall = true;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            _height = height;
            LowerStack.TranslateTo(0, _height - minheight, 10, Easing.Linear);
            Arrow.RotateTo(0, 10, Easing.Linear);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (NavStack.IsVisible)
            {
                ((DashboardViewModel)BindingContext).NavStackIsVisible = false;
                ((DashboardViewModel)BindingContext).CounterFuntion();
            }
            if (firstcall)
            {
                ((DashboardViewModel)BindingContext).GetAllDataASync(false, null);
            }
            firstcall = true;
        }
        #endregion

        #region SwipeGesture
        private async void UpSwiped(object sender, SwipedEventArgs e)
        {
            swiped = true;
            await LowerStack.TranslateTo(0, maxheight, _time, Easing.Linear);
            await Arrow.RotateTo(rotationangle, _time, Easing.Linear);
        }

        private async void DownSwiped(object sender, SwipedEventArgs e)
        {
            swiped = false;
            await LowerStack.TranslateTo(0, _height - minheight, _time, Easing.Linear);
            await Arrow.RotateTo(0, _time, Easing.Linear);
        }
        private async void LowerStackTapped(object sender, EventArgs e)
        {
            if (swiped)
            {
                await LowerStack.TranslateTo(0, _height - minheight, _time, Easing.Linear);
                await Arrow.RotateTo(0, _time, Easing.Linear);
                swiped = false;
            }
            else
            {
                await LowerStack.TranslateTo(0, maxheight, _time, Easing.Linear);
                await Arrow.RotateTo(rotationangle, _time, Easing.Linear);
                swiped = true;
            }
        }

        #endregion

        #region SettingImageClicked
        private async void SettingImgClicked(object sender, EventArgs e)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                ((DashboardViewModel)BindingContext).IsWorking = true;
                await Navigation.PushAsync(new SettingPage());
                ((DashboardViewModel)BindingContext).IsWorking = false;
                alreadyClicked = true;
            }
        }
        #endregion

        #region OnBackButtonPressed
        protected override bool OnBackButtonPressed()
        {
            if (((DashboardViewModel)BindingContext).NavStackIsVisible)
            {
                ((DashboardViewModel)BindingContext).IsPullToRefreshEnabled = true;
                ((DashboardViewModel)BindingContext).NavStackIsVisible = false;
                ((DashboardViewModel)BindingContext).SearchBarVisible = true;
                ((DashboardViewModel)BindingContext).SelectAllCheckBox = false;
                ((DashboardViewModel)BindingContext).CounterFuntion();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }
        #endregion

        #region MessageListViewItemLongPress
        private void Grid_LongPressing(object sender, MR.Gestures.LongPressEventArgs e)
        {
            if (!((DashboardViewModel)BindingContext).NavStackIsVisible)
            {
                ((DashboardViewModel)BindingContext).IsPullToRefreshEnabled = false;
                ((DashboardViewModel)BindingContext).NavStackIsVisible = true;
                ((DashboardViewModel)BindingContext).SearchBarVisible = false;
                ((DashboardViewModel)BindingContext).Counter = 1;
            }
            else
            {
                if (((Grid)sender).BackgroundColor == Color.FromHex("#590968A2"))
                {
                    ((DashboardViewModel)BindingContext).Counter--;
                }
                else
                {
                    ((DashboardViewModel)BindingContext).Counter++;
                }
            }
        }
        #endregion

        #region MessageListViewItemTapped
        private async void Grid_Tapped(object sender, MR.Gestures.TapEventArgs e)
        {
            if (_firstcall)
            {
                _firstcall = false;
                if (((DashboardViewModel)BindingContext).NavStackIsVisible)
                {
                    var id = Convert.ToInt32(((Grid)sender).ClassId);
                    if (((Grid)sender).BackgroundColor == Color.FromHex("#590968A2"))
                    {
                        ((DashboardViewModel)BindingContext).Counter--;
                    }
                    else
                    {
                        ((DashboardViewModel)BindingContext).Counter++;
                    }
                }
                else
                {
                    var classId = ((Grid)sender).ClassId;
                    var viewCellData = ((DashboardViewModel)BindingContext).ItemSource.Where(x => x.Id == Convert.ToInt32(classId)).FirstOrDefault();
                    await Navigation.PushAsync(new NotificationPage(classId));
                }
                _firstcall = true;
            }
        }
        #endregion

        #region LowerStackItemTapped
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var classId = ((Grid)sender).ClassId;
            ((DashboardViewModel)BindingContext).HeaderChange(classId);
            await LowerStack.TranslateTo(0, _height - minheight, _time, Easing.Linear);
            await Arrow.RotateTo(0, _time, Easing.Linear);
        }
        #endregion

        #region RoundedSearchBarTextChanged
        private async void RoundedSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            _itemSource = await SQLiteDatabase.GetAllTableDataAsync<MessageClass>();
            if (string.IsNullOrEmpty(SBText.Text))
            {
                ((DashboardViewModel)BindingContext).ItemSource = _itemSource;
            }
            else
            {
                ((DashboardViewModel)BindingContext).ItemSource = _itemSource.Where(
                    x => x.MessageTitle.ToLowerInvariant().Contains(SBText.Text.ToLowerInvariant()) ||
                    x.MessageFrom.ToLowerInvariant().Contains(SBText.Text.ToLowerInvariant()) ||
                    x.MessageBody.ToLowerInvariant().Contains(SBText.Text.ToLowerInvariant())
                ).ToList();
            }
        }
        #endregion
    }
}