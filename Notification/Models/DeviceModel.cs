using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Model
{
    public class DeviceModel
    {
        public int Id { get; set; }
        public string NotificationKey { get; set; }
        public string NotificationDismissalSync { get; set; }
        public string DeviceName { get; set; }
        public string DeviceKey { get; set; }
        public string DeviceToken { get; set; }
        public string Platform { get; set; }
        public string Message { get; set; }
        public bool? Success { get; set; }
        public string MessageCode { get; set; }
    }
}
