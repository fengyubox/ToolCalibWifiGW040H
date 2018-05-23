using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class LogRegister {

        private static string _logRegister = string.Format("{0}", AppDomain.CurrentDomain.BaseDirectory);

        public static bool Save(logregister _log) {
            try {
                string _logfile = string.Format("{0}\\{1}.csv", _logRegister, "_logRegister");

                string _title = "";
                _title = "DATE-TIME,MAC-ADDRESS,0x59,0x5A,0x5B,0x5F,0x60,0x61,0x143,0x144,0x148,0x149,0x14D,0x14E,0x157,0x158,0x15C,0x15D,0x161,0x162,0x166,0x167,0x16B,0x16C,0x170,0x171,0x175,0x176,0x17F,0x180,0x184,0x185,0x189,0x18A,0x18E,0x18F,TOTAL-RESULT";

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
    }
}
