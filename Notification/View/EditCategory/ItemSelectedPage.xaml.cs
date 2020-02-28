using Notification.Helpers;
using Notification.Strings;
using Notification.View.Setting.AlertSetting;
using Notification.ViewModel.EditCategory;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.EditCategory
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemSelectedPage : ContentPage
    {
        public ItemSelectedPage(string image, string from, string tone)
        {
            InitializeComponent();
            BindingContext = new ItemSelectedViewModel(this, image, from, tone);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Message();
        }

        private void Message()
        {
            MessagingCenter.Subscribe<ChooseDefaultTonePopup>(this, "CategoryTypeTone", (sender) =>
            {
                ((ItemSelectedViewModel)BindingContext).SelectedTone = sender.chooseDefaultTonePopup;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ChooseDefaultTonePopup>(this, "CategoryTypeTone");
        }

        //private async void Button_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        await CrossMedia.Current.Initialize();

        //        var response = await Application.Current.MainPage.DisplayActionSheet("Photo Source", "Cancel", null, "Camera", "Photo Album");

        //        if (response == "Camera")
        //        {
        //            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
        //            {
        //                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "Got It");
        //                return;
        //            }
        //            switch (Device.RuntimePlatform)
        //            {
        //                case Device.Android:

        //                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
        //                    {
        //                        Directory = Strings.NotificationStrings.ProjectName,
        //                        Name = DateTime.Now + ".jpg",
        //                        PhotoSize = PhotoSize.Full,
        //                        DefaultCamera = CameraDevice.Rear,
        //                        AllowCropping = true,
        //                        RotateImage = true,
        //                        SaveMetaData = false
        //                    });
        //                    break;

        //                case Device.iOS:

        //                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
        //                    {
        //                        Directory = "ProfilePictures",
        //                        Name = DateTime.Now + ".jpg",
        //                        PhotoSize = PhotoSize.Full,
        //                        DefaultCamera = CameraDevice.Rear,
        //                        AllowCropping = true,
        //                        RotateImage = true,
        //                        SaveMetaData = false
        //                    });


        //                    break;
        //            }

        //            if (file == null)
        //            {
        //                //DependencyService.Get<IMessage>().ShortAlert("Something went wrong..! Please try later.");
        //                return;
        //            }
        //            if (string.IsNullOrEmpty(file.Path))
        //            {
        //                //DependencyService.Get<IMessage>().ShortAlert("Something went wrong..! Please try later.");
        //                return;
        //            }


        //            byte[] orignalImage = System.IO.File.ReadAllBytes(file.Path);
        //            //string compressedPath = "";
        //            //switch (Device.RuntimePlatform)
        //            //{
        //            //    case Device.Android:
        //            //        compressedPath = await DependencyService.Get<IMediaService>().CompressImage(file.Path, null);
        //            //        break;
        //            //    case Device.iOS:
        //            //        compressedPath = await DependencyService.Get<IMediaService>().CompressImage(file.Path, orignalImage);
        //            //        break;
        //            //}


        //            ((ItemSelectedViewModel)BindingContext).GalleryImage = file.Path;

        //            byte[] imageArray = System.IO.File.ReadAllBytes(orignalImage.ToString());


        //            //if (updateUser != null)
        //            //{
        //            //    updateUser.picture = Convert.ToBase64String(imageArray);
        //            //    updateUser.DownloadedImageBlob = imageArray;
        //            //    updateUser.dateSynced = Constants.GetUniversalDateTime();
        //            //}

        //            ////await DataAccess.UpdateAsync<User>(updateUser);
        //            ////api call,,
        //            //await UpdateUserDetails(updateUser);

        //            file.Dispose();

        //        }
        //        else if (response == "Photo Album")
        //        {
        //            var pickerOptions = new PickMediaOptions();

        //            file = await CrossMedia.Current.PickPhotoAsync(pickerOptions);

        //            if (file == null)
        //            {
        //                //DependencyService.Get<IMessage>().ShortAlert("Something went wrong..! Please try later.");
        //                return;
        //            }
        //            if (string.IsNullOrEmpty(file.Path))
        //            {
        //                //DependencyService.Get<IMessage>().ShortAlert("Something went wrong..! Please try later.");
        //                return;
        //            }

        //            byte[] orignalImage = System.IO.File.ReadAllBytes(file.Path);

        //            //string compressedPath = "";
        //            //switch (Device.RuntimePlatform)
        //            //{
        //            //    case Device.Android:
        //            //        compressedPath = await DependencyService.Get<IMediaService>().CompressImage(file.Path, null);
        //            //        break;
        //            //    case Device.iOS:
        //            //        compressedPath = await DependencyService.Get<IMediaService>().CompressImage(file.Path, orignalImage);
        //            //        break;
        //            //}



        //            ((ItemSelectedViewModel)BindingContext).GalleryImage = file.Path;

        //            byte[] imageArray = System.IO.File.ReadAllBytes(orignalImage.ToString());

        //            //User updateUser = await DataAccess.GetLastRecord<User>("User");

        //            //if (updateUser != null)
        //            //{
        //            //    updateUser.picture = Convert.ToBase64String(imageArray);
        //            //    updateUser.DownloadedImageBlob = imageArray;
        //            //    updateUser.dateSynced = Constants.GetUniversalDateTime();
        //            //}

        //            //api call,,
        //            //await UpdateUserDetails(updateUser);

        //            file.Dispose();

        //        }
        //        else
        //        {
        //            //file.Dispose();
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

        private void RoundedEntry_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}