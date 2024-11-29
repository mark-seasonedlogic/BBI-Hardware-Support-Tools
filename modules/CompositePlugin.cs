using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using NLog;

namespace BBIHardwareSupport
{
    public class CompositePlugin : IDataGridModulePlugin
    {
        private readonly AirWatchApiClient clientApi; // Dependency for retrieving usernames
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public string Name => "Composite Plugin";

        public CompositePlugin(AirWatchApiClient clientApi)
        {
            this.clientApi = clientApi;
        }

        public void Initialize()
        {
            logger.Info("Initializing Composite Plugin.");
        }

        public async Task<DataTable> GetDataGridDataAsync(string parameter)
        {
            logger.Info("Fetching data from composite plugin.");

            // Retrieve all usernames
            var usernames = await clientApi.GetAllUsernamesAsync();

            // Create a combined DataTable
            var combinedDataTable = new DataTable();
            List<String> processedUserNames = new List<string>();

            foreach (var username in usernames)
            {
                //Username must match AndroidPOSi device user pattern
                string androidPOSiDevicePattern = @"^(OBS|BFG|CIG|FLM)\d{4}POS$";
                Regex regex = new Regex(@"^(OBS|BFG|CIG|FLM)\d{4}POS$");
                if (regex.IsMatch(username.Value<string>("UserName")))
                {

                    if (!processedUserNames.Contains(username.Value<string>("UserName")))
                    {
                        var deviceData = await clientApi.GetDevicesByUserAsync(username.Value<string>("UserName"));


                        foreach (var device in deviceData)
                        {
                            if (combinedDataTable.Columns.Count == 0)
                            {
                                foreach (var property in device.Children<JProperty>())
                                {
                                    combinedDataTable.Columns.Add(property.Name, typeof(string));
                                }
                            }

                            var row = combinedDataTable.NewRow();
                            foreach (var property in device.Children<JProperty>())
                            {
                                row[property.Name] = property.Value.ToString();
                            }
                            combinedDataTable.Rows.Add(row);
                        }
                    }


                }
                processedUserNames.Add(username.Value<string>("UserName"));
            }

            return combinedDataTable;
        }

        public IEnumerable<ToolStripMenuItem> GetContextMenuItems(Action<DataGridView> actionDataGrid)
        {
            // Aggregate context menu items from all sub-plugins if needed
            logger.Info("Getting context menu items from composite plugin.");
            return null;
        }
    }
}
