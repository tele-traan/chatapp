namespace ChatApp.Models
{
    public class RoomsUsers
    {
        public int Id { get; set; }
        public Room Room { get; set; }
        public User User { get; set; }
    }
}
