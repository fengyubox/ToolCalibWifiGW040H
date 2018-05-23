using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Windows;
using NationalInstruments.VisaNS; //Using .NET VISA DRIVER


namespace ToolCalibWifiForGW040H.Function {
    public abstract class Instrument {

        #region khai bao bien
        protected MessageBasedSession mbSession;
        protected string g_logfilePath;
        public int minEquip_DELAY = 50;
        public int avgEquip_DELAY = 100;
        public int maxEquip_DELAY = 200;
        public string logfile = "";
        protected string Cable_Loss = "";

        protected int TimeOutMs = 400;
        //Các biến dữ liệu kiểu decimal chứa thông số UpperLimit & LowerLimit chuẩn WLAN
        public Decimal PeakPWLow_decimal;
        public Decimal PeakPWUp_decimal;
        public Decimal AvgPWLow_decimal;
        public Decimal AvgPWUp_decimal;
        public Decimal EVMallUp_decimal;
        public Decimal EVMDataUp_decimal;
        public Decimal EVMPilotUp_decimal;
        public Decimal MaxAmpI_decimal;
        public Decimal MaxFreqErr_decimal;
        public Decimal MaxIQ_decimal;
        public Decimal MaxPhaseI_decimal;
        public Decimal MaxSymbolCLKErr_decimal;
        public Decimal MaxPhaseErr_decimal;

        public string Frequency_Error;
        public string Power_measured;
        public string EVM_measured;

        public string g_MainConfigFilePath = @"Config\Main-Config.cfg";

        //---Flag kiểm tra trạng thái cấu hình máy đo đã hoàn thành hay chưa
        protected bool config_done = false;
        public bool check_config_done {
            get { return config_done; }
            set { config_done = value; }
        }

        #endregion

        #region abstract function

        /// <summary>
        /// CẤU HÌNH BAN ĐẦU CHO MÁY ĐO
        /// </summary>
        /// <param name="RFIO">Cổng RF</param>
        /// <param name="Standard">Chuẩn WIFI a,b,g,n,ac</param>
        /// <returns>TRUE/FALSE</returns>
        public abstract bool config_Instrument_Total(string port, string Standard);

        /// <summary>
        /// CẤU HÌNH TẦN SỐ WIFI CHO MÁY ĐO
        /// </summary>
        /// <param name="channel_Freq">Tần số</param>
        /// <returns>TRUE/FALSE</returns>
        public abstract bool config_Instrument_Channel(string channel_Freq);

        /// <summary>
        /// HÀM ĐỌC GIÁ TRỊ ĐỘ LỆCH TẦN WIFI
        /// </summary>
        /// <param name="Trigger">RFB</param>
        /// <returns>Giá trị lệch tần số</returns>
        public abstract string config_Instrument_get_FreqErr(string Trigger, string wifi);

        /// <summary>
        /// HÀM ĐỌC GIÁ TRỊ CÔNG SUẤT
        /// </summary>
        /// <param name="Trigger"></param>
        /// <returns>Giá trị công suất</returns>
        public abstract string config_Instrument_get_Power(string Trigger, string wifi);

        /// <summary>
        /// HÀM ĐỌC TẤT CẢ GIÁ TRỊ ĐO
        /// </summary>
        /// <param name="Trigger"></param>
        /// <returns>Kết quả đo</returns>
        public abstract string config_Instrument_get_TotalResult(string Trigger, string wifi);

        /// <summary>
        /// CẤU HÌNH MÁY ĐO PHÁT WaveForm
        /// </summary>
        /// <param name="channel">Tần số</param>
        /// <param name="power">Công suất phát waveform</param>
        /// <param name="packet_number">Số gói tin</param>
        /// <param name="waveform_file">File waveform</param>
        /// <returns>TRUE/FALSE</returns>
        public abstract bool config_HT20_RxTest_MAC(string channel, string power, string packet_number, string waveform_file, string port);

        /// <summary>
        /// HÀM ĐỌC GIÁ TRỊ EVM
        /// </summary>
        /// <param name="Trigger"></param>
        /// <returns></returns>
        public abstract string config_Instrument_get_EVM(string Trigger, string wifi);

        #endregion

        #region Cac ham su dung chung


        public void ClearCommand() {
            mbSession.Clear();
        }

        public void Close() {
            mbSession.Dispose();
        }


        protected string ReplaceCommonEscapeSequences(string s) {
            return s.Replace("\\n", "\n").Replace("\\r", "\r");
        }

        protected string InsertCommonEscapeSequences(string s) {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        }

        protected bool checkBusyState(MessageBasedSession mbSession, string command) {
            string OK = "0";
            string Command_Line;
            string responseString = "1";
            byte counter = 0;

            if ((command == "*ESE?") || (command == "*ESR?"))
                OK = "0";
            else
                if ((command == "*OPC?") || (command == ":STAT:SRW:MEAS?"))
                OK = "1";

            while ((responseString.Trim() != OK) && (counter < 255)) {
                Command_Line = ReplaceCommonEscapeSequences(command + "\n");
                responseString = mbSession.Query(Command_Line);
                responseString = responseString.Substring(0, 1);
                Thread.Sleep(250);
                counter++;
            }

            if (counter == 1)
                return false;
            else
                return true;
        }

        protected void ClearLog() {
            try {
                File.WriteAllText(g_logfilePath, String.Empty);
            }
            catch {
                MessageBox.Show("Không xóa được logfile của E6640A");
            }
        }

        protected string GetLog() {
            string readConfigFile = "";
            if (File.Exists(g_logfilePath)) {
                StreamReader StreamR = File.OpenText(g_logfilePath);
                readConfigFile = StreamR.ReadToEnd();
                StreamR.Close();
            }
            return readConfigFile;
        }

        protected static void saveLogfile(string pathfile, string content)// 
        {
            try {
                if (File.Exists(pathfile)) {
                    StreamWriter StreamW = File.AppendText(pathfile);
                    StreamW.Write(content);
                    StreamW.Close();
                }
                else {
                    StreamWriter StreamW = null;
                    StreamW = File.CreateText(pathfile);
                    StreamW.Write(content);
                    StreamW.Close();
                }
            }
            catch {
                MessageBox.Show("Loi save logfile");
            }
            /*---------Lưu logfile kết quả test---------*/
        }

        protected void LimitTable(decimal PeakPWLow, decimal PeakPWUp, decimal AvgPWLow, decimal AvgPWUp,
                               decimal EVMallUp, decimal EVMDataUp, decimal EVMPilotUp, decimal MaxAmpI,
                               decimal MaxFreqErr, decimal MaxIQ, decimal MaxPhaseI, decimal MaxSymbolCLKErr,
                               decimal MaxPhaseErr) {
            //Update limit table 
            this.PeakPWLow_decimal = PeakPWLow;
            this.PeakPWUp_decimal = PeakPWUp;
            this.AvgPWLow_decimal = AvgPWLow;
            this.AvgPWUp_decimal = AvgPWUp;
            this.EVMallUp_decimal = EVMallUp;
            this.EVMDataUp_decimal = EVMDataUp;
            this.EVMPilotUp_decimal = EVMPilotUp;
            this.MaxAmpI_decimal = MaxAmpI;
            this.MaxFreqErr_decimal = MaxFreqErr;
            this.MaxIQ_decimal = MaxIQ;
            this.MaxPhaseI_decimal = MaxPhaseI;
            this.MaxSymbolCLKErr_decimal = MaxSymbolCLKErr;
            this.MaxPhaseErr_decimal = MaxPhaseErr;
            // 
        }

        public void Load_ConfigFile() {
            string readConfigFilePath = string.Empty;
            if (File.Exists(g_MainConfigFilePath)) {
                StreamReader StreamR = File.OpenText(g_MainConfigFilePath);
                readConfigFilePath = StreamR.ReadToEnd();
                StreamR.Close();
            }
            //------------------------------
            try {
                for (int i = 0; i < readConfigFilePath.Split('\n').Length; i++) {
                    if (readConfigFilePath.Split('\n')[i].Contains("Thiet Lap Cable Loss:")) {
                        Cable_Loss = readConfigFilePath.Split('\n')[i + 1].Trim();
                    }
                }
            }
            catch {
            }
        }

        /*
 * Hàm kiểm tra giá trị đo được là Pass/Fail bằng việc so sánh với giá trị LowerLimit và UpperLimit
 * Tham sô đầu vào:
 * - Decimal value: Giá trị đo được
 * - Decimal lowerlimit: Giá trị LowerLimit
 * - Decimal upperlimit: Giá trị UpperLimit
 * Tham số trong hàm:
 * - g_testPass_SUM: Tổng số thông số của tất cả các bài test pass
 * - g_testFail_SUM: Tổng số thông số của tất cả các bài test fail
 */
        public bool check_PassFail_limit(Decimal value, Decimal lowerlimit, Decimal upperlimit) {
            bool result = false;
            if ((value <= upperlimit) && (value >= lowerlimit)) {
                result = true;
            }
            else {
                result = false;
            }
            return result;
        }

        #endregion

    }
}
