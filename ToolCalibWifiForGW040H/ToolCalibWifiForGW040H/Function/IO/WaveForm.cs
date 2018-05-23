using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {

    public class WaveForm {

        static string fileName = string.Format("{0}Config\\Waveform-Config.csv", System.AppDomain.CurrentDomain.BaseDirectory);

        static WaveForm() {
            readFromFile();
        }

        //Load toàn bộ giá trị tên waveform từ file vào listWaveForm
        public static bool readFromFile() {
            try {
                GlobalData.listWaveForm = new List<waveformInfo>();
                if (File.Exists(fileName) == false) return false;
                var lines = File.ReadLines(fileName);
                foreach (var line in lines) {
                    if (!line.Contains("Instrument")) {
                        string[] buffer = line.Split(',');
                        waveformInfo wf = new waveformInfo() { instrument = buffer[0].Trim(), wifi = buffer[1].Trim(), mcs = buffer[2].Trim(), bandwidth = buffer[3].Trim(), waveform = buffer[4].Trim() };
                        GlobalData.listWaveForm.Add(wf);
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }


        //Search tên waveform từ listWaveForm
        public static bool getData(string _instrument, string _wifi, string _rate, string _bandwidth, out string _waveform) {
            _waveform = "";
            try {
                if (GlobalData.listWaveForm.Count == 0) return false;
                foreach (var item in GlobalData.listWaveForm) {
                    if (item.instrument == _instrument && item.wifi == _wifi && item.mcs == _rate && item.bandwidth == _bandwidth) {
                        _waveform = item.waveform;
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
