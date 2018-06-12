using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using NationalInstruments.VisaNS; //Using .NET VISA DRIVER

namespace ToolCalibWifiForGW040H.Function {


    public class MT8870A_VISA : Instrument {

        public MT8870A_VISA(string MeasureEquip_IP) {
            try {
                g_logfilePath = @"WIFI_LOGFILE_MT8870.LOG";
                mbSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(MeasureEquip_IP);
            }
            catch {
                MessageBox.Show("[MT8870A_VISA]Không kết nối được với máy đo IP= " + MeasureEquip_IP);
                saveLogfile(g_logfilePath, "[MT8870A_VISA]Không kết nối được với máy đo IP= " + MeasureEquip_IP + " \n");

            };
        }


        //------------------ Hàm thiết lập cấu hình cho máy đo lần đầu tiên ---------------------
        public override bool config_Instrument_Total(string port, string Standard) {
            try {
                string _wifiStandard = "";
                switch (Standard) {
                    case "b": {
                            _wifiStandard = "DSSS";
                            break;
                        }
                    default: {
                            _wifiStandard = "OFDM";
                            break;
                        }
                }

                string PortName = string.Format("PORT{0}", port.Replace("RFIO", "").Trim());
                mbSession.Write(":CONF:SRW:SEGM:CLE\n");
                mbSession.Write(string.Format(":CONF:SRW:SEGM:APP AUTO{0}\n", _wifiStandard));
                mbSession.Write(":CONF:SRW:TRIG LEVEL\n");
                mbSession.Write(":CONF:SRW:TDEL -1E-05\n");
                mbSession.Write(":CONF:SRW:TLEV -15\n");
                mbSession.Write(":CONF:SRW:CAPT:MODE PACKET\n");
                mbSession.Write(":CONF:SRW:TTIM 1\n");
                mbSession.Write(":CONF:SRW:PACK 10\n");
                mbSession.Write(_wifiStandard == "DSSS" ? ":CONF:SRW:WLB:FTYP GAUSSIAN\n" : ":CONF:SRW:OFDM:CEST FULLPACKET\n");
                //mbSession.Write(":CONF:SRW:OFDM:CEST LTSEQUENCE\n");
                mbSession.Write(":CONF:SRW:SEL:WLAN:POW ON\n");
                mbSession.Write(":CONF:SRW:SEL:WLAN:EVM ON\n");
                mbSession.Write(":CONF:SRW:POW 20\n");
                mbSession.Write(string.Format(":ROUT:PORT:CONN:DIR {0},{1}\n", PortName, PortName));
                mbSession.Write(":CONF:SRW:ALEV:TIME 0.01\n");
                return true;
            }
            catch {
                return false;
            }
        }


        //------------------ Hàm thiết lập tần số kênh ---------------------
        public override bool config_Instrument_Channel(string channel_Freq) {
            try {
                mbSession.Write(":CONF:SRW:FREQ " + channel_Freq + "\n"); //Thiết lập chế độ WLAN
                return true;
            }
            catch {
                return false;
            }
        }


        //------------------ Hàm đọc về độ lệch tần số ---------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Trigger"></param>
        /// <param name="wifi">a,b,g,n,ac</param>
        /// <returns></returns>
        public override string config_Instrument_get_FreqErr(string Trigger, string wifi) {
            try {
                mbSession.Write(":INIT:SRW:ALEV\n");
                mbSession.Write("*WAI\n");
                mbSession.Write(":CONF:SRW:POW?\n");
                mbSession.ReadString();
                mbSession.Write(":INIT:SRW\n");
                mbSession.Write("*WAI\n");

                int counter = 0;
                while (true) {
                    mbSession.Write(":STAT:SRW:MEAS?\n");
                    string x = mbSession.ReadString();
                    if (x.Replace("\r", "").Replace("\n", "").Trim() == "1") break;
                    Thread.Sleep(100);
                    if (counter >= 30) return "";
                }

                counter = 0;
                REP:
                counter++;
                mbSession.Write(string.Format(":FETC:SRW:SUMM:WLAN:{0}:EVM? 1\n", wifi == "b" ? "DSSS" : "OFDM"));
                string tmpStr = mbSession.ReadString();
                string[] buffer = tmpStr.Split(',');
                double data = 0;
                bool ret = false;
                try {
                    ret = double.TryParse(wifi == "b" ? buffer[10] : buffer[7], out data);
                }
                catch { }
                if (ret == false) {
                    if (counter < 3) goto REP;
                }
                return wifi == "b" ? buffer[10] : buffer[7];
            }
            catch {
                return "";
            }
        }


        //------------------ Hàm đọc về công suất ---------------------
        public override string config_Instrument_get_Power(string Trigger, string wifi) {
            try {
                mbSession.Write(":INIT:SRW:ALEV\n");
                //mbSession.Write("*OPC?\n");
                //mbSession.Write("*WAI\n");
                Thread.Sleep(200);
                mbSession.Write(":CONF:SRW:POW?\n");
                mbSession.ReadString();
                mbSession.Write(":INIT:SRW\n");
                //mbSession.Write("*WAI\n");
                Thread.Sleep(200);

                int counter = 0;
                while (true) {
                    counter++;
                    mbSession.Write(":STAT:SRW:MEAS?\n");
                    string x = mbSession.ReadString();
                    if (x.Replace("\r", "").Replace("\n", "").Trim() == "1") break;
                    Thread.Sleep(100);
                    if (counter >= 30) return "";
                }

                counter = 0;
                REP:
                counter++;
                mbSession.Write(":FETC:SRW:SUMM:WLAN:POW? 1,1\n");
                string tmpStr = mbSession.ReadString();
                string[] buffer = tmpStr.Split(',');
                double data = 0;
                bool ret = false;
                try {
                    ret = double.TryParse(buffer[6], out data);
                }
                catch { }
                if (ret == false) {
                    if (counter < 3) goto REP;
                }
                return buffer[6];
            }
            catch {
                return "";
            }
        }


        //------------------ Hàm đọc về EVM ---------------------
        public override string config_Instrument_get_EVM(string Trigger, string wifi) {
            try {
                mbSession.Write(":INIT:SRW:ALEV\n");
                mbSession.Write("*WAI\n");
                mbSession.Write(":CONF:SRW:POW?\n");
                mbSession.ReadString();
                mbSession.Write(":INIT:SRW\n");
                mbSession.Write("*WAI\n");

                int counter = 0;
                while (true) {
                    mbSession.Write(":STAT:SRW:MEAS?\n");
                    string x = mbSession.ReadString();
                    if (x.Replace("\r", "").Replace("\n", "").Trim() == "1") break;
                    Thread.Sleep(100);
                    if (counter >= 30) return "";
                }

                counter = 0;
                REP:
                counter++;
                mbSession.Write(string.Format(":FETC:SRW:SUMM:WLAN:{0}:EVM? 1\n", wifi == "b" ? "DSSS" : "OFDM"));
                string tmpStr = mbSession.ReadString();
                string[] buffer = tmpStr.Split(',');
                double data = 0;
                bool ret = false;
                try {
                    ret = double.TryParse(wifi == "b" ? buffer[7] : buffer[12], out data);
                }
                catch { }
                if (ret == false) {
                    if (counter < 3) goto REP;
                }
                return wifi == "b" ? buffer[7] : buffer[12];
            }
            catch {
                return "";
            }
        }


        //------------------ Hàm đọc tất cả các kết quả, rồi mới Split ra từng thông số ---------------------
        public override string config_Instrument_get_TotalResult(string Trigger, string wifi) {
            try {
                string[] temp = new string[] { "0", "EVM", "2", "3", "4", "5", "6", "Freq", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "Power" };
                mbSession.Write(":INIT:SRW:ALEV\n");
                mbSession.Write("*WAI\n");
                mbSession.Write(":CONF:SRW:POW?\n");
                mbSession.ReadString();
                mbSession.Write(":INIT:SRW\n");
                mbSession.Write("*WAI\n");

                int counter = 0;
                while (true) {
                    mbSession.Write(":STAT:SRW:MEAS?\n");
                    string x = mbSession.ReadString();
                    if (x.Replace("\r", "").Replace("\n", "").Trim() == "1") break;
                    Thread.Sleep(100);
                    if (counter >= 30) return "";
                }

                counter = 0;
                double data = 0;
                bool ret = false;
                REP1:
                counter++;
                mbSession.Write(":FETC:SRW:SUMM:WLAN:POW? 1,1\n");
                string tmpStr = mbSession.ReadString();
                string[] buffer = tmpStr.Split(',');
                temp[19] = buffer[6]; //Power
                try {
                    ret = double.TryParse(buffer[6], out data);
                }
                catch { }
                if (ret == false) {
                    if (counter < 3) goto REP1;
                }

                counter = 0;
                data = 0;
                ret = false;
                REP2:
                counter++;
                mbSession.Write(string.Format(":FETC:SRW:SUMM:WLAN:{0}:EVM? 1\n", wifi == "b" ? "DSSS" : "OFDM"));
                tmpStr = mbSession.ReadString();
                buffer = tmpStr.Split(',');
                temp[7] = wifi == "b" ? buffer[9] : buffer[7]; //Freq Error
                temp[1] = wifi == "b" ? buffer[7] : buffer[12]; //EVM
                try {
                    ret = double.TryParse(temp[7], out data) && double.TryParse(temp[1], out data);
                }
                catch { }
                if (ret == false) {
                    if (counter < 3) goto REP2;
                }

                string result = "";
                for (int i = 0; i < temp.Length; i++) {
                    result += temp[i] + ",";
                }
                return result;
            }
            catch {
                return "";
            }
        }


        //------------------ Hàm đọc cấu hình máy đo phát TX ---------------------
        public override bool config_HT20_RxTest_MAC(string channel, string power, string packet_number, string waveform_file, string Port) {
            try {
                string subStr = "";
                string PortName = string.Format("PORT{0}", Port.Replace("RFIO", ""));
                mbSession.Write(":SOUR:GPRF:GEN:MODE NORMAL\n");
                mbSession.Write(":SOUR:GPRF:GEN:ARB:FILE:LOAD 'ZERO_200000000Hz_100000p'\n");
                mbSession.Write(string.Format(":ROUT:PORT:CONN:DIR {0},{1}\n", PortName, PortName));
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:REIN 0\n");
                mbSession.Write(":SOUR:GPRF:GEN:STAT 1\n");
                mbSession.Write(":SOUR:GPRF:GEN:BBM CW\n");
                mbSession.Write(":SOUR:GPRF:GEN:RFS:LEV -100\n");
                mbSession.Write(string.Format(":SOUR:GPRF:GEN:ARB:FILE:LOAD '{0}'\n", waveform_file));
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:GEN:SST 1,1,2\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:GEN:REP 1,SINGLE\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:GEN:GOTO 1,1\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:ENDC 1,1,TRIGGER\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:ENDC 1,2,SNUMBER\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:TRIG:SOUR 1,1,WFGEND\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:TRIG:SOUR 1,2,WFGEND\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:TRIG:DEL 1,1,0.000\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:TRIG:DEL 1,2,0.000\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:NSLC 1,1,NSEGMENT\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:NSLC 1,2,LOOP\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:BBM 1,1,ARB\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:BBM 1,2,ARB\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:WCTR 1,1,OFF\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:WCTR 1,2,OFF\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:GEN:DM:POL 1,NORMAL\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:LEV 1,2,-100.0DBM\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:PATT:SEL 1,1,'ZERO_200000000Hz_100000p',1\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:ENDC 1,1,REPEAT\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:IREP 1,1,1\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:GETR 1,1,0\n");
                mbSession.Write(string.Format(":SOUR:GPRF:GEN:SEQ:WAV:PATT:SEL 1,2,'{0}',1\n", waveform_file));
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:ENDC 1,2,REPEAT\n");
                mbSession.Write(string.Format(":SOUR:GPRF:GEN:SEQ:WAV:IREP 1,2,{0}\n", packet_number));
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:GETR 1,2,1\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:PATT:SEL 1,1,'ZERO_200000000Hz_100000p',1\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:ENDC 1,3,REPEAT\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:GETR 1,3,0\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:WAV:PATT:DEL 1,4\n");
                mbSession.Write(":SOUR:GPRF:GEN:SEQ:COMB:PATT 1,1\n");
                mbSession.Write(":SOUR:GPRF:GEN:MODE SEQUENCE\n");
                mbSession.Write(string.Format(":SOUR:GPRF:GEN:SEQ:RX:LEV 1,1,{0}DBM\n", power));
                mbSession.Write(string.Format(":SOUR:GPRF:GEN:SEQ:RX:FREQ 1,1,{0}000000HZ\n", channel));
                mbSession.Write(string.Format(":SOUR:GPRF:GEN:SEQ:RX:FREQ 1,2,{0}000000HZ\n", channel));
                mbSession.Write(string.Format(":SOUR:GPRF:GEN:SEQ:RX:OUTP:STAT 1,1,{0}\n", PortName));
                mbSession.Write(string.Format(":SOUR:GPRF:GEN:SEQ:RX:OUTP:STAT 1,2,{0}\n", PortName));
                int counter = 0;
                while (counter < 10) {
                    counter++;
                    mbSession.Write(":SOUR:GPRF:GEN:SEQ:RX:ERR? 1\n");
                    subStr = mbSession.ReadString().Replace("\r", "").Replace("\n", "").Trim();
                    if (subStr == "0,0,0") counter = 199;
                    else Thread.Sleep(100);
                }
                if (counter != 199) return false;

                mbSession.Write(":SOUR:GPRF:GEN:SEQ:EXEC\n");

                counter = 0;
                while (counter < 10) {
                    counter++;
                    mbSession.Write(":SOUR:GPRF:GEN:SEQ:STAT?\n");
                    subStr = mbSession.ReadString().Replace("\r", "").Replace("\n", "").Trim();
                    if (subStr == "0") counter = 199;
                    else Thread.Sleep(100);
                }
                if (counter != 199) return false;

                return true;
            }
            catch {
                return false;
            }
        }

    }

}
