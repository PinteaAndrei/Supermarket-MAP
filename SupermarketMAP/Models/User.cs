using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketMAP.Models
{
    internal class User
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string type { get; set; }  
        public virtual ICollection<Bill> bills { get; set; }
    }
}
