using System;
using System.IO;
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

            File.WriteAllText("path.txt", lblDownloadPath.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBoxOptions.SelectedIndex = 0;
            comBoxFileType.SelectedIndex = 0;
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
            comBoxFileType.Enabled = !active;
            comBoxQuality.Enabled = !active;
        }

        private IProgress<int> progress;
        private async void btnDownload_Click(object sender, EventArgs e)
        {
            downloadingActive(true);
            lblMessage.Text = "Status: Downloading...";
            int option = comboBoxOptions.SelectedIndex;

            try
            {   
                progress = new Progress<int>(value =>
                {
                   downloadProgressBar.Value = value;
                });
                
                switch (option)
                {
                    case 0:
                        await Task.Run(() => DownloadVideo.DownloadVideoAsync(txtBoxLink.Text, downloadPath, progress));
                        break;
                    case 1:
                        await Task.Run(() => DownloadPlaylist.DownloadPlaylistAsync(txtBoxLink.Text, downloadPath, progress));
                        break;
                    default:
                        lblMessage.Text = "Status: Invalid Option";
                        break;
                }

                lblMessage.Text = " Status: Downloaded Successfully";

            }
            catch (Exception ex)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Status: Error " + ex.Message;
            }
            finally
            {
                downloadingActive(false);
            }
        }
    }
}
