using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPGrowth_TARD
{
    class FPTree
    {
        public List<Item> HeaderList { get; set; }
        public Dictionary<Item, Node> MapItemNode { get; set; }
        public bool hasMoreThanOnePath { get; set; }
        public Node root { get; set; }

        public FPTree()
        {
            HeaderList = new List<Item>();
            ItemComparer itemComparer = new ItemComparer();
            MapItemNode = new Dictionary<Item, Node>(itemComparer);
            hasMoreThanOnePath = false;
            root = new Node();
            root.initializeNode();
        }

        // method for adding a transaction to the FP-tree (for the initial construction of the FP-tree).
        public void addTransaction(List<Item> transaction)
        {
            Node currentNode = this.root;
            ItemComparer itemComparer = new ItemComparer();
            for (int i = 0; i < transaction.Count; i++)
            {
                Item item = transaction[i];
                Node child = currentNode.getChildWithID(item);

                if (child == null)
                {
                    Node newNode = new Node();
                    newNode.initializeNode();
                    newNode.item = item;
                    newNode.parent = currentNode;
                    currentNode.children.Add(newNode);
                    if (!this.hasMoreThanOnePath && currentNode.children.Count() > 1)
                    {
                        this.hasMoreThanOnePath = true;
                    }
                    currentNode = newNode;
                    // update the header table
                    if (!this.MapItemNode.Keys.Contains(item, itemComparer))
                    {
                        this.MapItemNode.Add(item, newNode);
                    }
                    else // link nodes in the Header List
                    {
                        Node headerNode = this.MapItemNode[item];
                        // find the last node with this item
                        while (headerNode.link.item != null)
                        {
                            headerNode = headerNode.link;
                        }
                        headerNode.link = newNode;
                    }
                }
                else
                {
                    child.counter++;
                    currentNode = child;
                }
            }
        }

        /**
	     * Method for adding a prefixpath to a fp-tree.
	     * @param prefixPath: The prefix path
	     * @param mapSupportBeta: The frequencies of items in the prefixpaths
	     * @param minSup
	     */
        public void addPrefixPath(List<Node> prefixPath, Dictionary<Item, int> mapSupportBeta, double minSup)
        {
            ItemComparer itemComparer = new ItemComparer();
            // the first element of the prefix path contains the path support
            int pathCount = prefixPath[0].counter;
            Node currentNode = root;
            // for each item in the transaction (in backward order) (and we ignore the first element of the prefix path)
            for (int i = prefixPath.Count() - 1; i >= 1; i--)
            {
                Node node_item = prefixPath[i];
                // if the item is not frequent we skip it
                if (mapSupportBeta[node_item.item] < minSup)
                {
                    continue;
                }

                // look if there is a node already in the FP-Tree
                Node child = currentNode.getChildWithID(node_item.item);
                if (child == null)
                {
                    // there is no node, we create a new one
                    Node newNode = new Node();
                    newNode.initializeNode();
                    newNode.item = node_item.item;
                    newNode.parent = currentNode;
                    newNode.counter = pathCount;  // set its support
                    currentNode.children.Add(newNode);
                    // check if more than one path
                    if (!this.hasMoreThanOnePath && currentNode.children.Count() > 1)
                    {
                        this.hasMoreThanOnePath = true;
                    }

                    currentNode = newNode;
                    // update the header table. check if there is already a node with this id in the header table
                    if (!this.MapItemNode.Keys.Contains(node_item.item, itemComparer))
                    {
                        // there is not
                        this.MapItemNode.Add(node_item.item, newNode);
                    }
                    else
                    {   // there is
                        // we find the last node with this id.
                        Node headerNode = this.MapItemNode[node_item.item];
                        while (headerNode.link.item != null)
                        {
                            headerNode = headerNode.link;
                        }
                        headerNode.link = newNode;
                    }
                }
                else
                {
                    // there is a node already, we update it
                    child.counter += pathCount;
                    currentNode = child;
                }
            }
        }

        void sortHeaderList(List<Item> HeaderList, Dictionary<Item, int> MapSupport)
        {
            for (int i = 0; i < HeaderList.Count(); i++)
            {
                for (int j = i + 1; j < HeaderList.Count(); j++)
                {
                    if (MapSupport[HeaderList[j]] > MapSupport[HeaderList[i]])
                    {
                        Item t = HeaderList[i];
                        HeaderList[i] = HeaderList[j];
                        HeaderList[j] = t;
                    }
                    else if (MapSupport[HeaderList[j]] == MapSupport[HeaderList[i]])
                    {
                        if (string.Compare(HeaderList[j].name, HeaderList[i].name, true) < 0) // j_name precedes i_name
                        {
                            Item t = HeaderList[i];
                            HeaderList[i] = HeaderList[j];
                            HeaderList[j] = t;
                        }
                    }
                }
            }
        }

        /**
        * Method for creating the list of items in the header table, in descending order of support.
        * @param MapSupport: The frequencies of each item (key: item, value: support)
        */
        public void createHeaderList(Dictionary<Item, int> MapSupport) // MapSupport phai initialized with ItemComparer
        {
            // create an array to store the header list with all the items stored in the map received as parameter
            this.HeaderList = new List<Item>(this.MapItemNode.Keys);
            // sort the header table by decreasing order of support
            sortHeaderList(this.HeaderList, MapSupport);
        }
    }
}
