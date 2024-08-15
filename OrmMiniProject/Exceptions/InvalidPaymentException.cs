using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Exceptions
{
    public class InvalidPaymentException : Exception
    {
        public InvalidPaymentException(string message) : base(message) { }
    }
}
