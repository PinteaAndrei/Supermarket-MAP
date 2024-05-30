using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketMAP.Models
{
    internal class Offer
    {
        public int Id { get; set; }
        public string reason { get; set; }
        public int productId { get; set; }
        public decimal percentage { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public virtual Product product { get; set; }
    }
}
