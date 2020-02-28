using Hangfire.Annotations;
using MR.Gestures;
using Notification.Strings;
using Notification.View.Dashboard.SingleMessageViewer;
using Notification.ViewModel.Base;
using Notification.ViewModel.Dashboard;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notification.Model
{
    public class MessageClass : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string MessageTitle { get; set; }
        public string MessageFrom { get; set; }
        public string MessageBody { get; set; }
        public string MessageType { get; set; }
        public DateTime MessageTime { get; set; }
        public string ImageUrl { get; set; }
        public string Tone { get; set; }
        public string MessageStatus { get; set; }

        [Ignore]
        public string DefualtTone
        {
            get
            {
                if (string.IsNullOrEmpty(Tone))
                    return (string)Application.Current.Properties[NotificationStrings.ChooseDefaultTone];
                return Tone;
            }
        }

        public Color color = Color.FromHex("#590968A2");
        [Ignore]
        public ICommand TappedCommand
        {
            get
            {
                return new Command<TapEventArgs>((x) =>
                {
                    if (x != null)
                    {
                        Tapped(x);
                    }
                });
            }
        }
        [Ignore]
        public ICommand LongPressingCommand
        {
            get
            {
                return new Command<LongPressEventArgs>((x) =>
                {
                    if (x != null)
                    {
                        LongPressing(x);
                    }
                });
            }
        }

        public int _Count;
        [Ignore]
        public int Count
        {
            get
            {
                return _Count;
            }
            set
            {
                _Count = value;
                OnPropertyChanged("Count");
            }
        }

        protected virtual void LongPressing(LongPressEventArgs obj)
        {
            if (BGColor != color)
                BGColor = color;
            else
                BGColor = Color.Transparent;
        }

        protected virtual void Tapped(TapEventArgs obj)
        {
            if (BGColor != color)
                BGColor = color;
            else
                BGColor = Color.Transparent;
        }

        private Color bgColor;
        [Ignore]
        public Color BGColor
        {
            get
            { return bgColor; }
            set
            {
                bgColor = value;
                OnPropertyChanged("BGColor");
            }
        }
        public bool _IsCheckCheckBox;

        [Ignore]
        public bool IsCheckCheckBox
        {
            get
            {
                return _IsCheckCheckBox;
            }
            set
            {
                _IsCheckCheckBox = value;
                OnPropertyChanged("IsCheckCheckBox");
            }
        }
        [Ignore]
        public int MaxLinesToPreview
        {
            get
            {
                if (Application.Current.Properties.ContainsKey(NotificationStrings.MessagePreviewInLine))
                {
                    if (((string)Application.Current.Properties[NotificationStrings.MessagePreviewInLine]) == "All")
                        return 500;
                    else
                        return Convert.ToInt16((string)Application.Current.Properties[NotificationStrings.MessagePreviewInLine]);
                }
                else
                {
                    return 7;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
