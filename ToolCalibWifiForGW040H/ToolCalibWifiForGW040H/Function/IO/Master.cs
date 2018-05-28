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

        public static bool Save() {
            try {
                if (GlobalData.autoCalculateMaster == null || GlobalData.autoCalculateMaster.Count == 0) return true;
                string fileName = string.Format("{0}tmpMaster\\Master_{1}_{2}.csv", System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));
                StreamWriter st = new StreamWriter(fileName);
                string _title = "Channel,Freq,Anten1,Anten2";
                st.WriteLine(_title);

                //1,2412,1,9.45
                //1.2412,2,8.54
                //tach list ban dau ra lam 2 list at1 va list at2
                List<masterinformation> _li1 = new List<masterinformation>();
                List<masterinformation> _li2 = new List<masterinformation>();
                foreach (var item in GlobalData.autoCalculateMaster) {
                    if (item.Anten == "1") {
                        masterinformation _mi = new masterinformation() { Channel = item.Channel, Frequency = item.Frequency, pwAnten1 = item.masterPower };
                        _li1.Add(_mi);
                    }
                    else {
                        masterinformation _mi = new masterinformation() { Channel = item.Channel, Frequency = item.Frequency, pwAnten2 = item.masterPower };
                        _li2.Add(_mi);
                    }
                }

                //add du lieu vao list attenuator
                List<masterinformation> _liMaster = new List<masterinformation>();
                for (int i = 0; i < _li1.Count; i++) {
                    masterinformation mf = new masterinformation() { Channel = _li1[i].Channel, Frequency = _li1[i].Frequency, pwAnten1 = _li1[i].pwAnten1, pwAnten2 = _li2[i].pwAnten2 };
                    _liMaster.Add(mf);
                }

                //ghi du lieu vao file
                foreach (var item in _liMaster) {
                    st.WriteLine(item.ToString());
                }

                st.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }

    }
}
