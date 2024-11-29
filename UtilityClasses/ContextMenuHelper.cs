using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace BBIHardwareSupport.Utilities
{

    // #region DataExportUtility
    public static class DataGridExportHelper
    {
        public static void ExportDataGridToCsv(DataGridView dataGridView)
        {
            if (dataGridView.DataSource is DataTable dataTable)
            {
                using (var saveFileDialog = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", FileName = "Export.csv" })
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            // Write headers
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                writer.Write(QuoteValue(column.ColumnName) + ",");
                            }
                            writer.WriteLine();

                            // Write rows
                            foreach (DataRow row in dataTable.Rows)
                            {
                                foreach (var cell in row.ItemArray)
                                {
                                    writer.Write(QuoteValue(cell?.ToString()) + ",");
                                }
                                writer.WriteLine();
                            }
                        }
                        MessageBox.Show("Data exported successfully.", "Export Complete");
                    }
                }
            }
            else
            {
                MessageBox.Show("No data to export.", "Export Error");
            }
        }

        private static string QuoteValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            // Escape double quotes and wrap in quotes if it contains commas or quotes
            if (value.Contains(",") || value.Contains("\""))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }
    }


    // #endregion





    /// <summary>
    /// A helper class to generate context menus dynamically based on plugin types.
    /// </summary>
    public static class ContextMenuHelper
    {
        /// <summary>
        /// Generates a context menu for a given plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance for which the context menu is created.</param>
        /// <returns>A ContextMenuStrip configured for the specified plugin.</returns>
        public static ContextMenuStrip CreateContextMenu(IModulePlugin plugin)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            switch (plugin)
            {
                case IDataGridModulePlugin dataGridPlugin:
                    InitializeDataGridContextMenu(contextMenu, dataGridPlugin);
                    break;

                case ITextModulePlugin textPlugin:
                    InitializeTextContextMenu(contextMenu, textPlugin);
                    break;

                default:
                    InitializeDefaultContextMenu(contextMenu);
                    break;
            }

            return contextMenu;
        }

        /// <summary>
        /// Initializes the context menu for a plugin implementing IDataGridModulePlugin.
        /// </summary>
        /// <param name="contextMenu">The ContextMenuStrip to configure.</param>
        /// <param name="plugin">The IDataGridModulePlugin instance.</param>
        private static void InitializeDataGridContextMenu(ContextMenuStrip contextMenu, IDataGridModulePlugin plugin)
        {
            var menuItems = plugin.GetContextMenuItems(null);
            foreach (var item in menuItems)
            {
                contextMenu.Items.Add(item);
            }
        }

        /// <summary>
        /// Initializes the context menu for a plugin implementing ITextModulePlugin.
        /// </summary>
        /// <param name="contextMenu">The ContextMenuStrip to configure.</param>
        /// <param name="plugin">The ITextModulePlugin instance.</param>
        private static void InitializeTextContextMenu(ContextMenuStrip contextMenu, ITextModulePlugin plugin)
        {
            contextMenu.Items.Add("Fetch Text Data", null, async (sender, e) =>
            {
                string parameter = PromptForInput($"Enter parameter for {plugin.Name}");
                string textData = await plugin.GetTextDataAsync(parameter);
                MessageBox.Show(textData, plugin.Name);
            });
        }

        /// <summary>
        /// Initializes a default context menu for plugins without specific configurations.
        /// </summary>
        /// <param name="contextMenu">The ContextMenuStrip to configure.</param>
        private static void InitializeDefaultContextMenu(ContextMenuStrip contextMenu)
        {
            contextMenu.Items.Add("Default Option", null, (sender, e) =>
            {
                MessageBox.Show("Default action executed.", "Info");
            });
        }

        /// <summary>
        /// Prompts the user for input with a simple dialog box.
        /// </summary>
        /// <param name="prompt">The prompt text to display.</param>
        /// <returns>The user input, or an empty string if canceled.</returns>
        private static string PromptForInput(string prompt)
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