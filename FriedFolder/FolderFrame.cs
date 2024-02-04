using SharpShell.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using File = System.IO.File;
using SharpShell.Interop;

namespace FriedFolder
{
    public partial class FolderFrame : UserControl
    {
        private readonly string MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FriedFolder");
        private string FilesPath => Path.Combine(MainPath, "files");
        private string UploadedFilesPath => Path.Combine(FilesPath, "uploaded");
        private string GeneratedFilesPath => Path.Combine(FilesPath, "generated");
        private string DataPath => Path.Combine(MainPath, "settings");
        private string DataPathFile => Path.Combine(DataPath, "settings.txt");

        #region settings
        public string Username { get; set; }
        public string Password { get; set; }
        public string Endpoint { get; set; }

        public bool PrivateEnabled { get; set; } = true;
        public bool PublicEnabled { get; set; } = false;
        public bool OtherEnabled { get; set; }  = false;
        #endregion

        public FolderFrame()
        {
            InitializeComponent();

            if (!Directory.Exists(MainPath))
                Directory.CreateDirectory(MainPath);

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            if (!Directory.Exists(FilesPath))
                Directory.CreateDirectory(FilesPath);

            if (!Directory.Exists(UploadedFilesPath))
                Directory.CreateDirectory(UploadedFilesPath);

            if (!Directory.Exists(GeneratedFilesPath))
                Directory.CreateDirectory(GeneratedFilesPath);

            //if (!File.Exists($@"{GeneratedFilesPath}\Webdav.lnk"))
            //    CreateShortcut();

            if (!File.Exists(DataPathFile))
            {
                File.WriteAllText(DataPathFile, "loading");
                SetSettings();
            }
            LoadSettings();


            //label1.Text = $"Username: {Username}";
            //label2.Text = $"Password: {Password}";
            //label3.Text = $"Url: {Url}";
            //label4.Text = $"Endpoint: {Endpoint}";
            //label5.Text = $"Path: {DataPathFile}";
            tmrLoad.Start();
        }

        private void LoadSettings() 
        {
            string settings = File.ReadAllText(DataPathFile);
            var options = settings.Split(';');
            foreach (var item in options)
            {
                if (string.IsNullOrEmpty(item.Trim()))
                    continue;

                var option = item.Split('=');
                if (option.Count() < 2)
                    continue;

                string key = option[0];
                string value = option[1];

                // Using reflection to set properties based on key-value pairs
                PropertyInfo property = this.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    // Convert the string value to the property type
                    object convertedValue = Convert.ChangeType(value, property.PropertyType);

                    // Set the property value
                    property.SetValue(this, convertedValue);
                }

            }
        }

        private void SetSettings()
        {
            string settings = string.Empty;
            // Using reflection to set properties based on key-value pairs
            PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            foreach (var property in properties)
            {
                // Check if the property is declared in FolderFrame class
                if (property.DeclaringType == typeof(FolderFrame))
                {
                    settings += $"{property.Name}={property.GetValue(this)};";
                }   
            }
            File.WriteAllText(DataPathFile, settings);
        }

        private void CreateShortcut(string Name = "Webdav", string Path = "C://")
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            //string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Notepad.lnk";
            string shortcutAddress = $@"{GeneratedFilesPath}\{Name}.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "New shortcut for a Notepad";
            //shortcut.Hotkey = "Ctrl+Shift+N";
            //shortcut.TargetPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\notepad.exe";
            shortcut.TargetPath = Path;
            shortcut.Save();
        }

        private void tmrLoad_Tick(object sender, EventArgs e)
        {
            tmrLoad.Enabled = false;
            UpdateColumns();
            foreach (var file in Directory.EnumerateFiles(GeneratedFilesPath))
            {
                File.Delete(file);
            }
            CreateShortcut("Public", $@"{Endpoint}\{Username}\Public");
            CreateShortcut("Private", $@"{Endpoint}\{Username}\Private");
            CreateShortcut("All Repos", $@"{Endpoint}");

            //var url = $"https://{Username}:{Password}@{Endpoint}";
            //var url = @"drive:\\repo.friedmonkey.nl@SSL\repodata";
            //MessageBox.Show($"Logging in with credentials: username={Username}, password={Password}");
            var result = Console.Execute($"net use {Endpoint} {Password} /USER:{Username}");
            //MessageBox.Show(result);
            //MessageBox.Show($"Navigating to:{GeneratedFilesPath}");
            //UriBuilder builder = new UriBuilder(url);
            //builder.UserName = Username;
            //builder.Password = Password;
            //var uri = builder.Uri;

            //navigate to a folder which has nothing but a single webdav shortcut
            webBrowserFiles.Navigate(GeneratedFilesPath);

            //simulate click
            //webBrowserFiles.cl
        }

        private void bttnBack_Click(object sender, EventArgs e)
        {
            webBrowserFiles.GoBack();
        }

        private void bttnForward_Click(object sender, EventArgs e)
        {
            webBrowserFiles.GoForward();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Type controlType = webBrowserFiles.GetType();
            //PropertyInfo[] properties = controlType.GetProperties();

            //foreach (PropertyInfo property in properties)
            //{
            //    Console.WriteLine($"{property.Name}: {property.GetValue(webBrowserFiles)}");
            //}
            Process.Start("notepad.exe", DataPathFile);
        }

        private void UpdateColumns() 
        {
            float enabledCount = 0;
            if (PrivateEnabled) enabledCount++;
            if (PublicEnabled) enabledCount++;
            if (OtherEnabled) enabledCount++;

            float procentage = (enabledCount / 3) * 100;

            tableLayoutPanel1.ColumnStyles[0].Width = PrivateEnabled ? procentage : 0;
            tableLayoutPanel1.ColumnStyles[1].Width = PublicEnabled ? procentage : 0;
            tableLayoutPanel1.ColumnStyles[2].Width = OtherEnabled ? procentage : 0;

            togglePrivateToolStripMenuItem.Checked = PrivateEnabled;
            togglePublicToolStripMenuItem.Checked = PublicEnabled;
            toggleOtherToolStripMenuItem.Checked = OtherEnabled;
        }

        private void togglePrivateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrivateEnabled = !PrivateEnabled;
            UpdateColumns();
        }

        private void togglePublicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PublicEnabled = !PublicEnabled;
            UpdateColumns();
        }

        private void toggleOtherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OtherEnabled = !OtherEnabled;
            UpdateColumns();
        }
    }
}
