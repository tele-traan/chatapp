using System;

namespace ChatApp.Models
{
    public class BanInfo
    {
        public int BanInfoId {get;set;}

        public DateTime Until { get; set; }
        public string Reason { get; set; }
        public string PunisherName { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
