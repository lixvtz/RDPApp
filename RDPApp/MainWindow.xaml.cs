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
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI;
using RDPApp.Pages;
using Windows.Networking.NetworkOperators;
using System.Runtime.InteropServices;
using System.Threading;
using WinDivertSharp;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using RDPApp;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RDPApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            var width = 480;
            var height = 320;
            SetWindowSize(width, height);
            CenterWindow(width, height);
            SetTitleBar(AppTitleBar);
            InitializeMica(4);
            // Set window non-resizable and non-maximizable
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;

            this.InitializeComponent();
        }

        public void InitializeMica(int Style=4)
        {
            switch (Style) {
                case 1:
                    ExtendsContentIntoTitleBar = true;
                    this.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.Base };
                    break;
                case 2:
                    ExtendsContentIntoTitleBar = true;
                    this.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
                    break;
                case 3:
                    ExtendsContentIntoTitleBar = true;
                    this.SystemBackdrop = new DesktopAcrylicBackdrop { };
                    break;
                case 4:
                    ExtendsContentIntoTitleBar = true;
                    this.SystemBackdrop = null;
                    break;
            }
        }
        public void SetWindowSize(int width, int height)
        {
            if (AppWindow is not null)
            {
                AppWindow.Resize(new SizeInt32(width, height));
            }
        }
        private void CenterWindow(int width, int height)
        {
            if (AppWindow is not null)
            {
                var displayArea = DisplayArea.GetFromWindowId(
                    AppWindow.Id, DisplayAreaFallback.Primary);

                AppWindow.Move(new PointInt32(
                    (displayArea.WorkArea.Width - width) / 2,
                    (displayArea.WorkArea.Height - height) / 2));
            }
        }
        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) // Tab Navigator
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string pageName = "RDPApp.Pages." + selectedItem.Tag;
                Type pageType = Type.GetType(pageName);
                ContentFrame.Navigate(pageType);
            }
        }
    }
}