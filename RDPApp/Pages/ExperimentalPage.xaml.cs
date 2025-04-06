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
using WinDivertSharp;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RDPApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExperimentalPage : Page
    {
        public ExperimentalPage()
        {
            this.InitializeComponent();
        }

        public void ValidateCheck_Click(object sender, RoutedEventArgs e)
        {
            string beingtestedfilter = FilterTextBox.Text;
            string errormessage = string.Empty;
            uint a = 0; //please someone fix errorpos functionality
            if (!WinDivert.WinDivertHelperCheckFilter(beingtestedfilter, WinDivertLayer.Network, out errormessage, ref a))
            {
                ValidationResult.Text = "Filter is invalid: " + errormessage + ", " + a;
            }
            else
            {
                ValidationResult.Text = "Filter is valid.";
            }
        }
    }
}
