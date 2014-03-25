using Microsoft.Win32;
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
using UploaderLibrary;
using CommonLibrary;

namespace UploaderSharkStream
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UploaderWindow : Window
    {
        public UploaderWindow()
        {
            InitializeComponent();
        }

        private void btnBrawse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Multiselect = true;
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                brawsePanel.Visibility = Visibility.Collapsed;
                startPanel.Visibility = Visibility.Visible;
                txtPath.Text = dlg.FileName;
            }            
        }

            

        private void btnStartPause_Click(object sender, RoutedEventArgs e)
        {
            statusGrid.Opacity = 1;
            btnStart.IsEnabled = false;

            UploadProcessor up = new UploadProcessor(txtPath.Text);
            up.ProgressChanged += up_ProgressChanged;
            up.Finished += up_Finished;
            up.StartUpload();
        }

        private void up_Finished(object sender, EventArgs e)
        {
            txtStatus.Text = "Finished";
            brawsePanel.Visibility = Visibility.Visible;
            startPanel.Visibility = Visibility.Collapsed;
            btnStart.IsEnabled = true;
        }

        private void up_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            txtCurrent.Text = e.Percent + "%";
            progress.Value = e.Percent;
        }    
    }
}
