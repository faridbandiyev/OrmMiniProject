using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Utils
{
    public static class ValidationExtensions
    {
        public static bool IsValidName(this string name)
        {
            return !name.Any(char.IsDigit);
        }

        public static bool IsValidEmail(this string email, List<string> existingEmails)
        {
            return !existingEmails.Contains(email);
        }
    }
}
