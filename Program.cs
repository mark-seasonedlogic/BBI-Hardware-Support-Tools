using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Forms;

namespace BBIHardwareSupport
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var httpClient = new HttpClient();
            var credentialsManager = new CredentialsManager(@"MDMcredentials.xml");
            var airWatchApiClient = new AirWatchApiClient(httpClient, credentialsManager);

            var plugins = new List<IModulePlugin>
            {
                new AirwatchDevicesByUser(airWatchApiClient),
                new AirwatchAppsByName(airWatchApiClient),
                new CompositePlugin(airWatchApiClient)
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(plugins));
        }
    }
}
