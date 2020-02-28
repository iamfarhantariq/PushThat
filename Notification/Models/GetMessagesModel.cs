using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Model
{
    public class GetMessagesModel
    {
        public int Id { get; set; }
        public string NotificationKey { get; set; }
        public bool? Success { get; set; }
        public string MessageCode { get; set; }
        public string Message { get; set; }
        public List<int> Ids { get; set; }
        public List<MessageList> MessageList { get; set; }
    }
    public class MessageList
    {
        public int Id { get; set; }
        public string MessageTitle { get; set; }
        public string From { get; set; }
        public string MessageBody { get; set; }
        public DateTime Date { get; set; }
        public string Image { get; set; }
    }
}
