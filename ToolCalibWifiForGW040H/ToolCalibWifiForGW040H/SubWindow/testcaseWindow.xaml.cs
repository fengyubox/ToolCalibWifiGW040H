using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ToolCalibWifiForGW040H.Function;

namespace ToolCalibWifiForGW040H {

    /// <summary>
    /// Interaction logic for testcaseWindow.xaml
    /// </summary>
    public partial class testcaseWindow : Window {

        ObservableCollection<string> listwifi { get; set; } = new ObservableCollection<string>() { "802.11b", "802.11g", "802.11a", "802.11n", "802.11ac" };
        ObservableCollection<string> listrate { get; set; } = new ObservableCollection<string>() { "MCS0", "MCS1", "MCS2", "MCS3", "MCS4", "MCS5", "MCS6", "MCS7", "MCS8", "MCS9" };
        ObservableCollection<string> listbandwidth { get; set; } = new ObservableCollection<string>() { "20", "40", "80", "160" };
        ObservableCollection<string> listanten { get; set; } = new ObservableCollection<string>() { "1", "2" };
        ObservableCollection<string> listchannelfreq2G { get; set; } = new ObservableCollection<string>() { "2412", "2417", "2422", "2427", "2432", "2437", "2442", "2447", "2452", "2457", "2462", "2467", "2472" };
        ObservableCollection<string> listchannelfreq5G { get; set; } = new ObservableCollection<string>() { "4920", "5080", "5180", "5190", "5210", "5240", "5260", "5320", "5500", "5510", "5530", "5580", "5600", "5680", "5700", "5775", "5785", "5795", "5805", "5825" };
        ObservableCollection<string> listchannelfreqAnten { get; set; } = new ObservableCollection<string>() { "2412", "2417", "2422", "2427", "2432", "2437", "2442", "2447", "2452", "2457", "2462", "2467", "2472", "4920", "5080", "5180", "5190", "5210", "5240", "5260", "5320", "5500", "5510", "5530", "5580", "5600", "5680", "5700", "5775", "5785", "5795", "5805", "5825" };
        ObservableCollection<int> listpacket { get; set; } = new ObservableCollection<int>() { 100, 500, 1000 };

        public testcaseWindow() {
            InitializeComponent();

            //TX-2G
            this.cbbtx2wifi.ItemsSource = listwifi;
            this.cbbtx2rate.ItemsSource = listrate;
            this.cbbtx2bandwidth.ItemsSource = listbandwidth;
            this.dgtxBand24.ItemsSource = GlobalData.tmplisttxWifi2G;

            //TX-5G
            this.cbbtx5wifi.ItemsSource = listwifi;
            this.cbbtx5rate.ItemsSource = listrate;
            this.cbbtx5bandwidth.ItemsSource = listbandwidth;
            this.dgtxBand50.ItemsSource = GlobalData.tmplisttxWifi5G;

            //RX-2G
            this.cbbrx2wifi.ItemsSource = listwifi;
            this.cbbrx2rate.ItemsSource = listrate;
            this.cbbrx2bandwidth.ItemsSource = listbandwidth;
            this.cbbrx2packet.ItemsSource = listpacket;
            this.dgrxBand24.ItemsSource = GlobalData.tmplistrxWifi2G;

            //RX-5G
            this.cbbrx5wifi.ItemsSource = listwifi;
            this.cbbrx5rate.ItemsSource = listrate;
            this.cbbrx5bandwidth.ItemsSource = listbandwidth;
            this.cbbrx5packet.ItemsSource = listpacket;
            this.dgrxBand50.ItemsSource = GlobalData.tmplistrxWifi5G;

            //TX-ANTEN1
            this.cbbat1wifi.ItemsSource = listwifi;
            this.cbbat1rate.ItemsSource = listrate;
            this.cbbat1bandwidth.ItemsSource = listbandwidth;
            this.cbbat1anten.ItemsSource = listanten;
            this.cbbat1channelfreq.ItemsSource = listchannelfreq2G;
            this.dganten1.ItemsSource = GlobalData.tmplisttestAnten1;

            //TX-ANTEN2
            this.cbbat2wifi.ItemsSource = listwifi;
            this.cbbat2rate.ItemsSource = listrate;
            this.cbbat2bandwidth.ItemsSource = listbandwidth;
            this.cbbat2anten.ItemsSource = listanten;
            this.cbbat2channelfreq.ItemsSource = listchannelfreq2G;
            this.dganten2.ItemsSource = GlobalData.tmplisttestAnten2;

            bool ret = GlobalData.initSetting.STATION == "Trước đóng vỏ";
            this.tabwifitx.Visibility = ret == true ? Visibility.Visible : Visibility.Collapsed;
            this.tabwifirx.Visibility = ret == true ? Visibility.Visible : Visibility.Collapsed;
            this.tabwifianten.Visibility = ret == true ? Visibility.Collapsed : Visibility.Visible;
            if (ret == true) this.tabwifitx.IsSelected = true;
            else this.tabwifianten.IsSelected = true;

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "Exit": { this.Close(); break; }
                case "Save": {
                        if (MessageBox.Show("Bạn muốn lưu thay đổi test case phải không?\n\n Chọn 'YES' để lưu\n Chọn 'NO' để hủy.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
                        if (TestCase.Save()) {
                            MessageBox.Show("Sucess.", "Save test case", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else MessageBox.Show("Fail.", "Save test case", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                        break;
                    }
                default: { break; }
            }
        }

    }
}
