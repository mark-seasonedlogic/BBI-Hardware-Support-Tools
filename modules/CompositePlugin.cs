using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BBIHardwareSupport.Utilities;
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
            List<string> processedUserNames = new List<string>();

            foreach (var username in usernames)
            {
                // Username must match AndroidPOSi device user pattern
                string androidPOSiDevicePattern = "^(OBS|BFG|CIG|FLM)\\d{4}POS$";
                Regex regex = new Regex(androidPOSiDevicePattern);
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

        public IEnumerable<ToolStripMenuItem> GetContextMenuItems(DataGridView grid)
        {
            logger.Debug("Generating context menu items for CompositePlugin");

            return new List<ToolStripMenuItem>
            {
                CreateMenuItem("Refresh Data", async (sender, e) => await OnRefreshDataClicked(grid)),
                CreateMenuItem("Export to CSV", (sender, e) => OnExportToCsvClicked(grid))
            };
        }

        private ToolStripMenuItem CreateMenuItem(string text, EventHandler onClickHandler)
        {
            var menuItem = new ToolStripMenuItem(text);
            menuItem.Click += onClickHandler;
            return menuItem;
        }

        private async Task OnRefreshDataClicked(DataGridView grid)
        {
            logger.Info("Refresh Data action triggered.");
            if (grid == null)
            {
                logger.Error("DataGridView is null. Cannot refresh data.");
                return;
            }

            string parameter = PromptForInput("Enter parameter to refresh data");
            var data = await GetDataGridDataAsync(parameter);

            grid.DataSource = data;
        }

        private void OnExportToCsvClicked(DataGridView grid)
        {
            logger.Info("Export to CSV action triggered.");
            if (grid == null)
            {
                logger.Error("DataGridView is null. Cannot export data.");
                return;
            }

            DataGridExportHelper.ExportDataGridToCsv(grid);
        }

        private string PromptForInput(string prompt)
        {
            using (var form = new Form())
            {
                form.Text = prompt;
                var textBox = new TextBox { Left = 10, Top = 10, Width = 200 };
                var button = new Button { Text = "OK", Left = 220, Top = 10, Width = 60 };
                button.Click += (sender, args) => form.DialogResult = DialogResult.OK;
                form.Controls.Add(textBox);
                form.Controls.Add(button);
                return form.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
            }
        }
    }
}
