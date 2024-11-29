using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using NLog;

namespace BBIHardwareSupport
{
    public class AirwatchDevicesByUser : IDataGridModulePlugin
    {
        private readonly AirWatchApiClient apiClient;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public string Name => "Airwatch Devices By User";

        public AirwatchDevicesByUser(AirWatchApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public void Initialize()
        {
            // Any initialization logic specific to the module
            logger.Info("Initializing AirwatchDevicesByUser module.");
        }

        public async Task<DataTable> GetDataGridDataAsync(string deviceUser)
        {
            logger.Info("Fetching data from AirWatch API...");
            var devicesJObject = await apiClient.GetDevicesByUserAsync(deviceUser);

            var dataTable = new DataTable();
                foreach (var device in devicesJObject)
                {
                    if (dataTable.Columns.Count == 0)
                    {
                        foreach (var property in device.Children<JProperty>())
                        {
                            dataTable.Columns.Add(property.Name, typeof(string));
                        }
                    }

                    var row = dataTable.NewRow();
                    foreach (var property in device.Children<JProperty>())
                    {
                        row[property.Name] = property.Value.ToString();
                    }
                    dataTable.Rows.Add(row);
                }
            return dataTable;
        }
        public IEnumerable<ToolStripMenuItem> GetContextMenuItems(Action<DataGridView> actionDataGrid)
        {
            logger.Debug("get menu items");
            return null;
        }
    }
}
