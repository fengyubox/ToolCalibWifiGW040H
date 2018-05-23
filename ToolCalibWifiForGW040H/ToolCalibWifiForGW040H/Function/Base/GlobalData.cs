using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public static class GlobalData {

        static GlobalData() {
            LimitTx.readFromFile();
            LimitRx.readFromFile();
            Attenuator.readFromFile();
            WaveForm.readFromFile();
            ChannelManagement.readFromFile();
            TestCase.Load();
        }

        public static List<waveformInfo> listWaveForm = null;
        public static List<attenuatorInfo> listAttenuator = null;
        public static List<limittx> listLimitWifiTX = null;
        public static List<limitrx> listLimitWifiRX = null;
        public static List<channelmanagement> listChannel = null;

        public static logdata logManager = null;
        public static logregister logRegister = null;
        public static defaultSetting initSetting = new defaultSetting();
        public static testinginfo testingData = new testinginfo();

        public static List<verifysignal> tmplisttxWifi2G = null;
        public static List<verifysignal> tmplisttxWifi5G = null;
        public static List<sensivitity> tmplistrxWifi2G = null;
        public static List<sensivitity> tmplistrxWifi5G = null;
        public static List<verifysignal> tmplisttestAnten1 = null;
        public static List<verifysignal> tmplisttestAnten2 = null;

        public static List<verifysignal> listTestAnten1 = null;
        public static List<verifysignal> listTestAnten2 = null;
        public static List<verifysignal> listVerifySignal2G = null;
        public static List<verifysignal> listVerifySignal5G = null;
        public static List<sensivitity> listSensivitity2G = null;
        public static List<sensivitity> listSensivitity5G = null;

        //public static List<verifysignal> tmplisttxWifi2G = new List<verifysignal>() {
        //    new verifysignal() { wifi = "802.11n", rate = "MCS7", bandwidth = "20", anten="1,2", channelfreq = "2412,2437,2462"},
        //};

        //public static List<verifysignal> tmplisttxWifi5G = new List<verifysignal>() {
        //    new verifysignal() { wifi = "802.11n", rate = "MCS0", bandwidth = "80", anten="1,2", channelfreq = "5180,5500,5825"},
        //    new verifysignal() { wifi = "802.11ac", rate = "MCS9", bandwidth = "80", anten="1,2", channelfreq = "5210,5530,5775"},
        //};

        //public static List<sensivitity> tmplistrxWifi2G = new List<sensivitity>() {
        //    new sensivitity() { wifi = "802.11n", rate="MCS7", bandwidth = "20", anten = "1,2", channelfreq = "2462", packet=1000 },
        //};
        //public static List<sensivitity> tmplistrxWifi5G = new List<sensivitity>() {
        //    new sensivitity() { wifi = "802.11ac", rate="MCS9", bandwidth = "80", anten = "1,2", channelfreq = "5775", packet=1000 },
        //};

        //public static List<verifysignal> tmplisttestAnten1 = new List<verifysignal>() {
        //    new verifysignal() { wifi = "802.11n", rate = "MCS7", bandwidth = "20", anten = "1", channelfreq = "2437" }
        //};
        //public static List<verifysignal> tmplisttestAnten2 = new List<verifysignal>() {
        //    new verifysignal() { wifi = "802.11n", rate = "MCS7", bandwidth = "20", anten = "2", channelfreq = "2437" }
        //};

        //public static List<verifysignal> listTestAnten1 = new List<verifysignal>() {
        //    new verifysignal() { wifi = "3", rate = "7", bandwidth = "0", anten = "1", channelfreq = "2437" }
        //};

        //public static List<verifysignal> listTestAnten2 = new List<verifysignal>() {
        //    new verifysignal() { wifi = "3", rate = "7", bandwidth = "0", anten = "2", channelfreq = "2437" }
        //};

        ////Cau hinh bai test Verify Signal TX - 2G
        //public static List<verifysignal> listVerifySignal2G = new List<verifysignal>() {
        //    //ANTEN1
        //    //802.11nHT20 - MCS7
        //    new verifysignal() { wifi = "3", rate = "7", bandwidth = "0", anten = "1", channelfreq = "2412" },
        //    new verifysignal() { wifi = "3", rate = "7", bandwidth = "0", anten = "1", channelfreq = "2437" },
        //    new verifysignal() { wifi = "3", rate = "7", bandwidth = "0", anten = "1", channelfreq = "2462" },
        //    //ANTEN2
        //    //802.11nHT20 - MCS7
        //    new verifysignal() { wifi = "3", rate = "7", bandwidth = "0", anten = "2", channelfreq = "2412" },
        //    new verifysignal() { wifi = "3", rate = "7", bandwidth = "0", anten = "2", channelfreq = "2437" },
        //    new verifysignal() { wifi = "3", rate = "7", bandwidth = "0", anten = "2", channelfreq = "2462" },
        //};

        ////Cau hinh bai test Verify Signal TX - 5G
        //public static List<verifysignal> listVerifySignal5G = new List<verifysignal>() {
        //    //ANTEN1
        //    //802.11acHT80 - MCS0
        //     new verifysignal() { wifi = "4", rate = "0", bandwidth = "2", anten = "1", channelfreq = "5180" },
        //     new verifysignal() { wifi = "4", rate = "0", bandwidth = "2", anten = "1", channelfreq = "5500" },
        //     new verifysignal() { wifi = "4", rate = "0", bandwidth = "2", anten = "1", channelfreq = "5825" },
        //     //802.11acHT80 - MCS9
        //     new verifysignal() { wifi = "4", rate = "9", bandwidth = "2", anten = "1", channelfreq = "5180" },
        //     new verifysignal() { wifi = "4", rate = "9", bandwidth = "2", anten = "1", channelfreq = "5500" },
        //     new verifysignal() { wifi = "4", rate = "9", bandwidth = "2", anten = "1", channelfreq = "5825" },
        //    //ANTEN2
        //    //802.11acHT80 - MCS0
        //     new verifysignal() { wifi = "4", rate = "0", bandwidth = "2", anten = "2", channelfreq = "5180" },
        //     new verifysignal() { wifi = "4", rate = "0", bandwidth = "2", anten = "2", channelfreq = "5500" },
        //     new verifysignal() { wifi = "4", rate = "0", bandwidth = "2", anten = "2", channelfreq = "5825" },
        //     //802.11acHT80 - MCS9
        //     new verifysignal() { wifi = "4", rate = "9", bandwidth = "2", anten = "2", channelfreq = "5180" },
        //     new verifysignal() { wifi = "4", rate = "9", bandwidth = "2", anten = "2", channelfreq = "5500" },
        //     new verifysignal() { wifi = "4", rate = "9", bandwidth = "2", anten = "2", channelfreq = "5825" },
        //};

        ////Cau hinh bai test Sensivitity RX - 2G
        //public static List<sensivitity> listSensivitity2G = new List<sensivitity>() {
        //    //802.11nHT20 ----------------------------------------------------------//
        //    //MCS7 - ANTEN1
        //    new sensivitity() { wifi = "3", rate = "7", bandwidth = "0", channelfreq = "2462", anten = "1", packet = 1000},
        //    //MCS7 - ANTEN2
        //    new sensivitity() { wifi = "3", rate = "7", bandwidth = "0", channelfreq = "2462", anten = "2", packet = 1000},
        //};

        ////Cau hinh bai test Sensivitity RX - 5G
        //public static List<sensivitity> listSensivitity5G = new List<sensivitity>() {
        //    //802.11acHT80 ----------------------------------------------------------//
        //    //MCS9 - ANTEN1
        //    new sensivitity() { wifi = "4", rate = "9", bandwidth = "2", channelfreq = "5775", anten = "1", packet = 1000},
        //    //MCS9 - ANTEN2
        //    new sensivitity() { wifi = "4", rate = "9", bandwidth = "2", channelfreq = "5775", anten = "2", packet = 1000},
        //};


        //Cau hinh bai test Calib Power TX - 2G

        public static List<calibpower> listCalibPower2G = new List<calibpower>() {
            //ANTEN1
            new calibpower() { anten = "1", channelfreq = "2422", register = "0x59"},
            new calibpower() { anten = "1", channelfreq = "2447", register = "0x5A"},
            new calibpower() { anten = "1", channelfreq = "2472", register = "0x5B"},
            //ANTEN2
            new calibpower() { anten = "2", channelfreq = "2422", register = "0x5F"},
            new calibpower() { anten = "2", channelfreq = "2447", register = "0x60"},
            new calibpower() { anten = "2", channelfreq = "2472", register = "0x61"},
        };

        //Cau hinh bai test Calib Power TX - 5G
        public static List<calibpower> listCalibPower5G = new List<calibpower>() {
            //ATEN1
            new calibpower() { anten = "1", channelfreq = "4920", register = "0x143"},
            new calibpower() { anten = "1", channelfreq = "5080", register = "0x144"},
            new calibpower() { anten = "1", channelfreq = "5180", register = "0x148"},
            new calibpower() { anten = "1", channelfreq = "5240", register = "0x149"},
            new calibpower() { anten = "1", channelfreq = "5260", register = "0x14D"},
            new calibpower() { anten = "1", channelfreq = "5320", register = "0x14E"},
            new calibpower() { anten = "1", channelfreq = "5500", register = "0x157"},
            new calibpower() { anten = "1", channelfreq = "5580", register = "0x158"},
            new calibpower() { anten = "1", channelfreq = "5600", register = "0x15C"},
            new calibpower() { anten = "1", channelfreq = "5680", register = "0x15D"},
            new calibpower() { anten = "1", channelfreq = "5700", register = "0x161"},
            new calibpower() { anten = "1", channelfreq = "5785", register = "0x162"},
            new calibpower() { anten = "1", channelfreq = "5805", register = "0x166"},
            new calibpower() { anten = "1", channelfreq = "5825", register = "0x167"},

            //ATEN2
            new calibpower() { anten = "2", channelfreq = "4920", register = "0x16B"},
            new calibpower() { anten = "2", channelfreq = "5080", register = "0x16C"},
            new calibpower() { anten = "2", channelfreq = "5180", register = "0x170"},
            new calibpower() { anten = "2", channelfreq = "5240", register = "0x171"},
            new calibpower() { anten = "2", channelfreq = "5260", register = "0x175"},
            new calibpower() { anten = "2", channelfreq = "5320", register = "0x176"},
            new calibpower() { anten = "2", channelfreq = "5500", register = "0x17F"},
            new calibpower() { anten = "2", channelfreq = "5580", register = "0x180"},
            new calibpower() { anten = "2", channelfreq = "5600", register = "0x184"},
            new calibpower() { anten = "2", channelfreq = "5680", register = "0x185"},
            new calibpower() { anten = "2", channelfreq = "5700", register = "0x189"},
            new calibpower() { anten = "2", channelfreq = "5785", register = "0x18A"},
            new calibpower() { anten = "2", channelfreq = "5805", register = "0x18E"},
            new calibpower() { anten = "2", channelfreq = "5825", register = "0x18F"},
        };

    }

}
