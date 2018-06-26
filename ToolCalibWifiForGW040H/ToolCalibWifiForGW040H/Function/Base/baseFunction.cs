using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function
{
    public class baseFunction
    {
        //Kết nối tới ONT
        static bool TelnetConnect_ONT(string ip, string user, string pass) {
            try {
                GlobalData.MODEM = new ModemTelnet(ip, 23);
                if (!GlobalData.MODEM.Login(user, pass, 400)) {
                    return false;
                }
                else {
                    GlobalData.testingData.MACADDRESS = GlobalData.MODEM.getMAC();
                    return true;
                }
            }
            catch (Exception Ex) {
                Ex.ToString();
                return false;
            }
        }

        //Ket noi toi thiet bi do
        public static bool Connect_Function() {
            int counter = 0;
            while (true) {
                if (!TelnetConnect_ONT(GlobalData.initSetting.ONTIP, GlobalData.initSetting.ONTUSER, GlobalData.initSetting.ONTPASS)) {
                    counter++;
                    GlobalData.testingData.LOGSYSTEM = string.Format("[FAIL] Telnet to ONT FAIL => Retry {0}\r\n", counter);
                    Thread.Sleep(500);
                    if (counter >= 20) return false;
                }
                else {
                    GlobalData.testingData.LOGSYSTEM = "[OK] Telnet to ONT Successful.\r\n";
                    GlobalData.testingData.LOGSYSTEM += string.Format("ONT MAC Address: {0}\r\n", GlobalData.testingData.MACADDRESS);

                    if (GlobalData.initSetting.INSTRUMENT == "E6640A") {
                        GlobalData.INSTRUMENT = new E6640A_VISA(GlobalData.initSetting.VISAADDRESS);
                    }
                    else if (GlobalData.initSetting.INSTRUMENT == "MT8870A") {
                        GlobalData.INSTRUMENT = new MT8870A_VISA(GlobalData.initSetting.VISAADDRESS);
                    }
                    return true;
                }
            }
        }

    }
}
