using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {
    public class CalculateAttenuator {

        ModemTelnet _modem = null;
        Instrument _instrument = null;

        public CalculateAttenuator(ModemTelnet _mt, Instrument _it) {
            this._modem = _mt;
            this._instrument = _it;
        }

        public bool Excute() {
            try {

                return true;
            } catch {
                return false;
            }
        }

    }
}
