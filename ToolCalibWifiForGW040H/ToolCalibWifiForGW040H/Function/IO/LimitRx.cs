using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class LimitRx {

        static string fileName = string.Format("{0}Config\\RxLimit-Config.csv", System.AppDomain.CurrentDomain.BaseDirectory);

        static LimitRx() {
            readFromFile();
        }

        //Load toàn bộ giá trị tên limit RX từ file vào listLimitWifiRX
        public static bool readFromFile() {
            try {
                GlobalData.listLimitWifiRX = new List<limitrx>();
                if (File.Exists(fileName) == false) return false;
                var lines = File.ReadLines(fileName);
                foreach (var line in lines) {
                    if (!line.Contains("RangeFrequency")) {
                        string[] buffer = line.Split(',');
                        limitrx lr = new limitrx() {
                            rangefreq = buffer[0],
                            wifi = buffer[1],
                            mcs = buffer[2],
                            power_Transmit = buffer[3],
                            PER = buffer[4]
                        };
                        GlobalData.listLimitWifiRX.Add(lr);
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }


        //Search tên limit RX từ listLimitWifiRX
        public static bool getData(string _rangefreq, string _wifi, string _mcs, out limitrx _limit) {
            _limit = new limitrx();
            try {
                if (GlobalData.listLimitWifiRX.Count == 0) return false;
                foreach (var item in GlobalData.listLimitWifiRX) {
                    if (item.rangefreq == _rangefreq && item.wifi == _wifi && item.mcs == _mcs) {
                        _limit = item;
                        break;
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
