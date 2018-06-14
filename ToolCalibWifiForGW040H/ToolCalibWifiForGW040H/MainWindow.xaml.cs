using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ToolCalibWifiForGW040H.Function;

namespace ToolCalibWifiForGW040H {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        DispatcherTimer timer = null;
        bool _isScroll = false;

        public MainWindow() {
            InitializeComponent();
            this.datagridTX.ItemsSource = GlobalData.datagridlogTX;
            this.DataContext = GlobalData.testingData;
            this.spBefore.Visibility = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? Visibility.Visible : Visibility.Collapsed;
            this.spAfter.Visibility = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? Visibility.Collapsed : Visibility.Visible;
            this.lblTitle.Content = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? "PHẦN MỀM CALIBRATION + TEST WIFI ONT GW040H" : "PHẦN MỀM TEST ANTEN WIFI ONT GW040H";
            this.miMaster.IsEnabled = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? true : false;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += ((sender, e) => {
                if (_isScroll == true) {
                    _scrollViewer.ScrollToEnd();
                    if (GlobalData.datagridlogTX.Count > 0) {
                        this.datagridTX.ScrollIntoView(this.datagridTX.Items.GetItemAt(GlobalData.datagridlogTX.Count - 1));
                    }
                }
            });
            timer.Start();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            Label l = sender as Label;
            switch (l.Content) {
                case "X": { this.Close(); break; }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            MenuItem menu = sender as MenuItem;
            string _text = menu.Header.ToString();
            switch (_text) {
                case "Exit": { this.Close(); break; }
                case "Result test": {
                        string _Str = string.Format("{0}Logtest", AppDomain.CurrentDomain.BaseDirectory);
                        Process.Start(_Str);
                        break; }
                case "Details": {
                        string _Str = string.Format("{0}Logdetail", AppDomain.CurrentDomain.BaseDirectory);
                        Process.Start(_Str);
                        break; }
                case "Thiết lập cấu hình": {
                        configWindow cfg = new configWindow();
                        cfg.ShowDialog();
                        break; }
                case "Thiết lập máy đo": {
                        instrumentWindow inst = new instrumentWindow();
                        inst.ShowDialog();
                        break; }
                case "Thiết lập tiêu chuẩn": {
                        if (GlobalData.initSetting.STATION == "Trước đóng vỏ") {
                            limitWindow lim = new limitWindow();
                            lim.ShowDialog();
                        }
                        else {
                            limitAntenWindow lim = new limitAntenWindow();
                            lim.ShowDialog();
                        }
                        break; }
                case "Thiết lập bài test": {
                        testcaseWindow tcw = new testcaseWindow();
                        tcw.ShowDialog();
                        break;
                    }
                case "Thiết lập suy hao": {
                        attenuatorWindow att = new attenuatorWindow();
                        att.ShowDialog();
                        break;
                    }
                case "Thiết lập master": {
                        masterWindow mw = new masterWindow();
                        mw.ShowDialog();
                        break;
                    }
                case "Phân tích kết quả test": {
                        analyzerWindow anal = new analyzerWindow();
                        anal.ShowDialog();
                        break; }
                case "About": {
                        About ab = new About();
                        ab.ShowDialog();
                        break;
                    }
                default: break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "START": {
                        GlobalData.testingData.Initialize();
                        GlobalData.datagridlogTX.Clear();
                        Thread t = new Thread(new ThreadStart(() => {
                            GlobalData.testingData.BUTTONCONTENT = "STOP";
                            RUNALL();
                            GlobalData.testingData.BUTTONCONTENT = "START";
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }
                default: break;
            }
        }


        #region MAIN_FUNCTION 


        private bool Read_All_Config_File() {
            try {
                if (!WaveForm.readFromFile()) return false;
                if (!Attenuator.readFromFile()) return false;
                if (!LimitTx.readFromFile()) return false;
                if (!LimitRx.readFromFile()) return false;
                return true;
            }
            catch {
                return false;
            }
        }


        private void RUNALL() {
            _isScroll = true;
            Stopwatch _st = new Stopwatch();
            _st.Start();
            bool _flag = true;
            GlobalData.logManager = null; //ko luu log neu logManager = null
            GlobalData.logRegister = null;
            GlobalData.testingData.TOTALRESULT = InitParameters.Statuses.Wait;

            //Đọc file cấu hình config
            if (!Read_All_Config_File()) {
                GlobalData.testingData.ERRORCODE = "{ ErrorCode: 0x001 }";
                _flag = false;
                goto Finished;
            }

            //Kết nối telnet tới ONT và máy đo
            if (!baseFunction.Connect_Function()) {
                GlobalData.testingData.ERRORCODE = "{ ErrorCode: 0x002 }";
                _flag = false;
                goto Finished;
            }

            GlobalData.logManager = new logdata(); //luu log test
            GlobalData.logManager.mac = GlobalData.testingData.MACADDRESS;

            if (GlobalData.initSetting.STATION == "Trước đóng vỏ") {
                Calibration calib = new Calibration(GlobalData.MODEM, GlobalData.INSTRUMENT);
                if (!calib.Excute()) {
                    _flag = false;
                    goto Finished;
                }
            }
            else {
                TestAnten testat = new TestAnten(GlobalData.MODEM, GlobalData.INSTRUMENT);
                if (!testat.Excute()) {
                    _flag = false;
                    goto Finished;
                }
            }

            Finished:
            GlobalData.testingData.LOGSYSTEM += string.Format("{0} ĐÃ HOÀN THÀNH.\r\n", _flag == true ? "[OK]" : "[NG]");
            _st.Stop();
            GlobalData.testingData.LOGSYSTEM += string.Format("Tổng thời gian test: {0} sec.\r\n", _st.ElapsedMilliseconds / 1000);
            GlobalData.testingData.TOTALRESULT = _flag == true ? "PASS" : "FAIL";

            GlobalData.logManager.errorCode = GlobalData.testingData.ERRORCODE;
            GlobalData.logManager.totalResult = GlobalData.testingData.TOTALRESULT;

            if (GlobalData.logManager != null) {
                LogFile.Savetestlog(GlobalData.logManager);
                LogFile.Savedetaillog(GlobalData.testingData.LOGSYSTEM);
                LogFile.Savereviewlog(GlobalData.logManager.mac);
            }
            if (GlobalData.logRegister != null) {
                GlobalData.logRegister.macaddress = GlobalData.logManager.mac;
                GlobalData.logRegister.totalresult = GlobalData.testingData.TOTALRESULT;
                LogRegister.Save(GlobalData.logRegister);
            }
            _isScroll = false;
        }

        #endregion


    }
}
