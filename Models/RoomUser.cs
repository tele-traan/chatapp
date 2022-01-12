namespace ChatApp.Models
{
    public class RoomUser 
    {
        public int RoomUserId { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public bool IsAdmin { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}