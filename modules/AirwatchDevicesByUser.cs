using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BBIHardwareSupport.Utilities;
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

        public IEnumerable<ToolStripMenuItem> GetContextMenuItems(DataGridView grid)
        {
            logger.Debug("Generating context menu items for AirwatchDevicesByUser");

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
