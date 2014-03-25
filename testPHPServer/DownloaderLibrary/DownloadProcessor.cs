using CommonLibrary;
using DownloaderLibrary.ru.e3w.testphpservice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DownloaderLibrary
{
    public class DownloadProcessor
    {
        FileInfoData fInfo;
        int currentCount;        
        hellotesting c = new hellotesting();

        public DownloadProcessor()
        {
            c.GetValCompleted += c_GetValCompleted;
            c.SetRequiredPartCompleted += c_SetRequiredPartCompleted;
            c.PullFileInfoCompleted += c_PullFileInfoCompleted;
        }

        private void c_PullFileInfoCompleted(object sender, PullFileInfoCompletedEventArgs e)
        {
            if (e.Result != null && !string.IsNullOrEmpty(e.Result.FileName))
            {
                fInfo = e.Result;
                c.SetRequiredPartAsync(currentCount + 1);
            }
            else
                this.StartDownload();
        }

        private void c_SetRequiredPartCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            c.GetValAsync();
        }

        private void c_GetValCompleted(object sender, GetValCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine(e.Error.ToString());
                return;
            }
            if (e.Result == null)
            {
                Console.WriteLine("Data null received");

                c.SetRequiredPartAsync(currentCount + 1);

                return;
            }
            if (e.Result.IsCompleted)
            {
                if (currentCount == 0/*ResultBytes.Count == 0*/)
                {
                    c.SetRequiredPartAsync(currentCount + 1);
                    return;
                }
            }

            if (!e.Result.IsCompleted)
            {
                if (currentCount != e.Result.id)
                {
                    currentCount = e.Result.id;

                    Console.WriteLine(" Part " + currentCount + " downloaded !");

                    var buffer = Convert.FromBase64String(e.Result.Data);

                    if (!Directory.Exists("Data"))
                        Directory.CreateDirectory("Data");

                    this.WriteChunk(buffer, currentCount - 1, "Data\\downloaded_" + fInfo.FileName);

                    if (this.ProgressChanged != null)
                    {
                        var percent = currentCount * (100 / fInfo.ChunkCount);
                        this.ProgressChanged(this, new ProgressChangedEventArgs { Percent = percent });
                    }
                
                }

                c.SetRequiredPartAsync(currentCount + 1);
            }
            else
            {
                if (this.Finished != null)
                    this.Finished(this, EventArgs.Empty);

                c.ClearVal();
            }
        }

        public void StartDownload()
        {
            c.PullFileInfoAsync();
        }

        public void Clear()
        {
            c.ClearVal();
        }

        private void WriteChunk(byte[] data, int position, string path)
        {
            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    //writer.Seek((int)(position * data.Count()), SeekOrigin.Begin);
                    writer.Seek((int)(position * fInfo.ChunkSize), SeekOrigin.Begin);
                    writer.Write(data, 0, data.Count());
                }
            }
        }


        public event EventHandler Finished;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
    } 
}
