using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.UI.Presenters;
using Trace.UI.Views;

namespace Trace.UI
{
    public partial class MainForm : Form, IMainView
    {
        private readonly MainPresenter _presenter;

        public MainForm()
        {
            InitializeComponent();

            this._presenter = new MainPresenter(this);
        }

        public event EventHandler FormLoad;
        public event EventHandler OpenForm;

        public void LoadSubForm(object form)
        {
            if (this.panelContainer.Controls.Count > 0)
                this.panelContainer.Controls.RemoveAt(0);

            Form frm = form as Form;
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;
            this.panelContainer.Controls.Add(frm);
            this.panelContainer.Tag = frm;
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

        public void HideSideBar()
        {
            panelMenu.Width = panelMenu.Width - 180;
            pictureBox2.Visible = false;
            pictureBox1.Visible = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (FormLoad != null)
                FormLoad(sender, e);
        }

        private void butHome_Click(object sender, EventArgs e)
        {
            if (OpenForm != null)
                OpenForm(sender, e);
        }

        private void butSearchHistories_Click(object sender, EventArgs e)
        {
            if (OpenForm != null)
                OpenForm(sender, e);
        }
    }
}
