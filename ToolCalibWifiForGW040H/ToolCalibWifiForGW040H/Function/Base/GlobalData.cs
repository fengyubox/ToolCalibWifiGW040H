using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            BIN.readFromFile();
            TestCase.Load();
            
        }

        public static int mtIndex = 0;
        public static bool mtIsOk = true;

        public static ModemTelnet MODEM = null;
        public static Instrument INSTRUMENT = null;

        public static List<waveformInfo> listWaveForm = null;
        public static List<attenuatorInfo> listAttenuator = null;
        public static List<limittx> listLimitWifiTX = null;
        public static List<limitrx> listLimitWifiRX = null;
        public static List<channelmanagement> listChannel = null;
        public static List<masterinformation> listMasterData = null;
        public static List<binregister> ListBinRegister = new List<binregister>();

        public static logdata logManager = null;
        public static logregister logRegister = null;
        public static defaultSetting initSetting = new defaultSetting();
        public static testinginfo testingData = new testinginfo();

        public static ObservableCollection<logreviewregister> reviewRegister = new ObservableCollection<logreviewregister>();
        public static ObservableCollection<logreviewtx> reviewTX = new ObservableCollection<logreviewtx>();
        public static ObservableCollection<logreviewrx> reviewRX = new ObservableCollection<logreviewrx>();
        public static ObservableCollection<logreviewtx> datagridlogTX = new ObservableCollection<logreviewtx>();

        public static ObservableCollection<autoattenuator> autoAttenuator = new ObservableCollection<autoattenuator>();
        public static ObservableCollection<calmaster> autoCalculateMaster = new ObservableCollection<calmaster>();

        public static List<verifysignal> tmplisttxWifi2G = null;
        public static List<verifysignal> tmplisttxWifi5G = null;
        public static List<sensivitity> tmplistrxWifi2G = null;
        public static List<sensivitity> tmplistrxWifi5G = null;
        public static List<verifysignal> tmplisttestAnten1 = null;
        public static List<verifysignal> tmplisttestAnten2 = null;
        public static List<verifysignal> tmplistCalAttenuator = null;

        public static List<verifysignal> listTestAnten1 = null;
        public static List<verifysignal> listTestAnten2 = null;
        public static List<verifysignal> listVerifySignal2G = null;
        public static List<verifysignal> listVerifySignal5G = null;
        public static List<sensivitity> listSensivitity2G = null;
        public static List<sensivitity> listSensivitity5G = null;
        public static List<verifysignal> listCalAttenuator = null;
        public static List<verifysignal> listCalMaster = null;

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
