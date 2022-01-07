using System.Collections.Generic;
namespace ChatApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public RoomUser RoomUser { get; set; }
        public GlobalChatUser GlobalChatUser { get; set; }
        public List<Room> ManagedRooms { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is not User) return false;

            User other = obj as User;

            return other.UserId == UserId
                && other.UserName == UserName;
        }
        public override int GetHashCode() => UserId.GetHashCode();
    }
}
