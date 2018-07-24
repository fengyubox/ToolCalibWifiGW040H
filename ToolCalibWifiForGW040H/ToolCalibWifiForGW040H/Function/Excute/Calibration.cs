
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class Calibration {

        ModemTelnet _modem = null;
        Instrument _instrument = null;

        public Calibration(ModemTelnet _mt, Instrument _it) {
            this._modem = _mt;
            this._instrument = _it;
        }

        public bool Excute() {

            bool _flag = true;
            if (GlobalData.initSetting.ENCALIBPW2G || GlobalData.initSetting.ENCALIBPW5G) {
                GlobalData.logRegister = new logregister();
            }

            //1. Calib tần số
            if (GlobalData.initSetting.ENCALIBFREQ == true) {
                GlobalData.testingData.CALIBFREQRESULT = InitParameters.Statuses.Wait;
                int count = 0;
                REP:
                count++;
                bool ret = Calibrate_Freq(GlobalData.testingData, _modem, _instrument, count);
                if (ret == false && count < 3) goto REP;
                GlobalData.logManager.calibFreqResult = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.CALIBFREQRESULT = GlobalData.logManager.calibFreqResult;
                if (ret == false) {
                    _flag = false;
                    //goto Finished;
                }
            }

            //2. Calib công suất 2G
            if (GlobalData.initSetting.ENCALIBPW2G == true) {
                GlobalData.testingData.CALIBPW2GRESULT = InitParameters.Statuses.Wait;
                bool ret = Calibrate_Pwr_2G_Total(GlobalData.testingData, _modem, _instrument);
                GlobalData.logManager.calibPower2GResult = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.CALIBPW2GRESULT = GlobalData.logManager.calibPower2GResult;
                if (ret == false) {
                    _flag = false;
                    //goto Finished;
                }
            }

            //3. Calib công suất 5G 
            if (GlobalData.initSetting.ENCALIBPW5G == true) {
                GlobalData.testingData.CALIBPW5GRESULT = InitParameters.Statuses.Wait;
                bool ret = Calibrate_Pwr_5G_Total(GlobalData.testingData, _modem, _instrument);
                GlobalData.logManager.calibPower5GResult = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.CALIBPW5GRESULT = GlobalData.logManager.calibPower5GResult;
                if (ret == false) {
                    _flag = false;
                    //goto Finished;
                }
            }

            //4. Lưu dữ liệu vào flash
            if (GlobalData.initSetting.ENCALIBFREQ == true || GlobalData.initSetting.ENCALIBPW2G == true || GlobalData.initSetting.ENCALIBPW5G == true) Save_Flash(GlobalData.testingData, _modem);

            //5. Test độ nhạy thu 2G
            if (GlobalData.initSetting.ENTESTRX2G == true) {
                GlobalData.testingData.TESTRX2GRESULT = InitParameters.Statuses.Wait;
                bool ret = Test_Sensitivity_2G(GlobalData.testingData, _modem, _instrument);
                GlobalData.logManager.testSens2GResult = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.TESTRX2GRESULT = GlobalData.logManager.testSens2GResult;
                if (ret == false) {
                    _flag = false;
                    //goto Finished;
                }
            }

            //6. Test độ nhạy thu 5G
            if (GlobalData.initSetting.ENTESTRX5G == true) {
                GlobalData.testingData.TESTRX5GRESULT = InitParameters.Statuses.Wait;
                bool ret = Test_Sensitivity_5G(GlobalData.testingData, _modem, _instrument);
                GlobalData.logManager.testSens5GResult = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.TESTRX5GRESULT = GlobalData.logManager.testSens5GResult;
                if (!ret) {
                    _flag = false;
                    //goto Finished;
                }
            }

            //7. Verify công suất phát 2G
            if (GlobalData.initSetting.ENTESTTX2G == true) {
                GlobalData.testingData.TESTTX2GRESULT = InitParameters.Statuses.Wait;
                bool ret = Verify_2G(GlobalData.testingData, _modem, _instrument);
                GlobalData.logManager.verify2GResult = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.TESTTX2GRESULT = GlobalData.logManager.verify2GResult;
                if (!ret) {
                    _flag = false;
                    //goto Finished;
                }
            }

            //8. Verify công suất phát 5G
            if (GlobalData.initSetting.ENTESTTX5G == true) {
                GlobalData.testingData.TESTTX5GRESULT = InitParameters.Statuses.Wait;
                bool ret = Verify_5G(GlobalData.testingData, _modem, _instrument);
                GlobalData.logManager.verify5GResult = ret == true ? "PASS" : "FAIL";
                GlobalData.testingData.TESTTX5GRESULT = GlobalData.logManager.verify5GResult;
                if (!ret) {
                    _flag = false;
                    goto Finished;
                }
            }

            //9. Hiển thị kết quả
            Finished:
            return _flag;
        }

        #region SubFunction

        string Name_measurement = GlobalData.initSetting.INSTRUMENT;
        string RF_Port = GlobalData.initSetting.RFPORT;
        double Target_Pwr_2G = double.Parse(GlobalData.initSetting.TARGETPOWER2G);
        double Target_Pwr_5G = double.Parse(GlobalData.initSetting.TARGETPOWER5G);

        double Power_Measure;
        double Power_diferent;

        /// <summary>
        /// GHI DỮ LIỆU VÀO FLASH ONT GW040H
        /// </summary>
        /// <param name="_ti"></param>
        /// <param name="ModemTelnet"></param>
        /// <returns></returns>
        bool Save_Flash(testinginfo _ti, ModemTelnet ModemTelnet) {
            try {
                Stopwatch st = new Stopwatch();
                st.Start();
                //write defaut bin
                if (GlobalData.initSetting.ENWRITEBIN == true) {
                    _ti.LOGSYSTEM += "------------------------------------------------------------\r\n";
                    _ti.LOGSYSTEM += "Thực hiện write giá trị file BIN.\r\n";
                    if (GlobalData.ListBinRegister.Count > 0) {
                        foreach (var item in GlobalData.ListBinRegister) {
                            ModemTelnet.Write_Register(item.Address, item.newValue);
                            Thread.Sleep(100);
                            _ti.LOGSYSTEM += string.Format("Write Register: {0} = {1}\r\n", item.Address, item.newValue);
                        }
                    }
                }
                //else {
                //    _ti.LOGSYSTEM += "------------------------------------------------------------\r\n";
                //    _ti.LOGSYSTEM += "Thực hiện reset giá trị file BIN.\r\n";
                //    if (GlobalData.oldListBinRegister.Count > 0) {
                //        foreach (var item in GlobalData.oldListBinRegister) {
                //            ModemTelnet.Write_Register(item.Address, item.Value);
                //            Thread.Sleep(100);
                //            _ti.LOGSYSTEM += string.Format("Write Register Back Value: {0} = {1}\r\n", item.Address, item.Value);
                //        }
                //    }
                //}

                //save flash
                _ti.LOGSYSTEM += "------------------------------------------------------------\r\n";
                _ti.LOGSYSTEM += "Thực hiện lưu vào FLASH.\r\n";
                ModemTelnet.Write_into_Flash();
                st.Stop();
                _ti.LOGSYSTEM += string.Format("Thời gian lưu vào FLASH : {0} ms\r\n", st.ElapsedMilliseconds);
                return true;
            }
            catch {
                return false;
            }
        }


        /// <summary>
        /// CALIB TẦN SỐ *********************************************
        /// 1. Calibrate_Freq -------------//Calib tần số
        /// 2. Calculate_NewValue ---------//
        /// 
        /// </summary>
        #region CALIB FREQUENCY

        //OK
        private bool Calibrate_Freq(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, int retry) {
            Stopwatch st = new Stopwatch();
            st.Start();
            _ti.LOGSYSTEM += "------------------------------------------------------------\r\n";
            string F4F6 = "";
            string F4 = "";
            string F4_6_0 = "";
            decimal F4_6_0_DEC;
            string F6 = "";
            string F6_5_0 = "";
            decimal F6_5_0_DEC;
            decimal FreOffset_new;
            string Current_Freq_Offset = "";
            string Freq_Err = "";
            string F7 = "";
            decimal F6_5_0_new_DEC;

            //if (!Connect_Function()) {
            //    Hien_Thi.Hienthi.SetText(rtb, "Không thực hiện được quá trình Calib tần số.");
            //    return false;
            //    //return "STOP";
            //}
            //else {
            try {
                _ti.LOGSYSTEM += "Bắt đầu quá trình Calib tần số." + "\r\n";
                _ti.LOGSYSTEM += "Đang đọc thanh ghi F4,F6..." + "\r\n";
                ModemTelnet.WriteLine("iwpriv ra0 e2p F4");
                Thread.Sleep(50);
                ModemTelnet.WriteLine("iwpriv ra0 e2p F6");
                Thread.Sleep(50);
                F4F6 = ModemTelnet.Read();
                for (int i = 0; i < F4F6.Split('\n').Length; i++) {
                    if (F4F6.Split('\n')[i].Contains("[0x00F4]")) {
                        F4 = F4F6.Split('\n')[i].Split(':')[1].Trim();
                    }

                    if (F4F6.Split('\n')[i].Contains("[0x00F6]")) {
                        F6 = F4F6.Split('\n')[i].Split(':')[1].Trim();
                    }
                }

                _ti.LOGSYSTEM += "0x00F4 = " + F4 + "; " + "0x00F6 = " + F6 + "\r\n";
                F4_6_0 = FunctionSupport.HextoBin(F4.Substring(4, 2)).Substring(1, 7);
                //MessageBox.Show(F4_6_0);
                F4_6_0_DEC = Convert.ToInt64(F4_6_0, 2);
                F6_5_0 = FunctionSupport.HextoBin(F6.Substring(4, 2)).Substring(2, 6);
                F6_5_0_DEC = Convert.ToInt64(F6_5_0, 2);
                _ti.LOGSYSTEM += "F4[6:0] = " + F4_6_0 + "; " + "F6[5:0] = " + F6_5_0 + "\r\n";
                _ti.LOGSYSTEM += "F4[6:0]_DEC = " + F4_6_0_DEC + "; " + "F6[5:0]_DEC = " + F6_5_0_DEC + "\r\n";
                Current_Freq_Offset = FunctionSupport.Plus_F4_With_F6(F4, F6); //Tính giá trị FreqOffset ban đầu
                _ti.LOGSYSTEM += "Giá trị Frequency Offset hiện tại = F4[6,0] +/- F6[5,0] = " + Current_Freq_Offset + "\r\n";

                //Gửi lệnh phát tín hiệu kèm Frequency Offset
                _ti.LOGSYSTEM += "Gửi lệnh phát tín hiệu kèm Current_Freq_Offset: " + Current_Freq_Offset + "\r\n";
                _ti.LOGSYSTEM += string.Format("Đang cấu hình lần đầu cho máy đo {0}...", Name_measurement) + "\r\n";

                bool _configInstrIsOk = false;
                int _index = 0;
                while (_configInstrIsOk == false) {
                    string _error = "";
                    _configInstrIsOk = instrument.config_Instrument_Total(RF_Port, "g", ref _error);
                    if (_error != "") _ti.LOGSYSTEM += string.Format("{0}\r\n", _error);
                    _error = "";
                    _configInstrIsOk = instrument.config_Instrument_Channel("2437000000", ref _error);
                    if (_error != "") _ti.LOGSYSTEM += string.Format("{0}\r\n", _error);
                    _index++;
                    if (_index > 20) break;
                }
                if (_configInstrIsOk == false) return false;

                _ti.LOGSYSTEM += string.Format("Đang phát tín hiệu ở Anten 1 - Channel 6 - Máy đo {0} - Offset = {1}", Name_measurement, Current_Freq_Offset) + "\r\n";
                ModemTelnet.CalibFrequency_SendCommand("1", "7", "0", "6", "1", Current_Freq_Offset); //(mode,rate,bw,channel,anten,freqOffset)
                                                                                                      //Thread.Sleep(500);
                                                                                                      //for (int i = 0; i < 2; i++) {
                Freq_Err = instrument.config_Instrument_get_FreqErr("RFB", "g"); //Lệnh đọc giá trị về từ máy đo
                                                                                 //MessageBox.Show(Freq_Err);
                _ti.LOGSYSTEM += "..." + "\r\n";
                _ti.LOGSYSTEM += "Lấy kết quả đo lần thứ: " + retry.ToString() + "\r\n";
                if (!Freq_Err.Contains("999")) {
                    if (Convert.ToDouble(Freq_Err) > -2000 && Convert.ToDouble(Freq_Err) < 2000) {
                        _ti.LOGSYSTEM += "Frequency Err = " + Freq_Err + "\r\n";
                        _ti.LOGSYSTEM += "Frequency Error đã đạt Target." + "\r\n";
                        _ti.LOGSYSTEM += "---------------------------------" + "\r\n";
                        FreOffset_new = Decimal.Parse(Current_Freq_Offset);
                        //Result_FreqErr_Calib = "PASS";
                        //break;
                    }
                    else {
                        _ti.LOGSYSTEM += "Giá trị Frequency Error = " + Freq_Err + "\r\n";
                        if (Convert.ToDouble(Freq_Err) < 0)
                            _ti.LOGSYSTEM += "Frequency Error < 0 -> Cần giảm Frequency Offset" + "\r\n";
                        else
                            _ti.LOGSYSTEM += "Frequency Error > 0 -> Cần tăng Frequency Offset" + "\r\n";

                        _ti.LOGSYSTEM += "Mỗi 1500 Khz bị lệch tương ứng với 1 giá trị Decimal => Giá trị DEC mà Current_Freq_Offset và F6[5,0] cần thay đổi = Freq_Err / 1500 = " + Math.Round((Decimal.Parse(Freq_Err)) / 1500) + "\r\n"; //2350                  
                        FreOffset_new = Math.Round(Decimal.Parse(Current_Freq_Offset) + Math.Round(Decimal.Parse(Freq_Err) / 1500)); //2350
                        _ti.LOGSYSTEM += "Freq_Offset_new = Current_Freq_Offset + Freq_Err/1500 = " + FreOffset_new + "\r\n";
                        string F6_DEC = FunctionSupport.HextoDec(F6.Substring(4, 2));
                        string F6_toBin = "";
                        if (Int32.Parse(F6_DEC) == 0) {
                            F6_toBin = "00000000";
                        }
                        else {
                            F6_toBin = FunctionSupport.DECtoBin(Int32.Parse(F6_DEC));
                        }
                        string F6_5_0_old = F6_toBin.Substring(2);
                        Decimal F6_5_0_old_toDEC = Convert.ToInt32(F6_5_0_old, 2);
                        _ti.LOGSYSTEM += "F6[5,0] cũ ở dạng DEC = " + F6_5_0_old_toDEC + "\r\n";

                        //F6_5_0_new_DEC = FreOffset_new - F4_6_0_DEC;
                        if (FreOffset_new > F4_6_0_DEC) {
                            F6_5_0_new_DEC = FreOffset_new - F4_6_0_DEC;
                        }
                        else {
                            F6_5_0_new_DEC = F4_6_0_DEC - FreOffset_new;
                        }

                        //F6_5_0_new_DEC = Math.Round(F6_5_0_old_toDEC - (Decimal.Parse(Freq_Err) / 1500)); //2350
                        _ti.LOGSYSTEM += "F6[5,0] mới ở dạng DEC = " + F6_5_0_new_DEC + "\r\n";
                        string F6_full_new_BIN = "";
                        F7 = F6.Substring(2, 2);

                        if (FreOffset_new < Convert.ToDecimal(Current_Freq_Offset)) {
                            F6_full_new_BIN = FunctionSupport.KiemtraF6("Can Giam", Int32.Parse(F6_5_0_new_DEC.ToString()));
                            _ti.LOGSYSTEM += "F6_Full_Bin_New = " + F6_full_new_BIN + "\r\n";
                            _ti.LOGSYSTEM += "Giá trị F6 mới cần truyền: " + FunctionSupport.BintoHexOnly(F6_full_new_BIN) + "\r\n"; ;

                            _ti.LOGSYSTEM += "iwpriv ra0 e2p F6=" + F7 + FunctionSupport.BintoHexOnly(F6_full_new_BIN) + "\r\n";
                            ModemTelnet.WriteLine("iwpriv ra0 e2p F6=" + F7 + FunctionSupport.BintoHexOnly(F6_full_new_BIN));
                            //break;
                        }
                        else if (FreOffset_new > Convert.ToDecimal(Current_Freq_Offset)) {
                            F6_full_new_BIN = FunctionSupport.KiemtraF6("Can Tang", Int32.Parse(F6_5_0_new_DEC.ToString()));
                            _ti.LOGSYSTEM += "F6_Full_Bin_New = " + F6_full_new_BIN + "\r\n";
                            _ti.LOGSYSTEM += "Giá trị F6 mới cần truyền: " + FunctionSupport.BintoHexOnly(F6_full_new_BIN) + "\r\n";

                            _ti.LOGSYSTEM += "iwpriv ra0 e2p F6=" + F7 + FunctionSupport.BintoHexOnly(F6_full_new_BIN) + "\r\n";
                            ModemTelnet.WriteLine("iwpriv ra0 e2p F6=" + F7 + FunctionSupport.BintoHexOnly(F6_full_new_BIN));
                            //break;
                        }
                        else {
                            ModemTelnet.WriteLine("iwpriv ra0 e2p F6=" + F6);
                            //break;
                        }
                        _ti.LOGSYSTEM += "-------------------------------------------" + "\r\n";
                        //_ti.LOGSYSTEM += "Thực hiện Write to Flash.");
                        //Save_Flash();

                        _ti.LOGSYSTEM += "-------------------------------------------" + "\r\n";
                        _ti.LOGSYSTEM += "Bắt đầu thực hiện Verify Frequency Error." + "\r\n";
                        _ti.LOGSYSTEM += string.Format("Đang phát tín hiệu ở Anten 1 - Channel 6") + "\r\n";
                        ModemTelnet.CalibFrequency_SendCommand("1", "7", "0", "6", "1", FreOffset_new.ToString()); //(mode,rate,bw,channel,anten,freqOffset)
                        Freq_Err = instrument.config_Instrument_get_FreqErr("RFB", "g"); //Lệnh đọc giá trị về từ máy đo
                        _ti.LOGSYSTEM += "Frequency Error = " + Freq_Err + "\r\n";
                        //break;
                    }
                }
                else {
                    //continue;
                }
                //}
                st.Stop();
                _ti.LOGSYSTEM += string.Format("Thời gian calib Freq : {0} ms\r\n", st.ElapsedMilliseconds);
                return Math.Abs(Convert.ToDouble(Freq_Err)) < 2000;
            }
            catch {
                st.Stop();
                _ti.LOGSYSTEM += string.Format("Thời gian calib Freq : {0} ms\r\n", st.ElapsedMilliseconds);
                return false;
                //MessageBox.Show(Ex.ToString());
                //return "STOP";
            }
            //}
        }

        private bool Calculate_NewValue(string _old, out string _new) {
            _new = "";
            try {
                string Chac1_Old_temp = "";
                string Chac2_Old_temp = "";

                double Value_New = 0;
                string Chac1_New_temp = "";
                string Chac2_New_temp = "";

                Chac1_Old_temp = _old.Substring(2, 2).Substring(0, 1);
                Chac2_Old_temp = _old.Substring(2, 2).Substring(1, 1);
                //MessageBox.Show("AAA");
                switch (Chac2_Old_temp) {
                    case "A": { Chac2_Old_temp = "10"; }
                        break;
                    case "B": { Chac2_Old_temp = "11"; }
                        break;
                    case "C": { Chac2_Old_temp = "12"; }
                        break;
                    case "D": { Chac2_Old_temp = "13"; }
                        break;
                    case "E": { Chac2_Old_temp = "14"; }
                        break;
                    case "F": { Chac2_Old_temp = "15"; }
                        break;
                }

                if (Chac1_Old_temp == "C") {
                    Value_New = Convert.ToDouble(Chac2_Old_temp) / 2 - Power_diferent;
                }
                else if (Chac1_Old_temp == "8") {
                    Value_New = Convert.ToDouble(Chac2_Old_temp) / (-2) - Power_diferent;
                }

                if (Value_New > 0) {
                    Chac1_New_temp = "C";
                    Chac2_New_temp = (Value_New * 2).ToString();

                    if (Convert.ToDouble(Chac2_New_temp) < 10) { }
                    else if (Convert.ToDouble(Chac2_New_temp) > 9 && Convert.ToDouble(Chac2_New_temp) < 16) {
                        switch (Convert.ToDouble(Chac2_New_temp).ToString()) {
                            case "10": { Chac2_New_temp = "A"; }
                                break;
                            case "11": { Chac2_New_temp = "B"; }
                                break;
                            case "12": { Chac2_New_temp = "C"; }
                                break;
                            case "13": { Chac2_New_temp = "D"; }
                                break;
                            case "14": { Chac2_New_temp = "E"; }
                                break;
                            case "15": { Chac2_New_temp = "F"; }
                                break;
                        }
                    }

                    else if (Convert.ToDouble(Chac2_New_temp) > 15) {
                        Chac2_New_temp = "ERROR";
                    }
                }
                else {
                    Chac1_New_temp = "8";
                    Chac2_New_temp = (Value_New * -2).ToString();

                    if (Convert.ToDouble(Chac2_New_temp) < 10) { }
                    else if (Convert.ToDouble(Chac2_New_temp) > 9 && Convert.ToDouble(Chac2_New_temp) < 16) {
                        switch (Convert.ToDouble(Chac2_New_temp).ToString()) {
                            case "10": { Chac2_New_temp = "A"; }
                                break;
                            case "11": { Chac2_New_temp = "B"; }
                                break;
                            case "12": { Chac2_New_temp = "C"; }
                                break;
                            case "13": { Chac2_New_temp = "D"; }
                                break;
                            case "14": { Chac2_New_temp = "E"; }
                                break;
                            case "15": { Chac2_New_temp = "F"; }
                                break;
                        }
                    }

                    else if (Convert.ToDouble(Chac2_New_temp) > 15) {
                        Chac2_New_temp = "ERROR";
                    }
                }
                _new = _old.Substring(0, 2) + Chac1_New_temp + Chac2_New_temp;
                return true;
            }
            catch {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// CALIB CÔNG SUẤT *******************************************
        /// 1. Calibrate_Pwr_Detail --------//Core
        /// 2. AutoCalibPower --------------//Hỗ trợ tự động test nhiều
        /// 3. Calibrate_Pwr_2G_Total ------//
        /// 4. Calibrate_Pwr_5G_Total ------//
        /// 
        /// </summary>
        #region CALIB POWER

        private bool Calibrate_Pwr_Detail(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, string Standard_2G_or_5G, string RFinput, string Anten, string Channel_Freq, string Register, double Attenuator) {
            string Register_Old_Value_Pwr = "", Register_New_Value_Pwr = "";
            bool _flag = true;
            try {
                Register_Old_Value_Pwr = ModemTelnet.Read_Register(Register.Split('x')[1]);
                if (Register_Old_Value_Pwr.Contains(Register.Split('x')[1])) {
                    Register_Old_Value_Pwr = ModemTelnet.Read_Register(Register.Split('x')[1]);
                    return false;
                }
                else {
                    _ti.LOGSYSTEM += "Giá trị thanh ghi " + Register + " hiện tại: " + Register_Old_Value_Pwr + "\r\n";
                    ModemTelnet.CalibPower_SendCommand(Standard_2G_or_5G, Anten, Channel_Freq);
                    string _error = "";
                    instrument.config_Instrument_Channel(Channel_Freq, ref _error);
                    ModemTelnet.Read_Register(Register.Split('x')[1]);

                    for (int i = 0; i < 2; i++) {
                        if (i == 0) {
                            if (Standard_2G_or_5G == "2G") {
                                instrument.config_Instrument_get_Power("RFB", "g");
                                Power_Measure = FunctionSupport.RoundDecimal(Convert.ToDouble(instrument.config_Instrument_get_Power("RFB", "g")) + Attenuator);
                                if (Convert.ToDouble(Power_Measure) < 15) {
                                    Power_Measure = FunctionSupport.RoundDecimal(Convert.ToDouble(instrument.config_Instrument_get_Power("RFB", "g")) + Attenuator);
                                }
                                _ti.LOGSYSTEM += "Công suất đo được: " + Power_Measure + " (dBm)" + "\r\n";
                            }
                            else if (Standard_2G_or_5G == "5G") {
                                instrument.config_Instrument_get_Power("VID", "g");
                                Power_Measure = FunctionSupport.RoundDecimal(Convert.ToDouble(instrument.config_Instrument_get_Power("VID", "g")) + Attenuator);
                                if (Convert.ToDouble(Power_Measure) < 15) {
                                    Power_Measure = FunctionSupport.RoundDecimal(Convert.ToDouble(instrument.config_Instrument_get_Power("VID", "g")) + Attenuator);
                                }
                                _ti.LOGSYSTEM += "Công suất đo được: " + Power_Measure + " (dBm)" + "\r\n";
                            }
                        }
                        if (i == 1) {
                            if (Standard_2G_or_5G == "2G") {
                                instrument.config_Instrument_get_Power("VID", "g");
                                Power_Measure = FunctionSupport.RoundDecimal(Convert.ToDouble(instrument.config_Instrument_get_Power("VID", "g")) + Attenuator);
                                if (Convert.ToDouble(Power_Measure) < 15) {
                                    Power_Measure = FunctionSupport.RoundDecimal(Convert.ToDouble(instrument.config_Instrument_get_Power("VID", "g")) + Attenuator);
                                }
                                _ti.LOGSYSTEM += "Công suất đo được: " + Power_Measure + " (dBm)" + "\r\n";
                            }
                            else if (Standard_2G_or_5G == "5G") {
                                instrument.config_Instrument_get_Power("RFB", "g");
                                Power_Measure = FunctionSupport.RoundDecimal(Convert.ToDouble(instrument.config_Instrument_get_Power("RFB", "g")) + Attenuator);
                                if (Convert.ToDouble(Power_Measure) < 15) {
                                    Power_Measure = FunctionSupport.RoundDecimal(Convert.ToDouble(instrument.config_Instrument_get_Power("RFB", "g")) + Attenuator);
                                }
                                _ti.LOGSYSTEM += "Công suất đo được: " + Power_Measure + " (dBm)" + "\r\n";
                            }
                        }

                        if (Standard_2G_or_5G == "2G") {
                            Power_diferent = Power_Measure - Convert.ToDouble(Target_Pwr_2G);
                        }
                        else if (Standard_2G_or_5G == "5G") {
                            Power_diferent = Power_Measure - Convert.ToDouble(Target_Pwr_5G);
                        }

                        _ti.LOGSYSTEM += "Độ lệch công suất: " + Power_diferent + " (dBm)" + "\r\n";

                        if (Power_diferent < 10) {
                            if (Power_diferent == 0) goto END;
                            Calculate_NewValue(Register_Old_Value_Pwr, out Register_New_Value_Pwr);

                            if (Register_New_Value_Pwr.Contains("ERROR")) {
                                _ti.LOGSYSTEM += "[FAIL] Bắt đầu thực hiện lại.\r\n";
                                _flag = false;
                                continue;
                            }
                            else {
                                _ti.LOGSYSTEM += "Giá trị cần truyền: " + Register_New_Value_Pwr + "\r\n";
                                ModemTelnet.Write_Register(Register.Split('x')[1], Register_New_Value_Pwr);
                                _flag = true;
                                break;
                            }
                        }
                        else {
                            //MessageBox.Show("Lỗi đọc Power.");
                            _flag = false;
                        }
                    }
                }
                END:
                var propInfo = GlobalData.logRegister.GetType().GetProperty(string.Format("_{0}", Register));
                if (propInfo != null) {
                    propInfo.SetValue(GlobalData.logRegister, Register_New_Value_Pwr == "" ? Register_Old_Value_Pwr.Substring(2, 2) : Register_New_Value_Pwr.Substring(2, 2), null);
                }
                return _flag;
            }
            catch {
                return false;
            }
        }

        private bool AutoCalibPower(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, string _CarrierFreq) {
            try {
                List<calibpower> list = null;
                _ti.LOGSYSTEM += "------------------------------------------------------------\r\n";
                _ti.LOGSYSTEM += string.Format("Bắt đầu thực hiện quá trình Calib Công Suất {0}.\r\n", _CarrierFreq);
                list = _CarrierFreq == "2G" ? GlobalData.listCalibPower2G : GlobalData.listCalibPower5G;
                string _error = "";
                bool _result = true;

                instrument.config_Instrument_Total(RF_Port, "g", ref _error);

                if (list.Count == 0) return true;
                foreach (var item in list) {
                    Stopwatch st = new Stopwatch();
                    st.Start();
                    string _eqChannel = string.Format("{0}000000", item.channelfreq);
                    double _attenuator = Attenuator.getAttenuator(item.channelfreq, item.anten);
                    string _channelNo = Attenuator.getChannelNumber(item.channelfreq);

                    _ti.LOGSYSTEM += "*************************************************************************\r\n";
                    _ti.LOGSYSTEM += string.Format("{0} - {1} - 802.11g - MCS7 - BW20 - Anten {2} - Channel {3} - {4}\r\n", _CarrierFreq, RF_Port, item.anten, _channelNo, item.register);
                    int count = 0;
                    REP:
                    count++;
                    if (!Calibrate_Pwr_Detail(_ti, ModemTelnet, instrument, _CarrierFreq, RF_Port, item.anten, _eqChannel, item.register, _attenuator)) {
                        if (count < 3) goto REP;
                        _result = false;
                    }
                    st.Stop();
                    _ti.LOGSYSTEM += string.Format("Thời gian calib : {0} ms\r\n", st.ElapsedMilliseconds);
                    _ti.LOGSYSTEM += "\r\n";
                }
                return _result;
            }
            catch {
                return false;
            }
        }

        private bool Calibrate_Pwr_2G_Total(testinginfo _ti, ModemTelnet _mt, Instrument _it) {

            return AutoCalibPower(_ti, _mt, _it, "2G");
        }

        private bool Calibrate_Pwr_5G_Total(testinginfo _ti, ModemTelnet _mt, Instrument _it) {

            return AutoCalibPower(_ti, _mt, _it, "5G");
        }

        #endregion

        /// <summary>
        /// XÁC NHẬN WIFI-TX *******************************************
        /// 1. Verify_Signal ----------------//Core
        /// 2. AutoVerifySignal -------------//Hỗ trợ tự động test nhiều
        /// 3. Verify_2G --------------------//
        /// 4. Verify_5G --------------------//
        /// </summary>
        #region VERIFY TX

        //OK
        private bool Verify_Signal(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, string standard_2G_5G, string Mode, string MCS, string BW, string Channel_Freq, string Anten, double Attenuator, ref string _pw, ref string _evm, ref string _freqerr, ref string _pstd, ref string _evmmax) {
            try {
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
                string _error = "";
                instrument.config_Instrument_Channel(Channel_Freq, ref _error);

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
                _pw = Pwr_measure_temp.ToString("0.##");
                _evm = EVM_measure_temp.ToString("0.##");
                _freqerr = FreqErr_measure_temp.ToString("0.##");
                _pstd = string.Format("{0}~{1}", _limit.power_MIN, _limit.power_MAX);
                _evmmax = string.Format("{0}", _limit.evm_MAX);
                _ti.LOGSYSTEM += "Power Limit = " + _pstd + " dBm, Average Power = " + _pw + " dBm\r\n";
                _ti.LOGSYSTEM += string.Format("EVM MAX = {0} {2}, EVM All Carriers = {1} {2}\r\n", _evmmax, _evm, _wifi == "b" ? " %" : " dB");
                _ti.LOGSYSTEM += "Center Frequency Error = " + _freqerr + " Hz\r\n";

                //So sánh kết quả đo với giá trị tiêu chuẩn
                bool _result = false, _powerOK = false, _evmOK = false, _freqerrOK = true;
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

        private bool AutoVerifySignal(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, string _CarrierFreq) {
            List<verifysignal> list = null;
            _ti.LOGSYSTEM += "------------------------------------------------------------\r\n";
            _ti.LOGSYSTEM += string.Format("Bắt đầu thực hiện quá trình Verify tín hiệu {0}.\r\n", _CarrierFreq);
            list = _CarrierFreq == "2G" ? GlobalData.listVerifySignal2G : GlobalData.listVerifySignal5G;

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
                    string _error = "";
                    instrument.config_Instrument_Total(RF_Port, _wifi, ref _error);
                    _oldwifi = _wifi;
                }

                _ti.LOGSYSTEM += "*************************************************************************\r\n";
                _ti.LOGSYSTEM += string.Format("{0} - {1} - {2} - MCS{3} - BW{4} - Anten {5} - Channel {6}\r\n", _CarrierFreq, RF_Port, FunctionSupport.Get_WifiStandard_By_Mode(item.wifi, item.bandwidth), item.rate, 20 * Math.Pow(2, double.Parse(item.bandwidth)), item.anten, _channelNo);
                int count = 0;
                string _Power = "", _Evm = "", _FreqErr = "", _pStd = "", _eMax = "";
                bool _kq = true;
                REP:
                count++;
                if (!Verify_Signal(_ti, ModemTelnet, instrument, _CarrierFreq, item.wifi, item.rate, item.bandwidth, _eqChannel, item.anten, _attenuator, ref _Power, ref _Evm, ref _FreqErr, ref _pStd, ref _eMax)) {
                    if (count < 2) {
                        _ti.LOGSYSTEM += string.Format("RETRY = {0}\r\n", count);
                        _kq = false;
                        goto REP;
                    }
                    else {
                        _ti.LOGSYSTEM += string.Format("Phán định = {0}", "FAIL\r\n");
                        _kq = false;
                        result = false;
                    }

                }
                else {
                    _ti.LOGSYSTEM += string.Format("Phán định = {0}\r\n", "PASS");
                    _kq = true;
                }
                App.Current.Dispatcher.Invoke(new Action(() => {
                    string _w = "", _bw = "";
                    switch (item.wifi) {
                        case "0": { _w = "802.11b"; break; }
                        case "1": { _w = "802.11g"; break; }
                        case "2": { _w = "802.11a"; break; }
                        case "3": { _w = "802.11n"; break; }
                        case "4": { _w = "802.11ac"; break; }
                    }
                    switch (item.bandwidth) {
                        case "0": { _bw = "20"; break; }
                        case "1": { _bw = "40"; break; }
                        case "2": { _bw = "80"; break; }
                        case "3": { _bw = "160"; break; }

                    }
                    GlobalData.datagridlogTX.Add(new logreviewtx() { rangeFreq = _CarrierFreq, Anten = item.anten, wifiStandard = _w, Rate = "MCS" + item.rate, Bandwidth = _bw, Channel = _channelNo, Result = _kq == true ? "PASS" : "FAIL", averagePower = _Power, centerFreqError = _FreqErr, Evm = _Evm, powerStd = _pStd, evmMAX = _eMax });
                }));
                st.Stop();
                _ti.LOGSYSTEM += string.Format("Thời gian verify : {0} ms\r\n", st.ElapsedMilliseconds);
                _ti.LOGSYSTEM += "\r\n";
            }
            return result;
        }

        //OK
        private bool Verify_2G(testinginfo _ti, ModemTelnet _mt, Instrument _it) {
            return AutoVerifySignal(_ti, _mt, _it, "2G");
        }

        //OK
        private bool Verify_5G(testinginfo _ti, ModemTelnet _mt, Instrument _it) {
            return AutoVerifySignal(_ti, _mt, _it, "5G");
        }

        #endregion

        /// <summary>
        /// XÁC NHẬN WIFI-RX *******************************************
        /// 1. Test_Sensivitity_Detail -------//Core
        /// 2. AutoTestSensivitity -----------//Hỗ trợ tự động test nhiều
        /// 3. Test_Sensitivity_2G -----------//
        /// 4. Test_Sensitivity_5G -----------//
        /// 
        /// </summary>
        #region VERIFY RX

        private bool Test_Sensivitity_Detail(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, string standard_2G_5G, string Mode, string MCS, string BW, string Channel_Freq, string Anten, int packet) {
            try {

                string wave_form_name = "";
                string _channelNo = Attenuator.getChannelNumber(Channel_Freq);
                double _attenuator = Attenuator.getAttenuator(Channel_Freq, Anten);
                double power_transmit = -1000;
                double stPER = 0.0;
                double PER = 0.0;
                int RXCounter = 0;

                //Đọc giá trị PER, POWER TRANSMIT
                limitrx _limit = null;
                LimitRx.getData(standard_2G_5G, FunctionSupport.Get_WifiStandard_By_Mode(Mode, BW), MCS, out _limit);
                power_transmit = _limit.power_Transmit.Trim() == "-" ? -1000 : double.Parse(_limit.power_Transmit);
                stPER = _limit.PER.Trim() == "-" ? 0 : double.Parse(_limit.PER.Trim().Replace("%", ""));

                //Lấy tên file wave form
                WaveForm.getData(Name_measurement, Mode, MCS, BW, out wave_form_name);

                //Cấu hình ONT về chế độ WIFI RX
                _ti.LOGSYSTEM += "Cấu hình ONT...\r\n";
                string _message = "";
                ModemTelnet.TestSensitivity_SendCommand(standard_2G_5G, Mode, MCS, BW, _channelNo, Anten, ref _message);
                //Hien_Thi.Hienthi.SetText(rtbAll, _message);

                //Điều khiển máy đo phát gói tin
                _ti.LOGSYSTEM += string.Format("Cấu hình máy đo phát tín hiệu: Power={0} dBm, waveform={1}\r\n", power_transmit, wave_form_name);
                instrument.config_HT20_RxTest_MAC(Channel_Freq, (power_transmit + _attenuator).ToString(), packet.ToString(), wave_form_name, RF_Port);

                //Đọc số gói tin nhận được từ ONT
                RXCounter = int.Parse(ModemTelnet.TestSensitivity_ReadPER_SendCommand(standard_2G_5G, ref _message));
                //Hien_Thi.Hienthi.SetText(rtbAll, _message);

                //Tính PER và hiển thị
                PER = Math.Round(((packet - RXCounter) * 100.0) / packet, 2);
                _ti.LOGSYSTEM += string.Format("PER = {0}%, Sent={1}, Received={2}\r\n", PER, packet, RXCounter);

                //So sánh PER với tiêu chuẩn
                bool _result = false;
                _result = PER <= stPER;

                return _result;
            }
            catch {
                return false;
            }
        }

        private bool AutoTestSensivitity(testinginfo _ti, ModemTelnet ModemTelnet, Instrument instrument, string _CarrierFreq) {
            List<sensivitity> list = null;
            _ti.LOGSYSTEM += "------------------------------------------------------------\r\n";
            _ti.LOGSYSTEM += string.Format("Bắt đầu thực hiện Test Sensitivity {0}.\r\n", _CarrierFreq);
            list = _CarrierFreq == "2G" ? GlobalData.listSensivitity2G : GlobalData.listSensivitity5G;

            if (list.Count == 0) return false;
            bool result = true;
            foreach (var item in list) {
                Stopwatch st = new Stopwatch();
                st.Start();

                string _channelNo = Attenuator.getChannelNumber(item.channelfreq);

                _ti.LOGSYSTEM += "*************************************************************************\r\n";
                _ti.LOGSYSTEM += string.Format("{0} - {1} - {2} - MCS{3} - BW{4} - Anten {5} - Channel {6}\r\n", _CarrierFreq, RF_Port, FunctionSupport.Get_WifiStandard_By_Mode(item.wifi, item.bandwidth), item.rate, 20 * Math.Pow(2, double.Parse(item.bandwidth)), item.anten, _channelNo);
                int count = 0;
                REP:
                count++;
                if (!Test_Sensivitity_Detail(_ti, ModemTelnet, instrument, _CarrierFreq, item.wifi, item.rate, item.bandwidth, item.channelfreq, item.anten, item.packet)) {
                    if (count < 3) {
                        _ti.LOGSYSTEM += string.Format("RETRY = {0}\r\n", count);
                        goto REP;
                    }
                    else {
                        _ti.LOGSYSTEM += string.Format("Phán định = {0}", "FAIL\r\n");
                        result = false;
                    }
                    //result = false;
                }
                else _ti.LOGSYSTEM += string.Format("Phán định = {0}", "PASS\r\n");
                st.Stop();
                _ti.LOGSYSTEM += string.Format("Thời gian test độ nhạy thu : {0} ms\r\n", st.ElapsedMilliseconds);
                _ti.LOGSYSTEM += "\r\n";
            }
            return result;
        }

        private bool Test_Sensitivity_2G(testinginfo _ti, ModemTelnet _mt, Instrument _it) {
            return AutoTestSensivitity(_ti, _mt, _it, "2G");
        }

        private bool Test_Sensitivity_5G(testinginfo _ti, ModemTelnet _mt, Instrument _it) {
            return AutoTestSensivitity(_ti, _mt, _it, "5G");
        }

        #endregion

        #endregion

    }
}
