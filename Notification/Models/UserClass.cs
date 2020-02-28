using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Model
{
    public class UserClass
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NotificationKey { get; set; }
        public string SecurityCode { get; set; }
        public bool? Success { get; set; }
        public string Token { get; set; }
        public string Licensing { get; set; }
        public string Message { get; set; }
        public string License { get; set; }
        public string MessageCode { get; set; }
    }
}
