using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ChatApp.Util;

namespace ChatApp.Models
{
    public class RoomViewModel : BaseViewModel
    {
        [Required(ErrorMessage ="Вы не ввели название комнаты")]
        public string RoomName { get; set; }
        public string Type { get; set; }
        public bool IsPrivate { get; set; }
        public string RoomPassword { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
        public IEnumerable<RoomUser> UsersInRoom { get; set; }
        public IEnumerable<User> RoomAdmins { get; set; }
        public List<Message> LastMessages { get; set; }
    }
}