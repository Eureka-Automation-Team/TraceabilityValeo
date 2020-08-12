namespace Trace.OpcHandlerMachine05
{
    partial class MonitoringForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitoringForm));
            this.butRequestVerifyCode1 = new System.Windows.Forms.Button();
            this.butCompletedLogging = new System.Windows.Forms.Button();
            this.butRequestLogging = new System.Windows.Forms.Button();
            this.butStatusMc = new System.Windows.Forms.Button();
            this.txtMessageResult = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.butRefresh = new System.Windows.Forms.Button();
            this.butConnect = new System.Windows.Forms.Button();
            this.txtServerUrl = new System.Windows.Forms.TextBox();
            this.butMakeReady = new System.Windows.Forms.Button();
            this.timerInter = new System.Windows.Forms.Timer(this.components);
            this.timerConnect = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // butRequestVerifyCode1
            // 
            this.butRequestVerifyCode1.AutoSize = true;
            this.butRequestVerifyCode1.BackColor = System.Drawing.Color.Gray;
            this.butRequestVerifyCode1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.butRequestVerifyCode1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butRequestVerifyCode1.Location = new System.Drawing.Point(163, 162);
            this.butRequestVerifyCode1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butRequestVerifyCode1.Name = "butRequestVerifyCode1";
            this.butRequestVerifyCode1.Size = new System.Drawing.Size(145, 24);
            this.butRequestVerifyCode1.TabIndex = 34;
            this.butRequestVerifyCode1.UseVisualStyleBackColor = false;
            this.butRequestVerifyCode1.Click += new System.EventHandler(this.butRequestVerifyCode1_Click);
            // 
            // butCompletedLogging
            // 
            this.butCompletedLogging.AutoSize = true;
            this.butCompletedLogging.BackColor = System.Drawing.Color.Gray;
            this.butCompletedLogging.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.butCompletedLogging.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butCompletedLogging.Location = new System.Drawing.Point(163, 131);
            this.butCompletedLogging.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butCompletedLogging.Name = "butCompletedLogging";
            this.butCompletedLogging.Size = new System.Drawing.Size(145, 24);
            this.butCompletedLogging.TabIndex = 33;
            this.butCompletedLogging.UseVisualStyleBackColor = false;
            // 
            // butRequestLogging
            // 
            this.butRequestLogging.AutoSize = true;
            this.butRequestLogging.BackColor = System.Drawing.Color.Gray;
            this.butRequestLogging.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.butRequestLogging.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butRequestLogging.Location = new System.Drawing.Point(163, 100);
            this.butRequestLogging.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butRequestLogging.Name = "butRequestLogging";
            this.butRequestLogging.Size = new System.Drawing.Size(145, 24);
            this.butRequestLogging.TabIndex = 32;
            this.butRequestLogging.UseVisualStyleBackColor = false;
            this.butRequestLogging.Click += new System.EventHandler(this.butRequestLogging_Click);
            // 
            // butStatusMc
            // 
            this.butStatusMc.AutoSize = true;
            this.butStatusMc.BackColor = System.Drawing.Color.Gray;
            this.butStatusMc.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.butStatusMc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butStatusMc.Location = new System.Drawing.Point(163, 69);
            this.butStatusMc.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butStatusMc.Name = "butStatusMc";
            this.butStatusMc.Size = new System.Drawing.Size(145, 24);
            this.butStatusMc.TabIndex = 31;
            this.butStatusMc.UseVisualStyleBackColor = false;
            // 
            // txtMessageResult
            // 
            this.txtMessageResult.Location = new System.Drawing.Point(14, 191);
            this.txtMessageResult.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMessageResult.Multiline = true;
            this.txtMessageResult.Name = "txtMessageResult";
            this.txtMessageResult.Size = new System.Drawing.Size(294, 58);
            this.txtMessageResult.TabIndex = 30;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(140)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(13, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 29;
            this.label2.Text = "OPC URL";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(140)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(13, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 24);
            this.label4.TabIndex = 25;
            this.label4.Text = "Status";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(140)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(13, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 24);
            this.label3.TabIndex = 26;
            this.label3.Text = "Request Logging";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(140)))));
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(13, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 24);
            this.label5.TabIndex = 27;
            this.label5.Text = "Completed Log";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(140)))));
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ForeColor = System.Drawing.Color.White;
            this.label29.Location = new System.Drawing.Point(13, 162);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(143, 24);
            this.label29.TabIndex = 28;
            this.label29.Text = "Verify Code";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butRefresh
            // 
            this.butRefresh.BackColor = System.Drawing.SystemColors.Control;
            this.butRefresh.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.butRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.butRefresh.Location = new System.Drawing.Point(225, 36);
            this.butRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butRefresh.Name = "butRefresh";
            this.butRefresh.Size = new System.Drawing.Size(83, 28);
            this.butRefresh.TabIndex = 24;
            this.butRefresh.Text = "Refresh";
            this.butRefresh.UseVisualStyleBackColor = false;
            this.butRefresh.Visible = false;
            this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
            // 
            // butConnect
            // 
            this.butConnect.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.butConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.butConnect.Location = new System.Drawing.Point(14, 36);
            this.butConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butConnect.Name = "butConnect";
            this.butConnect.Size = new System.Drawing.Size(112, 28);
            this.butConnect.TabIndex = 22;
            this.butConnect.Text = "Connect";
            this.butConnect.UseVisualStyleBackColor = true;
            this.butConnect.Click += new System.EventHandler(this.butConnect_Click);
            // 
            // txtServerUrl
            // 
            this.txtServerUrl.Location = new System.Drawing.Point(95, 11);
            this.txtServerUrl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(213, 20);
            this.txtServerUrl.TabIndex = 21;
            // 
            // butMakeReady
            // 
            this.butMakeReady.BackColor = System.Drawing.Color.Gray;
            this.butMakeReady.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.butMakeReady.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butMakeReady.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.butMakeReady.Location = new System.Drawing.Point(134, 36);
            this.butMakeReady.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butMakeReady.Name = "butMakeReady";
            this.butMakeReady.Size = new System.Drawing.Size(83, 28);
            this.butMakeReady.TabIndex = 23;
            this.butMakeReady.Text = "Not ready";
            this.butMakeReady.UseVisualStyleBackColor = false;
            this.butMakeReady.Visible = false;
            this.butMakeReady.Click += new System.EventHandler(this.butMakeReady_Click);
            // 
            // timerInter
            // 
            this.timerInter.Interval = 1000;
            this.timerInter.Tick += new System.EventHandler(this.timerInter_Tick);
            // 
            // timerConnect
            // 
            this.timerConnect.Enabled = true;
            this.timerConnect.Interval = 5000;
            this.timerConnect.Tick += new System.EventHandler(this.timerConnect_Tick);
            // 
            // MonitoringForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 261);
            this.Controls.Add(this.butRequestVerifyCode1);
            this.Controls.Add(this.butCompletedLogging);
            this.Controls.Add(this.butRequestLogging);
            this.Controls.Add(this.butStatusMc);
            this.Controls.Add(this.txtMessageResult);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.butRefresh);
            this.Controls.Add(this.butConnect);
            this.Controls.Add(this.txtServerUrl);
            this.Controls.Add(this.butMakeReady);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "MonitoringForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Station 4";
            this.Load += new System.EventHandler(this.MonitoringForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butRequestVerifyCode1;
        private System.Windows.Forms.Button butCompletedLogging;
        private System.Windows.Forms.Button butRequestLogging;
        private System.Windows.Forms.Button butStatusMc;
        private System.Windows.Forms.TextBox txtMessageResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button butRefresh;
        private System.Windows.Forms.Button butConnect;
        private System.Windows.Forms.TextBox txtServerUrl;
        private System.Windows.Forms.Button butMakeReady;
        private System.Windows.Forms.Timer timerInter;
        private System.Windows.Forms.Timer timerConnect;
    }
}

