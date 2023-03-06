using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPGrowth_TARD
{
    class Item
    {
        public Item()
        {
            this.name = "";
            this.description = "";
            this.Obidset = new List<int>();
            this.Tempidset = new List<string>();
        }

        public string name { get; set; }
        public string description { get; set; }
        public List<int> Obidset { get; set; }
        public List<string> Tempidset { get; set; }
    }
}
