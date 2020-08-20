namespace Trace.UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.butHideSideBar = new System.Windows.Forms.Button();
            this.butSearchHistories = new System.Windows.Forms.Button();
            this.butHome = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panelMain.SuspendLayout();
            this.panelMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelContainer);
            this.panelMain.Controls.Add(this.panelMenu);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1198, 657);
            this.panelMain.TabIndex = 0;
            // 
            // panelContainer
            // 
            this.panelContainer.AutoScroll = true;
            this.panelContainer.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelContainer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(230, 0);
            this.panelContainer.Margin = new System.Windows.Forms.Padding(2);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(968, 657);
            this.panelContainer.TabIndex = 3;
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.White;
            this.panelMenu.Controls.Add(this.butHideSideBar);
            this.panelMenu.Controls.Add(this.butSearchHistories);
            this.panelMenu.Controls.Add(this.butHome);
            this.panelMenu.Controls.Add(this.panel2);
            this.panelMenu.Controls.Add(this.panel1);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Margin = new System.Windows.Forms.Padding(2);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(230, 657);
            this.panelMenu.TabIndex = 2;
            // 
            // butHideSideBar
            // 
            this.butHideSideBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butHideSideBar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butHideSideBar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.butHideSideBar.FlatAppearance.BorderSize = 0;
            this.butHideSideBar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.butHideSideBar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(187)))), ((int)(((byte)(19)))));
            this.butHideSideBar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butHideSideBar.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butHideSideBar.ForeColor = System.Drawing.Color.Gainsboro;
            this.butHideSideBar.Image = ((System.Drawing.Image)(resources.GetObject("butHideSideBar.Image")));
            this.butHideSideBar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butHideSideBar.Location = new System.Drawing.Point(184, 617);
            this.butHideSideBar.Margin = new System.Windows.Forms.Padding(0);
            this.butHideSideBar.Name = "butHideSideBar";
            this.butHideSideBar.Size = new System.Drawing.Size(44, 35);
            this.butHideSideBar.TabIndex = 6;
            this.butHideSideBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butHideSideBar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butHideSideBar.UseVisualStyleBackColor = true;
            this.butHideSideBar.Click += new System.EventHandler(this.butHideSideBar_Click);
            // 
            // butSearchHistories
            // 
            this.butSearchHistories.Dock = System.Windows.Forms.DockStyle.Top;
            this.butSearchHistories.FlatAppearance.BorderSize = 0;
            this.butSearchHistories.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(187)))), ((int)(((byte)(19)))));
            this.butSearchHistories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butSearchHistories.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.butSearchHistories.Image = ((System.Drawing.Image)(resources.GetObject("butSearchHistories.Image")));
            this.butSearchHistories.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butSearchHistories.Location = new System.Drawing.Point(0, 129);
            this.butSearchHistories.Name = "butSearchHistories";
            this.butSearchHistories.Size = new System.Drawing.Size(230, 45);
            this.butSearchHistories.TabIndex = 5;
            this.butSearchHistories.Text = "         Search Histories";
            this.butSearchHistories.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butSearchHistories.UseVisualStyleBackColor = true;
            this.butSearchHistories.Click += new System.EventHandler(this.butSearchHistories_Click);
            // 
            // butHome
            // 
            this.butHome.Dock = System.Windows.Forms.DockStyle.Top;
            this.butHome.FlatAppearance.BorderSize = 0;
            this.butHome.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(187)))), ((int)(((byte)(19)))));
            this.butHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.butHome.Image = ((System.Drawing.Image)(resources.GetObject("butHome.Image")));
            this.butHome.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butHome.Location = new System.Drawing.Point(0, 84);
            this.butHome.Name = "butHome";
            this.butHome.Size = new System.Drawing.Size(230, 45);
            this.butHome.TabIndex = 3;
            this.butHome.Text = "         Home";
            this.butHome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butHome.UseVisualStyleBackColor = true;
            this.butHome.Click += new System.EventHandler(this.butHome_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(93)))), ((int)(((byte)(142)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 83);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(230, 1);
            this.panel2.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(230, 83);
            this.panel1.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(170, 65);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(14, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(200, 78);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1198, 657);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(140)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Traceability";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelMain.ResumeLayout(false);
            this.panelMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button butHome;
        private System.Windows.Forms.Button butSearchHistories;
        private System.Windows.Forms.Button butHideSideBar;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

