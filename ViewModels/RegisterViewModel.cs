using System.ComponentModel.DataAnnotations;
namespace ChatApp.Models
{
    public class RegisterViewModel : BaseViewModel
    {
        [Required(ErrorMessage ="Введите никнейм")]
        [RegularExpression(@"(?!\s)([0-9]|[a-zA-Z]|[а-яА-Я]){3,21}", ErrorMessage ="Ник должен быть длиной от 3 до 20 символов, " +
            "содержать только цифры и буквы, не иметь пробелов")]
        public new string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Введите пароль")]
        [RegularExpression(@"(?!\s)(?=.*[0-9])(?=.*([a-zA-Z]|[а-яА-Я])).{6,30}", ErrorMessage ="Пароль должен содержать буквы и цифры " +
            "и иметь длину от 6 до 30 символов, не иметь пробелов")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Подтвердите пароль")]
        [Compare(nameof(Password), ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}