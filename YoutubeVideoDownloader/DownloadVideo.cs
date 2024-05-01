using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

namespace YoutubeVideoDownloader
{
    internal class DownloadVideo
    {
        public static async Task DownloadVideoAsync(string link, string downloadPath, IProgress<int> progress)
        {
            var youtube = YouTube.Default;
            var video = await Task.Run(() => youtube.GetVideo(link));

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

