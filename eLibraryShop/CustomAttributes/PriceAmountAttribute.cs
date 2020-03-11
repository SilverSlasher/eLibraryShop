using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.CustomAttributes
{
    public class PriceAmountAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _value = value.ToString();
            bool result = double.TryParse(_value, out double number);

            //Check if variable is number and is it positive
            if (!result && number <= 0)
            {
                return new ValidationResult(GetErrorMessage());
            }

            //Check if the value has 2 decimal places
            if (!int.TryParse((number*100).ToString(),out int _tempVariable))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return "Podaj poprawną cenę";
        }
    }
}
