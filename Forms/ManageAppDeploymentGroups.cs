using System;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Forms;

namespace BBIHardwareSupport
{
    public partial class ManageAppDeploymentGroupsForm : Form
    {
        private readonly JsonArray _applicationGroups;
        private readonly JsonArray _topLevelGroups;
        private readonly Panel pnlAppGroups;
        private readonly Panel pnlMoveGroups;
        private readonly Panel pnlTopGroups;
        private Button btnMoveRight;
        private Button btnMoveLeft;

        public ManageAppDeploymentGroupsForm(string applicationGroupsJSON, string topLevelGroupsJSON)
        {
            // Parse JSON strings
            _applicationGroups = JsonNode.Parse(applicationGroupsJSON)?["groups"]?.AsArray() ?? new JsonArray();
            _topLevelGroups = JsonNode.Parse(topLevelGroupsJSON)?["groups"]?.AsArray() ?? new JsonArray();

            // Configure form
            InitializeForm();

            // Create panels
            pnlAppGroups = CreatePanel(0, 0, 2.0 / 5, Color.LightBlue);
            pnlMoveGroups = CreatePanel(pnlAppGroups.Width, 0, 1.0 / 5, Color.LightGray);
            pnlTopGroups = CreatePanel(pnlAppGroups.Width + pnlMoveGroups.Width, 0, 2.0 / 5, Color.LightGreen);

            // Add panels to form
            Controls.Add(pnlAppGroups);
            Controls.Add(pnlMoveGroups);
            Controls.Add(pnlTopGroups);

            // Create buttons
            CreateAppGroupButtons();
            CreateMoveButtons();
        }

        private void InitializeForm()
        {
            var screenSize = Screen.PrimaryScreen.Bounds;
            Width = (int)(screenSize.Width * 0.3);
            Height = (int)(screenSize.Height * 0.3);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "JSON Groups Manager";
        }

        private Panel CreatePanel(int x, int y, double widthRatio, Color backColor)
        {
            return new Panel
            {
                Location = new Point(x, y),
                Width = (int)(Width * widthRatio),
                Height = Height,
                BackColor = backColor,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void CreateAppGroupButtons()
        {
            int buttonHeight = 40;
            for (int i = 0; i < _applicationGroups.Count; i++)
            {
                var group = _applicationGroups[i]?.ToString() ?? $"Group {i + 1}";
                var button = new Button
                {
                    Text = group,
                    Width = pnlAppGroups.Width - 10,
                    Height = buttonHeight,
                    Location = new Point(5, 5 + i * (buttonHeight + 5)),
                    Tag = group
                };
                button.MouseDown += Button_MouseDown;
                pnlAppGroups.Controls.Add(button);
            }
        }

        private void CreateMoveButtons()
        {
            int buttonWidth = pnlMoveGroups.Width - 20;
            btnMoveRight = new Button
            {
                Text = ">>",
                Width = buttonWidth,
                Height = 50,
                Location = new Point(10, pnlMoveGroups.Height / 3 - 25)
            };
            btnMoveRight.Click += BtnMoveRight_Click;

            btnMoveLeft = new Button
            {
                Text = "<<",
                Width = buttonWidth,
                Height = 50,
                Location = new Point(10, 2 * pnlMoveGroups.Height / 3 - 25)
            };
            btnMoveLeft.Click += BtnMoveLeft_Click;

            pnlMoveGroups.Controls.Add(btnMoveRight);
            pnlMoveGroups.Controls.Add(btnMoveLeft);
        }

        private void BtnMoveRight_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(pnlAppGroups, pnlTopGroups);
        }

        private void BtnMoveLeft_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(pnlTopGroups, pnlAppGroups);
        }

        private void MoveSelectedItems(Panel fromPanel, Panel toPanel)
        {
            var selectedButtons = fromPanel.Controls
                .OfType<Button>()
                .Where(b => b.FlatAppearance.BorderSize == 2) // Selected items
                .ToList();

            foreach (var button in selectedButtons)
            {
                fromPanel.Controls.Remove(button);
                button.FlatAppearance.BorderSize = 0;
                button.Location = new Point(5, toPanel.Controls.Count * (button.Height + 5));
                toPanel.Controls.Add(button);
            }
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var button = sender as Button;
                if (button != null)
                {
                    button.DoDragDrop(button, DragDropEffects.Move);
                }
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Button)))
            {
                var button = (Button)e.Data.GetData(typeof(Button));
                pnlTopGroups.Controls.Add(button);
                pnlAppGroups.Controls.Remove(button);
            }
        }


    }
}
