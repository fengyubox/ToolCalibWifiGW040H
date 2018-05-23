using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class ChannelManagement {

        static string fileName = string.Format("{0}Config\\ChannelManagement.csv", System.AppDomain.CurrentDomain.BaseDirectory);

        static ChannelManagement() {
            readFromFile();
        }

        //Load toàn bộ giá trị tên waveform từ file vào listWaveForm
        public static bool readFromFile() {
            try {
                GlobalData.listChannel = new List<channelmanagement>();
                if (File.Exists(fileName) == false) return false;
                var lines = File.ReadLines(fileName);
                foreach (var line in lines) {
                    if (!line.Contains("RangeFrequency")) {
                        string[] buffer = line.Split(',');
                        channelmanagement cm = new channelmanagement() { rangefreq = buffer[0].Trim(), channel = buffer[1].Trim(), channelfreq = buffer[2].Trim() };
                        GlobalData.listChannel.Add(cm);
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
