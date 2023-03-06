using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPGrowth_TARD
{
    class ItemComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item it1, Item it2)
        {
            if (it1.name == it2.name)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(Item it)
        {
            return it.name.GetHashCode();
        }
    }
}
