using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function
{
    public class Master
    {
        private string _file = "";

        public Master(string _fpath) {
            this._file = _fpath;
            readFromFile();
        }

        bool readFromFile() {
            try {
                GlobalData.listMasterData = new List<masterinformation>();
                if (File.Exists(_file) == false) return false;
                var lines = File.ReadLines(_file);
                foreach (var line in lines) {
                    if (!line.Contains("Channel")) {
                        string[] buffer = line.Split(',');
                        masterinformation mf = new masterinformation() {
                             Channel = buffer[0],
                             Frequency = buffer[1],
                             pwAnten1 = buffer[2],
                             pwAnten2 = buffer[3]
                        };
                        GlobalData.listMasterData.Add(mf);
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }
        
        public static string getPower(string _freq, string _anten) {
            try {
                string _result = "";
                foreach (var item in GlobalData.listMasterData) {
                    if (item.Frequency == _freq) {
                        _result = _anten == "1" ? item.pwAnten1 : item.pwAnten2;
                        break;
                    }
                }
                return _result;
            } catch {
                return "";
            }
        }

        public static string getChannel (string _freq) {
            try {
                string _result = "";
                foreach (var item in GlobalData.listMasterData) {
                    if (item.Frequency == _freq) {
                        _result = item.Channel;
                        break;
                    }
                }
                return _result;
            }
            catch {
                return "";
            }
        }

    }
}
