using CommonLibrary;
using DownloaderLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DownloaderSharkStream
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DownloaderWindow : Window
    {
        public DownloaderWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            statusGrid.Opacity = 1;

            DownloadProcessor d = new DownloadProcessor();
            d.Finished += d_Finished;
            d.ProgressChanged += d_ProgressChanged;
            d.StartDownload();
        }

        private void d_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            txtCurrent.Text = e.Percent + "%";
            progress.Value = e.Percent;
        }

        private void d_Finished(object sender, EventArgs e)
        {
            btnStart.IsEnabled = true;
            txtStatus.Text = "Finished";
        }
    }
}
