using Notification.Strings;
using Notification.View.Setting.AlertSetting;
using Notification.View.Setting.MessageSetting;
using Notification.ViewModel.Setting;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.Setting
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
            DefaultSetting();
            BindingContext = new SettingViewModel(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessageCenterValuePass();
        }
        private void MessageCenterValuePass()
        {
            MessagingCenter.Subscribe<MaxMessagePopup>(this, "MaxMessage", (sender) =>
            {
                ((SettingViewModel)BindingContext).MaxMessageToKeep = (int)Application.Current.Properties[NotificationStrings.MaxMessageToKeep];
            });
            MessagingCenter.Subscribe<MessagePreviewLinePopup>(this, "MessagePreview", (sender) =>
            {
                ((SettingViewModel)BindingContext).MessagePreviewInLine = (string)Application.Current.Properties[NotificationStrings.MessagePreviewInLine];
            });
            MessagingCenter.Subscribe<ChooseDefaultTonePopup>(this, "ChooseDefaultTone", (sender) =>
            {
                ((SettingViewModel)BindingContext).ChooseDefaultTone = (string)Application.Current.Properties[NotificationStrings.ChooseDefaultTone];
            });
            MessagingCenter.Subscribe<VibrateIterationPopup>(this, "VibrateIteration", (sender) =>
            {
                ((SettingViewModel)BindingContext).VibrateIteration = (int)Application.Current.Properties[NotificationStrings.VibrateIteration];
            });
        }
        private async void DefaultSetting()
        {
            if (!Application.Current.Properties.ContainsKey(NotificationStrings.MaxMessageToKeep))
            {
                Application.Current.Properties[NotificationStrings.MaxMessageToKeep] = 250;
            }
            if (!Application.Current.Properties.ContainsKey(NotificationStrings.MessagePreviewInLine))
            {
                Application.Current.Properties[NotificationStrings.MessagePreviewInLine] = "6";
            }
            if (!Application.Current.Properties.ContainsKey(NotificationStrings.ChooseDefaultTone) &&
                !Application.Current.Properties.ContainsKey(NotificationStrings.CategoryTypeTone))
            {
                Application.Current.Properties[NotificationStrings.ChooseDefaultTone] = "Notification";
                Application.Current.Properties[NotificationStrings.CategoryTypeTone] = "Notification";
            }
            if (!Application.Current.Properties.ContainsKey(NotificationStrings.VibrateIteration))
            {
                Application.Current.Properties[NotificationStrings.VibrateIteration] = 2;
            }
            if (!Application.Current.Properties.ContainsKey(NotificationStrings.NotificationDismissalSync))
            {
                Application.Current.Properties[NotificationStrings.NotificationDismissalSync] = false;
            }
            if (!Application.Current.Properties.ContainsKey(NotificationStrings.CopyMessagesTitle))
            {
                Application.Current.Properties[NotificationStrings.CopyMessagesTitle] = true;
            }
            await Application.Current.SavePropertiesAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<MaxMessagePopup>(this, "MaxMessage");
            MessagingCenter.Unsubscribe<MessagePreviewLinePopup>(this, "MessagePreview");
            MessagingCenter.Unsubscribe<ChooseDefaultTonePopup>(this, "ChooseDefaultTone");
            MessagingCenter.Unsubscribe<VibrateIterationPopup>(this, "VibrateIteration");
        }
    }
}