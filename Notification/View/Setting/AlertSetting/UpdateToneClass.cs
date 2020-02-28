using Notification.Model;
using Notification.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.View.Setting.AlertSetting
{
    public class UpdateToneClass
    {
        private static async Task<List<MessageClass>> GetData(string catType)
        {
            var messages = await SQLiteDatabase.GetAllTableDataAsync<MessageClass>();
            if (messages == null)
                return null;

            List<MessageClass> target = messages.Where(x => x.MessageFrom == catType).ToList();
            if (target == null)
                return null;

            return target;
        }

        public static async void Update(string tone, string catType)
        {
            var target = GetData(catType).Result;
            if (target == null)
                return;

            for (int i = 0; i < target.Count; i++)
            {
                MessageClass message = new MessageClass()
                {
                    Id = target[i].Id,
                    MessageId = target[i].MessageId,
                    MessageFrom = target[i].MessageFrom,
                    MessageBody = target[i].MessageBody,
                    MessageTitle = target[i].MessageTitle,
                    MessageTime = target[i].MessageTime,
                    MessageType = target[i].MessageType,
                    MessageStatus = target[i].MessageTitle,
                    ImageUrl = target[i].ImageUrl,
                    Tone = tone
                };
                await SQLiteDatabase.UpdateAsync(message);
            }
        }

        public static string GetTone(string catType)
        {
            var row = GetData(catType).Result;
            var tone = row.FirstOrDefault();
            return tone.Tone;
        }
    }
}
