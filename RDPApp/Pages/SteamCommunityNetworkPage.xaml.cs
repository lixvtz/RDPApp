using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RDPApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SteamCommunityNetworkPage : Page
    {
        private const string ConfigFilePath = "SCN/SCNConfig.json";
        private List<SCNEntry> _entries = new();
        public SteamCommunityNetworkPage()
        {
            this.InitializeComponent();
            LoadSCNConfig();
        }

        private async void LoadSCNConfig()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, ConfigFilePath);

                if (File.Exists(filePath))
                {
                    string jsonContent = await File.ReadAllTextAsync(filePath);
                    _entries = JsonSerializer.Deserialize<List<SCNEntry>>(jsonContent) ?? new List<SCNEntry>();
                    RefreshUI();
                }
                else
                {
                    Debug.WriteLine($"File not found: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading SCNConfig: " + ex.Message);
            }
        }

        private void RefreshUI()
        {
            ButtonsPanel.Children.Clear();
            foreach (var entry in _entries)
            {
                AddButton(entry);
            }
        }

        private void AddButton(SCNEntry entry)
        {
            Button btn = new Button
            {
                Content = entry.Name,
                Margin = new Thickness(0, 0, 0, 10)
            };
            btn.Click += (s, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start("http://"+entry.Url);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error opening URL: " + ex.Message);
                }
            };
            ButtonsPanel.Children.Add(btn);
        }

        private async void NewEntry_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Add New Entry",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Content.XamlRoot
            };

            StackPanel panel = new StackPanel();
            TextBox nameBox = new TextBox { PlaceholderText = "Name", Margin = new Thickness(0, 0, 0, 10) };
            TextBox urlBox = new TextBox { PlaceholderText = "URL" };
            panel.Children.Add(nameBox);
            panel.Children.Add(urlBox);
            dialog.Content = panel;

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string name = nameBox.Text.Trim();
                string url = urlBox.Text.Trim();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                {
                    var newEntry = new SCNEntry { Name = name, Url = url };
                    _entries.Add(newEntry);
                    SaveSCNConfig();
                    AddButton(newEntry);
                }
            }
        }

        private async void SaveSCNConfig()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, ConfigFilePath);

                string jsonContent = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error saving SCNConfig: " + ex.Message);
            }
        }

        private class SCNEntry
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }

}
