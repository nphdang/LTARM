using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FPGrowth_TARD
{
    class Node
    {       
        public Item item { get; set; }
        public int counter { get; set; } // # of rows containing this node
        public Node parent { get; set; }
        public List<Node> children { get; set; }
        public Node link { get; set; }

        public void initializeNode()
        {
            item = new Item();
            counter = 1;
            parent = new Node();
            children = new List<Node>();
            link = new Node();
        }

        public Node getChildWithID(Item it)
        {
            foreach (Node child in this.children)
            {
                if (child.item.name == it.name)
                {
                    return child;
                }
            }

            return null;
        }
    }
}
