using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace WgetLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            checkChildProcessCount();
        }

        private void btnSaveTo_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlgSaveTo = new CommonOpenFileDialog();
            dlgSaveTo.InitialDirectory = txtDestination.Text;
            dlgSaveTo.IsFolderPicker = true;
            if (dlgSaveTo.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtDestination.Text = dlgSaveTo.FileName;
            }
        }

        private void cbHideConsoleWindows_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            // Grab simple config values from controls in the window
            string strDownloadURL = txtURL.Text;
            string strDownloadRate = txtDownloadRate.Text;
            string strRejectFiles = txtRejectFiles.Text;
            string strUserAgent = "--user-agent='Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.8.1.6) Gecko/20070802 SeaMonkey/1.1.4' ";
            bool boolHideConsoleWindows = cbHideConsoleWindows.IsChecked.Value;
            bool boolMinimizeConsoleWindows = cbMinimizeConsoleWindows.IsChecked.Value;
            
            // Grabs txtDestination setting, applies lower casing to the first character (will always be a letter), replaces backslashes with forward slashes, and removes the colon
            // Essentially it's a set of functions to convert the Windows-generated path from txtDestination to a path that Bash can use
            string strDestination = (Char.ToLowerInvariant(txtDestination.Text[0]) + txtDestination.Text.Substring(1)).Replace("\\", "/").Remove(1, 1);

            // Build the arguments to be passed to Bash, which includes the entirety of wget and its parameters
            // We have to do it this way afaik because we need to wrap the entire string in quotes for the bash -c command.
            // We might not actually need to but doing it this way makes it so much easier to manage.
            string strArguments = "cd /mnt/" + strDestination + "; wget -r -c -nc -np -e robots=off --random-wait --limit-rate=" + strDownloadRate + " -R \"" + strRejectFiles + "\" " + strUserAgent + strDownloadURL;


            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Windows\Sysnative\bash.exe"; // Sysnative tells Winx86_64 to actually use System32 instead of redirecting to SysWOW64
            start.Arguments = "-c \"" + strArguments +"\"";
            if (boolHideConsoleWindows)
            {
                start.WindowStyle = ProcessWindowStyle.Hidden;
            }
            if(boolMinimizeConsoleWindows)
            {
                start.WindowStyle = ProcessWindowStyle.Minimized;
            }
            Process proc = Process.Start(start);
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void checkChildProcessCount()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("wget");
            labelProcessCount.Content = "Processes: " + (procs.Length).ToString();
        }


        private void btnKillWget_Click(object sender, RoutedEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("wget"))
            {
                process.Kill();
            }
        }
    }
}
