using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace FPGrowth_TARD
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        // rows and columns
        int row = 0, col = 0;
        // the number of patients
        int nPatient = 0;
        // map of patient ID
        Dictionary<int, int> patients = new Dictionary<int, int>();
        // store frequent itemsets
        List<Itemset> FIs = new List<Itemset>();
        // store rules
        List<Rule> TARs = new List<Rule>();
        CultureInfo culture = new CultureInfo("en-AU");

        #region buttons
        private void btBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDatabase.Text = openFileDialog1.FileName;
            }
        }
        #endregion

        #region find supports of all 1-items
        void findMapSupport(string[,] transactions, Dictionary<Item, int> MapSupport)
        {
            ItemComparer itemComparer = new ItemComparer();
            for (int i = 0; i < row; i++)
            {
                Item item = new Item();
                item.name = transactions[i, 2];

                if (!MapSupport.Keys.Contains(item, itemComparer)) // new item
                {
                    int patient = int.Parse(transactions[i, 0]);
                    int patient_code = patients[patient]; // get map code of a patient from dictionary
                    item.Obidset.Add(patient_code);
                    string[] tempidset = new string[nPatient];
                    item.Tempidset = new List<string>(tempidset);
                    item.Tempidset[patient_code] = transactions[i, 1];
                    string des = transactions[i, 3];
                    if (chkFullNodeLabel.Checked == false)
                    {
                        if (des.Length > 30)
                        {
                            des = des.Substring(0, 30) + "...";
                        }
                    }
                    item.description = des;
                    MapSupport.Add(item, 1);
                }
                else // old item
                {
                    foreach (var kv in MapSupport)
                    {
                        if (kv.Key.name == item.name)
                        {
                            int patient = int.Parse(transactions[i, 0]);
                            int patient_code = patients[patient]; // get map code of a patient from dictionary
                            kv.Key.Obidset.Add(patient_code);
                            kv.Key.Tempidset[patient_code] = transactions[i, 1];
                            break;
                        }
                    }
                    MapSupport[item]++; // update support                        
                }
            }
        }
        #endregion

        #region intersect of two Obidsets
        List<int> intersectObidsets(List<int> a1, List<int> a2)
        {
            List<int> a = new List<int>();
            if (a1.Count == 0 || a2.Count == 0)
            {
                return a;
            }

            int i = 0, j = 0;
            a2.Add(a1[a1.Count - 1] + 1);
            while (i < a1.Count)
            {
                if (a1[i] < a2[j])
                {
                    i++;
                }
                else if (a1[i] > a2[j])
                {
                    j++;
                }
                else
                {
                    a.Add(a1[i++]);
                    j++;
                }
            }
            a2.RemoveAt(a2.Count - 1);

            return a;
        }
        #endregion        

        #region read dataset
        string[,] readDataset(string _sFile)
        {
            string[,] transactions = null;            
            string[] line = null;
            using (StreamReader sr = File.OpenText(_sFile))
            {
                line = sr.ReadLine().Split(',');
                row = int.Parse(line[0]);
                col = int.Parse(line[1]);
                transactions = new string[row, col];                               

                for (int i = 0; i < row; i++)
                {                    
                    line = sr.ReadLine().Split(',');
                    for (int j = 0; j < col; j++)
                    {
                        transactions[i, j] = line[j];
                    }

                    int patient = int.Parse(transactions[i, 0]);
                    if (!patients.Keys.Contains(patient))
                    {
                        patients.Add(patient, nPatient++);
                    }               
                }                
            }
            return transactions;         
        }
        #endregion                               

        #region sort a transaction in descending order of support
        void sortRow(List<Item> items, Dictionary<Item, int> MapSupport)
        {
            for (int i = 0; i < items.Count(); i++)
            {
                for (int j = i + 1; j < items.Count(); j++)
                {
                    if (MapSupport[items[j]] > MapSupport[items[i]])
                    {
                        Item t = items[i];
                        items[i] = items[j];
                        items[j] = t;
                    }
                    else if (MapSupport[items[j]] == MapSupport[items[i]])
                    {
                        if (string.Compare(items[j].name, items[i].name, true) < 0) // j_name precedes i_name
                        {
                            Item t = items[i];
                            items[i] = items[j];
                            items[j] = t;
                        }
                    }
                }
            }
        }
        #endregion

        #region FP-Growth
        void fp_growth(FPTree tree, Itemset prefixAlpha, int prefixSupport, Dictionary<Item, int> MapSupport, double minSup)
        {
            if (tree.hasMoreThanOnePath == false)
            {
                addAllCombinationsWithPrefix(tree.root.children[0], prefixAlpha);
            }
            else
            {
                fp_growthMoreThanOnePath(tree, prefixAlpha, prefixSupport, MapSupport, minSup);
            }
        }

        void fp_growthMoreThanOnePath(FPTree tree, Itemset prefixAlpha, int prefixSupport, Dictionary<Item, int> MapSupport, double minSup)
        {
            // for each frequent item in the header table list of the tree in reverse order.
            for (int i = tree.HeaderList.Count() - 1; i >= 0; i--)
            {
                // get the item
                Item item = tree.HeaderList[i];
                int support = MapSupport[item];
                // if the item is not frequent, we skip it
                if (support < minSup)
                {
                    continue;
                }

                // create Beta by concatenating Alpha with the current item and add it to the list of frequent patterns
                Itemset beta = new Itemset();
                beta.items = new List<Item>(prefixAlpha.items);
                beta.items.Add(item);
                if (prefixAlpha.items.Count() == 0) // prefixAlpha is null
                {
                    beta.Obidset = item.Obidset;
                }
                else
                {
                    beta.Obidset = intersectObidsets(prefixAlpha.Obidset, item.Obidset);
                }
                // calculate the support of beta
                beta.support = (prefixSupport < support) ? prefixSupport : support;
                FIs.Add(beta);

                // === Construct beta's conditional pattern base ===
                // it is a subdatabase which consists of the set of prefix paths in the FP-tree co-occurring with the suffix pattern.
                List<List<Node>> prefixPaths = new List<List<Node>>();
                Node node_item = tree.MapItemNode[item];

                while (node_item.item != null)
                {
                    // if the path is not just the root node
                    if (node_item.parent.item.name != "")
                    {
                        // create the prefixpath
                        List<Node> prefixPath = new List<Node>();
                        // add this node.
                        prefixPath.Add(node_item);
                        // NOTE: we add it just to keep its support, actually it should not be part of the prefixPath

                        // recursively add all the parents of this node.
                        Node parent = node_item.parent;
                        while (parent.item.name != "")
                        {
                            prefixPath.Add(parent);
                            parent = parent.parent;
                        }
                        // add the path to the list of prefixpaths
                        prefixPaths.Add(prefixPath);
                    }
                    // we will look for the next prefixpath
                    node_item = node_item.link;
                }

                // (A) Calculate the frequency of each item in the prefixpath
                ItemComparer itemComparer = new ItemComparer();
                Dictionary<Item, int> MapSupportBeta = new Dictionary<Item, int>(itemComparer);

                // for each prefixpath
                foreach (List<Node> prefixPath in prefixPaths)
                {
                    // the support of the prefixpath is the support of its first node.
                    int pathCount = prefixPath[0].counter;
                    // for each node in the prefixpath, except the first one, we count the frequency
                    for (int j = 1; j < prefixPath.Count(); j++)
                    {
                        Node node = prefixPath[j];
                        // if the first time we see that node id
                        if (!MapSupportBeta.Keys.Contains(node.item, itemComparer))
                        {
                            // just add the path count
                            MapSupportBeta.Add(node.item, pathCount);
                        }
                        else
                        {
                            // otherwise, make the sum with the value already stored
                            MapSupportBeta[node.item] += pathCount; // KIEM TRA LAI!!!
                        }
                    }
                }

                // (B) Construct beta's conditional FP-Tree
                // create the tree.
                FPTree treeBeta = new FPTree();
                // add each prefixpath in the FP-tree.
                foreach (List<Node> prefixPath in prefixPaths)
                {
                    treeBeta.addPrefixPath(prefixPath, MapSupportBeta, minSup);
                }
                // create the header list.
                treeBeta.createHeaderList(MapSupportBeta);

                // mine recursively the Beta tree if the root as child(s)
                if (treeBeta.root.children.Count() > 0)
                {
                    // recursive call
                    fp_growth(treeBeta, beta, beta.support, MapSupportBeta, minSup);
                }
            }
        }

        void addAllCombinationsWithPrefix(Node node, Itemset prefix)
        {
            // Concatenate the node item to the current prefix
            Itemset itemset = new Itemset();
            itemset.items = new List<Item>(prefix.items);
            itemset.items.Add(node.item);
            itemset.Obidset = intersectObidsets(prefix.Obidset, node.item.Obidset);
            itemset.support = node.counter;
            FIs.Add(itemset);

            // recursive call if there is a node link
            if (node.children.Count() > 0)
            {
                addAllCombinationsWithPrefix(node.children[0], itemset);
                addAllCombinationsWithPrefix(node.children[0], prefix);
            }
        }
        #endregion          
   
        #region get power set
        List<List<Item>> PowerSet(List<Item> list)
        {
            int n = 1 << list.Count;
            List<List<Item>> powerset = new List<List<Item>>();
            for (int i = 0; i < n; i++)
            {
                List<Item> set = new List<Item>();
                int j = 0;
                for (int bits = i; bits != 0; j++)
                {
                    if ((bits & 1) != 0)
                    {
                        set.Add(list[j]);
                    }
                    bits >>= 1;
                }
                powerset.Add(set);
            }

            return powerset;
        }
        #endregion

        #region ENUMERATE-TAR function
        void ENUMERATE_TAR(double minDirConf, int gap)
        {
            foreach (Itemset itemset in FIs)
            {
                if (itemset.items.Count > 1)
                {
                    List<List<Item>> subset = PowerSet(itemset.items);
                    subset.RemoveAt(0);
                    subset.RemoveAt(subset.Count() - 1);
                    // suport of rule
                    int Sup_LHS_RHS = itemset.support;

                    foreach (List<Item> LHS in subset)
                    {                                               
                        // compute support of LHS
                        List<int> LHS_Obidset = new List<int>(LHS[0].Obidset);
                        for (int i = 1; i < LHS.Count(); i++)
                        {
                            LHS_Obidset = intersectObidsets(LHS_Obidset, LHS[i].Obidset);
                        }
                        int Sup_LHS = LHS_Obidset.Count();
                        // compute confidence
                        double Conf = (double)Sup_LHS_RHS / Sup_LHS;

                        // compute RHS
                        List<Item> RHS = new List<Item>(itemset.items);
                        foreach (Item it in LHS)
                        {
                            RHS.RemoveAll(r => r.name == it.name);
                        }
                        // compute support of RHS
                        List<int> RHS_Obidset = new List<int>(RHS[0].Obidset);
                        for (int i = 1; i < RHS.Count(); i++)
                        {
                            RHS_Obidset = intersectObidsets(RHS_Obidset, RHS[i].Obidset);
                        }
                        int Sup_RHS = RHS_Obidset.Count();
                        double Lift_Rule = (double)(nPatient * Sup_LHS_RHS) / (Sup_LHS * Sup_RHS);
                        double Conv = (double)((1 - (Sup_RHS / nPatient)) / (1 - Conf));
                        // compute direction support
                        int DirSup = 0;
                        foreach (int p in itemset.Obidset) // for each patient
                        {
                            // compute max(LHS)
                            DateTime date_max = Convert.ToDateTime(LHS[0].Tempidset[p], culture);
                            for (int i = 1; i < LHS.Count(); i++)
                            {
                                DateTime date = Convert.ToDateTime(LHS[i].Tempidset[p], culture);
                                if (date > date_max)
                                {
                                    date_max = date;
                                }
                            }
                            // compute min(RHS)
                            DateTime date_min = Convert.ToDateTime(RHS[0].Tempidset[p], culture);
                            for (int i = 1; i < RHS.Count(); i++)
                            {
                                DateTime date = Convert.ToDateTime(RHS[i].Tempidset[p], culture);
                                if (date < date_min)
                                {
                                    date_min = date;
                                }
                            }
                            if ((date_min - date_max).Days >= gap)
                            {
                                DirSup++;
                            }
                        }
                        double DirConf = (double)DirSup / Sup_LHS;
                        if (DirConf >= minDirConf)
                        {
                            if (chkSingleRadiation.Checked)
                            {
                                if (LHS.Count() == 1 && LHS[0].name == "1526900")
                                {
                                    Rule rule = new Rule();
                                    rule.LHS = LHS; rule.RHS = RHS;
                                    rule.Sup = Math.Round((double)Sup_LHS_RHS / nPatient, 3); rule.DirSup = Math.Round((double)DirSup / nPatient, 3);
                                    rule.Conf = Math.Round(Conf, 3); rule.DirConf = Math.Round(DirConf, 3);
                                    rule.Lift = Math.Round(Lift_Rule, 3);
                                    rule.Conv = Math.Round(Conv, 3);
                                    TARs.Add(rule);
                                }
                            }
                            else
                            {
                                Rule rule = new Rule();
                                rule.LHS = LHS; rule.RHS = RHS;
                                rule.Sup = Math.Round((double)Sup_LHS_RHS / nPatient, 3); rule.DirSup = Math.Round((double)DirSup / nPatient, 3);
                                rule.Conf = Math.Round(Conf, 3); rule.DirConf = Math.Round(DirConf, 3);
                                rule.Lift = Math.Round(Lift_Rule, 3);
                                rule.Conv = Math.Round(Conv, 3);
                                TARs.Add(rule);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        private void btMining_Click(object sender, EventArgs e)
        {
            // reset values
            nPatient = 0; patients = new Dictionary<int, int>();
            FIs = new List<Itemset>();
            TARs = new List<Rule>();
            string _sFile = txtDatabase.Text.Trim();
            string[,] transactions = readDataset(_sFile);
            double minSup = (double.Parse(txtMinSup.Text.Trim()) * nPatient) / 100;
            double minDirConf = double.Parse(txtDirConf.Text.Trim()) / 100;
            int gap = int.Parse(txtGap.Text.Trim());
            txtPatients.Text = nPatient.ToString();

            TreeNode tnode = treeResult.Nodes.Add("minSup = " + txtMinSup.Text + "%, minDirConf = " + txtDirConf.Text + 
                                                    "%, gap = " + txtGap.Text + " days");
            // find frequent patterns
            Stopwatch sw1 = Stopwatch.StartNew();
            // find supports of single 1-itemsets
            ItemComparer itemComparer = new ItemComparer();
            Dictionary<Item, int> MapSupport = new Dictionary<Item, int>(itemComparer);
            findMapSupport(transactions, MapSupport);
            // build FP-Growth tree
            FPTree tree = new FPTree();
            string[] lines = File.ReadAllLines(_sFile);
            List<Item> items = new List<Item>();
            for (int i = 1; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(',');                
                Item item = new Item() { name = line[2], description = line[3] };
                if (MapSupport[item] >= minSup)
                {
                    foreach (var kv in MapSupport)
                    {
                        if (kv.Key.name == item.name)
                        {
                            item.Obidset = kv.Key.Obidset;
                            item.Tempidset = kv.Key.Tempidset;
                            break;
                        }
                    }
                    items.Add(item);
                }
                if (i < lines.Length - 1)
                {
                    string[] next_line = lines[i + 1].Split(',');
                    if (line[0] != next_line[0])
                    {
                        sortRow(items, MapSupport);
                        tree.addTransaction(items);
                        items = new List<Item>();
                    }
                }
                else // end of file
                {
                    sortRow(items, MapSupport);
                    tree.addTransaction(items);
                }
            }    
            // mine frequent itemsets
            tree.createHeaderList(MapSupport);
            Itemset prefixAlpha = new Itemset();
            fp_growth(tree, prefixAlpha, patients.Count(), MapSupport, minSup);
            sw1.Stop();
            long time_FI = sw1.ElapsedMilliseconds;
            // find temporal association rules
            Stopwatch sw2 = Stopwatch.StartNew();
            ENUMERATE_TAR(minDirConf, gap);
            sw2.Stop();
            long time_Rule = sw2.ElapsedMilliseconds;
            tnode.Nodes.Add("FIs: " + FIs.Count + ". Rules: " + TARs.Count + ". Time_FI: " + time_FI / 1000.0 + " (s). Time_Rule: " + 
                            time_Rule / 1000.0 + " (s). Time: " + (time_FI + time_Rule) / 1000.0 + " (s).");
            MessageBox.Show("Finish!");
        }
    }
}
