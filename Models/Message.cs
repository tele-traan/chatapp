using System;

namespace ChatApp.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public DateTime DateTime { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }

        public int? RoomId { get; set; }
        public Room Room { get; set; }
    }
}