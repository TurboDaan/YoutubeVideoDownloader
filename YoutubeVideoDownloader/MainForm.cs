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
using VideoLibrary;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static YoutubeVideoDownloader.DownloadVideo;

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

            File.WriteAllText("path.txt", lblDownloadPath.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBoxOptions.SelectedIndex = 0;
            if (File.Exists("path.txt"))
            {
                downloadPath = File.ReadAllText("path.txt");
                lblDownloadPath.Text = downloadPath;
            }
            else
                lblDownloadPath.Text = defaultPath;

        }

        private void downloadingActive(bool active)
        {
            txtBoxLink.Enabled = !active;
            btnDownload.Enabled = !active;
            choosePathDialog.Enabled = !active;
            comboBoxOptions.Enabled = !active;
        }

        private IProgress<int> progress;
        private async void btnDownload_Click(object sender, EventArgs e)
        {
            downloadingActive(true);
            lblMessage.Text = "Downloading...";
            int option = comboBoxOptions.SelectedIndex;

            try
            {   
                progress = new Progress<int>(value =>
                {
                    downloadProgressBar.Invoke((MethodInvoker)(() => downloadProgressBar.Value = value));
                });
                
                switch (option)
                {
                    case 0:
                        await Task.Run(() => DownloadVideoAsync(txtBoxLink.Text, downloadPath, progress));
                        break;
                    case 1:
                        await Task.Run(() => DownloadPlaylist.DownloadPlaylistAsync(txtBoxLink.Text, downloadPath, progress));
                        break;
                    default:
                        lblMessage.Invoke((MethodInvoker)(() => lblMessage.Text = "Invalid Option"));
                        break;
                }

                lblMessage.Visible = true;
                lblMessage.Text = "Downloaded Successfully";

            }
            catch (Exception ex)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Error: " + ex.Message;
            }
            finally
            {
                downloadingActive(false);
            }
        }
    }
}
