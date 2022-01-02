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
        public bool IsPasswordRequired { get; set; } = false;

        [RequiredIf(nameof(IsPasswordRequired))]
        public string RoomPassword { get; set; }

        public IEnumerable<Room> Rooms { get; set; }
        public IEnumerable<RoomUser> UsersInRoom { get; set; }
    }
}