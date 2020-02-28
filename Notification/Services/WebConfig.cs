using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Services
{
    public static class WebConfig
    {
        public static string BaseUrl = "ServerAddress";
        public static string CreateAccount = "signup";
        public static string Login = "login";
        public static string ForgetPassword = "forgetPassword";
        public static string ConfirmSecurityCode = "confirmSecurityCode";
        public static string CreateNewPassword = "createNewPassword";
        public static string RefreshToken = "refreshToken";
        public static string AllMessagesAsync = "fetchMsgs";
        public static string DeleteOne = "deleteSingleMsg";
        public static string DeleteMultiple = "deleteSelectedMsgs";
        public static string DeleteAll = "deleteAllMsgs";
        public static string AddDevice = "addDevice";
        public static string NotificationDismissalSync = "notificationDismissalSync";
        public static string LastSync = "deviceLastSync";
        public static string Logout = "deviceLogout";
    }
}
