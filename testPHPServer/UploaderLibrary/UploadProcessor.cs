using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UploaderLibrary.ru.e3w.testphpservice;

namespace UploaderLibrary
{
    public class UploadProcessor
    {
        
        int currentCount = 0;
        bool finished = false;
        hellotesting c = new hellotesting();
        int chunkSize = 1024 * 1024;
        int lastChunkSize;
        int chunkCount;
        string filePath;

        public UploadProcessor()
        {

        }

        public UploadProcessor(string path)
        {
            filePath = path;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                chunkCount = (int)fs.Length / chunkSize;

                if (((int)fs.Length - (chunkCount * chunkSize)) > 0)
                {
                    lastChunkSize = (int)fs.Length - (chunkCount * chunkSize);
                    chunkCount += 1;
                }
            }

            c.PushFileInfo(new FileInfoData { FileName = Path.GetFileName(path), ChunkCount = chunkCount, ChunkSize = chunkSize, LastChunkSize = lastChunkSize });
            Console.WriteLine("Total parts count " + chunkCount);
            c.PushValCompleted += c_PushValCompleted;
            c.GetRequiredPartCompleted += c_GetRequiredPartCompleted;
        }

        public void c_GetRequiredPartCompleted(object sender, GetRequiredPartCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != 0)
            {
                currentCount = e.Result;
                PartialData p = new PartialData();

                if (chunkCount >= currentCount)
                {
                    byte[] buffer = ReadChunk();
                    p.id = currentCount;
                    p.Data = Convert.ToBase64String(buffer);

                    var ar = new ProgressChangedEventArgs();
                    ar.Percent = currentCount * (100 / chunkCount);
                    if (this.ProgressChanged != null)
                        this.ProgressChanged(this,ar);
                }
                else
                {
                    finished = p.IsCompleted = true;
                }

                c.PushValAsync(p);
            }
            else
                c.GetRequiredPartAsync();
        }

        private void c_PushValCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (finished)
            {
                Console.WriteLine("Finished!");
                if (this.Finished != null)
                    this.Finished(this, EventArgs.Empty);
            }
            else
                c.GetRequiredPartAsync();
        }

        public void StartUpload()
        {
            c.GetRequiredPartAsync();
        }

        public void Clear()
        {
            c.ClearVal();
        }

        private byte[] ReadChunk()
        {
            byte[] buffer = new byte[chunkSize];
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    br.BaseStream.Position = (currentCount - 1) * chunkSize;
                    if (currentCount == chunkCount)//if last chunk
                    {
                        buffer = new byte[lastChunkSize];
                        buffer = br.ReadBytes(lastChunkSize);
                    }
                    else
                        buffer = br.ReadBytes(chunkSize);
                }
            }
            return buffer;
        }


        public event EventHandler Finished;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
    }    
}
