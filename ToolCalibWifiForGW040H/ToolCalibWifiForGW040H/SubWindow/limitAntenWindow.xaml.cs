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
    /// Interaction logic for limitAntenWindow.xaml
    /// </summary>
    public partial class limitAntenWindow : Window {
        public limitAntenWindow() {
            InitializeComponent();
            this.DataContext = GlobalData.initSetting;
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
                        GlobalData.initSetting.STDPWANTEN1 = "-7";
                        GlobalData.initSetting.STDPWANTEN2 = "-15";
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
