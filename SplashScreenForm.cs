using System.Windows.Forms;
using System.IO;

using NLog;
namespace BBIHardwareSupport
{
    public class SplashScreenForm : Form
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private WebBrowser pdfViewer;

        public SplashScreenForm(string pdfPath)
        {
            InitializeComponent(pdfPath);
        }

        private void InitializeComponent(string pdfPath)
        {
            this.pdfViewer = new WebBrowser();
            pdfViewer.Dock = DockStyle.Fill;
            if (File.Exists(pdfPath))
            {
                pdfViewer.Navigate(pdfPath);
            }
            else
            {
                MessageBox.Show("Splash screen PDF not found.");
            }
            this.Controls.Add(pdfViewer);
        }
    }
}
