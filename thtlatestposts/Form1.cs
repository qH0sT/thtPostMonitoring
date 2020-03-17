using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace thtlatestposts
{
    public partial class Form1 : Form
    {
        /*
  _________                                     ____  __.           __                      
 /   _____/____     ____   ____ ___________    |    |/ _|____      |__| _____   ___________ 
 \_____  \\__  \   / ___\ /  _ \\____ \__  \   |      < \__  \     |  |/     \_/ __ \_  __ \
 /        \/ __ \_/ /_/  >  <_> )  |_> > __ \_ |    |  \ / __ \_   |  |  Y Y  \  ___/|  | \/
/_______  (____  /\___  / \____/|   __(____  / |____|__ (____  /\__|  |__|_|  /\___  >__|   
        \/     \//_____/        |__|       \/          \/    \/\______|     \/     \/        
        
             */
        public Form1()
        {
            InitializeComponent();           
            string[] cats = File.ReadAllLines("cats.txt");
            for (int j = 0; j < cats.Length; j++)
            {
                if (!string.IsNullOrEmpty(cats[j]))
                {
                    Kategoriler.Add(cats[j].Split('|')[0], cats[j].Split('|')[1]);
                }
            }
            if(Properties.Settings.Default.wavPath != "")
            {
                checkBox1.Checked = true;
                textBox1.Text = Properties.Settings.Default.wavPath;
            }
            CheckForIllegalCrossThreadCalls = false;
        }
        Dictionary<string, string> Kategoriler = new Dictionary<string, string>();
        List<Thread> openedThread = new List<Thread>();
        private void KonuCek(string kategori, string uri)
        {
            try
            {
                var web = new HtmlWeb();
                web.OverrideEncoding = Encoding.Default;
                while (true)
                {
                    var doc = web.Load(uri);
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//td[contains(@class, 'alt1')]"))
                    {
                        try
                        {
                            if (node.Attributes["id"].Value.Contains("statusicon") == false)
                            {
                                int y = node.SelectNodes(".//span")[0].ChildNodes.Count;
                                if (y == 0)
                                {
                                    IEnumerable<ListViewItem> aytim = listView1.Items.Cast<ListViewItem>().Where(items => items.Text == kategori);
                                    if (aytim.FirstOrDefault().SubItems[1].Text == "")
                                    {
                                        aytim.FirstOrDefault().SubItems[1].Text = node.SelectNodes(".//a")[0].InnerText;
                                        aytim.FirstOrDefault().SubItems[2].Text = node.SelectNodes(".//a")[0].Attributes["href"].Value;
                                        aytim.FirstOrDefault().SubItems[3].Text = node.SelectNodes(".//div")[1].InnerText.Replace("&quot;", @"""");
                                    }
                                    else
                                    {
                                        if (aytim.FirstOrDefault().SubItems[1].Text != node.SelectNodes(".//a")[0].InnerText)
                                        {
                                            Invoke((MethodInvoker)delegate
                                            {
                                                new Notify(node.SelectNodes(".//a")[0].InnerText, node.SelectNodes(".//div")[1].SelectNodes(".//span")[0].InnerText.Replace("&quot;", @""""),
                                                    node.SelectNodes(".//a")[0].Attributes["href"].Value, kategori).Show();
                                                if (checkBox1.Checked)
                                                {
                                                    SoundPlayer sp = new SoundPlayer(textBox1.Text);
                                                    sp.Play();
                                                }
                                            });
                                            aytim.FirstOrDefault().SubItems[1].Text = node.SelectNodes(".//a")[0].InnerText;
                                            aytim.FirstOrDefault().SubItems[2].Text = node.SelectNodes(".//a")[0].Attributes["href"].Value;
                                            aytim.FirstOrDefault().SubItems[3].Text = node.SelectNodes(".//div")[1].InnerText.Replace("&quot;", @"""");
                                        }
                                    }
                                    break;
                                }

                            }
                        }
                        catch (Exception) { }
                    }
                    Thread.Sleep((int)numericUpDown1.Value * 1000);
                }
            }
            catch (Exception) { }        
        }
        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            foreach(KeyValuePair<string,string> keys in Kategoriler)
            {
                ListViewItem lvi = new ListViewItem(keys.Key);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                listView1.Items.Add(lvi);
                Thread session = new Thread(() => KonuCek(keys.Key, keys.Value));
                session.Start();
                openedThread.Add(session);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listView1.Items.Count > 0)
            { 
            foreach(var the in openedThread)
            {
                the.Abort();
               Thread.Sleep(100);
            }
            MessageBox.Show("Tüm işlemler durduruldu.","THT Latest Posts Monitoring", MessageBoxButtons.OK,MessageBoxIcon.Information);
             }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Herhangi bir .wav dosyası seçin.";
                op.Filter = "Ses Dalgası Dosyası (*.wav)|*.wav";
                op.Multiselect = false;
                if (op.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = op.FileName;
                    Properties.Settings.Default.wavPath = textBox1.Text;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void seçiliKonuyaGitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(listView1.SelectedItems[0].SubItems[2].Text);
            }
            catch (Exception) { }
        }
    }
}
