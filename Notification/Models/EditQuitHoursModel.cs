using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Model
{
    public class EditQuitHoursModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public string Day { get; set; }
        public string LabelMessage { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Ignore]
        public string StartTimeS
        {
            get
            {
                if (StartTime == new TimeSpan() || StartTime == new TimeSpan(12, 00, 00))
                {
                    return "Set Start Time";
                }
                return StartTime.ToString();
            }
        }

        [Ignore]
        public string EndTimeS
        {
            get
            {
                if (EndTime == new TimeSpan() || EndTime == new TimeSpan(12, 00, 00))
                {
                    return "Set End Time";
                }
                return EndTime.ToString();
            }
        }
    }
}
