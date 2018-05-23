using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using NationalInstruments.VisaNS; //Using .NET VISA DRIVER

namespace ToolCalibWifiForGW040H.Function {

    public class E6640A_VISA : Instrument {

        public E6640A_VISA(string MeasureEquip_IP) {
            try {
                g_logfilePath = @"WIFI_LOGFILE_E6640A.LOG";
                mbSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(MeasureEquip_IP);
            }
            catch {
                MessageBox.Show("[E6640A_VISA]Không kết nối được với máy đo IP= " + MeasureEquip_IP);
            };
        }


        //------------------ Hàm thiết lập cấu hình cho máy đo lần đầu tiên ---------------------
        public override bool config_Instrument_Total(string port, string Standard) {
            bool enable_nSISO_Testing = false;
            try {
                string _wifiStandard = "";
                switch (Standard) {
                    case "g": { _wifiStandard = "GDO"; break; }
                    case "b": { _wifiStandard = "BG"; break; }
                    case "a": { _wifiStandard = "AG"; break; }
                    case "n20": { _wifiStandard = "N20"; break; }
                    case "n40": { _wifiStandard = "N40"; break; }
                    case "ac20": { _wifiStandard = "AC20"; break; }
                    case "ac40": { _wifiStandard = "AC40"; break; }
                    case "ac80": { _wifiStandard = "AC80"; break; }
                    case "ac160": { _wifiStandard = "AC160"; break; }
                }

                mbSession.Write(":SYSTem:PRESet\n"); //Preset máy đo

                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":FEED:RF:PORT " + port + "\n"); //Chọn RF input

                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write("INST:SEL WLAN\n"); //Thiết lập chế độ WLAN

                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":RAD:STAN " + _wifiStandard + "\n"); //Thiết lập chuẩn b/g/n kèm bandwidth

                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":INITiate:EVM\n"); //Thiết lập đo ở chế độ EVM

                enable_nSISO_Testing = true;
            }
            catch {
                enable_nSISO_Testing = false;
                config_done = true;
                saveLogfile(g_logfilePath, "[E6640A]ERROR CODE: [Equip_Config] \n Error tai qua trinh cau hinh cho thiet bi do E6640A \n");
            }
            return enable_nSISO_Testing;
        }


        //------------------ Hàm thiết lập tần số kênh ---------------------
        public override bool config_Instrument_Channel(string channel_Freq) {
            bool enable_nSISO_Testing = false;
            try {
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");     
                mbSession.Write("FREQ:CENT " + channel_Freq + "\n");
                enable_nSISO_Testing = true;
                config_done = true;
            }
            catch {
                enable_nSISO_Testing = false;
                config_done = true;
            }
            return enable_nSISO_Testing;
        }


        //------------------ Hàm đọc về độ lệch tần số ---------------------
        public override string config_Instrument_get_FreqErr(string Trigger, string wifi) {
            try {
                string result_Value = string.Empty;

                mbSession.Write("TRIG:EVM:SOUR " + Trigger + "\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":POW:RANG 15.0\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                //mbSession.Write(":SENSe:POWer:RF:RANGe:OPTimize IMMediate\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                //mbSession.Write(":SENSe:POWer:RF:RANGe:OPTimize IMMediate\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":INIT:CONT 0\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                result_Value = mbSession.Query("READ:EVM?");
                result_Value = InsertCommonEscapeSequences(result_Value);
                if (result_Value.Trim() == "") {
                    return null;
                }
                else {
                    try {
                        string[] MODulation_Value = result_Value.Split(new Char[] { ',' });
                        Decimal measureResult = 0;
                        measureResult = Decimal.Parse(MODulation_Value[7], System.Globalization.NumberStyles.Float);
                        //Hien_Thi.Hienthi.SetText(rtb, "- Center Frequency Error = " + measureResult.ToString("0.####") + " Hz");
                        return measureResult.ToString();
                    }
                    catch {
                        //Hien_Thi.Hienthi.SetText(rtb, "Lỗi quá trình đọc kết quả");
                        return null;
                    }
                }
            }
            catch {
                //Hien_Thi.Hienthi.SetText(rtb, "Lỗi quá trình Kiểm tra");
                return null;
            }
        }


        //------------------ Hàm đọc về công suất ---------------------
        public override string config_Instrument_get_Power(string Trigger, string wifi) {
            try {
                string result_Value = string.Empty;
                mbSession.Write("TRIG:EVM:SOUR " + Trigger + "\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":POW:RANG 15.0\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");

                //mbSession.Write(":SENSe:POWer:RF:RANGe:OPTimize IMMediate\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                //mbSession.Write(":SENSe:POWer:RF:RANGe:OPTimize IMMediate\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");

                mbSession.Write(":INIT:CONT 0\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                result_Value = mbSession.Query("READ:EVM?");
                result_Value = InsertCommonEscapeSequences(result_Value);
                if (result_Value.Trim() == "") {
                    return null;
                }
                else {
                    try {
                        string[] MODulation_Value = result_Value.Split(new Char[] { ',' });
                        Decimal measureResult = 0;
                        measureResult = Decimal.Parse(MODulation_Value[19], System.Globalization.NumberStyles.Float);
                        //MessageBox.Show(measureResult.ToString());
                        //Hien_Thi.Hienthi.SetText(rtb, "- Average Power = " + measureResult.ToString("0.####") + " dBm");
                        return measureResult.ToString();
                    }
                    catch {
                        //Hien_Thi.Hienthi.SetText(rtb, "Lỗi quá trình đọc kết quả");
                        return null;
                    }
                }
            }
            catch {
                //Hien_Thi.Hienthi.SetText(rtb, "Lỗi quá trình Kiểm tra");
                return null;
            }
        }

        //------------------ Hàm đọc về EVM ---------------------
        public override string config_Instrument_get_EVM(string Trigger, string wifi) {
            try {
                string result_Value = string.Empty;
                mbSession.Write("TRIG:EVM:SOUR " + Trigger + "\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":POW:RANG 15.0\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");

                //mbSession.Write(":SENSe:POWer:RF:RANGe:OPTimize IMMediate\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                //mbSession.Write(":SENSe:POWer:RF:RANGe:OPTimize IMMediate\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":INIT:CONT 0\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                result_Value = mbSession.Query("READ:EVM?");
                result_Value = InsertCommonEscapeSequences(result_Value);
                if (result_Value.Trim() == "") {
                    return null;
                }
                else {
                    try {
                        string[] MODulation_Value = result_Value.Split(new Char[] { ',' });
                        Decimal measureResult = 0;
                        measureResult = Decimal.Parse(MODulation_Value[1], System.Globalization.NumberStyles.Float);
                        //Hien_Thi.Hienthi.SetText(rtb, "- EVM All Carriers = " + measureResult.ToString("0.####") + " dB");
                        return measureResult.ToString();
                    }
                    catch {
                        //Hien_Thi.Hienthi.SetText(rtb, "Lỗi quá trình đọc kết quả");
                        return null;
                    }
                }
            }
            catch {
                //Hien_Thi.Hienthi.SetText(rtb, "Lỗi quá trình Kiểm tra");
                return null;
            }
        }


        //------------------ Hàm đọc tất cả các kết quả, rồi mới Split ra từng thông số ---------------------
        public override string config_Instrument_get_TotalResult(string Trigger, string wifi) {
            string result_Value = string.Empty;
            try {
                mbSession.Write("TRIG:EVM:SOUR " + Trigger + "\n");
                Thread.Sleep(50);
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                mbSession.Write(":POW:RANG 15.0\n");
                Thread.Sleep(50);
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");

                //mbSession.Write(":SENSe:POWer:RF:RANGe:OPTimize IMMediate\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");
                //mbSession.Write(":SENSe:POWer:RF:RANGe:OPTimize IMMediate\n");
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");

                mbSession.Write(":INIT:CONT 0\n");
                Thread.Sleep(50);
                //checkBusyState(mbSession, "*ESR?");
                //checkBusyState(mbSession, "*OPC?");

                result_Value = mbSession.Query("READ:EVM?");
                Thread.Sleep(100);
                result_Value = InsertCommonEscapeSequences(result_Value);
                if (result_Value.Trim() == "") {
                    return "NULL";
                }
                else {
                    return result_Value;
                }
            }
            catch {
                return "ERROR";
            }
        }


        //------------------ Hàm đọc cấu hình máy đo phát TX ---------------------
        public override bool config_HT20_RxTest_MAC(string channel, string power, string packet_number, string waveform_file, string port) {
            bool enable_nSISO_Testing = false;
            try {

                string frequency = "";
                frequency = ((int.Parse(channel) * 5) + 2407).ToString();

                //saveLogfile(g_logfilePath, "[BOL] Khởi tạo lần đầu cho thiết bị!\n");
                // Cấu hình khối Source
                mbSession.Write(":SOURce:PRESet" + "\n");
                //Thread.Sleep(100);
                mbSession.Write(":SOUR:RAD:ARB:LOAD \"D:\\\\Waveform\\\\" + waveform_file + "\"" + "\n");
                mbSession.Write(":SOUR:LIST:NUMB:STEP 3" + "\n");   //Tạo 3 step
                //Thread.Sleep(100);
                mbSession.Write(":SOUR:LIST:STEP1:SET IMM,0.00000E+00,NONE,DOWN," + frequency + "MHz" + ",-1.2000000E+02,\"CW\",TIME,1.0000E-03,0,1" + "\n");   //Thiết lập thông số cho step1
                //Thread.Sleep(1000);
                mbSession.Write(":SOUR:LIST:STEP1:SET INT,0.00000E+00,NONE,DOWN," + frequency + "MHz" + "," + power + "dBm" + ",\"" + waveform_file + "\",COUN," + packet_number + ",0,1" + "\n");   //Thiết lập thông số cho step2
                //Thread.Sleep(1000);
                mbSession.Write(":SOUR:LIST:STEP3:SET INT,1.00E-03,NONE,DOWN," + frequency + "MHz" + ",-1.2000000E+02,\"CW\",TIME,1.0000E-03,1,1" + "\n");  //Thiết lập thông số cho step3
                //Thread.Sleep(1000);
                mbSession.Write(":SOUR:LIST ON" + "\n");
                //Thread.Sleep(100);
                mbSession.Write(":SOUR:LIST:TRIG" + "\n");  //Bắt đầu phát waveform

            }
            catch {
                enable_nSISO_Testing = false;
                config_done = true;
                saveLogfile(g_logfilePath, "[E6640A]ERROR CODE: [Equip_Config] \n Error tai qua trinh cau hinh cho thiet bi do E6640A \n");
            }
            return enable_nSISO_Testing;
        }

    }

}
