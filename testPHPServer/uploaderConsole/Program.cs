using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using uploaderConsole.ru.e3w.testphpservice;

namespace uploaderConsole
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Type \"start\" to start uploading.");

            var filePath = Directory.GetFiles("Data").FirstOrDefault();                        
            
            UploadProcessor up = new UploadProcessor(filePath);            

            var result = Console.ReadLine();
            if (result.ToLower() == "start")
                up.StartUpload();
            if (result.ToLower() == "clear")
                up.Clear();

            Console.ReadLine();
        }
        
    }

    public class UploadProcessor
    {
        public event EventHandler Finished;        
        int currentCount = 0;
        bool finished = false;
        hellotesting c = new hellotesting();        
        int chunkSize = 1024 * 1024;
        int lastChunkSize;
        int chunkCount;
        string filePath;


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

        void c_GetRequiredPartCompleted(object sender, GetRequiredPartCompletedEventArgs e)
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
                }
                else
                    finished = p.IsCompleted = true;

                c.PushValAsync(p);
            }
            else
                c.GetRequiredPartAsync();
        }
      
        private void c_PushValCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (finished)
            {
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
    }

    public class Chunk
    {
        public string Value { get; set; }
    }
}
