namespace ChatApp.Models
{
    public class GlobalChatUser
    {
        public int GlobalChatUserId { get; set; }

        public string UserName { get; set; }
        public string ConnectionId { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}