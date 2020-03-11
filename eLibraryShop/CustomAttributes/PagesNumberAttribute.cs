using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace eLibraryShop.Infrastructure
{
    public class PagesNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _value = value.ToString();
            bool result = int.TryParse(_value, out int number);

            if (!result && number <= 0)
            {
                return new ValidationResult(GetErrorMessage());
            }


            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return "Podaj dodatnią liczbę całkowitą";
        }
    }
}
