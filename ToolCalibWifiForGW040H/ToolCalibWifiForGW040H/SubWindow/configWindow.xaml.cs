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
    /// Interaction logic for configWindow.xaml
    /// </summary>
    public partial class configWindow : Window {
        public configWindow() {
            InitializeComponent();
            this.DataContext = GlobalData.initSetting;
            this.spBefore.Visibility = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? Visibility.Visible : Visibility.Collapsed;
            this.spAfter.Visibility = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "OK": {
                        Properties.Settings.Default.Save();
                        LimitTx.readFromFile();
                        MessageBox.Show("Success.","Save Setting", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                        break;
                    }
                case "Default": {
                        GlobalData.initSetting.ONTIP = "192.168.1.1";
                        GlobalData.initSetting.ONTUSER = "admin";
                        GlobalData.initSetting.ONTPASS = "ttcn@77CN";
                        GlobalData.initSetting.ENCALIBFREQ = true;
                        GlobalData.initSetting.ENCALIBPW2G = true;
                        GlobalData.initSetting.ENCALIBPW5G = true;
                        GlobalData.initSetting.ENWRITEBIN = true;
                        GlobalData.initSetting.ENTESTRX2G = true;
                        GlobalData.initSetting.ENTESTRX5G = true;
                        GlobalData.initSetting.ENTESTTX2G = true;
                        GlobalData.initSetting.ENTESTTX5G = true;
                        GlobalData.initSetting.ENTESTANTEN1 = true;
                        GlobalData.initSetting.ENTESTANTEN2 = true;
                        break;
                    }
                case "Cancel": {
                        this.Close();
                        break;
                    }
                default: break;
            }
        }
    }
}
