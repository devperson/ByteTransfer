using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using downloaderConsole.ru.e3w.testphpservice;

namespace downloaderConsole
{
    class Program
    {        
        static void Main(string[] args)
        {
            Console.WriteLine("Type \"start\" to start downloading.");
            DownloadProcessor d = new DownloadProcessor();
            d.Finished += d_Finished;            

            var result = Console.ReadLine();
            if (result.ToLower() == "start")
                d.StartDownload();
            if (result.ToLower() == "clear")
                d.Clear();

            Console.ReadLine();
        }

        static void d_Finished(object sender, EventArgs e)
        {
            Console.WriteLine("Finished !");          
        }
    }

    public class DownloadProcessor
    {
        FileInfoData fInfo;
        int currentCount;
        public event EventHandler Finished;        
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
                    this.WriteChunk(buffer, currentCount - 1, "Data\\downloaded_" + fInfo.FileName);                                      
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
    } 
}
