using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace ZusiMeter
{
    /// <summary>
    /// Interaktionslogik für HelpDlg.xaml
    /// </summary>
    public partial class HelpDlg : Window
    {
        public HelpDlg()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async(null);
            string relativePath = "help/ZusiMeter_Docu_Deutsch.pdf";
            string absolutePath = Path.GetFullPath(relativePath);
            webView.Source = new Uri($"file:///{absolutePath}");
        }
    }
}
