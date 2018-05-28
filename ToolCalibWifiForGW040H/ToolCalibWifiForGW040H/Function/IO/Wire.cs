using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class Wire {

        private string _file = "";

        public Wire(string _fpath) {
            this._file = _fpath;
            GlobalData.autoCalculateMaster.Clear();
            readFromFile();
        }

        bool readFromFile() {
            try {
                GlobalData.listCalMaster = new List<verifysignal>();
                if (File.Exists(_file) == false) return false;
                var lines = File.ReadLines(_file);
                foreach (var line in lines) {
                    if (!line.Contains("Channel")) {
                        string[] buffer = line.Split(',');
                        calmaster mf = new calmaster() {
                            Channel = buffer[0],
                            Frequency = buffer[1],
                            Anten = "1",
                            wirePower = buffer[2]
                        };
                        verifysignal vs = new verifysignal() { wifi = "1", rate = "7", anten = "1", bandwidth = "0",  channelfreq = buffer[1] };
                        GlobalData.autoCalculateMaster.Add(mf);
                        GlobalData.listCalMaster.Add(vs);
                    }
                }
                foreach (var line in lines) {
                    if (!line.Contains("Channel")) {
                        string[] buffer = line.Split(',');
                        calmaster mf = new calmaster() {
                            Channel = buffer[0],
                            Frequency = buffer[1],
                            Anten = "2",
                            wirePower = buffer[3]
                        };
                        verifysignal vs = new verifysignal() { wifi = "1", rate = "7", anten = "2", bandwidth = "0", channelfreq = buffer[1] };
                        GlobalData.autoCalculateMaster.Add(mf);
                        GlobalData.listCalMaster.Add(vs);
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
