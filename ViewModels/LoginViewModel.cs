using System.ComponentModel.DataAnnotations;
namespace ChatApp.Models
{
    public class LoginViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Введите никнейм")]
        public new string UserName { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
