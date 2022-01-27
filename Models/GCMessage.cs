using System;

namespace ChatApp.Models
{
    public class GCMessage
    {
        public int GCMessageId { get; set; }

        public DateTime DateTime { get; set; }
        public string SenderName { get; set; }
        public string BgColor { get; set; }
        public string Text { get; set; }
    }
}