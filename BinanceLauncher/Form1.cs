using BinanceLauncher.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BinanceLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _countries.Sort();
            comboBox1.Items.AddRange(_countries.ToArray());
            textBox1.Text = Configuration.Instance.BinancePath;
            comboBox1.Text = Configuration.Instance.Country;
            checkBox1.Checked = Configuration.Instance.AutoLaunch;

        }
        private readonly List<string> _countries =new List<string>()
        {
            "Netherlands",
            "Switzerland",
            "Romania",
            "Russia",
            "Italy",
            "Finland",
            "France",
            "Sweden",
            "Germany",
            "United Kingdom",
            "Austria",
            "Denmark",
            "Norway",
            "Ireland",
            "Turkey"
        };

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.RestoreDirectory = true;
                ofd.Title = "Select Binance Path";
                ofd.Filter = "EXE files (*.exe)|*.exe";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = Configuration.Instance.BinancePath = ofd.FileName;
                    Configuration.RewriteConfiguration();
                }
               
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool isBinanceRunning = Process.GetProcessesByName("binance").Any();
            if (isBinanceRunning)
            {
                if(MessageBox.Show(GlobalResource.MessageBoxText, 
                    "Binance", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var processes = Process.GetProcessesByName("binance");
                    foreach (var process in processes)
                    {
                        process.CloseMainWindow();
                    }
                }
                else
                {
                    Application.Exit();
                }
            }
      
           
           
            timer1.Enabled = true;
            timer1.Start();
            this.Focus();
        }

       


        

       
        
        private bool _run = false;
        private int _seconds = 5;

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if(Configuration.Instance.AutoLaunch && _run)
            {
                if (_seconds > 0) button1.Text = string.Format(GlobalResource.LaunchButtonText, _seconds--);
                else
                {
                    LaunchBinance();
                    timer1.Stop();
                    Thread.Sleep(3000);
                    Application.Exit();
                }
            }
            else if (!Configuration.Instance.AutoLaunch)
            {
                _seconds = 5;
            }

            var result = await IpRequest.QueryIPInformation();
            if(result?.Success == true)
            {
                lblStatus.Text = string.Format(GlobalResource.LabelText, result.Country, result.City, result.IPAddress);
                if(result.Country == Configuration.Instance.Country)
                {
                    _run = true;
                    button1.Enabled = true;
                }
                else
                {
                    _run = false;
                    button1.Enabled = false;
                }
            }
            else
            {
                lblStatus.Text = GlobalResource.NotConnected;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Instance.AutoLaunch = checkBox1.Checked;
            button1.Enabled = !checkBox1.Checked;
            Configuration.RewriteConfiguration();
        }

        
     
        private void button1_Click(object sender, EventArgs e)
        {
            LaunchBinance();
        }

        private void LaunchBinance()
        {
            if (File.Exists(Configuration.Instance.BinancePath))
            {
                Process process = new Process();
                process.StartInfo.FileName = Configuration.Instance.BinancePath;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(Configuration.Instance.BinancePath) ?? "";
                process.Start();
            }
            else
            {
                MessageBox.Show(this, GlobalResource.BinanceNotFound, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void persianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Calture"] = "fa-IR";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Calture"] = "en-US";
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Configuration.Instance.Country = comboBox1.SelectedItem?.ToString();
            Configuration.RewriteConfiguration();
        }
    }
}
