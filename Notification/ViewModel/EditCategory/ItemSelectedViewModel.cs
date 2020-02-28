using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.SQLite;
using Notification.Strings;
using Notification.View.EditCategory;
using Notification.View.Setting.AlertSetting;
using Notification.ViewModel.Base;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notification.ViewModel.EditCategory
{
    public class ItemSelectedViewModel : BaseViewModel
    {
        private ItemSelectedPage itemSelectedPage;
        private bool alreadyClicked = true;
        private MemoryStream croppedStream;
        private HttpClient httpClient;
        private string image;
        private string from;
        private string tone;
        private MediaFile mediaFile;
        public ICommand ChangedImageTappedCommand { get; private set; }
        public ICommand SelectedToneTappedCommand { get; private set; }
        public ICommand SaveChangesCommand { get; private set; }
        public static byte[] CroppedImage;
        public ItemSelectedViewModel(ItemSelectedPage itemSelectedPage, string image, string from, string tone)
        {
            this.itemSelectedPage = itemSelectedPage;
            this.image = image;
            this.from = from;
            this.tone = tone;
            NavigationLabel = "Edit " + from;
            GalleryImage = image;
            From = from;
            ChangedImageTappedCommand = new Command(ChangedImage);
            SelectedToneTappedCommand = new Command(SelectedToneTapped);
            SaveChangesCommand = new Command(SaveChanges);
        }

        private async void SelectedToneTapped(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                await PopupNavigation.Instance.PushAsync(new ChooseDefaultTonePopup(SelectedTone, 2, From));
                alreadyClicked = true;
            }
        }

        private async void SaveChanges(object obj)
        {
            try
            {
                httpClient = new HttpClient();
                Debug.WriteLine(croppedStream);
                var token = (string)Application.Current.Properties[NotificationStrings.Token];
                httpClient.DefaultRequestHeaders.Add("Authorization", token);

                StreamContent content = new StreamContent(mediaFile.GetStream());
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    FileName = "newimage",
                    Name = "image"
                };
                content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                var multi = new MultipartFormDataContent();
                multi.Add(content);

                var httpResponseMessage = await httpClient.PostAsync(WebConfig.BaseUrl, multi);
                var response = await httpResponseMessage.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
            }
            catch (WebException)
            {
                //DependencyService.Get<IMessage>().LongAlert(ex.ToString());
                return;
            }
            catch (IOException)
            {
                //DependencyService.Get<IMessage>().LongAlert(ex.ToString());
                return;
            }
            catch (Exception)
            {
                //DependencyService.Get<IMessage>().LongAlert(ex.ToString());
                return;
            }
            finally
            {
                if (httpClient != null)
                    httpClient.Dispose();
            }
        }

        private async void ChangedImage(object obj)
        {
            if (alreadyClicked)
            {
                alreadyClicked = false;
                GalleryImage = null;
                try
                {
                    Application.Current.Properties[NotificationStrings.GalleryOpened] = true;

                    mediaFile = await CrossMedia.Current.PickPhotoAsync();

                    GalleryImage = ImageSource.FromStream(() =>
                    {
                        return mediaFile.GetStream();
                    });

                    var memoryStream = new MemoryStream();
                    await mediaFile.GetStream().CopyToAsync(memoryStream);
                    byte[] imageAsByte = memoryStream.ToArray();
                    await itemSelectedPage.Navigation.PushModalAsync(new CropView(imageAsByte, Refresh));

                    Application.Current.Properties[NotificationStrings.GalleryOpened] = false;
                }
                catch (System.Exception ex)
                {
                    Application.Current.Properties.Remove(NotificationStrings.GalleryOpened);
                    Debug.WriteLine(ex.Message);
                }
                alreadyClicked = true;
            }
        }

        private void Refresh()
        {
            try
            {
                if (CroppedImage != null)
                {
                    croppedStream = new MemoryStream(CroppedImage);
                    GalleryImage = ImageSource.FromStream(() => croppedStream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //private void OpenCamera()
        //{
        //    //if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
        //    //{
        //    //    await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "Got It");
        //    //    return;
        //    //}
        //    //switch (Device.RuntimePlatform)
        //    //{
        //    //    case Device.Android:

        //    //        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
        //    //        {
        //    //            Directory = Strings.NotificationStrings.ProjectName,
        //    //            Name = DateTime.Now + ".jpg",
        //    //            PhotoSize = PhotoSize.Full,
        //    //            DefaultCamera = CameraDevice.Rear,
        //    //            AllowCropping = true,
        //    //            RotateImage = true,
        //    //            SaveMetaData = false
        //    //        });
        //    //        break;

        //    //    case Device.iOS:

        //    //        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
        //    //        {
        //    //            Directory = "ProfilePictures",
        //    //            Name = DateTime.Now + ".jpg",
        //    //            PhotoSize = PhotoSize.Full,
        //    //            DefaultCamera = CameraDevice.Rear,
        //    //            AllowCropping = true,
        //    //            RotateImage = true,
        //    //            SaveMetaData = false
        //    //        });
        //    //        break;
        //    //}

        //    //if (file == null)
        //    //{
        //    //    return;
        //    //}
        //    //if (string.IsNullOrEmpty(file.Path))
        //    //{
        //    //    return;
        //    //}

        //    //byte[] orignalImage = System.IO.File.ReadAllBytes(file.Path);
        //    //GalleryImage = file.Path;
        //    //byte[] imageArray = System.IO.File.ReadAllBytes(orignalImage.ToString());
        //    //file.Dispose();
        //}

        public string _NavigationLabel;
        public string NavigationLabel
        {
            get { return _NavigationLabel; }
            set
            {
                _NavigationLabel = value;
                OnPropertyChanged("NavigationLabel");
            }
        }

        public string _From;
        public string From
        {
            get { return _From; }
            set
            {
                _From = value;
                OnPropertyChanged("From");
            }
        }

        public string SelectedTone
        {
            get
            {
                if (string.IsNullOrEmpty(tone))
                    return (string)Application.Current.Properties[NotificationStrings.ChooseDefaultTone];

                // tone = UpdateToneClass.GetTone(From);
                return tone;
            }
            set { tone = value; OnPropertyChanged("SelectedTone"); }
        }

        public ImageSource _GalleryImage = "NoImage.png";
        public ImageSource GalleryImage
        {
            get
            { return _GalleryImage; }
            set
            {
                if (_GalleryImage == null)
                    _GalleryImage = "NoImage.png";
                else
                    _GalleryImage = value;
                OnPropertyChanged("GalleryImage");
            }
        }
    }
}
