namespace ChatApp.Models
{
    public class GlobalChatUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public int RegularUserId { get; set; }
        public virtual RegularUser RegularUser { get; set; }
    }
}