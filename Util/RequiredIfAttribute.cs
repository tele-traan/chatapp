using System.ComponentModel.DataAnnotations;
namespace ChatApp.Util
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;
        private readonly RequiredAttribute attr = new();
        public RequiredIfAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty(_otherProperty);
            if ((bool)field.GetValue(validationContext.ObjectInstance))
            {
                if (attr.IsValid(value)) return ValidationResult.Success;
                return new ValidationResult("Вы не ввели пароль для комнаты");
            }
            return ValidationResult.Success;
        }
    }
}