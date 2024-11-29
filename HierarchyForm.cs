using System;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace BBIHardwareSupport
{
    public partial class HierarchyForm : Form
    {
        readonly string _jsonFilePath = string.Empty;
        public HierarchyForm()
        {
            InitializeComponent();
            this.Load += HierarchyForm_Load;
        }
        public HierarchyForm(string jsonFilePath)
        {

            InitializeComponent();
            _jsonFilePath = jsonFilePath;
            this.Load += HierarchyForm_Load;
        }

        private void InitializeComponent()
        {
            //throw new NotImplementedException();
        }

        private void HierarchyForm_Load(object sender, EventArgs e)
        {
            // Set up TreeView for JSON display
            this.treeView1 = new TreeView();
            this.treeView1.Dock = DockStyle.Fill;
            this.Controls.Add(this.treeView1);
            //If file was passed into constructor, just build the tree
            if (!string.IsNullOrEmpty(_jsonFilePath))
            {
                string jsonString = System.IO.File.ReadAllText(_jsonFilePath);
                JObject jsonObject = JObject.Parse(jsonString);

                treeView1.Nodes.Clear();
                TreeNode rootNode = new TreeNode("JSON Root");
                treeView1.Nodes.Add(rootNode);

                AddJsonNodes(jsonObject, rootNode);
                treeView1.ExpandAll();

            }
            // Button to load JSON
            Button loadJsonButton = new Button { Text = "Load JSON", Dock = DockStyle.Top };
            loadJsonButton.Click += LoadJsonButton_Click;
            this.Controls.Add(loadJsonButton);
        }

        private void LoadJsonButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string jsonString = System.IO.File.ReadAllText(openFileDialog.FileName);
                    JObject jsonObject = JObject.Parse(jsonString);

                    treeView1.Nodes.Clear();
                    TreeNode rootNode = new TreeNode("JSON Root");
                    treeView1.Nodes.Add(rootNode);

                    AddJsonNodes(jsonObject, rootNode);
                    treeView1.ExpandAll();
                }
            }
        }

        private void AddJsonNodes(JToken token, TreeNode treeNode)
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    TreeNode newTreeNode = new TreeNode(property.Name);
                    treeNode.Nodes.Add(newTreeNode);
                    AddJsonNodes(property.Value, newTreeNode);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    TreeNode newTreeNode = new TreeNode($"[{i}]");
                    treeNode.Nodes.Add(newTreeNode);
                    AddJsonNodes(array[i], newTreeNode);
                }
            }
            else
            {
                // For simple values (e.g., strings, numbers, booleans)
                treeNode.Nodes.Add(new TreeNode(token.ToString()));
            }
        }
    }
}
