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
            var bytesWritten = 0;

            // Start progress reporting in a separate task
            await Task.Run(async () =>
            {
                using (var memoryStream = new MemoryStream(video.GetBytes()))
                {
                    while (bytesWritten < totalBytes)
                    {
                        var bytesRead = await memoryStream.ReadAsync(buffer, 0, bufferSize);
                        if (bytesRead == 0) break;

                        bytesWritten += bytesRead;
                        var progressPercentage = (int)((bytesWritten / (double)totalBytes) * 100);
                        progress.Report(progressPercentage);
                    }
                }
            });

            // Write to file
            using (var fileStream = new FileStream(Path.Combine(downloadPath, video.FullName), FileMode.Create, FileAccess.Write))
            {
                using (var memoryStream = new MemoryStream(video.GetBytes()))
                {
                    while (bytesWritten < totalBytes)
                    {
                        var bytesRead = await memoryStream.ReadAsync(buffer, 0, bufferSize);
                        if (bytesRead == 0) break;

                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        bytesWritten += bytesRead;
                    }
                }
            }
        }
    }
}
