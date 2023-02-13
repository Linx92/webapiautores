using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validaciones
{
    public class PrimeraLetraMayusculaAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null ||  string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var mayuscula = value.ToString()[0].ToString();
            if (mayuscula != mayuscula.ToUpper()) 
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }
}
