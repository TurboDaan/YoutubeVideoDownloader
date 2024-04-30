using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutubeVideoDownloader
{
    public partial class MainForm : Form
    {
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        string downloadPath;
        public MainForm()
        {
            InitializeComponent();
        }

        private void choosePathDialog_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog pathDialog = new FolderBrowserDialog();
            pathDialog.ShowDialog();                  

            downloadPath = pathDialog.SelectedPath;
            lblDownloadPath.Text = downloadPath;

            if (downloadPath == "")
                lblDownloadPath.Text = defaultPath;

            // write the download path to a file
            File.WriteAllText("path.txt", lblDownloadPath.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("path.txt"))
            {
                downloadPath = File.ReadAllText("path.txt");
                lblDownloadPath.Text = downloadPath;
            }
            else
            lblDownloadPath.Text = defaultPath;
            
        }
    }
}
