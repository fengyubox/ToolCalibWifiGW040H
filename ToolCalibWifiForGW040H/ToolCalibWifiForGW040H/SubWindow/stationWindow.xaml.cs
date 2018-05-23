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
    /// Interaction logic for stationWindow.xaml
    /// </summary>
    public partial class stationWindow : Window {
        public stationWindow() {
            InitializeComponent();
            this.cbbStation.ItemsSource = InitParameters.ListStation;
            this.DataContext = GlobalData.initSetting;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "OK": {
                        Properties.Settings.Default.Save();
                        MainWindow m = new MainWindow();
                        m.Show();
                        this.Close();
                        break;
                    }
                case "Exit": {
                        this.Close();
                        break;
                    }
            }
        }
    }
}
