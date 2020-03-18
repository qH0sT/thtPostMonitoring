using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace thtlatestposts
{
    public partial class Notify : Form
    {
        public Notify(string baslik, string konuSahibi, string uri, string cat)
        {
            InitializeComponent();
            label1.Text = baslik;
            label3.Text = konuSahibi;
            label2.Text = cat;
            linkLabel1.Text = uri;
        }
        protected override void OnLoad(EventArgs e)
        {
            Screen ekran = Screen.FromPoint(Location);
            Location = new Point(ekran.WorkingArea.Right - Width, ekran.WorkingArea.Bottom - Height);
            base.OnLoad(e);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(linkLabel1.Text);
            }
            catch (Exception) { }
        }
    }
}
