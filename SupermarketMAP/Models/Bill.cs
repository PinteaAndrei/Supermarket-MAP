using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketMAP.Models
{
    internal class Bill
    {
        public int Id { get; set; }
        public DateTime releaseDate { get; set; }
        public int cashierId { get; set; }
        public decimal receivedSum { get; set; }
        public virtual User cashier { get; set; }
        public virtual ICollection<ProductBill> productBills { get; set; }
    }
}
