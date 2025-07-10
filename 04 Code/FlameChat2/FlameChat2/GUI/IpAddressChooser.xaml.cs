using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlameChatAdmin.GUIDialogs
{
    /// <summary>
    /// Interaction logic for IpAddressChooser.xaml
    /// </summary>
    public partial class IpAddressChooser : Window
    {

        public IPAddress ChosenIPAddress { get; set; }

        public IpAddressChooser()
        {
            InitializeComponent();
        }

        private void BtnChooseIP_Click(object sender, RoutedEventArgs e)
        {
            ChosenIPAddress = (IPAddress)listIPs.SelectedItem;
            DialogResult = true;
            Close();
        }

    }
}
