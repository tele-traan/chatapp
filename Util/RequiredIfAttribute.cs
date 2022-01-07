using System.ComponentModel.DataAnnotations;
namespace ChatApp.Util
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _boolPropertyName;
        private readonly string _errorMessage;
        private readonly RequiredAttribute attr = new();
        public RequiredIfAttribute(string boolPropertyName, string errorMessage)
        {
            _boolPropertyName = boolPropertyName;
            _errorMessage = errorMessage;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty(_boolPropertyName);
            if ((bool)field.GetValue(validationContext.ObjectInstance))
            {
                if (attr.IsValid(value)) return ValidationResult.Success;
                return new ValidationResult(_errorMessage);
            }
            return ValidationResult.Success;
        }
    }

    public class RequiredIfNotAttribute:ValidationAttribute
    {
        private readonly string _boolPropertyName;
        private readonly string _errorMessage;
        private readonly RequiredAttribute attr = new();
        public RequiredIfNotAttribute(string boolPropertyName, string errorMessage)
        {
            _boolPropertyName = boolPropertyName;
            _errorMessage = errorMessage;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty(_boolPropertyName);
            if (!(bool)field.GetValue(validationContext.ObjectInstance))
            {
                if (attr.IsValid(value)) return ValidationResult.Success;
                return new ValidationResult(_errorMessage);
            }
            return ValidationResult.Success;
        }
    }
}