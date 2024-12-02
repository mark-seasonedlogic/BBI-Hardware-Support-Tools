using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Windows.Forms;
using BBIHardwareSupport.Utilities;
using Newtonsoft.Json.Linq;
using NLog;

namespace BBIHardwareSupport
{
    public class AirwatchAppsByName : IDataGridModulePlugin
    {
        private readonly AirWatchApiClient apiClient;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public string Name => "Airwatch Apps By Name";

        public AirwatchAppsByName(AirWatchApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public void Initialize()
        {
            logger.Info("Initializing AirwatchAppsByName module.");
        }

        public async Task<DataTable> GetDataGridDataAsync(string appName)
        {
            logger.Info("Fetching app data from AirWatch API...");
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

        public async Task<DataTable> GetAssignmentDataGridDataAsync(string smartGroups)
        {
            logger.Info("Fetching assignment data from Airwatch application data...");
            JArray groupsJArray = JArray.Parse(smartGroups);

            var dataTable = new DataTable();
            foreach (var smartGroup in groupsJArray)
            {
                if (dataTable.Columns.Count == 0)
                {
                    foreach (var property in smartGroup.Children<JProperty>())
                    {
                        dataTable.Columns.Add(property.Name, typeof(string));
                    }
                }

                var row = dataTable.NewRow();
                foreach (var property in smartGroup.Children<JProperty>())
                {
                    row[property.Name] = property.Value.ToString();
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public IEnumerable<ToolStripMenuItem> GetContextMenuItems(DataGridView grid)
        {
            logger.Debug("Generating context menu items for AirwatchAppsByName");

            var refreshMenuItem = CreateMenuItem("Refresh Data", async (sender, e) => await OnRefreshDataClicked(grid));
            var getAssignmentMenuItem = CreateMenuItem("Get Assignment Groups", async (sender, e) => await OnGetAssignmentClicked(grid));
            var exportMenuItem = CreateMenuItem("Export to CSV", (sender, e) => OnExportToCsvClicked(grid));

            // Initially disable the getAssignmentMenuItem
            getAssignmentMenuItem.Enabled = false;

            // Add event handlers to enable/disable getAssignmentMenuItem based on row selection
            grid.SelectionChanged += (sender, e) =>
            {
                // Enable only if exactly one row is selected
                getAssignmentMenuItem.Enabled = grid.SelectedRows.Count == 1;
            };

            return new List<ToolStripMenuItem> { refreshMenuItem, getAssignmentMenuItem, exportMenuItem };
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

            string parameter = PromptForInput("Enter app name to refresh data");
            var data = await GetDataGridDataAsync(parameter);

            grid.DataSource = data;
        }

        private async Task OnGetAssignmentClicked(DataGridView grid)
        {
            logger.Info("Get Assignment Groups action triggered.");
            if (grid == null || grid.SelectedRows.Count != 1)
            {
                logger.Error("DataGridView is null or no row is selected. Cannot fetch assignment groups.");
                return;
            }

            var selectedRow = grid.SelectedRows[0];
            var smartGroups = selectedRow.Cells["SmartGroups"]?.Value?.ToString();
            if (string.IsNullOrEmpty(smartGroups))
            {
                logger.Error("No assignment groups found in the selected row.");
                return;
            }

            var data = await GetAssignmentDataGridDataAsync(smartGroups);
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
