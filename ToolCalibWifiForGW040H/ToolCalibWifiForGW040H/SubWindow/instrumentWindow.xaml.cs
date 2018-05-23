using System;
using System.Collections.Generic;
using System.Linq;
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
using ToolCalibWifiForGW040H.Function;

namespace ToolCalibWifiForGW040H {
    /// <summary>
    /// Interaction logic for instrumentWindow.xaml
    /// </summary>
    public partial class instrumentWindow : Window {
        public instrumentWindow() {
            InitializeComponent();
            this.cbbInstruments.ItemsSource = InitParameters.ListInstrument;
            this.cbbRFPorts.ItemsSource = InitParameters.ListRFPort;
            this.DataContext = GlobalData.initSetting;
            this.wpPwcalib2G.Visibility = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? Visibility.Visible : Visibility.Collapsed;
            this.wpPwcalib5G.Visibility = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "OK": {
                        Properties.Settings.Default.Save();
                        MessageBox.Show("Success.", "Save Setting", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                        break;
                    }
                case "Default": {
                        GlobalData.initSetting.INSTRUMENT = "MT8870A";
                        GlobalData.initSetting.VISAADDRESS = "TCPIP0::192.168.1.2::inst0::INSTR";
                        GlobalData.initSetting.RFPORT = "RFIO1";
                        GlobalData.initSetting.TARGETPOWER2G = "19.5";
                        GlobalData.initSetting.TARGETPOWER5G = "18";
                        break;
                    }
                case "Cancel": {
                        this.Close();
                        break;
                    }
            }
        }
    }
}
