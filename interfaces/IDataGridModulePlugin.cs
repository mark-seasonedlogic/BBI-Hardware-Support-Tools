using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BBIHardwareSupport
{
    public interface IDataGridModulePlugin : IModulePlugin
    {
        Task<DataTable> GetDataGridDataAsync(string parameter);
        IEnumerable<ToolStripMenuItem> GetContextMenuItems(Action<DataGridView> dataGridAction);

    }
}
