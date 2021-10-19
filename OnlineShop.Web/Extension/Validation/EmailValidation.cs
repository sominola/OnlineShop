using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineShop.Web.Extension.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class EmailValidation : ValidationAttribute
    {
        private const string Pattern =
            @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is not string valueAsString)
            {
                return false;
            }
            
            var email = valueAsString;


            return Regex.IsMatch(email, Pattern, RegexOptions.IgnoreCase);
        }
    }
}