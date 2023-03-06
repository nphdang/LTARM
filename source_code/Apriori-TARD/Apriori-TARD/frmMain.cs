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

namespace Apriori_TARD
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
        Dictionary<string, int> patients = new Dictionary<string, int>(); 
        // store frequent itemsets
        List<Node> FI = new List<Node>();
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

        #region union of two itemsets
        List<Item> unionItemsets(List<Item> a1, List<Item> a2)
        {
            List<Item> a = new List<Item>();
            for (int i = 0; i < a1.Count; i++)
            {
                a.Add(a1[i]);
            }
            a.Add(a2[a2.Count - 1]);

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

                    string patient = transactions[i, 0].ToString();
                    if (!patients.Keys.Contains(patient))
                    {
                        patients.Add(patient, nPatient++);
                    }
                }
            }
            return transactions;
        }
        #endregion

        #region construct OID Table
        List<Node> constructOID_Table(double minSup, string[,] transactions)
        {            
            List<Node> F1 = new List<Node>();
            // array contains items
            Item[] items = new Item[row];
            // the real number of items
            int nCode = 0;
            int k = 0;

            for (int i = 0; i < row; i++)
            {
                // check old items
                for (k = 0; k < nCode; k++)
                {
                    if (items[k].name == transactions[i, 2])
                    {
                        // count Sup
                        items[k].support++;
                        string patient = transactions[i, 0].ToString();
                        int patient_code = patients[patient]; // get map code of a patient from dictionary
                        items[k].Obidset.Add(patient_code);
                        //items[k].Tempidset[patient_code] = transactions[i, 1];
                        items[k].Tempidset.Add(transactions[i, 1]);
                        break;
                    }
                }
                if (k == nCode) // new item
                {
                    items[nCode] = new Item();
                    items[nCode].id = nCode;
                    items[nCode].support = 1;
                    string patient = transactions[i, 0].ToString();
                    int patient_code = patients[patient]; // get map code of a patient from dictionary
                    items[nCode].Obidset.Add(patient_code);
                    //string[] tempidset = new string[nPatient];
                    //items[nCode].Tempidset = new List<string>(tempidset);
                    //items[nCode].Tempidset[patient_code] = transactions[i, 1];
                    items[nCode].Tempidset.Add(transactions[i, 1]);
                    string des = transactions[i, 3];
                    if (chkFullNodeLabel.Checked == false)
                    {
                        if (des.Length > 30)
                        {
                            des = des.Substring(0, 30) + "...";
                        }
                    }
                    items[nCode].description = des;
                    items[nCode++].name = transactions[i, 2];
                }

            }

            for (k = 0; k < nCode; k++)
            {
                Node node = new Node();

                if (items[k].support >= minSup)
                {
                    node.itemset.Add(items[k]);
                    node.Obidset = items[k].Obidset;                    
                    F1.Add(node);
                    FI.Add(node);
                }
            }
            // return the set of frequent 1-itemsets
            return F1;
        }
        #endregion                                

        #region Apriori fuction
        List<Node> Apriori(double minSup, Node node, List<Node> FrequentItemsets, int idx)
        {
            List<Node> L = new List<Node>();            
            if (node.itemset.Count > 1) // k-itemset
            {
                // combine this itemset with others (after the itemset)
                for (int i = idx + 1; i < FrequentItemsets.Count(); i++)
                {
                    // the next_itemset is after the itemset
                    Node next_node = FrequentItemsets[i];                    
                    // two itemsets have the same prefix items
                    bool same_branch = true;
                    for (int j = 0; j < node.itemset.Count - 1; j++)
                    {
                        if (node.itemset[j].name != next_node.itemset[j].name)
                        {
                            same_branch = false;
                            break;
                        }                        
                    }

                    if (same_branch) // two itemsets have the same prefix items
                    {
                        Node O = new Node();
                        O.Obidset = intersectObidsets(node.Obidset, next_node.Obidset);                                              

                        if (O.Obidset.Count >= minSup)
                        {
                            O.itemset = unionItemsets(node.itemset, next_node.itemset);
                            L.Add(O);
                            FI.Add(O);
                        }
                    }
                                        
                }
            }
            else // 1-itemset
            {
                // combine this itemset with others (after the itemset)
                for (int i = idx + 1; i < FrequentItemsets.Count(); i++)
                {
                    // the next_itemset is after the itemset
                    Node next_node = FrequentItemsets[i];                    
                    Node O = new Node();
                    O.Obidset = intersectObidsets(node.Obidset, next_node.Obidset);

                    if (O.Obidset.Count >= minSup)
                    {
                        O.itemset = unionItemsets(node.itemset, next_node.itemset);
                        L.Add(O);
                        FI.Add(O);
                    }
                    
                }
            }

            // return the set of frequent itemsets generated by the itemset
            return L;
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
            foreach (Node node_L in FI)
            {
                if (node_L.itemset.Count > 1)
                {
                    List<List<Item>> subset = PowerSet(node_L.itemset);
                    subset.RemoveAt(0);
                    subset.RemoveAt(subset.Count() - 1);

                    foreach (List<Item> LHS in subset)
                    {                        
                        int Sup_LHS_RHS = node_L.Obidset.Count;
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
                        List<Item> RHS = new List<Item>(node_L.itemset); // node_L.itemset \ LHS
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
                        foreach (int p in node_L.Obidset) // for each patient
                        {
                            // compute max(LHS)
                            //DateTime date_max = Convert.ToDateTime(LHS[0].Tempidset[p], culture);
                            // find index of p
                            int p_index = LHS[0].Obidset.IndexOf(p);
                            DateTime date_max = Convert.ToDateTime(LHS[0].Tempidset[p_index], culture);
                            for (int i = 1; i < LHS.Count(); i++)
                            {
                                //DateTime date = Convert.ToDateTime(LHS[i].Tempidset[p], culture);
                                // find index of p
                                p_index = LHS[i].Obidset.IndexOf(p);
                                DateTime date = Convert.ToDateTime(LHS[i].Tempidset[p_index], culture);
                                if (date > date_max)
                                {
                                    date_max = date;
                                }
                            }
                            // compute min(RHS)
                            //DateTime date_min = Convert.ToDateTime(RHS[0].Tempidset[p], culture);
                            // find index of p
                            p_index = RHS[0].Obidset.IndexOf(p);
                            DateTime date_min = Convert.ToDateTime(RHS[0].Tempidset[p_index], culture);
                            for (int i = 1; i < RHS.Count(); i++)
                            {
                                //DateTime date = Convert.ToDateTime(RHS[i].Tempidset[p], culture);
                                // find index of p
                                p_index = RHS[i].Obidset.IndexOf(p);
                                DateTime date = Convert.ToDateTime(RHS[i].Tempidset[p_index], culture);
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

        #region write rules to text file (csv and txt)
        void writeRuleText(string file_csv, string file_text)
        {
            // write rules to file CSV
            using (StreamWriter sw = new StreamWriter(file_csv))
            {
                sw.WriteLine("LHS,RHS,sup,dirsup,conf,dirconf,lift,conv");
                foreach (Rule rule in TARs)
                {
                    string LHS = ""; string RHS = "";
                    foreach (Item item in rule.LHS)
                    {
                        LHS += item.name + ": " + item.description + " & ";
                    }
                    LHS = LHS.Remove(LHS.Length - 3);
                    foreach (Item item in rule.RHS)
                    {
                        RHS += item.name + ": " + item.description + " & ";
                    }
                    RHS = RHS.Remove(RHS.Length - 3);
                    sw.WriteLine(LHS + "," + RHS + "," + rule.Sup + "," + rule.DirSup + "," + rule.Conf + "," + rule.DirConf +
                                    "," + rule.Lift + "," + rule.Conv);
                }
            }

            // write rules to file TEXT
            using (StreamWriter sw = new StreamWriter(file_text))
            {
                foreach (Rule rule in TARs)
                {
                    string LHS = ""; string RHS = "";
                    foreach (Item item in rule.LHS)
                    {
                        LHS += item.name + ": " + item.description + " & ";
                    }
                    LHS = LHS.Remove(LHS.Length - 3);
                    foreach (Item item in rule.RHS)
                    {
                        RHS += item.name + ": " + item.description + " & ";
                    }
                    RHS = RHS.Remove(RHS.Length - 3);
                    sw.WriteLine(LHS + " ==> " + RHS + " (Sup=" + rule.Sup + ", DirSup=" + rule.DirSup + ", Conf=" + rule.Conf +
                                    ", DirConf=" + rule.DirConf + ", Lift=" + rule.Lift + ", Conv=" + rule.Conv + ")");
                }
            }
        }
        #endregion

        #region write rules to graph file (PMML format)
        void writeRuleGraph(string file, List<Node> Lr)
        {
            using (XmlWriter writer = XmlWriter.Create(file))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("PMML");
                writer.WriteStartElement("AssociationModel");
                writer.WriteAttributeString("functionName", "associationRules");
                writer.WriteAttributeString("numberOfTransactions", nPatient.ToString());
                writer.WriteAttributeString("numberOfItems", Lr.Count().ToString());
                writer.WriteAttributeString("minimumSupport", txtMinSup.Text + "%");
                writer.WriteAttributeString("minimumDirConfidence", txtDirConf.Text + "%");
                writer.WriteAttributeString("timeGap", txtGap.Text + "days");
                writer.WriteAttributeString("numberOfItemsets", FI.Count().ToString());
                writer.WriteAttributeString("numberOfRules", TARs.Count().ToString());

                foreach (Node node in Lr)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteAttributeString("id", node.itemset[0].id.ToString());
                    writer.WriteAttributeString("value", node.itemset[0].name + ": " + node.itemset[0].description);
                    writer.WriteEndElement();
                }
                foreach (Node node in FI)
                {
                    writer.WriteStartElement("Itemset");
                    string id = "";
                    foreach (Item item in node.itemset)
                    {
                        id += item.id;
                    }
                    writer.WriteAttributeString("id", id);
                    writer.WriteAttributeString("numberOfItems", node.itemset.Count().ToString());
                    foreach (Item item in node.itemset)
                    {
                        writer.WriteStartElement("ItemRef");
                        writer.WriteAttributeString("itemRef", item.id.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                foreach (Rule rule in TARs)
                {
                    writer.WriteStartElement("AssociationRule");
                    writer.WriteAttributeString("support", rule.Sup.ToString());
                    writer.WriteAttributeString("dir_support", rule.DirSup.ToString());
                    writer.WriteAttributeString("confidence", rule.Conf.ToString());
                    writer.WriteAttributeString("dir_confidence", rule.DirConf.ToString());
                    writer.WriteAttributeString("lift", rule.Lift.ToString());
                    writer.WriteAttributeString("conviction", rule.Conv.ToString());
                    string antecedent = "";
                    foreach (Item item in rule.LHS)
                    {
                        antecedent += item.id;
                    }
                    writer.WriteAttributeString("antecedent", antecedent);
                    string consequent = "";
                    foreach (Item item in rule.RHS)
                    {
                        consequent += item.id;
                    }
                    writer.WriteAttributeString("consequent", consequent);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        #endregion

        private void btMining_Click(object sender, EventArgs e)
        {
            nPatient = 0; patients = new Dictionary<string, int>();
            FI = new List<Node>();
            TARs = new List<Rule>();
            string _sFile = txtDatabase.Text.Trim();
            string[,] transactions = readDataset(_sFile);
            double minSup = (double.Parse(txtMinSup.Text.Trim()) * nPatient) / 100;
            double minDirConf = double.Parse(txtDirConf.Text.Trim()) / 100;
            int gap = int.Parse(txtGap.Text.Trim());
            txtPatients.Text = nPatient.ToString();

            TreeNode tnode = treeResult.Nodes.Add("minSup = " + txtMinSup.Text + "%, minDirConf = " + txtDirConf.Text + 
                                                    "%, gap = " + txtGap.Text + " days");
            Stopwatch sw1 = Stopwatch.StartNew();
            // obtain frequent 1-itemsets
            List<Node> FrequentItemsets = constructOID_Table(minSup, transactions);
            List<Node> Lr = new List<Node>(FrequentItemsets);
            while (FrequentItemsets.Count > 0)
            {
                List<Node> allFrequentItemsets = new List<Node>();
                for (int i = 0; i < FrequentItemsets.Count(); i++)
                {
                    Node itemset = FrequentItemsets[i];
                    // the set of frequent itemsets generated by each itemset
                    // similar to each equivalence class
                    List<Node> eachFrequentItemsets = Apriori(minSup, itemset, FrequentItemsets, i);
                    if (eachFrequentItemsets.Count > 0)
                    {
                        foreach (Node it in eachFrequentItemsets)
                        {
                            allFrequentItemsets.Add(it);
                        }
                    }
                }

                // obtain frequent k-itemsets
                FrequentItemsets = new List<Node>(allFrequentItemsets);
            }
            sw1.Stop();
            long time_FI = sw1.ElapsedMilliseconds;
            Stopwatch sw2 = Stopwatch.StartNew();
            ENUMERATE_TAR(minDirConf, gap);
            sw2.Stop();
            long time_Rule = sw2.ElapsedMilliseconds;
            tnode.Nodes.Add("FIs: " + FI.Count + ". Rules: " + TARs.Count + ". Time_FI: " + time_FI / 1000.0 + " (s). Time_Rule: " + 
                            time_Rule / 1000.0 + " (s). Time: " + (time_FI + time_Rule) / 1000.0 + " (s).");
            string file_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (chkSingleRadiation.Checked)
            {
                file_path += @"\Apriori_Rad_Rules_" + txtMinSup.Text + "_" + txtDirConf.Text + "_" + gap;
            }
            else
            {
                file_path += @"\Apriori_All_Rules_" + txtMinSup.Text + "_" + txtDirConf.Text + "_" + gap;
            }
            if (chkWriteText.Checked)
            {
                if (TARs.Count > 0)
                {
                    writeRuleText(file_path + ".csv", file_path + ".txt");
                }
            }
            if (chkWriteGraph.Checked)
            {
                writeRuleGraph(file_path + ".xml", Lr);
            } 
            MessageBox.Show("Finish!");
        }                 
    }
}
