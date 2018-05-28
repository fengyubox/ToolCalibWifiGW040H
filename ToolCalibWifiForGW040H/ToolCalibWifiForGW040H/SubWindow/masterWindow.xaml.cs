using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// Interaction logic for masterWindow.xaml
    /// </summary>
    public partial class masterWindow : Window {

        formattinfo fInfo = new formattinfo() { FILENAME = string.Format("{0}Config\\Attenuator-Config.csv", System.AppDomain.CurrentDomain.BaseDirectory) };

        bool _isScroll = false;
        DispatcherTimer timer = null;
        Wire w;


        public masterWindow() {
            InitializeComponent();
            this.DataContext = fInfo;
            this.dgCalMaster.ItemsSource = GlobalData.autoCalculateMaster;
            w = new Wire(fInfo.FILENAME);

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += ((sender, e) => {
                if (_isScroll == true) {
                    this.dgCalMaster.Items.Refresh();
                    if (GlobalData.mtIndex > 1)
                        this.dgCalMaster.ScrollIntoView(this.dgCalMaster.Items.GetItemAt(GlobalData.mtIndex - 1));
                    _scrollViewer.ScrollToEnd();
                }
            });
            timer.Start();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "Browser...": {
                        OpenFileDialog dlg = new OpenFileDialog();
                        dlg.Filter = "*.csv|*.csv";
                        dlg.InitialDirectory = string.Format("{0}Wire", System.AppDomain.CurrentDomain.BaseDirectory);
                        if (dlg.ShowDialog() == true) {
                            fInfo.FILENAME = dlg.FileName;
                            w = new Wire(fInfo.FILENAME);
                        }
                        break;
                    }

                case "Calculate Master": {
                        if (fInfo.FILENAME.Trim() == "") return;
                        fInfo.LOGDATA = "";
                        Thread t = new Thread(new ThreadStart(() => {
                            App.Current.Dispatcher.BeginInvoke(new Action(() => { b.IsEnabled = false; }));
                            _isScroll = true;
                            //Kết nối telnet tới ONT và máy đo
                            if (!baseFunction.Connect_Function()) return;
                            CalculateMaster cal = new CalculateMaster(GlobalData.MODEM, GlobalData.INSTRUMENT, this.fInfo);
                            string _error;
                            cal.Excute(out _error);
                            this.fInfo.LOGDATA += _error + "\r\n";
                            _isScroll = false;
                            MessageBox.Show("Success.", "Calculate Master", MessageBoxButton.OK, MessageBoxImage.Information);
                            App.Current.Dispatcher.BeginInvoke(new Action(() => { b.IsEnabled = true; }));
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }
                default: break;
            }
        }
    }
}
