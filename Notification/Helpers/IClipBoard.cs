using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Helpers
{
    public interface IClipBoard
    {
        string GetTextFromClipboard();
        void SendTextToClipboard(string text);
    }
}
