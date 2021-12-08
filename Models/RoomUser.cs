namespace ChatApp.Models
{
    public class RoomUser
    {
        public int RoomUserId { get; set; }
        public string UserName { get; set; }
        public string UserConnectionId { get; set; }
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}