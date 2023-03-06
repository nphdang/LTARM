using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPGrowth_TARD
{
    class Itemset
    {
        public Itemset()
        {            
            this.items = new List<Item>();
            this.support = -1;
            this.Obidset = new List<int>();
        }
        
        public List<Item> items { get; set; }
        public int support { get; set; }
        public List<int> Obidset { get; set; }
    }
}
