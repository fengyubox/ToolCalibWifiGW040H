using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function
{
    public class TestAnten
    {
        ModemTelnet _modem = null;
        Instrument _instrument = null;

        public TestAnten(ModemTelnet _mt, Instrument _it) {
            this._modem = _mt;
            this._instrument = _it;
        }

        public bool Excute() {
            bool _flag = true;

            //1. Verify Anten1
            if (GlobalData.initSetting.ENTESTANTEN1 == true) {
                GlobalData.testingData.TESTANTEN1RESULT = InitParameters.Statuses.Wait;
                bool ret = Verify_Anten1(GlobalData.testingData, _modem, _instrument);
                GlobalData.logManager.testAnten1Result = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.TESTANTEN1RESULT = GlobalData.logManager.testAnten1Result;
                if (!ret) {
                    _flag = false;
                    goto Finished;
                }
            }

            //2. Verify Anten2
            if (GlobalData.initSetting.ENTESTANTEN2 == true) {
                GlobalData.testingData.TESTANTEN2RESULT = InitParameters.Statuses.Wait;
                bool ret = Verify_Anten2(GlobalData.testingData, _modem, _instrument);
                GlobalData.logManager.testAnten2Result = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.TESTANTEN2RESULT = GlobalData.logManager.testAnten2Result;
                if (!ret) {
                    _flag = false;
                    goto Finished;
                }
            }

            //3. Hiển thị kết quả
            Finished:
            return _flag;
        }

        #region SubFunction

        string Name_measurement = GlobalData.initSetting.INSTRUMENT;
        string RF_Port = GlobalData.initSetting.RFPORT;

        private bool Verify_Signal(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, string standard_2G_5G, string Mode, string MCS, string BW, string Channel_Freq, string Anten, double Attenuator) {
            try {
                standard_2G_5G = int.Parse(Channel_Freq.Substring(0, 4)) > 3000 ? "5G" : "2G";
                string Result_Measure_temp = "";
                decimal Pwr_measure_temp, EVM_measure_temp, FreqErr_measure_temp;
                string _wifi = "";
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

                //Đọc giá trị tiêu chuẩn
                limittx _limit = null;
                LimitTx.getData(standard_2G_5G, FunctionSupport.Get_WifiStandard_By_Mode(Mode, BW), MCS, out _limit);

                //Thiết lập tần số máy đo
                instrument.config_Instrument_Channel(Channel_Freq);

                //Gửi lệnh yêu cầu ONT phát WIFI TX
                string _message = "";
                ModemTelnet.Verify_Signal_SendCommand(standard_2G_5G, Mode, MCS, BW, Channel_Freq, Anten, ref _message);
                //Hien_Thi.Hienthi.SetText(rtbAll, _message);

                //Đọc kết quả từ máy đo
                Result_Measure_temp = instrument.config_Instrument_get_TotalResult("RFB", _wifi);
                //Hien_Thi.Hienthi.SetText(rtbAll, Result_Measure_temp);

                //Lấy dữ liệu Power
                Pwr_measure_temp = Decimal.Parse(Result_Measure_temp.Split(',')[19], System.Globalization.NumberStyles.Float) + Convert.ToDecimal(Attenuator);
                if (Pwr_measure_temp < 15) {
                    instrument.config_Instrument_get_TotalResult("VID", _wifi);
                    Result_Measure_temp = instrument.config_Instrument_get_TotalResult("VID", _wifi);
                    //MessageBox.Show(Result_Measure_temp.ToString());
                    Pwr_measure_temp = Decimal.Parse(Result_Measure_temp.Split(',')[19], System.Globalization.NumberStyles.Float) + Convert.ToDecimal(Attenuator);
                }

                //Lấy dữ liệu EVM
                EVM_measure_temp = Decimal.Parse(Result_Measure_temp.Split(',')[1], System.Globalization.NumberStyles.Float);

                //Lấy dữ liệu Frequency Error
                FreqErr_measure_temp = Decimal.Parse(Result_Measure_temp.Split(',')[7], System.Globalization.NumberStyles.Float);

                //Hiển thị kết quả đo lên giao diện phần mềm (RichTextBox)
                _ti.LOGSYSTEM += "Average Power = " + Pwr_measure_temp.ToString("0.##") + " dBm\r\n";
                _ti.LOGSYSTEM += string.Format("EVM All Carriers = {0} {1}", EVM_measure_temp.ToString("0.##"), _wifi == "b" ? " %" : " dB\r\n");
                _ti.LOGSYSTEM += "Center Frequency Error = " + FreqErr_measure_temp.ToString("0.##") + " Hz\r\n";

                //So sánh kết quả đo với giá trị tiêu chuẩn
                bool _result = false, _powerOK = false, _evmOK = false, _freqerrOK = true;
                _limit.power_MAX = "25";
                _limit.power_MIN = Anten == "1" ? GlobalData.initSetting.STDPWANTEN1 : GlobalData.initSetting.STDPWANTEN2;
                _powerOK = FunctionSupport.Compare_TXMeasure_With_Standard(_limit.power_MAX, _limit.power_MIN, Pwr_measure_temp);
                _evmOK = FunctionSupport.Compare_TXMeasure_With_Standard(_limit.evm_MAX, _limit.evm_MIN, EVM_measure_temp);
                _freqerrOK = FunctionSupport.Compare_TXMeasure_With_Standard(_limit.freqError_MAX, _limit.freqError_MIN, FreqErr_measure_temp);

                if (_powerOK == false)
                    _ti.LOGSYSTEM += "FAIL: Power\r\n";
                else if (_evmOK == false)
                    _ti.LOGSYSTEM += "FAIL: EVM\r\n";
                else if (_freqerrOK == false)
                    _ti.LOGSYSTEM += "FAIL: Frequency Error\r\n";
                _result = _powerOK && _evmOK && _freqerrOK;

                return _result;
            }
            catch {
                return false;
            }
        }

        private bool AutoVerifySignal(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, int _anten, string _CarrierFreq) {
            List<verifysignal> list = null;
            _ti.LOGSYSTEM += "------------------------------------------------------------\r\n";
            _ti.LOGSYSTEM += string.Format("Bắt đầu thực hiện quá trình kiểm tra anten {0}.\r\n", _anten);
            list = _anten == 1 ? GlobalData.listTestAnten1 : GlobalData.listTestAnten2;

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

                _ti.LOGSYSTEM += "*************************************************************************\r\n";
                _ti.LOGSYSTEM += string.Format("{0} - {1} - {2} - MCS{3} - BW{4} - Anten {5} - Channel {6}\r\n", _CarrierFreq, RF_Port, FunctionSupport.Get_WifiStandard_By_Mode(item.wifi, item.bandwidth), item.rate, 20 * Math.Pow(2, double.Parse(item.bandwidth)), item.anten, _channelNo);
                int count = 0;
                REP:
                count++;
                if (!Verify_Signal(_ti, ModemTelnet, instrument, _CarrierFreq, item.wifi, item.rate, item.bandwidth, _eqChannel, item.anten, _attenuator)) {
                    if (count < 2) {
                        _ti.LOGSYSTEM += string.Format("RETRY = {0}\r\n", count);
                        goto REP;
                    }
                    else {
                        _ti.LOGSYSTEM += string.Format("Phán định = {0}", "FAIL\r\n");
                        result = false;
                    }

                }
                else _ti.LOGSYSTEM += string.Format("Phán định = {0}\r\n", "PASS");
                st.Stop();
                _ti.LOGSYSTEM += string.Format("Thời gian verify : {0} ms\r\n", st.ElapsedMilliseconds);
                _ti.LOGSYSTEM += "\r\n";
            }
            return result;
        }


        //OK
        private bool Verify_Anten1(testinginfo _ti, ModemTelnet _mt, Instrument _it) {
            return AutoVerifySignal(_ti, _mt, _it, 1, "2G");
        }

        //OK
        private bool Verify_Anten2(testinginfo _ti, ModemTelnet _mt, Instrument _it) {
            return AutoVerifySignal(_ti, _mt, _it, 2, "2G");
        }

        #endregion

    }
}
