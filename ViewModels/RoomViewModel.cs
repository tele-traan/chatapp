using ChatApp.Util;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ChatApp.Models
{
    public class RoomViewModel : BaseViewModel
    {
        [Required(ErrorMessage ="Вы не ввели название комнаты")]
        public string RoomName { get; set; }
        public string Type { get; set; }
        public bool IsPrivate { get; set; } = false;

        [RequiredIf(nameof(IsPrivate), "Вы не ввели пароль для комнаты")]
        public string RoomPassword { get; set; }

        public IEnumerable<Room> Rooms { get; set; }
        public IEnumerable<RoomUser> UsersInRoom { get; set; }
        public IEnumerable<User> RoomAdmins { get; set; }
    }
}