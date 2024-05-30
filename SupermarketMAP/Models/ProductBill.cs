using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketMAP.Models
{
    internal class ProductBill
    {
        public int Id { get; set; }
        public int billId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public decimal subtotal { get; set; }
        public virtual Bill bill { get; set; }
        public virtual Product product { get; set; }
    }
}
