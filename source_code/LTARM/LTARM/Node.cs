using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LTARM
{
    class Node // Equivalence Temporal Association Rule tree
    {
        public Node()
        {
            this.id = -1;
            this.itemset = new List<Item>();
            this.Obidset = new List<int>();
            this.traverse = false;
            this.childrenEC = new List<int>();
            this.childrenL = new List<int>();            
        }

        public int id { get; set; }
        public List<Item> itemset { get; set; }
        public List<int> Obidset { get; set; }
        public bool traverse { get; set; } // avoid duplicate rules
        public List<int> childrenEC { get; set; }
        public List<int> childrenL { get; set; }        
    }
}
