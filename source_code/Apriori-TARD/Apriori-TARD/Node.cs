using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Apriori_TARD
{
    class Node
    {
        public Node()
        {            
            this.itemset = new List<Item>();
            this.Obidset = new List<int>();            
        }
        
        public List<Item> itemset { get; set; }
        public List<int> Obidset { get; set; }        
    }
}
