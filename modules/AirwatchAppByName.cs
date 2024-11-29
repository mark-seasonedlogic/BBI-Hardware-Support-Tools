using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using NLog;

namespace BBIHardwareSupport
{
    public class AirwatchAppByName : IDataGridModulePlugin
    {
        private readonly AirWatchApiClient apiClient;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public string Name => "Airwatch App By Name";

        public AirwatchAppByName(AirWatchApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public void Initialize()
        {
            // Any initialization logic specific to the module
            logger.Info("Initializing AirwatchAppByNameAndVersion module.");
        }
        public IEnumerable<ToolStripMenuItem> GetContextMenuItems(Action<DataGridView> dataGridAction)
        {
            return new List<ToolStripMenuItem>
            {
                new ToolStripMenuItem("Export to CSV", null, (sender, e) => ExportToCsv(dataGridAction)),
            };
        }
        private void ExportToCsv(Action<DataGridView> dataGridAction)
        {
            MessageBox.Show("Export to CSV action triggered.");
        }


        public async Task<DataTable> GetDataGridDataAsync(string appName)
        {
            logger.Info("Fetching data from AirWatch API...");
            var appsJObject = await apiClient.GetAppByNameAsync(appName);

            var dataTable = new DataTable();
            foreach (var app in appsJObject)
            {
                if (dataTable.Columns.Count == 0)
                {
                    foreach (var property in app.Children<JProperty>())
                    {
                        dataTable.Columns.Add(property.Name, typeof(string));
                    }
                }

                var row = dataTable.NewRow();
                foreach (var property in app.Children<JProperty>())
                {
                    row[property.Name] = property.Value.ToString();
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}
