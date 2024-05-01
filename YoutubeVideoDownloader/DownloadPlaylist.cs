using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

namespace YoutubeVideoDownloader
{
    internal class DownloadPlaylist
    {       
        public static async Task DownloadPlaylistAsync(string playlistLink, string downloadPath, IProgress<int> progress)
        {
            var youtube = YouTube.Default;
            var videos = await Task.Run(() => youtube.GetAllVideosAsync(playlistLink));
            
            long totalBytes = 0;
           
            foreach (var video in videos)
            {
                totalBytes += video.GetBytes().Length;
            }
                       
            long bytesWritten = 0;

            foreach (var video in videos)
            {              
                await DownloadVideoAsync(video, downloadPath);
                bytesWritten += video.GetBytes().Length;
                
                int progressPercentage = (int)((bytesWritten / (double)totalBytes) * 100);
                progress.Report(progressPercentage);
            }
        }

        private static async Task DownloadVideoAsync(Video video, string downloadPath)
        {
            const int bufferSize = 1024 * 1024; // 1MB
            var buffer = new byte[bufferSize];
            var bytesRead = 0;

            using (var fileStream = new FileStream(Path.Combine(downloadPath, video.FullName), FileMode.Create, FileAccess.Write))
            {
                using (var memoryStream = new MemoryStream(video.GetBytes()))
                {
                    while ((bytesRead = await memoryStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }
            }
        }
        
    }
}
