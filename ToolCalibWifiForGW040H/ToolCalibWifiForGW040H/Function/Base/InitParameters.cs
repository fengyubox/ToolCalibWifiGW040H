using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class InitParameters {

        public static List<string> ListInstrument = new List<string>() { "MT8870A", "E6640A" };
        public static List<string> ListRFPort = new List<string>() { "RFIO1", "RFIO2", "RFIO3", "RFIO4" };
        public static List<string> ListStation = new List<string>() { "Trước đóng vỏ", "Sau đóng vỏ" };

        public struct Statuses {
            public static string Ready = "Ready";
            public static string Wait = "Wait";
            public static string Waiting = "Waiting";
            public static string Pass = "PASS";
            public static string Fail = "FAIL";
            public static string None = "NONE";
            public static string Null = "NULL";
        }
    }
}
