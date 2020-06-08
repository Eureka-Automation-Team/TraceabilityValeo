﻿namespace Trace.Monitoring
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.txtTraceabilityRdy = new System.Windows.Forms.TextBox();
            this.butConnect = new System.Windows.Forms.Button();
            this.butMakeReady = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtServerUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timerInter = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // txtTraceabilityRdy
            // 
            this.txtTraceabilityRdy.Location = new System.Drawing.Point(150, 58);
            this.txtTraceabilityRdy.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTraceabilityRdy.Name = "txtTraceabilityRdy";
            this.txtTraceabilityRdy.ReadOnly = true;
            this.txtTraceabilityRdy.Size = new System.Drawing.Size(139, 26);
            this.txtTraceabilityRdy.TabIndex = 0;
            // 
            // butConnect
            // 
            this.butConnect.Location = new System.Drawing.Point(531, 15);
            this.butConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butConnect.Name = "butConnect";
            this.butConnect.Size = new System.Drawing.Size(230, 35);
            this.butConnect.TabIndex = 1;
            this.butConnect.Text = "Connect";
            this.butConnect.UseVisualStyleBackColor = true;
            this.butConnect.Click += new System.EventHandler(this.butConnect_Click);
            // 
            // butMakeReady
            // 
            this.butMakeReady.BackColor = System.Drawing.Color.Lime;
            this.butMakeReady.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.butMakeReady.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butMakeReady.Location = new System.Drawing.Point(769, 15);
            this.butMakeReady.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butMakeReady.Name = "butMakeReady";
            this.butMakeReady.Size = new System.Drawing.Size(178, 35);
            this.butMakeReady.TabIndex = 2;
            this.butMakeReady.Text = "Ready";
            this.butMakeReady.UseVisualStyleBackColor = false;
            this.butMakeReady.Click += new System.EventHandler(this.butMakeReady_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(270, 103);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(151, 26);
            this.textBox2.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(18, 177);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1092, 152);
            this.panel1.TabIndex = 4;
            // 
            // txtServerUrl
            // 
            this.txtServerUrl.Location = new System.Drawing.Point(150, 18);
            this.txtServerUrl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(370, 26);
            this.txtServerUrl.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "OPC URL:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 63);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "System Ready :";
            // 
            // timerInter
            // 
            this.timerInter.Interval = 1000;
            this.timerInter.Tick += new System.EventHandler(this.timerInter_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1133, 541);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtServerUrl);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.butMakeReady);
            this.Controls.Add(this.butConnect);
            this.Controls.Add(this.txtTraceabilityRdy);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitoring OPC";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTraceabilityRdy;
        private System.Windows.Forms.Button butConnect;
        private System.Windows.Forms.Button butMakeReady;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtServerUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timerInter;
    }
}

