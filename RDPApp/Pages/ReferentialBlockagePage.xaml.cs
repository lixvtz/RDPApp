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
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Drawing;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Notifications;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using WinDivertSharp;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RDPApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReferentialBlockagePage : Page
    {
        public ReferentialBlockagePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            //this.Loaded += InitializeAppStorage;
        }
        private CancellationTokenSource _cancellationTokenSource;
        private async void f_ToggleWDServices(object sender, RoutedEventArgs e)
        {
            if (Checkbox1.IsChecked == true || Checkbox2.IsChecked == true || Checkbox3.IsChecked == true || Checkbox4.IsChecked == true) //real -iq
            {
                if (WDServicesToggleButton.IsChecked == true)
                {
                    WDServicesIndicator.Text = "WinDivert is Running";
                    WDServicesIndicator.Foreground = new SolidColorBrush(Microsoft.UI.Colors.LightGreen);
                    Checkbox1.IsEnabled = false;
                    Checkbox2.IsEnabled = false;
                    Checkbox3.IsEnabled = false;
                    Checkbox4.IsEnabled = false;
                    var mediaPlayer = new MediaPlayer();
                    mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-winsoundevent:Notification.Reminder"));
                    mediaPlayer.Play();
                    WDServicesToggleButton.Content = "Unblock";

                    _cancellationTokenSource = new CancellationTokenSource();
                    await Task.Run(() => CursedWinDivertServiceInitializeFunctionX(_cancellationTokenSource.Token));
                }
                else
                {
                    _cancellationTokenSource?.Cancel();
                    WDServicesIndicator.Text = "Pick your flavour";
                    WDServicesIndicator.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White);
                    Checkbox1.IsEnabled = true;
                    Checkbox2.IsEnabled = true;
                    Checkbox3.IsEnabled = true;
                    Checkbox4.IsEnabled = true;
                    var mediaPlayer = new MediaPlayer();
                    mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-winsoundevent:Notification.SMS"));
                    mediaPlayer.Play();
                    WDServicesToggleButton.Content = "Block";
                }
            }
            else
            {
                WDServicesToggleButton.IsChecked = false;
                DialogUp("No options selected", "You should select at least one option to proceed.", "OK");
            }
        }
        private void f_ROSSAToggle(object sender, RoutedEventArgs e)
        {
            //please make function
        }
        private unsafe void CursedWinDivertServiceInitializeFunctionX(CancellationToken token) //real -iq2  
        {
            string filter = "outbound && !loopback && ip && tcp.DstPort == 80 && tcp.PayloadLength > 0"; // for now just catching all the HTTP connection//s, later we will add more filters for the other protocols like UDP and ICMP and maybe even DNS if we are feeling spicy. Also, this is a very bad filter, please don't use it in production. I am not responsible for any damage caused by this code. Use at your own risk. Also, this is a very bad filter, please don't use it in production. I am not responsible for any damage caused by this code. Use at your own risk. Also, this is a very bad filter, please don't use it in production. I am not responsible for any damage caused by this code. Use at your own risk. //wtf Copilot is making -iq commmentos.
            var handle = WinDivert.WinDivertOpen(filter, WinDivertSharp.WinDivertLayer.Network, 510, WinDivertOpenFlags.Debug);

            if (handle == IntPtr.Zero || handle == new IntPtr(-1))
            {
                Debug.WriteLine("Failed to open WinDivert handle with Win32 error code {0}.", Marshal.GetLastWin32Error());
                return;
            }

            var packetBuffer = new WinDivertBuffer();
            WinDivertAddress addr = new WinDivertAddress();
            uint readLen = 0;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("CancellationToken Received.");
                    break;
                }

                if (WinDivert.WinDivertRecv(handle, packetBuffer, ref addr, ref readLen))
                {
                    var parsed = WinDivert.WinDivertHelperParsePacket(packetBuffer, readLen);

                    if (parsed.TcpHeader != null)
                    {
                        parsed.TcpHeader->Rst = 0; // I think there will be a better way to failize the packet but my -iq momentos can't find sugobye
                        parsed.TcpHeader->Ack = 0; 
                        parsed.TcpHeader->Syn = 0; 
                        parsed.TcpHeader->Fin = 1; // why is RST still not dropping the packet the rage7 based entertainment blockbuster is still waiting for saving data.. FIN is working
                        parsed.TcpHeader->Psh = 0; 
                        parsed.TcpHeader->Urg = 0; 
                    }

                    WinDivert.WinDivertHelperCalcChecksums(packetBuffer, readLen, ref addr, WinDivertChecksumHelperParam.All);

                    WinDivert.WinDivertSend(handle, packetBuffer, readLen, ref addr);
                    Debug.WriteLine("Packet is safely sent in a unsafe function ;)");
                }
                else
                {
                    Debug.WriteLine("Failed to receive packet with Win32 error code {0}.", Marshal.GetLastWin32Error());
                }
            }
            WinDivert.WinDivertClose(handle);
        }

        //private async void CursedWinDivertServiceInitializeFunctionPrimeX(object sender, RoutedEventArgs e) //testing ;)
        //{
        //    using (var cts = new CancellationTokenSource())
        //    {
        //        var token = cts.Token;
        //        cts.CancelAfter(TimeSpan.FromSeconds(1));

        //        try
        //        {
        //            await Task.Run(() => CursedWinDivertServiceInitializeFunctionX(token), token);
        //        }
        //        catch (OperationCanceledException)
        //        {
        //            Debug.WriteLine("Exception yoooooooooo");
        //        }

        //    }
        //}

        public async void DialogUp(string Title, string Content, string PrimaryButtonText)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = Title,
                Content = Content,
                PrimaryButtonText = PrimaryButtonText,
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }

        //private async void InitializeAppStorage(object sender, RoutedEventArgs e) //f0rking micro brain soft how could you make UWP ApplicationData only available on packaged apps? u r forcing me to paying annual 400$ certificaitons????
        //{
        //    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        //    string RDPAppFolderPath = Path.Combine(appDataPath, "RDPApp");

        //    if (!Directory.Exists(RDPAppFolderPath))
        //    {
        //        //string localFolder = ApplicationData.Current.LocalFolder.Path; only for Packged App. But packaging requires code signing goddammit. i have no money to do boom lets stick with old method
        //        Directory.CreateDirectory(RDPAppFolderPath);
        //        var basewindow = (MainWindow)((App)Application.Current).m_window;
        //        basewindow.InitializeMica(1);
        //        DialogUp("Hello, World!", "We have created a configuration folder at" + RDPAppFolderPath, "OK");
        //    }
        //}
    }
}
