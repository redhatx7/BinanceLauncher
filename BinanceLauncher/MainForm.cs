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
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using File = System.IO.File;

namespace BinanceLauncher
{
    public partial class MainForm : Form
    {

        private readonly ResourceManager _resourceManager;
        public MainForm()
        {
            _resourceManager = new ResourceManager(typeof(MainForm));
            InitializeComponent();
            _countries.Sort();
            countriesCombo.Items.AddRange(_countries.ToArray());
            txtBinancePath.Text = Configuration.Instance.BinancePath;
            countriesCombo.Text = Configuration.Instance.Country;
            chkAutoLaunch.Checked = Configuration.Instance.AutoLaunch;

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

        private void btnSelectPath_Click(object sender, EventArgs e)
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
                    txtBinancePath.Text = Configuration.Instance.BinancePath = ofd.FileName;
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
                btnLaunch.Enabled = false;
                if (_seconds > 0) btnLaunch.Text = string.Format(GlobalResource.LaunchButtonText, _seconds--);
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
                    if (!btnLaunch.Enabled && !Configuration.Instance.AutoLaunch)
                        btnLaunch.Enabled = true;
                }
                else
                {
                    _run = false;
                    btnLaunch.Enabled = false;
                }
            }
            else
            {
                lblStatus.Text = GlobalResource.NotConnected;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Instance.AutoLaunch = chkAutoLaunch.Checked;
            btnLaunch.Enabled = !chkAutoLaunch.Checked;
            
            btnLaunch.Text = _resourceManager.GetObject("btnLaunch.Text")?.ToString();
            Configuration.RewriteConfiguration();
        }

        
     
        private void btnLaunch_Click(object sender, EventArgs e)
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

        private void CreateDesktopShortcut()
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\BinanceLauncher.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Shortcut for Binance launcher";
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            shortcut.Save();
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
            Configuration.Instance.Country = countriesCombo.SelectedItem?.ToString();
            Configuration.RewriteConfiguration();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog(this);
        }

        private void createDesktopShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDesktopShortcut();
            MessageBox.Show(this, GlobalResource.ShortcutCreated, "Shortcut", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
