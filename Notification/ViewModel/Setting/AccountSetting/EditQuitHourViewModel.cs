using Notification.Helpers;
using Notification.Model;
using Notification.SQLite;
using Notification.View.Setting.AccountSettings;
using Notification.ViewModel.Base;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notification.ViewModel.Setting.AccountSetting
{
    public class EditQuitHourViewModel : BaseViewModel
    {
        private EditQuitHourPage editQuitHourPage;
        private List<EditQuitHoursModel> quitHoursModels = null;
        private List<EditQuitHoursModel> wholedaylist;
        private List<EditQuitHoursModel> customHourslist;
        private List<EditQuitHoursModel> customDayHourslist;
        private string[] images;
        private List<string> days;
        private List<string> imgs;
        private string prevDay = string.Empty;
        private string categoryActive = string.Empty;
        public ICommand WholeDaysTappedCommand { get; private set; }
        public ICommand CustomHoursTappedCommand { get; private set; }
        public ICommand CustomDaysHoursTappedCommand { get; private set; }
        public ICommand ImageTappedCommand { get; private set; }
        public ICommand StartTimeTappedCommand { get; private set; }
        public ICommand EndTimeTappedCommand { get; private set; }

        public EditQuitHourViewModel(EditQuitHourPage editQuitHourPage)
        {
            this.editQuitHourPage = editQuitHourPage;
            WholeDaysTappedCommand = new Command(WholeDaysTapped);
            CustomHoursTappedCommand = new Command(CustomHoursTapped);
            CustomDaysHoursTappedCommand = new Command(CustomDaysHoursTapped);
            ImageTappedCommand = new Command(ImageTapped);
            StartTimeTappedCommand = new Command(StartTimeTapped);
            EndTimeTappedCommand = new Command(EndTimeTapped);

            GetData();
            ShowSchedule(0);
        }
        private async void GetData()
        {
            images = new string[7] { MondayImage, TuesdayImage, WednesdayImage, ThursdayImage, FridayImage, SaturdayImage, SundayImage };
            days = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            imgs = new List<string>() { "LMon.png", "LTue.png", "LWed.png", "LThu.png", "LFri.png", "LSat.png", "LSun.png" };

            //new SQLiteDatabase();
            quitHoursModels = await SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>();

            //Defualt Images Source
            if (quitHoursModels.Count == 0)
            {
                for (int i = 0; i < days.Count(); i++)
                {
                    EditQuitHoursModel hoursModel = new EditQuitHoursModel()
                    {
                        Category = "WholeDay",
                        Day = days[i],
                        Active = false,
                        Image = imgs[i]
                    };
                    await SQLiteDatabase.InsertAsync(hoursModel);
                }

                for (int i = 0; i < days.Count(); i++)
                {
                    EditQuitHoursModel hoursModel = new EditQuitHoursModel()
                    {
                        Category = "CustomHours",
                        Day = days[i],
                        Active = false,
                        Image = imgs[i],
                        StartTime = new TimeSpan(12, 00, 00),
                        EndTime = new TimeSpan(12, 00, 00)

                    };
                    await SQLiteDatabase.InsertAsync(hoursModel);
                }

                for (int i = 0; i < days.Count(); i++)
                {
                    EditQuitHoursModel hoursModel = new EditQuitHoursModel()
                    {
                        Category = "CustomDaysHour",
                        Day = days[i],
                        Active = false,
                        Image = imgs[i],
                        StartTime = new TimeSpan(12, 00, 00),
                        EndTime = new TimeSpan(12, 00, 00)
                    };
                    await SQLiteDatabase.InsertAsync(hoursModel);
                }
                quitHoursModels = await SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>();
            }

            if (quitHoursModels.Count == 21)
                SQLiteDatabase.Dispose().Wait();
        }
        private void ImageTapped(object obj)
        {
            var imageButton = obj as ImageButton;
            if (imageButton.ClassId == string.Empty)
                return;
            int _classId = Convert.ToInt16(imageButton.ClassId);

            switch (_classId)
            {
                case 1:
                    MondayImage = ImageProcessing("Monday", MondayImage, categoryActive);
                    break;

                case 2:
                    TuesdayImage = ImageProcessing("Tuesday", TuesdayImage, categoryActive);
                    break;

                case 3:
                    WednesdayImage = ImageProcessing("Wednesday", WednesdayImage, categoryActive);
                    break;

                case 4:
                    ThursdayImage = ImageProcessing("Thursday", ThursdayImage, categoryActive);
                    break;

                case 5:
                    FridayImage = ImageProcessing("Friday", FridayImage, categoryActive);
                    break;

                case 6:
                    SaturdayImage = ImageProcessing("Saturday", SaturdayImage, categoryActive);
                    break;

                case 7:
                    SundayImage = ImageProcessing("Sunday", SundayImage, categoryActive);
                    break;
            }
        }
        private string ImageProcessing(string day, string sourceImage, string categoryActive)
        {
            bool? active;
            string prevSouce = sourceImage;
            string source;
            if (sourceImage[0] == 'L')
            {
                source = $"D{day.Substring(0, 3)}.png";
                active = true;
            }
            else
            {
                source = $"L{day.Substring(0, 3)}.png";
                active = false;
            }

            if (categoryActive == "CustomDaysHour" && ItemSource.Count != 0)
            {
                if (sourceImage[0] == 'D')
                {
                    source = $"L{day.Substring(0, 3)}.png";
                    active = false;
                }
                else
                {
                    var response = Validate();
                    if (!response)
                        return prevSouce;
                }
            }

            UpdateToDatabase(day, source, active, categoryActive, $"{new TimeSpan(12, 00, 00)} to {new TimeSpan(12, 00, 00)}", new TimeSpan(12, 00, 00), new TimeSpan(12, 00, 00));
            return source;
        }
        public async void UpdateToDatabase(string day, string sourceImage, bool? active, string categoryActive, string labelMessage, TimeSpan startTime, TimeSpan endTime)
        {
            quitHoursModels = await SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>();

            var target = quitHoursModels.Where(x => x.Category == categoryActive && x.Day == day).FirstOrDefault();

            if (target == null)
                return;

            EditQuitHoursModel update = new EditQuitHoursModel()
            {
                Id = target.Id,
                Category = categoryActive,
                Image = sourceImage,
                Active = (bool)active,
                Day = day,
                LabelMessage = labelMessage,
                StartTime = startTime,
                EndTime = endTime,
            };

            if (target.StartTime == update.StartTime &&
                target.EndTime == update.EndTime &&
                target.Active == update.Active)
                return;
            else
                SQLiteDatabase.UpdateAsync(update).Wait();

            if (categoryActive == "CustomDaysHour")
            {
                var data = await SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>();
                customDayHourslist = data.Where(x => x.Category == "CustomDaysHour" && x.Active == true).ToList();
                if (customDayHourslist.Count == 0)
                { LvIsVisible = false; return; }
                else
                {
                    LvIsVisible = true;
                    ListViewHeightSet();
                }
                ItemSource = customDayHourslist;
            }
            await SQLiteDatabase.Dispose();
        }
        private void ListViewHeightSet()
        {
            var _count = customDayHourslist.Count;
            CHDHeightRequest = 45 * _count;
        }
        private void ShowImages(string v)
        {
            var list = quitHoursModels.Where(x => x.Category == v).ToList();

            MondayImage = list[0].Image;
            TuesdayImage = list[1].Image;
            WednesdayImage = list[2].Image;
            ThursdayImage = list[3].Image;
            FridayImage = list[4].Image;
            SaturdayImage = list[5].Image;
            SundayImage = list[6].Image;
        }
        private async void ShowSchedule(int call)
        {
            quitHoursModels = await SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>();

            #region WholeDay
            if (call == 1 || call == 0)
            {
                wholedaylist = quitHoursModels.Where(x => x.Category == "WholeDay" && x.Active == true).ToList();
                if (wholedaylist.Count != 0)
                {
                    var stringWD = string.Empty;
                    foreach (var item in wholedaylist)
                    {
                        stringWD += $"{item.Day.Substring(0, 3)}, ";
                    }
                    WholeDayText = stringWD.Substring(0, stringWD.Length - 2);
                }
                else
                    WholeDayText = "Set Schedule";

                await Task.Delay(250);
            }
            #endregion

            #region CustomHours
            if (call == 2 || call == 0)
            {


                customHourslist = quitHoursModels.Where(x => x.Category == "CustomHours" && x.Active == true).ToList();
                if (customHourslist.Count != 0)
                {
                    var time = string.Empty;
                    var stringCH = string.Empty;
                    foreach (var item in customHourslist)
                    {
                        stringCH += $"{item.Day.Substring(0, 3)}, ";
                        time = item.LabelMessage;
                    }
                    stringCH = stringCH.Substring(0, stringCH.Length - 2);
                    CustomHoursText = $"{stringCH} (from {customHourslist[0].StartTime} to {customHourslist[0].EndTime})";
                }
                else
                    CustomHoursText = "Set Schedule";

                await Task.Delay(250);
            }
            #endregion

            #region CustomDaysHour
            if (call == 3 || call == 0)
            {
                customDayHourslist = quitHoursModels.Where(x => x.Category == "CustomDaysHour" && x.Active == true).ToList();
                if (customDayHourslist.Count != 0)
                {
                    var stringCH = string.Empty;
                    foreach (var item in customDayHourslist)
                    {
                        stringCH += $"{item.Day} ({item.LabelMessage}), ";
                    }
                    stringCH = stringCH.Substring(0, stringCH.Length - 2);
                    CustomDayHoursText = stringCH;
                }
                else
                    CustomDayHoursText = "Set Schedule";

                await Task.Delay(250);
            }
            #endregion

            if (quitHoursModels.Count == 21)
                await SQLiteDatabase.Dispose();
        }

        public double _CHDHeightRequest;
        public double CHDHeightRequest
        {
            get { return _CHDHeightRequest; }
            set { _CHDHeightRequest = value; OnPropertyChanged("CHDHeightRequest"); }
        }

        #region ItemSource
        public List<EditQuitHoursModel> _ItemSource;
        public List<EditQuitHoursModel> ItemSource
        {
            get { return _ItemSource; }
            set { _ItemSource = value; OnPropertyChanged("ItemSource"); }
        }
        #endregion

        #region TimePicker
        private void StartTimeTapped(object obj)
        {
            var startTimePicker = obj as TimePicker;
            startTimePicker.Focus();
            startTimePicker.PropertyChanged += StartTimePicker_PropertyChanged;
            startTimePicker.Unfocus();
        }
        private void StartTimePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (categoryActive == "CustomHours")
            //{
            CHStartTime = ((TimePicker)sender).Time.ToString();
            //    var target = quitHoursModels.Where(x => x.Category == categoryActive && x.Active == true).ToList();
            //    for (int i = 0; i < target.Count(); i++)
            //    {
            //        UpdateToDatabase(target[i].Day, target[i].Image, target[i].Active, categoryActive, $"{TimeSpan.Parse(CHStartTime)} to {TimeSpan.Parse(CHEndTime)}", TimeSpan.Parse(CHStartTime), TimeSpan.Parse(CHEndTime));
            //    }
            //}
        }
        private void EndTimeTapped(object obj)
        {
            var endTimePicker = obj as TimePicker;
            endTimePicker.Focus();
            endTimePicker.PropertyChanged += EndTimePicker_PropertyChanged;
            endTimePicker.Unfocus();
        }
        private void EndTimePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (categoryActive == "CustomHours")
            //{
            CHEndTime = ((TimePicker)sender).Time.ToString();
            //    var target = quitHoursModels.Where(x => x.Category == categoryActive && x.Active == true).ToList();
            //    for (int i = 0; i < target.Count(); i++)
            //    {
            //        UpdateToDatabase(target[i].Day, target[i].Image, target[i].Active, categoryActive, $"{TimeSpan.Parse(CHStartTime)} to {TimeSpan.Parse(CHEndTime)}", TimeSpan.Parse(CHStartTime), TimeSpan.Parse(CHEndTime));
            //    }
            //}
        }
        #endregion

        #region CategoriesTapped
        private void WholeDaysTapped(object obj)
        {
            IsWorking = true;
            var _view = obj as Grid;
            var _status = Validate();
            if (!_status)
            {
                IsWorking = false;
                return;
            }
            IsVisibleCH = false;
            IsVisibleCDH = false;
            if (_view.IsVisible)
            {
                categoryActive = string.Empty;
                IsVisibleWD = false;
                ShowSchedule(1);
            }
            else
            {
                categoryActive = "WholeDay";
                IsVisibleWD = true;
                WholeDayText = "Set Schedule";
                ShowSchedule(2);
                ShowSchedule(3);
                ShowImages("WholeDay");
            }
            IsWorking = false;
        }
        private void CustomHoursTapped(object obj)
        {
            IsWorking = true;
            var _view = obj as Grid;
            var _status = Validate();
            if (!_status)
            {
                IsWorking = false;
                return;
            }
            IsVisibleWD = false;
            IsVisibleCDH = false;
            if (_view.IsVisible)
            {
                categoryActive = string.Empty;
                IsVisibleCH = false;
                ShowSchedule(2);
            }
            else
            {
                categoryActive = "CustomHours";
                IsVisibleCH = true;
                CustomHoursText = "Set Schedule";
                var _customHourslist = quitHoursModels.Where(x => x.Category == "CustomHours" && x.Active == true).FirstOrDefault();
                if (_customHourslist != null)
                {
                    if (_customHourslist.StartTime != new TimeSpan() || _customHourslist.EndTime != new TimeSpan())
                    {
                        CHStartTime = _customHourslist.StartTime.ToString();
                        CHEndTime = _customHourslist.EndTime.ToString();
                    }
                }
                else
                {
                    CHStartTime = "12:00:00";
                    CHEndTime = "12:00:00";
                }
                ShowSchedule(1);
                ShowSchedule(3);
                ShowImages("CustomHours");
            }
            IsWorking = false;
        }
        private void CustomDaysHoursTapped(object obj)
        {
            IsWorking = true;
            var _view = obj as Grid;
            var _status = Validate();
            if (!_status)
            {
                IsWorking = false;
                return;
            }
            IsVisibleWD = false;
            IsVisibleCH = false;
            if (_view.IsVisible)
            {
                categoryActive = string.Empty;
                IsVisibleCDH = false;
                ShowSchedule(3);
            }
            else
            {
                categoryActive = "CustomDaysHour";
                IsVisibleCDH = true;
                CustomDayHoursText = "Set Schedule";
                ShowSchedule(1);
                ShowSchedule(2);
                ShowImages("CustomDaysHour");
                ItemSource = quitHoursModels.Where(x => x.Category == "CustomDaysHour" && x.Active == true).ToList();
                if (ItemSource.Count == 0)
                    LvIsVisible = false;
                else
                {
                    LvIsVisible = true;
                    ListViewHeightSet();
                }
            }
            IsWorking = false;
        }
        private bool Validate()
        {
            quitHoursModels = SQLiteDatabase.GetAllTableDataAsync<EditQuitHoursModel>().Result;
            if (categoryActive == "CustomHours")
            {
                var startTime = TimeSpan.Parse(CHStartTime);
                var endTime = TimeSpan.Parse(CHEndTime);

                var data = quitHoursModels.Where(x => x.Category == categoryActive && x.Active == true).ToList();
                if (data.Count == 0)
                    return true;

                if (startTime != new TimeSpan() && endTime != new TimeSpan())
                {
                    var diff = endTime.Subtract(startTime).TotalMinutes;
                    if (diff == 0)
                    {
                        DependencyService.Get<IMessage>().ShortAlert("Start and End Time are same!");
                        return false;
                    }
                    if (diff >= 1400)
                    {
                        DependencyService.Get<IMessage>().ShortAlert("Start and End Time are invalid!");
                        return false;
                    }
                }

                if (startTime == new TimeSpan() && endTime == new TimeSpan())
                {
                    DependencyService.Get<IMessage>().ShortAlert("Start Time and End Time are not Set!");
                    return false;
                }

                if (startTime == new TimeSpan() || startTime == new TimeSpan(12, 00, 00))
                {
                    DependencyService.Get<IMessage>().ShortAlert("Start Time is not Set!");
                    return false;
                }

                if (endTime == new TimeSpan() || endTime == new TimeSpan(12, 00, 00))
                {
                    DependencyService.Get<IMessage>().ShortAlert("End Time is not Set!");
                    return false;
                }

                var target = quitHoursModels.Where(x => x.Category == categoryActive && x.Active == true).ToList();
                for (int i = 0; i < target.Count(); i++)
                {
                    UpdateToDatabase(target[i].Day, target[i].Image, target[i].Active, categoryActive, $"{TimeSpan.Parse(CHStartTime)} to {TimeSpan.Parse(CHEndTime)}", TimeSpan.Parse(CHStartTime), TimeSpan.Parse(CHEndTime));
                }

                return true;
            }

            var sat = true;
            if (categoryActive == "CustomDaysHour")
            {
                ItemSource = quitHoursModels.Where(x => x.Category == "CustomDaysHour" && x.Active == true).ToList();

                for (int i = 0; i < ItemSource.Count; i++)
                {
                    if (ItemSource[i].StartTime == new TimeSpan() && ItemSource[i].EndTime == new TimeSpan())
                    {
                        DependencyService.Get<IMessage>().ShortAlert($"Start Time and End Time are not Set for {ItemSource[i].Day}!");
                        Task.Delay(500);
                        sat = false;
                        break;
                    }

                    if (ItemSource[i].StartTimeS == "Set Start Time" && ItemSource[i].EndTimeS == "Set End Time")
                    {
                        DependencyService.Get<IMessage>().ShortAlert($"Start Time and End Time are not Set for {ItemSource[i].Day}!");
                        Task.Delay(500);
                        sat = false;
                        break;
                    }

                    if (ItemSource[i].StartTime == new TimeSpan() || ItemSource[i].StartTime == new TimeSpan(12, 00, 00) || ItemSource[i].StartTimeS == "Set Start Time")
                    {
                        DependencyService.Get<IMessage>().ShortAlert($"Start Time is not Set for {ItemSource[i].Day}!");
                        Task.Delay(500);
                        sat = false;
                        break;
                    }

                    if (ItemSource[i].EndTime == new TimeSpan() || ItemSource[i].EndTime == new TimeSpan(12, 00, 00) || ItemSource[i].EndTimeS == "Set End Time")
                    {
                        DependencyService.Get<IMessage>().ShortAlert($"End Time is not Set for {ItemSource[i].Day}!");
                        Task.Delay(500);
                        sat = false;
                        break;
                    }

                    if (ItemSource[i].StartTime != new TimeSpan() && ItemSource[i].EndTime != new TimeSpan())
                    {
                        var diff = ItemSource[i].EndTime.Subtract(ItemSource[i].StartTime).TotalMinutes;
                        if (diff == 0)
                        {
                            DependencyService.Get<IMessage>().ShortAlert($"Start and End Time are same for {ItemSource[i].Day}!");
                            Task.Delay(500);
                            sat = false;
                            break;
                        }
                        if (diff >= 1400)
                        {
                            DependencyService.Get<IMessage>().ShortAlert($"Start and End Time are invalid for {ItemSource[i].Day}!");
                            Task.Delay(500);
                            sat = false;
                            break;
                        }
                    }
                }
            }

            SQLiteDatabase.Dispose().Wait();

            if (sat == false)
                return false;
            else
                return true;

        }
        #endregion    

        #region StartTimeEndTime

        public TimeSpan _StartTime = new TimeSpan(12, 00, 00);
        public TimeSpan StartTimeProperty
        {
            get { return _StartTime; }
            set { _StartTime = value; OnPropertyChanged("StartTimeProperty"); }
        }

        public TimeSpan _EndTime = new TimeSpan(12, 00, 00);
        public TimeSpan EndTimeProperty
        {
            get { return _EndTime; }
            set { _EndTime = value; OnPropertyChanged("EndTimeProperty"); }
        }

        public string _CHStartTime;
        public string CHStartTime
        {
            get
            {
                if (_CHStartTime == "00:00:00")
                {
                    _CHStartTime = "12:00:00";
                }
                return _CHStartTime;
            }
            set { _CHStartTime = value; OnPropertyChanged("CHStartTime"); }
        }

        public string _CHEndTime;
        public string CHEndTime
        {
            get
            {
                if (_CHEndTime == "00:00:00")
                {
                    _CHEndTime = "12:00:00";
                }
                return _CHEndTime;
            }
            set { _CHEndTime = value; OnPropertyChanged("CHEndTime"); }
        }
        #endregion

        #region IsVisible

        public bool _ViewVisible = false;
        public bool ViewVisible
        {
            get { return _ViewVisible; }
            set { _ViewVisible = value; OnPropertyChanged("ViewVisible"); }
        }

        public bool _LvIsVisible;
        public bool LvIsVisible
        {
            get { return _LvIsVisible; }
            set { _LvIsVisible = value; OnPropertyChanged("LvIsVisible"); }
        }

        public bool _IsVisibleWD = false;
        public bool IsVisibleWD
        {
            get { return _IsVisibleWD; }
            set { _IsVisibleWD = value; OnPropertyChanged("IsVisibleWD"); }
        }

        public bool _IsVisibleCH = false;
        public bool IsVisibleCH
        {
            get { return _IsVisibleCH; }
            set { _IsVisibleCH = value; OnPropertyChanged("IsVisibleCH"); }
        }

        public bool _IsVisibleCDH = false;
        public bool IsVisibleCDH
        {
            get { return _IsVisibleCDH; }
            set { _IsVisibleCDH = value; OnPropertyChanged("IsVisibleCDH"); }
        }
        #endregion

        #region CategoryText
        public string _WholeDayText = "Set Schedule";
        public string WholeDayText
        {
            get { return _WholeDayText; }
            set { _WholeDayText = value; OnPropertyChanged("WholeDayText"); }
        }

        public string _CustomHoursText = "Set Schedule";
        public string CustomHoursText
        {
            get { return _CustomHoursText; }
            set { _CustomHoursText = value; OnPropertyChanged("CustomHoursText"); }
        }

        public string _CustomDayHoursText = "Set Schedule";
        public string CustomDayHoursText
        {
            get { return _CustomDayHoursText; }
            set { _CustomDayHoursText = value; OnPropertyChanged("CustomDayHoursText"); }
        }
        #endregion

        #region Images
        public string _MondayImage;
        public string MondayImage
        {
            get { return _MondayImage; }
            set { _MondayImage = value; OnPropertyChanged("MondayImage"); }
        }
        public string _TuesdayImage;
        public string TuesdayImage
        {
            get { return _TuesdayImage; }
            set { _TuesdayImage = value; OnPropertyChanged("TuesdayImage"); }
        }
        public string _WednesdayImage;
        public string WednesdayImage
        {
            get { return _WednesdayImage; }
            set { _WednesdayImage = value; OnPropertyChanged("WednesdayImage"); }
        }
        public string _ThursdayImage;
        public string ThursdayImage
        {
            get { return _ThursdayImage; }
            set { _ThursdayImage = value; OnPropertyChanged("ThursdayImage"); }
        }
        public string _FridayImage;
        public string FridayImage
        {
            get { return _FridayImage; }
            set { _FridayImage = value; OnPropertyChanged("FridayImage"); }
        }
        public string _SaturdayImage;

        public string SaturdayImage
        {
            get { return _SaturdayImage; }
            set { _SaturdayImage = value; OnPropertyChanged("SaturdayImage"); }
        }
        public string _SundayImage;

        public string SundayImage
        {
            get { return _SundayImage; }
            set { _SundayImage = value; OnPropertyChanged("SundayImage"); }
        }
        #endregion
    }
}
