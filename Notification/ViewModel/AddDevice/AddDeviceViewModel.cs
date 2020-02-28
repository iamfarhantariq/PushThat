using System.Windows.Input;
using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.Strings;
using Notification.View.Dashboard;
using Notification.ViewModel.Base;
using Plugin.FirebasePushNotification;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Notification.ViewModel.AddDevice
{
    public class AddDeviceViewModel : BaseViewModel
    {
        private bool alreadyClicked = true;
        private Page addDevicePage;

        public string _addDevice = DeviceInfo.Name;
        private object status;
        private DeviceModel data;

        public string AddDevice
        {
            get
            {
                return _addDevice;
            }
            set
            {
                _addDevice = value;
                OnPropertyChanged("AddDevice");
            }
        }
        public ICommand AddDeviceCommand { get; private set; }
        public AddDeviceViewModel(Page addDevicePage)
        {
            this.addDevicePage = addDevicePage;
            NavigationPage.SetHasNavigationBar(addDevicePage, false);
            AddDeviceCommand = new Command(AddDeviceClicked);
        }

        public Color _AddDeviceColor = Color.Default;
        public Color AddDeviceColor
        {
            get
            {
                return _AddDeviceColor;
            }
            set
            {
                _AddDeviceColor = value;
                OnPropertyChanged("AddDeviceColor");
            }
        }
        private async void AddDeviceClicked(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                IsWorking = true;
                if (string.IsNullOrEmpty(AddDevice))
                {
                    AddDeviceColor = Color.FromHex(NotificationStrings.EntryErrorColor);
                    DependencyService.Get<IMessage>().ShortAlert("Device Name is Empty");
                }
                else
                {
                    IDevice device = DependencyService.Get<IDevice>();
                    string deviceIdentifier = "";
                    if (device != null)
                    {
                        deviceIdentifier = device.GetIdentifier();
                    }
                    var deviceToken = CrossFirebasePushNotification.Current.Token;
                    //System.Guid? installId = await AppCenter.GetInstallIdAsync();
                    DeviceModel _obj = new DeviceModel()
                    {
                        NotificationKey = (string)Application.Current.Properties[NotificationStrings.NotificationKey],
                        DeviceName = AddDevice,
                        DeviceKey = deviceIdentifier,
                        DeviceToken = deviceToken,
                        Platform = DeviceInfo.Platform.ToString()
                    };
                    status = await ApiServices.Post(WebConfig.AddDevice, _obj);
                    if (status != null)
                        data = JsonConvert.DeserializeObject<DeviceModel>(status.ToString());
                    if (data != null && data.Success == true)
                    {
                        Application.Current.Properties[NotificationStrings.DeviceName] = AddDevice;
                        Application.Current.Properties[NotificationStrings.DeviceKey] = _obj.DeviceKey;
                        Application.Current.Properties[NotificationStrings.DeviceToken] = _obj.DeviceToken;
                        await Application.Current.SavePropertiesAsync();
                        addDevicePage.Navigation.InsertPageBefore(new DashboardPage(), addDevicePage);
                        await addDevicePage.Navigation.PopAsync();
                        DependencyService.Get<IMessage>().ShortAlert("Success");
                    }
                    else if (data != null)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(data.Message);
                    }
                }
                IsWorking = false;
                alreadyClicked = true;
            }
        }
    }
}
