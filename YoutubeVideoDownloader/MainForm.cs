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
            downloadProgressBar.Visible = active;
        }

        private IProgress<int> progress;
        private async void btnDownload_Click(object sender, EventArgs e)
        {
            downloadingActive(true);

            var youtube = YouTube.Default;
            int option = comboBoxOptions.SelectedIndex;

            progress = new Progress<int>(value =>
            {
                downloadProgressBar.Value = value;
            });

            try
            {
                lblMessage.Text = "Downloading...";
                switch (option)
                {
                    case 0:
                        var video = await Task.Run(() => youtube.GetVideo(txtBoxLink.Text));
                        await DownloadWithProgress(video, progress);
                        break;
                    case 1:
                        var videos = await Task.Run(() => youtube.GetAllVideos(txtBoxLink.Text));
                        foreach (var v in videos)
                        {
                            await DownloadWithProgress(v, progress);
                        }
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

        private async Task DownloadWithProgress(Video video, IProgress<int> progress)
        {
            const int bufferSize = 1024 * 1024; // 1MB
            var totalBytes = video.GetBytes().Length;
            var buffer = new byte[bufferSize];
            var bytesRead = 0;
            var bytesWritten = 0;

            using (var fileStream = new FileStream(Path.Combine(downloadPath, video.FullName), FileMode.Create, FileAccess.Write))
            {
                using (var memoryStream = new MemoryStream(video.GetBytes()))
                {
                    while ((bytesRead = await memoryStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        bytesWritten += bytesRead;
                        var progressPercentage = (int)((bytesWritten / (double)totalBytes) * 100);
                        progress.Report(progressPercentage);
                    }
                }
            }
        }
    }
}
