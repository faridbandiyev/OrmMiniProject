using OrmMiniProject.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Models
{
    public class OrderDetail:BaseEntity
    {
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal PricePerItem { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
