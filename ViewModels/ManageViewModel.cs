using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

using ChatApp.Util;

namespace ChatApp.Models
{
    public class ManageViewModel : BaseViewModel
    {
        public bool IsNameChanging { get; set; }
        public new string UserName { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public ReadOnlyCollection<Room> ManagedRooms { get; set; }
    }
}