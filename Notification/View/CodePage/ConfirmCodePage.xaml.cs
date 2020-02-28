using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Model;
using Notification.Services;
using Notification.Strings;
using Notification.ViewModel.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notification.View.CodePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmCodePage : ContentPage
    {
        private string Code = string.Empty;
        private bool firstcall = true;
        private string _Email;
        private object _status;
        private UserClass _data;
        private List<Entry> entries = new List<Entry>();
        public ConfirmCodePage(string forgetPassword)
        {
            InitializeComponent();
            _Email = forgetPassword;
            BindingContext = new ConfirmCode(this, forgetPassword);
            EntOne.Focus();
            entries.Add(EntOne);
            entries.Add(EntTwo);
            entries.Add(EntThree);
            entries.Add(EntFour);
            entries.Add(EntFive);
            entries.Add(EntSix);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentFocused = (Entry)sender;
            //if (!Regex.IsMatch(@"[^\d]", ((Entry)sender).Text))
            //{
            //    ((Entry)sender).Text = string.Empty;
            //    return;
            //}

            if (currentFocused.Text != string.Empty)
            {
                switch (currentFocused.ClassId)
                {
                    case "0":
                        EntTwo.Focus();
                        break;
                    case "1":
                        EntThree.Focus();
                        break;
                    case "2":
                        EntFour.Focus();
                        break;
                    case "3":
                        EntFive.Focus();
                        break;
                    case "4":
                        EntSix.Focus();
                        break;
                    case "5":
                        EntSix.Unfocus();
                        break;
                    default:
                        break;
                }
            }

        }

        private void DefaultEntries()
        {
            var _length = entries.Count;
            for (int i = 0; i < _length; i++)
            {
                entries[i].Text = string.Empty;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EntOne.Text) || string.IsNullOrEmpty(EntTwo.Text) || string.IsNullOrEmpty(EntThree.Text) ||
                string.IsNullOrEmpty(EntFour.Text) || string.IsNullOrEmpty(EntFive.Text) || string.IsNullOrEmpty(EntSix.Text))
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter confirmation code!");
            }
            else
            {
                Code = EntOne.Text + EntTwo.Text + EntThree.Text + EntFour.Text + EntFive.Text + EntSix.Text;
                if (firstcall)
                {
                    firstcall = false;
                    ((ConfirmCode)BindingContext).IsWorking = true;
                    UserClass _obj = new UserClass()
                    {
                        Email = _Email,
                        SecurityCode = Code
                    };
                    _status = await ApiServices.Post(WebConfig.ConfirmSecurityCode, _obj);
                    if (_status != null)
                    {
                        _data = JsonConvert.DeserializeObject<UserClass>(_status.ToString());
                    }
                    if (_data != null && _data.Success == true)
                    {
                        DependencyService.Get<IMessage>().ShortAlert("Confirmed");

                        await Navigation.PushAsync(new NewPasswordPage(_Email));
                    }
                    else if (_data != null)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(_data.Message);
                    }
                    DefaultEntries();
                    ((ConfirmCode)BindingContext).IsWorking = false;
                    firstcall = true;
                }
            }
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            var currentFocused = Convert.ToInt16(((Entry)sender).ClassId);
            FocusValidate(currentFocused);
            //if (currentFocused.Text != string.Empty)
            //{
            //    switch (currentFocused.ClassId)
            //    {
            //        case "1":
            //            EntOne.Focus();
            //            break;
            //        case "2":
            //            if (EntOne.Text == string.Empty)
            //                EntOne.Focus();
            //            else
            //                EntTwo.Focus();
            //            break;
            //        case "3":
            //            if (EntTwo.Text == string.Empty)
            //                EntTwo.Focus();
            //            else
            //                EntThree.Focus();
            //            break;
            //        case "4":
            //            EntFive.Focus();
            //            break;
            //        case "5":
            //            EntSix.Focus();
            //            break;
            //        case "6":
            //            EntSix.Unfocus();
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private int FocusValidate(short currentFocused)
        {
            if (currentFocused == 0)
            {
                entries[currentFocused].Focus();
                return currentFocused;
            }

            if (string.IsNullOrEmpty(entries[currentFocused].Text) && string.IsNullOrEmpty(entries[currentFocused - 1].Text))
            {
                entries[currentFocused].Unfocus();
                entries[currentFocused - 1].Focus();
                return currentFocused - 1;
            }
            else
                entries[currentFocused].Focus();

            //if (string.IsNullOrEmpty(entries[currentFocused].Text) && !string.IsNullOrEmpty(entries[currentFocused - 1].Text))
            //{
            //    entries[currentFocused].Focus();
            //}

            return currentFocused;
        }
    }
}