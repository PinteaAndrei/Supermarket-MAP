using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketMAP.Models
{
    internal class Producer
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string countryOfOrigin { get; set; }
        public virtual ICollection<Product> products { get; set; }
    }
}
