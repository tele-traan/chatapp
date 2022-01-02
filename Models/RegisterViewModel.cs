using System.ComponentModel.DataAnnotations;
namespace ChatApp.Models
{
    public class RegisterViewModel : BaseViewModel
    {
        [Required(ErrorMessage ="Введите никнейм")]
        public new string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Введите пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Подтвердите пароль")]
        [Compare("Password", ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}