using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class Attenuator {

        static string fileName = string.Format("{0}Config\\Attenuator-Config.csv", System.AppDomain.CurrentDomain.BaseDirectory);

        static Attenuator() {
            readFromFile();
        }

        //Load toàn bộ giá trị tên waveform từ file vào listWaveForm
        public static bool readFromFile() {
            try {
                GlobalData.listAttenuator = new List<attenuatorInfo>();
                if (File.Exists(fileName) == false) return false;
                var lines = File.ReadLines(fileName);
                foreach (var line in lines) {
                    if (!line.Contains("ChannelNumber")) {
                        string[] buffer = line.Split(',');
                        attenuatorInfo at = new attenuatorInfo() { channelnumber = buffer[0].Trim(), channelfreq = buffer[1].Trim(), at1_attenuator = double.Parse(buffer[2].Trim()), at2_attenuator = double.Parse(buffer[3].Trim()) };
                        GlobalData.listAttenuator.Add(at);
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool Save() {
            try {
                if (GlobalData.autoAttenuator == null || GlobalData.autoAttenuator.Count == 0) return true;
                StreamWriter st = new StreamWriter(fileName);
                string _title = "ChannelNumber,ChannelFreq (Hz),Anten1-Attenuator (dBm),Anten2-Attenuator (dBm)";
                st.WriteLine(_title);

                //1,2412,1,9.45
                //1.2412,2,8.54
                //tach list ban dau ra lam 2 list at1 va list at2
                List<autoattenuator> _li1 = new List<autoattenuator>();
                List<autoattenuator> _li2 = new List<autoattenuator>();
                foreach (var item in GlobalData.autoAttenuator) {
                    if (item.Anten == "1") _li1.Add(item);
                    else _li2.Add(item);
                }

                //add du lieu vao list attenuator
                List<masterinformation> _liAttenuator = new List<masterinformation>();
                for (int i = 0; i < _li1.Count; i++) {
                    masterinformation mf = new masterinformation() { Channel = _li1[i].Channel, Frequency = _li1[i].Frequency, pwAnten1 = _li1[i].Attenuator, pwAnten2 = _li2[i].Attenuator };
                    _liAttenuator.Add(mf);
                }

                //ghi du lieu vao file
                foreach(var item in _liAttenuator) {
                    st.WriteLine(item.ToString());
                }
                
                st.Dispose();
                return true;
            } catch {
                return false;
            }
        }


        //Search ChannelNumber từ listAttenuator
        public static string getChannelNumber(string _channelFreq) {
            if (GlobalData.listAttenuator.Count == 0) return "";
            string result = "";
            foreach (var item in GlobalData.listAttenuator) {
                if (item.channelfreq == _channelFreq) {
                    result = item.channelnumber;
                    break;
                }
            }
            return result;
        }


        //Search Attenuator từ listAttenuator
        public static double getAttenuator(string _channelFreq, string _anten) {
            if (GlobalData.listAttenuator.Count == 0) return double.MinValue;
            double result = double.MinValue;
            foreach (var item in GlobalData.listAttenuator) {
                if (item.channelfreq == _channelFreq) {
                    if (_anten.Trim() == "1") result = item.at1_attenuator;
                    if (_anten.Trim() == "2") result = item.at2_attenuator;
                    break;
                }
            }
            return result;
        }

    }
}
