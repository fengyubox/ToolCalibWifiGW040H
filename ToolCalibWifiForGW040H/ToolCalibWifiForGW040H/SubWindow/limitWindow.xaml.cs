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
    /// Interaction logic for limitWindow.xaml
    /// </summary>
    public partial class limitWindow : Window {
        public limitWindow() {
            InitializeComponent();
            this.dgtxlimit.ItemsSource = GlobalData.listLimitWifiTX;
            this.dgrxlimit.ItemsSource = GlobalData.listLimitWifiRX;
            this.dgattenuator.ItemsSource = GlobalData.listAttenuator;
            this.dgwaveform.ItemsSource = GlobalData.listWaveForm;
            this.dgchannelmanagement.ItemsSource = GlobalData.listChannel;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }
    }
}
