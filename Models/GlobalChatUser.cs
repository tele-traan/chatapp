namespace ChatApp.Models
{
    public class GlobalChatUser
    {
        public int GlobalChatUserId { get; set; }

        public string UserName { get; set; }
        public string ConnectionId { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is null || obj is not GlobalChatUser) return false;
            GlobalChatUser other = obj as GlobalChatUser;
            return other.GlobalChatUserId == GlobalChatUserId
                && other.UserName == UserName;
        }
        public override int GetHashCode() => GlobalChatUserId.GetHashCode();
    }
}