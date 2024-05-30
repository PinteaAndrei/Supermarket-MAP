using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketMAP.Models
{
    internal class Product
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string barcode { get; set; }
        public int categoryId { get; set; }
        public int producerId { get; set; }
        public virtual Category category { get; set; }
        public virtual Producer producer { get; set; }
        public virtual ICollection<Stock> stocks { get; set; } = new List<Stock>();
        public virtual ICollection<ProductBill> productBills { get; set; }
        public virtual ICollection<Offer> offers { get; set; }
    }
}
