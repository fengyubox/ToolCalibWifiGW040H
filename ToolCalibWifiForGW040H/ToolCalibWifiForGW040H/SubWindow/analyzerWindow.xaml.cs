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
    /// Interaction logic for analyzerWindow.xaml
    /// </summary>
    public partial class analyzerWindow : Window {

        Dictionary<string, string> dictRegisterGroupChannel = new Dictionary<string, string>() {
            { "0x59", "1~6" },
            { "0x5A", "7~10" },
            { "0x5B", "11~14" },
            { "0x5F", "1~6" },
            { "0x60", "7~10" },
            { "0x61", "11~14" },
            { "0x143", "184,188,192" },
            { "0x144", "196,8,12,16" },
            { "0x148", "36,40" },
            { "0x149", "44,48" },
            { "0x14D", "52,56" },
            { "0x14E", "60,64" },
            { "0x157", "100,104" },
            { "0x158", "108,112,116" },
            { "0x15C", "120,124" },
            { "0x15D", "128,132,136" },
            { "0x161", "140,144" },
            { "0x162", "149,153,157" },
            { "0x166", "161" },
            { "0x167", "165" },
            { "0x16B", "184,188,192" },
            { "0x16C", "196,8,12,16" },
            { "0x170", "36,40" },
            { "0x171", "44,48" },
            { "0x175", "52,56" },
            { "0x176", "60,64" },
            { "0x17F", "100,104" },
            { "0x180", "108,112,116" },
            { "0x184", "120,124" },
            { "0x185", "128,132,136" },
            { "0x189", "140,144" },
            { "0x18A", "149,153,157" },
            { "0x18E", "161" },
            { "0x18F", "165" }
        };

        public analyzerWindow() {
            InitializeComponent();
            this.dgCalib.ItemsSource = GlobalData.reviewRegister;
            this.dgRX.ItemsSource = GlobalData.reviewRX;
            this.dgTX.ItemsSource = GlobalData.reviewTX;

            GlobalData.reviewRegister.Clear();
            GlobalData.reviewRX.Clear();
            GlobalData.reviewTX.Clear();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "Data Analysis": {
                        string _txt = _getDatalog();
                        if (_txt == null || _txt.Trim().Length == 0 || _txt.Trim() == "") return;

                        //Get log calib input into datagrid
                        _getlogCalib_InputInto_Datagrid(_txt);

                        //Get log verify tx input into datagrid
                        _getlogVerifyTx_InputInto_Datagrid(_txt);

                        //Get log sensivity rx input into datagrid
                        _getlogSensivityRx_InputInto_Datagrid(_txt);

                        MessageBox.Show("Completed.","Data Analysis", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
            }
        }

        string _getDatalog() {
            string _text = new TextRange(this.fldDatalog.ContentStart, this.fldDatalog.ContentEnd).Text;
            return _text;
        }

        bool _getlogCalib_InputInto_Datagrid(string _logdata) {
            try {
                if (_logdata.Contains("Thời gian calib") == true) {
                    GlobalData.reviewRegister.Clear();
                    string[] buffer = _logdata.Split(new string[] { "Thời gian calib" }, StringSplitOptions.None);
                    
                    for (int i = 0; i < buffer.Length; i++) { 
                        string _txt = buffer[i]; //Dữ liệu calib của từng thanh ghi - toàn bộ (ko chia theo dòng)
                        string[] lines = _txt.Split('\n');
                        int _maxIndex = lines.Length - 2;
                        if (lines[_maxIndex].Contains("Giá trị cần truyền") || lines[_maxIndex].Contains("Độ lệch công suất")) {
                            List<string> _li = new List<string>();
                            for(int k = _maxIndex; k >= 0; k--) {
                                if (lines[k].Contains("***")) break;
                                _li.Add(lines[k]);
                            }
                            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                           
                            logreviewregister _log = new logreviewregister();
                            foreach (var item in _li) { //Dữ liệu calib của từng thanh ghi - theo từng dòng
                                //Gia tri thanh ghi
                                if (_txt.Contains("Giá trị cần truyền")) {
                                    if (item.Contains("Giá trị cần truyền"))
                                        _log.registerValue = item.Split(new string[] { "Giá trị cần truyền" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace(":", "").Replace("(dBm)", "").Trim();
                                } else {
                                    if (item.Contains("Giá trị thanh ghi"))
                                        _log.registerValue = item.Split(new string[] { "hiện tại" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace(":", "").Replace("(dBm)", "").Trim();
                                }
                                //Do lech cong suat
                                if (item.Contains("Độ lệch công suất")) {
                                    _log.diffPower = item.Split(new string[] { "Độ lệch công suất" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace(":", "").Replace("(dBm)", "").Trim();
                                    _log.diffPower = _log.diffPower.Contains("-") == true ? _log.diffPower.Replace("-", "+") : string.Format("-{0}", _log.diffPower);
                                }
                                //Công suất đo được
                                if(item.Contains("Công suất đo được"))
                                    _log.currentPower = item.Split(new string[] { "Công suất đo được" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace(":", "").Replace("(dBm)", "").Trim();
                            }

                            string _tmp = _li[_li.Count - 1];
                            string[] _datas = _tmp.Split('-');
                            //Range frequency
                            _log.rangeFreq = _datas[0].Trim();
                            //Anten
                            _log.Anten = _datas[5].Trim();
                            //Ten thanh ghi
                            _log.Register = _datas[7].Trim();
                            //Group channel
                            string _ttt = "";
                             bool ret = dictRegisterGroupChannel.TryGetValue(_log.Register, out _ttt);
                            _log.groupChannel = _ttt;
                            GlobalData.reviewRegister.Add(_log);
                            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                        }
                    }
                }
                return true;
            } catch {
                return false;
            }
        }

        bool _getlogVerifyTx_InputInto_Datagrid(string _logdata) {
            try {
                if (_logdata.Contains("Thời gian verify") == true) {
                    GlobalData.reviewTX.Clear();
                    string[] buffer = _logdata.Split(new string[] { "Thời gian verify" }, StringSplitOptions.None);

                    for (int i = 0; i < buffer.Length; i++) {
                        string _txt = buffer[i]; //Dữ liệu verify của từng bai test - toàn bộ (ko chia theo dòng)
                        string[] lines = _txt.Split('\n');
                        int _maxIndex = lines.Length - 2;

                        if (lines[_maxIndex].Contains("Phán định")) {
                            List<string> _li = new List<string>();
                            for (int k = _maxIndex; k >= 0; k--) {
                                if (lines[k].Contains("***")) break;
                                _li.Add(lines[k]);
                            }
                            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                            logreviewtx _log = new logreviewtx();
                            //Phan dinh
                            if (_li[0].Contains("Phán định"))
                                _log.Result = _li[0].Split(new string[] { "Phán định" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace("=", "").Trim();
                            //Center Frequency Error
                            string _tk = _log.Result == "PASS" ? _li[1] : _li[2];
                            if (_tk.Contains("Center Frequency Error"))
                                _log.centerFreqError = _tk.Split(new string[] { "Center Frequency Error" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace("=", "").Replace("Hz", "").Trim();
                            //EVM
                            _tk = _log.Result == "PASS" ? _li[2] : _li[3];
                            if (_tk.Contains("EVM All Carriers"))
                                _log.Evm = _tk.Split(new string[] { "EVM All Carriers" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace("=", "").Replace("dB", "").Trim();
                            //Average Power
                            _tk = _log.Result == "PASS" ? _li[3] : _li[4];
                            if (_tk.Contains("Average Power"))
                                _log.averagePower = _tk.Split(new string[] { "Average Power" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace("=", "").Replace("dBm", "").Trim();

                            string _tmp = _li[_li.Count - 1];
                            string[] _datas = _tmp.Split('-');
                            //Range frequency
                            _log.rangeFreq = _datas[0].Trim();
                            //Anten
                            _log.Anten = _datas[5].Trim();
                            //Channel
                            _log.Channel = _datas[6].Replace("Channel", "").Trim();
                            //Wifi
                            _log.wifiStandard = _datas[2].Replace("HT20","").Replace("HT40","").Replace("HT80","").Replace("HT160","").Trim();
                            //bandwidth
                            _log.Bandwidth = _datas[4].Replace("BW","").Trim();
                            //rate
                            _log.Rate = _datas[3].Trim();
                            GlobalData.reviewTX.Add(_log);
                            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                        }
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }

        bool _getlogSensivityRx_InputInto_Datagrid(string _logdata) {
            try {
                if (_logdata.Contains("Thời gian test độ nhạy thu") == true) {
                    GlobalData.reviewRX.Clear();
                    string[] buffer = _logdata.Split(new string[] { "Thời gian test độ nhạy thu" }, StringSplitOptions.None);

                    for (int i = 0; i < buffer.Length; i++) {
                        string _txt = buffer[i]; //Dữ liệu sensivity của từng bai test - toàn bộ (ko chia theo dòng)
                        string[] lines = _txt.Split('\n');
                        int _maxIndex = lines.Length - 2;

                        if (lines[_maxIndex].Contains("Phán định")) {
                            List<string> _li = new List<string>();
                            for (int k = _maxIndex; k >= 0; k--) {
                                if (lines[k].Contains("***")) break;
                                _li.Add(lines[k]);
                            }
                            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                            logreviewrx _log = new logreviewrx();
                            //Phan dinh
                            if (_li[0].Contains("Phán định"))
                                _log.Result = _li[0].Split(new string[] { "Phán định" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace("=", "").Trim();
                            //PER
                            if (_li[1].Contains("PER")) {
                                string _ttt = _li[1].Split(',')[0];
                                _log.Per = _ttt.Split(new string[] { "=" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace("=", "").Trim();
                            }  
                            //Transmit Power
                            if (_li[2].Contains("Cấu hình máy đo phát tín hiệu")) {
                                string _ttt = _li[2].Split(',')[0];
                                _log.transmitPower = _ttt.Split(new string[] { "Power" }, StringSplitOptions.None)[1].Replace("\r", "").Replace("\n", "").Replace("=", "").Replace("dBm", "").Trim();
                            }
                               
                            string _tmp = _li[_li.Count - 1];
                            string[] _datas = _tmp.Split('-');
                            //Range frequency
                            _log.rangeFreq = _datas[0].Trim();
                            //Anten
                            _log.Anten = _datas[5].Trim();
                            //Channel
                            _log.Channel = _datas[6].Replace("Channel", "").Trim();
                            //Wifi
                            _log.wifiStandard = _datas[2].Replace("HT20", "").Replace("HT40", "").Replace("HT80", "").Replace("HT160", "").Trim();
                            //bandwidth
                            _log.Bandwidth = _datas[4].Replace("BW", "").Trim();
                            //rate
                            _log.Rate = _datas[3].Trim();
                            GlobalData.reviewRX.Add(_log);
                            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                        }
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }

    }
}
