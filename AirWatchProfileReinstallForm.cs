using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using NLog;
namespace BBIHardwareSupport
{
    public partial class AirWatchProfileReinstallForm : Form
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private TextBox textBoxSerial;
        private TextBox responseTextBox;
        private ListBox listBoxProfiles;
        private Dictionary<string, int> profiles;

        public AirWatchProfileReinstallForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "AirWatch Profile Reinstall";
            this.Width = 500;
            this.Height = 450;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Panel
            var panel = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };

            // Label for Profiles
            var labelProfiles = new Label { Text = "Select Profiles:", Top = 20, Left = 20, AutoSize = true };
            panel.Controls.Add(labelProfiles);

            // ListBox for Profiles
            listBoxProfiles = new ListBox { SelectionMode = SelectionMode.MultiExtended, Top = 50, Left = 20, Width = 280, Height = 100 };
            panel.Controls.Add(listBoxProfiles);

            // Profiles Dictionary
            profiles = new Dictionary<string, int>
            {
                { "AND-CUST-SETTINGS-HUB-KSP", 459 },
                { "AND-CUSTOM_SETTINGS_BAT_OPT_POSI", 457 },
                { "AND-APP-CONTROL-DENY", 424 },
                { "AND-RESTRICTION-USB", 446 }
            };
            listBoxProfiles.Items.AddRange(profiles.Keys.ToArray());

            // Label for Serial Number
            var labelSerial = new Label { Text = "Enter Device Serial Number:", Top = 160, Left = 20, AutoSize = true };
            panel.Controls.Add(labelSerial);

            // TextBox for Serial Number
            textBoxSerial = new TextBox { Top = 185, Left = 20, Width = 280 };
            panel.Controls.Add(textBoxSerial);

            // OK Button
            var buttonOK = new Button { Text = "OK", Top = 220, Left = 20 };
            buttonOK.Click += OnOkButtonClicked;
            panel.Controls.Add(buttonOK);

            // Cancel Button
            var buttonCancel = new Button { Text = "Cancel", Top = 220, Left = 120 };
            buttonCancel.Click += (sender, e) => this.Close();
            panel.Controls.Add(buttonCancel);

            // Response TextBox
            responseTextBox = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Top = 270, Left = 20, Width = this.ClientSize.Width - 40, Height = 120 };
            this.Controls.Add(responseTextBox);

            // Add panel to form and center it
            this.Controls.Add(panel);
        }

        private void OnOkButtonClicked(object sender, EventArgs e)
        {
            var selectedProfiles = listBoxProfiles.SelectedItems.Cast<string>().Select(item => profiles[item]).ToList();
            var deviceSerialNumber = textBoxSerial.Text;
            responseTextBox.Clear();

            if (string.IsNullOrEmpty(deviceSerialNumber))
            {
                responseTextBox.AppendText("Device Serial Number cannot be blank!\n");
                return;
            }

            foreach (var profileId in selectedProfiles)
            {
                // Call your PowerShell function to reinstall the profile
                var response = UpdateAirWatchReinstallProfileBySerial(deviceSerialNumber, profileId);

                // Display response
                if (response.StatusCode == 200)
                {
                    responseTextBox.AppendText($"Profile ID {profileId}: Success\n");
                }
                else if (!string.IsNullOrEmpty(response.ErrorBody))
                {
                    responseTextBox.AppendText($"Error Status Code: {response.StatusCode}\n");
                    responseTextBox.AppendText($"Error Details: {response.ErrorBody}\n");
                }
                else
                {
                    responseTextBox.AppendText("Unexpected response format or unknown error.\n");
                }
            }
        }
        // This should be updated to live in a module
        private (int StatusCode, string ErrorBody) UpdateAirWatchReinstallProfileBySerial(string serialNumber, int profileId)
        {
            // Placeholder: Call PowerShell module function here and handle response
            // You may need to use System.Management.Automation to invoke PowerShell if it's part of the AirWatch integration.
            // Return dummy data for demonstration purposes.
            return (200, null);
        }
    }
}
