using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.UI.Views;

namespace Trace.UI
{
    public partial class MainForm : Form, IMainView
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public event EventHandler FormLoad;

        public void LoadSubForm(object form)
        {
            if (this.panelMain.Controls.Count > 0)
                this.panelMain.Controls.RemoveAt(0);

            Form frm = form as Form;
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(frm);
            this.panelMain.Tag = frm;
            frm.Show();
        }

        private void butHideSideBar_Click(object sender, EventArgs e)
        {
            if (panelMenu.Width <= 50)
            {
                panelMenu.Width = panelMenu.Width + 180;
                pictureBox2.Visible = true;
                pictureBox1.Visible = false;
            }
            else
            {
                panelMenu.Width = panelMenu.Width - 180;
                pictureBox2.Visible = false;
                pictureBox1.Visible = true;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
