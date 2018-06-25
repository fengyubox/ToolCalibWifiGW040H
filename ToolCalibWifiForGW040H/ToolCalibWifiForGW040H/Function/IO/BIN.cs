using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class BIN {

        static string fileName = string.Format("{0}BIN\\BIN.csv", System.AppDomain.CurrentDomain.BaseDirectory);

        //Load toàn bộ giá trị BIN file vào ListBinRegister
        public static bool readFromFile() {
            try {
                GlobalData.ListBinRegister = new List<binregister>();
                if (File.Exists(fileName) == false) return false;
                var lines = File.ReadLines(fileName);
                foreach (var line in lines) {
                    if (!line.Contains("Register")) {
                        string[] buffer = line.Split(',');
                        binregister br = new binregister() {
                            Address = buffer[0],
                            oldValue = buffer[1],
                            newValue = buffer[2]
                        };
                        GlobalData.ListBinRegister.Add(br);
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
