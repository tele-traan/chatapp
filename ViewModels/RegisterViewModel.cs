using System.ComponentModel.DataAnnotations;
namespace ChatApp.Models
{
    public class RegisterViewModel : BaseViewModel
    {
        [Required(ErrorMessage ="Введите никнейм")]
        [MinLength(4, ErrorMessage ="Ник должен быть длиннее 3 символов")]
        [MaxLength(21, ErrorMessage = "Ник должен быть короче 20 символов")]
        public new string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Введите пароль")]
        [MinLength(7, ErrorMessage ="Пароль должен быть длиннее 6 символов")]
        [MaxLength(30,ErrorMessage = "Пароль слишком длинный")]
        [RegularExpression(@"[\w\d]+", ErrorMessage ="Пароль должен содержать буквы и цифры")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Подтвердите пароль")]
        [Compare(nameof(Password), ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}