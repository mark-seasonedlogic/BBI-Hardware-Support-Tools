using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BBIHardwareSupport.Utilities
{
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