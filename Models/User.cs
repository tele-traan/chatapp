namespace ChatApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserConnectionId { get; set; }

        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}
