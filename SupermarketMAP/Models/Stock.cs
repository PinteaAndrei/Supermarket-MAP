using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketMAP.Models
{
    internal class Stock
    {
        public int Id { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public string unitOfMeasurement { get; set; }
        public DateTime supplyDate { get; set; }
        public DateTime? expirationDate { get; set; }
        public decimal supplyPrice { get; set; }
        public decimal salePrice { get; set; }
        public virtual Product product { get; set; }
    }
}
