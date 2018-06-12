using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class TestCase {

        static Dictionary<string, string> dictWifi = new Dictionary<string, string>() {
            { "802.11b", "0" },
            { "802.11g", "1" },
            { "802.11a", "2" },
            { "802.11n", "3" },
            { "802.11ac", "4" }
        };

        static Dictionary<string, string> dictRate = new Dictionary<string, string>() {
            { "MCS0", "0" },
            { "MCS1", "1" },
            { "MCS2", "2" },
            { "MCS3", "3" },
            { "MCS4", "4" },
            { "MCS5", "5" },
            { "MCS6", "6" },
            { "MCS7", "7" },
            { "MCS8", "8" },
            { "MCS9", "9" }
        };

        static Dictionary<string, string> dictBandwidth = new Dictionary<string, string>() {
            { "20", "0" },
            { "40", "1" },
            { "80", "2" },
            { "160", "3" }
        };

        static string _dirName = string.Format("{0}TestCase", System.AppDomain.CurrentDomain.BaseDirectory);

        static string _filename_txwifi2 = string.Format("{0}TestCase\\txWifi-2G.csv", System.AppDomain.CurrentDomain.BaseDirectory);
        static string _filename_txwifi5 = string.Format("{0}TestCase\\txWifi-5G.csv", System.AppDomain.CurrentDomain.BaseDirectory);
        static string _filename_rxwifi2 = string.Format("{0}TestCase\\rxWifi-2G.csv", System.AppDomain.CurrentDomain.BaseDirectory);
        static string _filename_rxwifi5 = string.Format("{0}TestCase\\rxWifi-5G.csv", System.AppDomain.CurrentDomain.BaseDirectory);
        static string _filename_at1wifi2 = string.Format("{0}TestCase\\at1Wifi-2G.csv", System.AppDomain.CurrentDomain.BaseDirectory);
        static string _filename_at2wifi2 = string.Format("{0}TestCase\\at2Wifi-2G.csv", System.AppDomain.CurrentDomain.BaseDirectory);
        static string _filename_att = GlobalData.initSetting.STATION == "Trước đóng vỏ" ? string.Format("{0}TestCase\\Att-TestCase.csv", System.AppDomain.CurrentDomain.BaseDirectory) : string.Format("{0}TestCase\\Att-TestCase-Anten.csv", System.AppDomain.CurrentDomain.BaseDirectory);

        static TestCase() {
            if (Directory.Exists(_dirName) == false) Directory.CreateDirectory(_dirName);
        }

        //Load dữ liệu từ file vào tmpList, chuyển dữ liệu từ tmpList sang List test
        public static bool Load() {
            try {
                //Load dữ liệu từ file vào tmpList
                _load_File_To_tmpList(_filename_txwifi2, out GlobalData.tmplisttxWifi2G);
                _load_File_To_tmpList(_filename_txwifi5, out GlobalData.tmplisttxWifi5G);
                _load_File_To_tmpList(_filename_rxwifi2, out GlobalData.tmplistrxWifi2G);
                _load_File_To_tmpList(_filename_rxwifi5, out GlobalData.tmplistrxWifi5G);
                _load_File_To_tmpList(_filename_at1wifi2, out GlobalData.tmplisttestAnten1);
                _load_File_To_tmpList(_filename_at2wifi2, out GlobalData.tmplisttestAnten2);
                _load_File_To_tmpList(_filename_att, out GlobalData.tmplistCalAttenuator);


                //Chuyển dữ liệu từ tmpList sang List test
                _convert_tmpList_To_testList(GlobalData.tmplisttxWifi2G, out GlobalData.listVerifySignal2G);
                _convert_tmpList_To_testList(GlobalData.tmplisttxWifi5G, out GlobalData.listVerifySignal5G);
                _convert_tmpList_To_testList(GlobalData.tmplistrxWifi2G, out GlobalData.listSensivitity2G);
                _convert_tmpList_To_testList(GlobalData.tmplistrxWifi5G, out GlobalData.listSensivitity5G);
                _convert_tmpList_To_testList(GlobalData.tmplisttestAnten1, out GlobalData.listTestAnten1);
                _convert_tmpList_To_testList(GlobalData.tmplisttestAnten2, out GlobalData.listTestAnten2);
                _convert_tmpList_To_testList(GlobalData.tmplistCalAttenuator, out GlobalData.listCalAttenuator);

                return true;
            }
            catch {
                return false;
            }
        }

        //Save dữ liệu từ tmpList vào file, chuyển dữ liệu từ tmpList sang List test
        public static bool Save() {
            try {
                //Save dữ liệu từ tmpList vào file
                _save_tmpList_To_File(GlobalData.tmplisttxWifi2G, _filename_txwifi2);
                _save_tmpList_To_File(GlobalData.tmplisttxWifi5G, _filename_txwifi5);
                _save_tmpList_To_File(GlobalData.tmplistrxWifi2G, _filename_rxwifi2);
                _save_tmpList_To_File(GlobalData.tmplistrxWifi5G, _filename_rxwifi5);
                _save_tmpList_To_File(GlobalData.tmplisttestAnten1, _filename_at1wifi2);
                _save_tmpList_To_File(GlobalData.tmplisttestAnten2, _filename_at2wifi2);

                //Chuyển dữ liệu từ tmpList sang List test
                _convert_tmpList_To_testList(GlobalData.tmplisttxWifi2G, out GlobalData.listVerifySignal2G);
                _convert_tmpList_To_testList(GlobalData.tmplisttxWifi5G, out GlobalData.listVerifySignal5G);
                _convert_tmpList_To_testList(GlobalData.tmplistrxWifi2G, out GlobalData.listSensivitity2G);
                _convert_tmpList_To_testList(GlobalData.tmplistrxWifi5G, out GlobalData.listSensivitity5G);
                _convert_tmpList_To_testList(GlobalData.tmplisttestAnten1, out GlobalData.listTestAnten1);
                _convert_tmpList_To_testList(GlobalData.tmplisttestAnten2, out GlobalData.listTestAnten2);

                return true;
            }
            catch {
                return false;
            }
        }

        //Load dữ liệu từ file vào tmpList
        static bool _load_File_To_tmpList(string _filename, out List<verifysignal> _lo) {
            _lo = new List<verifysignal>();
            if (File.Exists(_filename) == false) return false;
            try {
                string line;
                StreamReader sr = new StreamReader(_filename);
                while ((line = sr.ReadLine()) != null) {
                    if (line.Trim() != "" || line.Trim() != string.Empty) {
                        string[] buffer = line.Split('|');
                        _lo.Add(new verifysignal() { wifi = buffer[0], rate = buffer[1], bandwidth = buffer[2], anten = buffer[3], channelfreq = buffer[4] });
                    }
                }
                sr.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }

        static bool _load_File_To_tmpList(string _filename, out List<sensivitity> _lo) {
            _lo = new List<sensivitity>();
            if (File.Exists(_filename) == false) return false;
            try {
                string line;
                StreamReader sr = new StreamReader(_filename);
                while ((line = sr.ReadLine()) != null) {
                    if (line.Trim() != "" || line.Trim() != string.Empty) {
                        string[] buffer = line.Split('|');
                        _lo.Add(new sensivitity() { wifi = buffer[0], rate = buffer[1], bandwidth = buffer[2], anten = buffer[3], channelfreq = buffer[4], packet = int.Parse(buffer[5]) });
                    }
                }
                sr.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }

        //Save dữ liệu từ tmpList vào file
        static bool _save_tmpList_To_File(List<verifysignal> _li, string _filename) {
            if (_li == null || _li.Count == 0) return false;

            try {
                StreamWriter st = new StreamWriter(_filename);
                foreach (var item in _li) {
                    if (item.ToString()!= "||||") st.WriteLine(item.ToString());
                }
                st.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }

        static bool _save_tmpList_To_File(List<sensivitity> _li, string _filename) {
            if (_li == null || _li.Count == 0) return false;

            try {
                StreamWriter st = new StreamWriter(_filename);
                foreach (var item in _li) {
                    if (item.ToString()!= "|||||0") st.WriteLine(item.ToString());
                }
                st.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }

        //chuyển dữ liệu từ tmpList sang List test
        static bool _convert_tmpList_To_testList(List<verifysignal> _li, out List<verifysignal> _lo) {
            _lo = new List<verifysignal>();
            if (_li == null || _li.Count == 0) return false;

            try {
                // -- 802.11b | MCS0 | 20 | 1,2 | 2412,2437,2462 --
                foreach (var item in _li) {
                    if (item.ToString() != "||||") {

                        bool _ret = false;

                        //Wifi Standard
                        string _wifi = "";
                        _ret = dictWifi.TryGetValue(item.wifi, out _wifi);

                        //Rate
                        string _mcs = "";
                        _ret = dictRate.TryGetValue(item.rate, out _mcs);

                        //Bandwidth
                        string _bw = "";
                        _ret = dictBandwidth.TryGetValue(item.bandwidth, out _bw);

                        //Channel Frequency
                        List<string> _lisch = new List<string>();
                        if (item.channelfreq.Contains(",")) {
                            string[] buffer = item.channelfreq.Split(',');
                            for (int i = 0; i < buffer.Length; i++) { _lisch.Add(buffer[i]); }
                        }
                        else _lisch.Add(item.channelfreq);

                        //Anten
                        List<string> _lisat = new List<string>();
                        if (item.anten.Contains(",")) {
                            string[] buffer = item.anten.Split(',');
                            for (int i = 0; i < buffer.Length; i++) { _lisat.Add(buffer[i]); }
                        }
                        else _lisat.Add(item.anten);

                        //Input data to list
                        foreach (var i in _lisat) {
                            foreach (var j in _lisch) {
                                _lo.Add(new verifysignal() { wifi = _wifi, rate = _mcs, bandwidth = _bw, anten = i, channelfreq = j });
                            }
                        }
                    }  
                }
                return true;
            }
            catch {
                return false;
            }
        }
        static bool _convert_tmpList_To_testList(List<sensivitity> _li, out List<sensivitity> _lo) {
            _lo = new List<sensivitity>();
            if (_li == null || _li.Count == 0) return false;

            try {
                // -- 802.11b | MCS0 | 20 | 1,2 | 2412,2437,2462 | 1000 --
                foreach (var item in _li) {
                    if (item.ToString() != "|||||0") {

                        bool _ret = false;

                        //Wifi Standard
                        string _wifi = "";
                        _ret = dictWifi.TryGetValue(item.wifi, out _wifi);

                        //Rate
                        string _mcs = "";
                        _ret = dictRate.TryGetValue(item.rate, out _mcs);

                        //Bandwidth
                        string _bw = "";
                        _ret = dictBandwidth.TryGetValue(item.bandwidth, out _bw);

                        //Channel Frequency
                        List<string> _lisch = new List<string>();
                        if (item.channelfreq.Contains(",")) {
                            string[] buffer = item.channelfreq.Split(',');
                            for (int i = 0; i < buffer.Length; i++) { _lisch.Add(buffer[i]); }
                        }
                        else _lisch.Add(item.channelfreq);

                        //Anten
                        List<string> _lisat = new List<string>();
                        if (item.anten.Contains(",")) {
                            string[] buffer = item.anten.Split(',');
                            for (int i = 0; i < buffer.Length; i++) { _lisat.Add(buffer[i]); }
                        }
                        else _lisat.Add(item.anten);

                        //Input data to list
                        foreach (var i in _lisat) {
                            foreach (var j in _lisch) {
                                _lo.Add(new sensivitity() { wifi = _wifi, rate = _mcs, bandwidth = _bw, anten = i, channelfreq = j, packet = item.packet });
                            }
                        }
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
