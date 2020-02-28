using Notification.Strings;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Notification.View.EditCategory
{
    public class CropView : ContentPage
    {
        public byte[] Image;
        public Action RefreshAction;
        public bool DidCrop = false;

        public CropView(byte[] imageAsByte, Action refreshAction)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.Black;
            Image = imageAsByte;
            RefreshAction = refreshAction;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (DidCrop)
                RefreshAction.Invoke();

            Application.Current.Properties[NotificationStrings.GalleryOpened] = false;
        }
    }
}
