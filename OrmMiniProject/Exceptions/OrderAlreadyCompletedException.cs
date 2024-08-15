using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Exceptions
{
    public class OrderAlreadyCompletedException:Exception
    {
        public OrderAlreadyCompletedException(string message) : base(message)
        {
        }
    }
}
