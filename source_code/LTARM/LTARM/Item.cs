using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTARM
{
    class Item
    {
        public Item()
        {
            this.id = -1;
            this.name = "";
            this.description = "";
            this.support = -1;
            this.Obidset = new List<int>();
            this.Tempidset = new List<string>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int support { get; set; }
        public List<int> Obidset { get; set; }
        public List<string> Tempidset { get; set; }
    }
}
