using ChatApp.Util;

using System.ComponentModel.DataAnnotations;
namespace ChatApp.Models
{
    public class ManageViewModel : BaseViewModel
    {
        [Required]
        public bool IsNameChanging { get; set; }
        [RequiredIf(nameof(IsNameChanging), "Введите никнейм")]
        public new string UserName { get; set; }
        [RequiredIf(nameof(IsNameChanging), "Подтвердите пароль")]
        public string Password { get; set; }


        [RequiredIfNot(nameof(IsNameChanging), "Подтвердите пароль")]
        public string OldPassword { get; set; }
        [RequiredIfNot(nameof(IsNameChanging), "Введите новый пароль")]
        public string NewPassword { get; set; }
    }
}
