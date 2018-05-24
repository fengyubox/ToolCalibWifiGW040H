using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ToolCalibWifiForGW040H.Function;
using Microsoft.Win32;
using System.Windows.Threading;

namespace ToolCalibWifiForGW040H
{
    /// <summary>
    /// Interaction logic for attenuatorWindow.xaml
    /// </summary>
    public partial class attenuatorWindow : Window
    {
        
        formattinfo fInfo = new formattinfo();
        bool _isScroll = false;
        DispatcherTimer timer = null;

        public attenuatorWindow()
        {
            InitializeComponent();
            this.DataContext = fInfo;
            this.dgAttenuator.ItemsSource = GlobalData.autoAttenuator;
            GlobalData.autoAttenuator.Clear();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += ((sender, e) => {
                if (_isScroll == true) {
                    if (GlobalData.autoAttenuator.Count > 0)
                        this.dgAttenuator.ScrollIntoView(this.dgAttenuator.Items.GetItemAt(this.dgAttenuator.Items.Count - 1));

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
                        dlg.InitialDirectory = string.Format("{0}MasterData", System.AppDomain.CurrentDomain.BaseDirectory);
                        if (dlg.ShowDialog() == true) {
                            fInfo.FILENAME = dlg.FileName;
                        }
                        break;
                    }
                case "Auto Calculate Attenuator": {
                        if (fInfo.FILENAME.Trim() == "") return;
                        GlobalData.autoAttenuator.Clear();
                        fInfo.LOGDATA = "";
                        Thread t = new Thread(new ThreadStart(() => {
                            App.Current.Dispatcher.BeginInvoke(new Action(() => { b.IsEnabled = false; }));
                            Master _mt = new Master(fInfo.FILENAME);
                            _isScroll = true;
                            //Kết nối telnet tới ONT và máy đo
                            if (!baseFunction.Connect_Function()) return;
                            CalculateAttenuator cal = new CalculateAttenuator(GlobalData.MODEM, GlobalData.INSTRUMENT, this.fInfo);
                            string _error;
                            cal.Excute(out _error);
                            this.fInfo.LOGDATA += _error + "\r\n";
                            _isScroll = false;
                            MessageBox.Show("Success.", "Calculate Attenuator", MessageBoxButton.OK, MessageBoxImage.Information);
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
