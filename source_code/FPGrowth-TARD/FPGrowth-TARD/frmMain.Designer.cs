﻿namespace FPGrowth_TARD
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.btBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMinSup = new System.Windows.Forms.TextBox();
            this.btMining = new System.Windows.Forms.Button();
            this.treeResult = new System.Windows.Forms.TreeView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtGap = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDirConf = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPatients = new System.Windows.Forms.TextBox();
            this.chkFullNodeLabel = new System.Windows.Forms.CheckBox();
            this.chkSingleRadiation = new System.Windows.Forms.CheckBox();
            this.chkWriteText = new System.Windows.Forms.CheckBox();
            this.chkWriteGraph = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dataset";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(81, 16);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(397, 20);
            this.txtDatabase.TabIndex = 1;
            // 
            // btBrowse
            // 
            this.btBrowse.Location = new System.Drawing.Point(484, 14);
            this.btBrowse.Name = "btBrowse";
            this.btBrowse.Size = new System.Drawing.Size(75, 23);
            this.btBrowse.TabIndex = 2;
            this.btBrowse.Text = "Browse";
            this.btBrowse.UseVisualStyleBackColor = true;
            this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "minSup";
            // 
            // txtMinSup
            // 
            this.txtMinSup.Location = new System.Drawing.Point(81, 44);
            this.txtMinSup.Name = "txtMinSup";
            this.txtMinSup.Size = new System.Drawing.Size(36, 20);
            this.txtMinSup.TabIndex = 5;
            this.txtMinSup.Text = "5";
            // 
            // btMining
            // 
            this.btMining.Location = new System.Drawing.Point(25, 80);
            this.btMining.Name = "btMining";
            this.btMining.Size = new System.Drawing.Size(92, 41);
            this.btMining.TabIndex = 24;
            this.btMining.Text = "Discover";
            this.btMining.UseVisualStyleBackColor = true;
            this.btMining.Click += new System.EventHandler(this.btMining_Click);
            // 
            // treeResult
            // 
            this.treeResult.Location = new System.Drawing.Point(12, 127);
            this.treeResult.Name = "treeResult";
            this.treeResult.Size = new System.Drawing.Size(550, 321);
            this.treeResult.TabIndex = 28;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(123, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "%";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(268, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "gap";
            // 
            // txtGap
            // 
            this.txtGap.Location = new System.Drawing.Point(299, 44);
            this.txtGap.Name = "txtGap";
            this.txtGap.Size = new System.Drawing.Size(36, 20);
            this.txtGap.TabIndex = 11;
            this.txtGap.Text = "30";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(341, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "days";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(247, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "%";
            // 
            // txtDirConf
            // 
            this.txtDirConf.Location = new System.Drawing.Point(208, 44);
            this.txtDirConf.Name = "txtDirConf";
            this.txtDirConf.Size = new System.Drawing.Size(33, 20);
            this.txtDirConf.TabIndex = 9;
            this.txtDirConf.Text = "10";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(144, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "minDirConf";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(123, 80);
            this.label10.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "Patients";
            // 
            // txtPatients
            // 
            this.txtPatients.Location = new System.Drawing.Point(170, 78);
            this.txtPatients.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.txtPatients.Name = "txtPatients";
            this.txtPatients.Size = new System.Drawing.Size(82, 20);
            this.txtPatients.TabIndex = 15;
            // 
            // chkFullNodeLabel
            // 
            this.chkFullNodeLabel.AutoSize = true;
            this.chkFullNodeLabel.Checked = true;
            this.chkFullNodeLabel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFullNodeLabel.Location = new System.Drawing.Point(379, 80);
            this.chkFullNodeLabel.Name = "chkFullNodeLabel";
            this.chkFullNodeLabel.Size = new System.Drawing.Size(100, 17);
            this.chkFullNodeLabel.TabIndex = 32;
            this.chkFullNodeLabel.Text = "Full Node Label";
            this.chkFullNodeLabel.UseVisualStyleBackColor = true;
            // 
            // chkSingleRadiation
            // 
            this.chkSingleRadiation.AutoSize = true;
            this.chkSingleRadiation.Location = new System.Drawing.Point(256, 79);
            this.chkSingleRadiation.Name = "chkSingleRadiation";
            this.chkSingleRadiation.Size = new System.Drawing.Size(103, 17);
            this.chkSingleRadiation.TabIndex = 34;
            this.chkSingleRadiation.Text = "Single Radiation";
            this.chkSingleRadiation.UseVisualStyleBackColor = true;
            // 
            // chkWriteText
            // 
            this.chkWriteText.AutoSize = true;
            this.chkWriteText.Location = new System.Drawing.Point(256, 104);
            this.chkWriteText.Name = "chkWriteText";
            this.chkWriteText.Size = new System.Drawing.Size(117, 17);
            this.chkWriteText.TabIndex = 35;
            this.chkWriteText.Text = "Write Rules to Text";
            this.chkWriteText.UseVisualStyleBackColor = true;
            // 
            // chkWriteGraph
            // 
            this.chkWriteGraph.AutoSize = true;
            this.chkWriteGraph.Location = new System.Drawing.Point(379, 104);
            this.chkWriteGraph.Name = "chkWriteGraph";
            this.chkWriteGraph.Size = new System.Drawing.Size(125, 17);
            this.chkWriteGraph.TabIndex = 36;
            this.chkWriteGraph.Text = "Write Rules to Graph";
            this.chkWriteGraph.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 455);
            this.Controls.Add(this.chkWriteGraph);
            this.Controls.Add(this.chkWriteText);
            this.Controls.Add(this.chkSingleRadiation);
            this.Controls.Add(this.chkFullNodeLabel);
            this.Controls.Add(this.txtPatients);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtDirConf);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtGap);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.treeResult);
            this.Controls.Add(this.btMining);
            this.Controls.Add(this.txtMinSup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btBrowse);
            this.Controls.Add(this.txtDatabase);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "FPGrowth-based Temporal Association Rule Discovery (FPGrowth-TARD)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Button btBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMinSup;
        private System.Windows.Forms.Button btMining;
        private System.Windows.Forms.TreeView treeResult;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtGap;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDirConf;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtPatients;
        private System.Windows.Forms.CheckBox chkFullNodeLabel;
        private System.Windows.Forms.CheckBox chkSingleRadiation;
        private System.Windows.Forms.CheckBox chkWriteText;
        private System.Windows.Forms.CheckBox chkWriteGraph;
    }
}

