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

namespace LTARM
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
        // map of patient ID: key is the patient ID in data file and value is the new patient ID in program
        Dictionary<string, int> patients = new Dictionary<string, int>();
        // the number of nodes in lattice
        int nNode = 0;
        // store Lattice
        Dictionary<int, Node> Lattice = new Dictionary<int, Node>();
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
            // sort a
            a1.Sort();

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

        #region find Lr - the root lattice contains single 1-itemsets
        List<Node> findLr(double minSup, string[,] transactions)
        {
            List<Node> Lr = new List<Node>();
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
                        items[k].Tempidset[patient_code] = transactions[i, 1];
                        //items[k].Tempidset.Add(transactions[i, 1]);
                        break;
                    }
                }
                if (k == nCode) // new item
                {
                    items[nCode] = new Item();
                    items[nCode].id = nCode;
                    items[nCode].support = 1;
                    string patient = transactions[i, 0];
                    int patient_code = patients[patient]; // get map code of a patient from dictionary
                    items[nCode].Obidset.Add(patient_code);
                    string[] tempidset = new string[nPatient];
                    items[nCode].Tempidset = new List<string>(tempidset);
                    items[nCode].Tempidset[patient_code] = transactions[i, 1];
                    //items[nCode].Tempidset.Add(transactions[i, 1]);
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
            
            //// write codes along with supports to file
            //string file_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string file_code = file_path + @"\codes.csv";
            //using (StreamWriter sw = new StreamWriter(file_code))
            //{
            //    sw.WriteLine("code,description,support");
            //    for (k = 0; k < nCode; k++)
            //    {
            //        string code = items[k].name;
            //        string desc = items[k].description;
            //        string supp = items[k].support.ToString();
            //        sw.WriteLine(code + "," + desc + "," + supp);
            //    }
            //}
            
            for (k = 0; k < nCode; k++)
            {
                Node node = new Node();                

                if (items[k].support >= minSup)
                {
                    node.itemset.Add(items[k]);
                    node.Obidset = items[k].Obidset;
                    node.id = nNode;
                    // increase the number of nodes in lattice
                    nNode++;
                    node.childrenEC = new List<int>();
                    node.childrenL = new List<int>();                    
                    Lr.Add(node);
                    Lattice.Add(node.id, node);
                }
            }                  
            return Lr;
        }
        #endregion        

        #region TARD function
        void TARD(List<Node> Lr, double minSup)
        {
            int root_len = Lr.Count;
            for (int i = 0; i < root_len; i++)
            {
                Node li = (Node)Lr[i];
                List<Node> P_i = new List<Node>();
                for (int j = i + 1; j < root_len; j++)
                {
                    Node lj = (Node)Lr[j];
                    Node O = new Node();
                    O.Obidset = intersectObidsets(li.Obidset, lj.Obidset);

                    if (O.Obidset.Count >= minSup)
                    {
                        O.id = nNode;
                        nNode++;
                        O.itemset = unionItemsets(li.itemset, lj.itemset);
                        li.childrenEC.Add(O.id);
                        lj.childrenL.Add(O.id);                                                
                        UPDATE_LATTICE(li, O);                        
                        P_i.Add(O);
                        Lattice.Add(O.id, O);
                    }
                }

                TARD(P_i, minSup);
            }
        }
        #endregion

        #region UPDATE-LATTICE function
        void UPDATE_LATTICE(Node parent, Node X)
        {
            foreach (int li in parent.childrenL)
            {
                Node node_li = (Node)Lattice[li];
                foreach (int lj in node_li.childrenEC)
                {
                    Node node_lj = (Node)Lattice[lj];
                    bool containt = true;
                    foreach (Item item_X in X.itemset)
                    {
                        bool subset = false;
                        foreach (Item item_lj in node_lj.itemset)
                        {
                            if (item_X.name == item_lj.name)
                            {
                                subset = true;
                                break;
                            }                            
                        }
                        containt &= subset;
                        if (containt == false)
                        {
                            break;
                        }
                    }
                    if (containt)
                    {
                        X.childrenL.Add(lj);
                    }
                }
            }
        }
        #endregion

        #region TRAVERSE-LATTICE
        void TRAVERSE_LATTICE(List<Node> Lr, double minDirConf, int gap)
        {
            foreach (Node node_Lc in Lr)
            {
                if (!node_Lc.traverse)
                {
                    ENUMERATE_TAR(node_Lc, minDirConf, gap);
                    node_Lc.traverse = true;
                    List<Node> node_EC = new List<Node>();
                    foreach (int i in node_Lc.childrenEC)
                    {
                        node_EC.Add((Node)Lattice[i]);
                    }
                    TRAVERSE_LATTICE(node_EC, minDirConf, gap);
                    List<Node> node_L = new List<Node>();
                    foreach (int i in node_Lc.childrenL)
                    {
                        node_L.Add((Node)Lattice[i]);
                    }
                    TRAVERSE_LATTICE(node_L, minDirConf, gap);
                }
            }            
        }
        #endregion

        #region ENUMERATE-TAR function
        void ENUMERATE_TAR(Node node_Lc, double minDirConf, int gap)
        {            
            Queue<Node> node_queue = new Queue<Node>();
            List<int> inQueue = new List<int>();

            foreach (int i in node_Lc.childrenEC)
            {
                Node node_Ls = (Node)Lattice[i];
                node_queue.Enqueue(node_Ls);
                inQueue.Add(node_Ls.id);
            }
            foreach (int i in node_Lc.childrenL)
            {
                Node node_Ls = (Node)Lattice[i];
                node_queue.Enqueue(node_Ls);
                inQueue.Add(node_Ls.id);
            }
            while (node_queue.Count() != 0)
            {
                Node node_L = node_queue.Dequeue();
                List<Item> LHS = node_Lc.itemset;                
                int Sup_LHS_RHS = node_L.Obidset.Count;
                int Sup_LHS = node_Lc.Obidset.Count();
                // compute confidence
                double Conf = (double)Sup_LHS_RHS / Sup_LHS;                                                 
                               
                if (Conf >= minDirConf)
                {
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
                        DateTime date_max = Convert.ToDateTime(LHS[0].Tempidset[p], culture);
                        // find index of p
                        //int p_index = LHS[0].Obidset.IndexOf(p);
                        //DateTime date_max = Convert.ToDateTime(LHS[0].Tempidset[p_index], culture);
                        for (int i = 1; i < LHS.Count(); i++)
                        {
                            DateTime date = Convert.ToDateTime(LHS[i].Tempidset[p], culture);
                            // find index of p
                            //p_index = LHS[i].Obidset.IndexOf(p);
                            //DateTime date = Convert.ToDateTime(LHS[i].Tempidset[p_index], culture);
                            if (date > date_max)
                            {
                                date_max = date;
                            }
                        }
                        // compute min(RHS)
                        DateTime date_min = Convert.ToDateTime(RHS[0].Tempidset[p], culture);
                        // find index of p
                        //p_index = RHS[0].Obidset.IndexOf(p);
                        //DateTime date_min = Convert.ToDateTime(RHS[0].Tempidset[p_index], culture);
                        for (int i = 1; i < RHS.Count(); i++)
                        {
                            DateTime date = Convert.ToDateTime(RHS[i].Tempidset[p], culture);
                            // find index of p
                            //p_index = RHS[i].Obidset.IndexOf(p);
                            //DateTime date = Convert.ToDateTime(RHS[i].Tempidset[p_index], culture);
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
                        foreach (int i in node_L.childrenEC)
                        {
                            Node node_Ls = (Node)Lattice[i];
                            if (!inQueue.Contains(node_Ls.id))
                            {
                                node_queue.Enqueue(node_Ls);
                                inQueue.Add(node_Ls.id);
                            }
                        }
                        foreach (int i in node_L.childrenL)
                        {
                            Node node_Ls = (Node)Lattice[i];
                            if (!inQueue.Contains(node_Ls.id))
                            {
                                node_queue.Enqueue(node_Ls);
                                inQueue.Add(node_Ls.id);
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
                writer.WriteAttributeString("numberOfItemsets", Lattice.Values.Count().ToString());
                writer.WriteAttributeString("numberOfRules", TARs.Count().ToString());

                foreach (Node node in Lr)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteAttributeString("id", node.itemset[0].id.ToString());
                    writer.WriteAttributeString("value", node.itemset[0].name + ": " + node.itemset[0].description);
                    writer.WriteEndElement();
                }
                foreach (Node node in Lattice.Values)
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
            nNode = 0; Lattice = new Dictionary<int, Node>();
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
            List<Node> Lr = findLr(minSup, transactions);
            TARD(Lr, minSup);
            sw1.Stop();
            long time_Lattice = sw1.ElapsedMilliseconds;
            Stopwatch sw2 = Stopwatch.StartNew();
            TRAVERSE_LATTICE(Lr, minDirConf, gap);
            sw2.Stop();
            long time_Rule = sw2.ElapsedMilliseconds;
            tnode.Nodes.Add("FIs: " + nNode + ". Rules: " + TARs.Count + ". Time_Lattice: " + time_Lattice / 1000.0 + " (s). Time_Rule: " + 
                            time_Rule / 1000.0 + " (s). Time: " + (time_Lattice + time_Rule) / 1000.0 + " (s).");
            string file_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (chkSingleRadiation.Checked)
            {
                file_path += @"\LTARM_Rad_Rules_" + txtMinSup.Text + "_" + txtDirConf.Text + "_" + gap;
            }
            else
            {
                file_path += @"\LTARM_All_Rules_" + txtMinSup.Text + "_" + txtDirConf.Text + "_" + gap;
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
