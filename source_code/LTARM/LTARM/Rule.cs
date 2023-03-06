using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LTARM
{
    class Rule
    {
        public Rule()
        {
            this.LHS = new List<Item>();
            this.RHS = new List<Item>();
            this.Sup = 0;
            this.DirSup = 0;
            this.Conf = 0;
            this.DirConf = 0;
            this.Lift = 0;
            this.Conv = 0;
        }

        public List<Item> LHS { get; set; }
        public List<Item> RHS { get; set; }
        public double Sup { get; set; }
        public double DirSup { get; set; }
        public double Conf { get; set; }
        public double DirConf { get; set; }
        public double Lift { get; set; }
        public double Conv { get; set; }
    }
}
