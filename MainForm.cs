using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BBIHardwareSupport.Utilities;
using NLog;

namespace BBIHardwareSupport
{
    public class MainForm : Form
    {
        private readonly List<IModulePlugin> plugins;
        private DataGridView dataGrid;

        public MainForm(IEnumerable<IModulePlugin> plugins)
        {
            this.plugins = plugins.ToList();
            dataGrid = new DataGridView { Dock = DockStyle.Fill };
            InitializeComponents();
            InitializeDynamicMenu();
        }

        private void InitializeComponents()
        {
            Controls.Add(dataGrid);
        }

        private void InitializeDynamicMenu()
        {
            var menuStrip = new MenuStrip();
            var pluginsMenu = new ToolStripMenuItem("Plugins");

            foreach (var plugin in plugins)
            {
                var menuItem = new ToolStripMenuItem(plugin.Name);
                menuItem.Click += async (sender, args) => await OnPluginClicked(plugin);
                pluginsMenu.DropDownItems.Add(menuItem);
            }

            menuStrip.Items.Add(pluginsMenu);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
        }

        private async Task OnPluginClicked(IModulePlugin plugin)
        {
            string parameter = PromptForInput($"Enter parameter for {plugin.Name}");

            if (plugin is IDataGridModulePlugin dataGridPlugin)
            {
                var data = await dataGridPlugin.GetDataGridDataAsync(parameter);
                dataGrid.DataSource = data;

                // Generate and attach context menu items
                var contextMenu = new ContextMenuStrip();
                foreach (var menuItem in dataGridPlugin.GetContextMenuItems(dataGrid))
                {
                    contextMenu.Items.Add(menuItem);
                }
                dataGrid.ContextMenuStrip = contextMenu;
            }
            else if (plugin is ITextModulePlugin textPlugin)
            {
                string text = await textPlugin.GetTextDataAsync(parameter);
                MessageBox.Show(text, plugin.Name);
            }
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
