namespace ChatApp.Models
{
    public class RegularUser
    {
        public int RegularUserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public RoomUser RoomUser { get; set; }
        public GlobalChatUser GlobalChatUser { get; set; }
    }
}
