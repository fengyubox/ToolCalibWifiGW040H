using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class CalculateMaster {

        ModemTelnet _modem = null;
        Instrument _instrument = null;
        formattinfo _formi = null;
        string Name_measurement = GlobalData.initSetting.INSTRUMENT;
        string RF_Port = GlobalData.initSetting.RFPORT;

        public CalculateMaster(ModemTelnet _mt, Instrument _it, formattinfo _fi) {
            this._modem = _mt;
            this._instrument = _it;
            this._formi = _fi;
        }

        public bool Excute(out string error) {
            error = "";
            try {
                //Do suy hao
                GlobalData.mtIndex = 0;
                GlobalData.mtIsOk = true;
                if (!Verify_Master(_formi, _modem, _instrument)) return false;
                //Attenuator.Save();
                return true;
            }
            catch (Exception ex) {
                error = ex.ToString();
                return false;
            }
        }

        private bool Verify_Signal(formattinfo _fi, ModemTelnet ModemTelnet, Instrument instrument, string Mode, string MCS, string BW, string Channel_Freq, string Anten) {
            try {
                string Result_Measure_temp = "";
                decimal Pwr_measure_temp = 0;
                string _wifi = "";
                string standard_2G_5G = int.Parse(Channel_Freq.Substring(0, 4)) < 3000 ? "2G" : "5G";

                switch (Mode) {
                    case "0": { _wifi = "b"; break; }
                    case "1": { _wifi = "g"; break; }
                    case "3": { _wifi = string.Format("n{0}", BW == "0" ? "20" : "40"); break; }
                    case "4": {
                            switch (BW) {
                                case "0": { _wifi = "ac20"; break; }
                                case "1": { _wifi = "ac40"; break; }
                                case "2": { _wifi = "ac80"; break; }
                                case "3": { _wifi = "ac160"; break; }
                            }
                            break;
                        }
                }

                string[] values = new string[5];
                
                for (int i = 0; i < 5; i++) {

                    RE:
                    //Thiết lập tần số máy đo
                    instrument.config_Instrument_Channel(Channel_Freq);


                    //Gửi lệnh yêu cầu ONT phát WIFI TX
                    string _message = "";
                    ModemTelnet.Verify_Signal_SendCommand(standard_2G_5G, Mode, MCS, BW, Channel_Freq, Anten, ref _message);

                    //Đọc kết quả từ máy đo
                    Result_Measure_temp = instrument.config_Instrument_get_TotalResult("RFB", _wifi);
                    //Hien_Thi.Hienthi.SetText(rtbAll, Result_Measure_temp);

                    //Lấy dữ liệu Power
                    try {
                        Pwr_measure_temp = Decimal.Parse(Result_Measure_temp.Split(',')[19], System.Globalization.NumberStyles.Float);
                        if (standard_2G_5G == "2G" && Pwr_measure_temp < 8) goto RE;
                    }
                    catch {
                        goto RE;
                    }

                    values[i] = Pwr_measure_temp.ToString();

                    //Hiển thị kết quả đo lên giao diện phần mềm (RichTextBox)
                    _fi.LOGDATA += "Average Power = " + Pwr_measure_temp.ToString("0.##") + " dBm\r\n";
                }

                App.Current.Dispatcher.BeginInvoke(new Action(() => {
                    foreach (var item in GlobalData.autoCalculateMaster) {
                        if (item.Anten == Anten && item.Frequency == Channel_Freq.Substring(0, 4)) {
                            item.Value1 = values[0];
                            item.Value2 = values[1];
                            item.Value3 = values[2];
                            item.Value4 = values[3];
                            item.Value5 = values[4];

                            double _diff1 = Math.Abs(double.Parse(item.Value1) - double.Parse(item.Value2));
                            double _diff2 = Math.Abs(double.Parse(item.Value1) - double.Parse(item.Value3));
                            double _diff3 = Math.Abs(double.Parse(item.Value1) - double.Parse(item.Value4));
                            double _diff4 = Math.Abs(double.Parse(item.Value1) - double.Parse(item.Value5));

                            if (_diff1 > 0.5 || _diff2 > 0.5 || _diff3 > 0.5 || _diff4 > 0.5) {
                                item.masterPower = "FAIL";
                                GlobalData.mtIsOk = false;
                            }
                            else {
                                double _avr = Math.Round((double.Parse(item.Value1) + double.Parse(item.Value2) + double.Parse(item.Value3) + double.Parse(item.Value4) + double.Parse(item.Value5)) / 5, 2);
                                item.masterPower = (_avr + double.Parse(item.wirePower)).ToString();
                            }
                            GlobalData.mtIndex++;
                            break;
                        }
                    }
                }));
                return true;
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }

        private bool AutoVerifySignal(formattinfo _fi, ModemTelnet ModemTelnet, Instrument instrument) {
            List<verifysignal> list = null;
            _fi.LOGDATA += "------------------------------------------------------------\r\n";
            _fi.LOGDATA += string.Format("Bắt đầu thực hiện quá trình đo master power.\r\n");
            list = GlobalData.listCalMaster;

            if (list.Count == 0) return false;
            bool result = true;
            string _oldwifi = "";
            foreach (var item in list) {
                Stopwatch st = new Stopwatch();
                st.Start();

                string _eqChannel = string.Format("{0}000000", item.channelfreq);
                double _attenuator = Attenuator.getAttenuator(item.channelfreq, item.anten);
                string _channelNo = Attenuator.getChannelNumber(item.channelfreq);
                string _wifi = "";
                switch (item.wifi) {
                    case "0": { _wifi = "b"; break; }
                    case "1": { _wifi = "g"; break; }
                    case "3": { _wifi = string.Format("n{0}", item.bandwidth == "0" ? "20" : "40"); break; }
                    case "4": {
                            switch (item.bandwidth) {
                                case "0": { _wifi = "ac20"; break; }
                                case "1": { _wifi = "ac40"; break; }
                                case "2": { _wifi = "ac80"; break; }
                                case "3": { _wifi = "ac160"; break; }
                            }
                            break;
                        }
                }
                if (_oldwifi != _wifi) {
                    instrument.config_Instrument_Total(RF_Port, _wifi);
                    _oldwifi = _wifi;
                }

                _fi.LOGDATA += "*************************************************************************\r\n";
                _fi.LOGDATA += string.Format("{0} - {1} - MCS{2} - BW{3} - Anten {4} - Channel {5}\r\n", RF_Port, FunctionSupport.Get_WifiStandard_By_Mode(item.wifi, item.bandwidth), item.rate, 20 * Math.Pow(2, double.Parse(item.bandwidth)), item.anten, _channelNo);
                int count = 0;
                REP:
                count++;
                if (!Verify_Signal(_fi, ModemTelnet, instrument, item.wifi, item.rate, item.bandwidth, _eqChannel, item.anten)) {
                    if (count < 2) {
                        _fi.LOGDATA += string.Format("RETRY = {0}\r\n", count);
                        goto REP;
                    }
                    else {
                        _fi.LOGDATA += string.Format("Phán định = {0}", "FAIL\r\n");
                        result = false;
                    }

                }
                else _fi.LOGDATA += string.Format("Phán định = {0}\r\n", "PASS");
                st.Stop();
                _fi.LOGDATA += string.Format("Thời gian đo : {0} ms\r\n", st.ElapsedMilliseconds);
                _fi.LOGDATA += "\r\n";

                if (GlobalData.mtIsOk == false) {
                    result = false;
                    break;
                }
            }

            //Save master data
            if (result == true) Master.Save();
            else
                System.Windows.MessageBox.Show("Mạch không đủ tiêu chuẩn làm Master.","Master", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return result;
        }


        private bool Verify_Master(formattinfo _fi, ModemTelnet _mt, Instrument _it) {
            return AutoVerifySignal(_fi, _mt, _it);
        }


    }
}
