using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class LogFile {

        private static string _logTest = string.Format("{0}Logtest", AppDomain.CurrentDomain.BaseDirectory);
        private static string _logDetail = string.Format("{0}Logdetail", AppDomain.CurrentDomain.BaseDirectory);

        static LogFile() {
            if (Directory.Exists(_logTest) == false) Directory.CreateDirectory(_logTest);
            if (Directory.Exists(_logDetail) == false) Directory.CreateDirectory(_logDetail);
        }

        public static bool Savetestlog(logdata _log) {
            try {
                string _logfile = string.Format("{0}\\{1}.csv", _logTest, DateTime.Now.ToString("yyyyMMdd"));

                string _title = "";
                if (GlobalData.initSetting.STATION == "Trước đóng vỏ")
                    _title = "DATE-TIME,MAC-ADDRESS,CALIB-FREQ,CALIB-POWER-2G,CALIB-POWER-5G,TEST-SENS-2G,TEST-SENS-5G,VERIFY-SIGNAL-2G,VERIFY-SIGNAL-5G,ERROR-CODE,TOTAL-RESULT";
                else
                    _title = "DATE-TIME,MAC-ADDRESS,TEST-ANTEN1,TEST-ANTEN2,ERROR-CODE,TOTAL-RESULT";

                StreamWriter st = null;
                if (File.Exists(_logfile) == false) {
                    st = new StreamWriter(_logfile, true);
                    st.WriteLine(_title);
                }
                else st = new StreamWriter(_logfile, true);

                st.WriteLine(_log.ToString());
                st.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool Savedetaillog(string _data) {
            try {
                string _logfile = string.Format("{0}\\{1}.txt", _logDetail, DateTime.Now.ToString("yyyyMMdd"));
                StreamWriter st = new StreamWriter(_logDetail, true);
                st.WriteLine(_data);
                st.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
