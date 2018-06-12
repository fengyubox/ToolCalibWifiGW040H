using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ToolCalibWifiForGW040H.Function {

    public class ModemTelnet {
        /*----------------------------------------------------------*/
        TelnetConnection telnet2modem;
        //RichTextBox rtb_temp = new RichTextBox();
        public int TimeOutMs = 400;
        public int maxModem_DELAY = 200;
        public int avgModem_DELAY = 100;
        public int minModem_DELAY = 50;

        /*----------------------------------------------------------*/
        public ModemTelnet(string IPaddress, int port) {
            try {
                telnet2modem = new TelnetConnection(IPaddress, port);
            }
            catch {
            };
        }
        /*----------------------------------------------------------*/
        public bool Login(string username, string pass, int timeout) {
            try {
                string str = telnet2modem.Login(username, pass, timeout);
                str = str.TrimEnd();
                str = str.Substring(str.Length - 1, 1);
                if ((str == "$") || (str == "#")) {
                    telnet2modem.WriteLine("iwpriv rai0 e2p f6");
                    string temp = telnet2modem.Read();
                    if (temp.Contains("[0x00F6]")) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else
                    return false;
            }
            catch {
                return false;
            };
        }
        /*----------------------------------------------------------*/
        public void TelnetClose() {
            telnet2modem.TelnetClose();
        }
        /*----------------------------------------------------------*/
        public void WriteLine(string cmd) {
            try {
                telnet2modem.WriteLine(cmd);
            }
            catch {

            };
        }
        /*----------------------------------------------------------*/
        public string Read() {
            string value = "NULL";
            try {
                value = telnet2modem.Read();
                return value;
            }
            catch {
                return value;
            };
        }
        /*----------------------------------------------------------*/
        // Ham ghi noi dung vao 1 Rich Text Box
        //private void SetText(RichTextBox rtb_temp, string text) {
        //    //Them hieu ung mau sac...vv cho text hien thi
        //    if (text.Contains("FAIL") || text.Contains("Fail") || text.Contains("ERROR") || text.Contains("Bai Test Wifi Chua The Thuc Hien!")) {
        //        rtb_temp.SelectionColor = Color.Red;
        //        //rtb_temp.SelectionFont = new Font(rtb_temp.SelectionFont, FontStyle.Bold);
        //    }
        //    else if (text.Contains("PASS") || text.Contains("Xong") || text.Contains("xong") || text.Contains("thanh cong") || text.Contains("Da Ping duoc")) {
        //        rtb_temp.SelectionColor = Color.Green;
        //        //rtb_temp.SelectionFont = new Font(rtb_temp.SelectionFont, FontStyle.Bold);
        //    }
        //    else if (text.Contains("Bai test thu:") || text.Contains("Do chuan") || text.Contains("Antena") || text.Contains("MAC Address=") || text.Contains("Firmware version")) {
        //        rtb_temp.SelectionColor = Color.Black;
        //        rtb_temp.SelectionFont = new Font(rtb_temp.SelectionFont, FontStyle.Bold);
        //    }
        //    else {
        //        rtb_temp.SelectionColor = Color.Black;
        //    }
        //    //
        //    rtb_temp.AppendText(text);
        //    rtb_temp.Refresh();
        //}
        ///*----------------------------------------------------------*/
        //// Ham xoa RichTextBox
        //public void ClearLog() {
        //    rtb_temp.Clear();
        //    rtb_temp.Refresh();
        //}
        ///*----------------------------------------------------------*/
        //// Lấy thông báo từ RichTextBox
        //public string GetLog() {
        //    return rtb_temp.Text;
        //}

        public void CalibPower_SendCommand(string Standard_2G_or_5G, string Anten, string Channel) {
            string ChannelNumber = Attenuator.getChannelNumber(Channel.Replace("000000", ""));
            List<string> ATEcommands = new List<string>() { string.Format("iwpriv {0} set ATE=ATESTART\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ATECHANNEL={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0", ChannelNumber),
                                                            string.Format("iwpriv {0} set ATETXMODE=1\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ATETXMCS=7\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ATETXBW=0\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ATETXANT={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0",Anten),
                                                            string.Format("iwpriv {0} set ATETXCNT=0\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ATE=TXFRAME\r\n", Standard_2G_or_5G == "2G" ? "ra0":"rai0") };

            try {
                if (telnet2modem.IsConnected) {
                    string _tmpStr = "";
                    foreach (var item in ATEcommands) {
                        //telnet2modem.WriteLine(item);
                        //telnet2modem.WriteLineAndWaitComplete(item);
                        _tmpStr += item;
                    }
                    telnet2modem.WriteLineAndWaitComplete(_tmpStr);
                    Thread.Sleep(50);
                }
            }
            catch (Exception Ex) {
                MessageBox.Show("ERROR CODE: " + Ex.ToString());
            }
        }

        /*-----------------------------------------------------------*/
        //Hàm gửi lệnh phát tín hiệu ở cả 2G và 5G
        public void Verify_Signal_SendCommand(string Standard_2G_or_5G, string Mode, string MCS, string BW, string Channel, string Anten, ref string message) {
            string ChannelNumber = Attenuator.getChannelNumber(Channel.Replace("000000", ""));
            List<string> ATEcommands = new List<string>() { string.Format("iwpriv {0} set ATE=ATESTART\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ATECHANNEL={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0", ChannelNumber),
                                                            string.Format("iwpriv {0} set ATETXMODE={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0", Mode),
                                                            string.Format("iwpriv {0} set ATETXMCS={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0", MCS),
                                                            string.Format("iwpriv {0} set ATETXBW={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0", BW),
                                                            string.Format("iwpriv {0} set ATETXANT={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0",Anten),
                                                            string.Format("iwpriv {0} set ATETXCNT=0\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ATE=TXFRAME\r\n", Standard_2G_or_5G == "2G" ? "ra0":"rai0") };

            try {
                if (telnet2modem.IsConnected) {
                    string _tmpStr = "";
                    foreach (var item in ATEcommands) {
                        //telnet2modem.WriteLine(item);
                        //telnet2modem.WriteLineAndWaitComplete(item);
                        _tmpStr += item;
                        message += item;
                    }
                    telnet2modem.WriteLineAndWaitComplete(_tmpStr);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception Ex) {
                MessageBox.Show("ERROR CODE: " + Ex.ToString());
            }
        }
        /*----------------------------------------------------------*/
        //Hàm gửi lệnh Calib tần số.
        public bool CalibFrequency_SendCommand(string mode, string rate, string bw, string channel, string annten, string FreOffset) {
            bool boolResult = false;
            string[] ATEcommand = { "iwpriv ra0 set ATE=ATESTART\r\n",
                                    "iwpriv ra0 set ATETXMODE=" + mode + "\r\n",       //mode=2 (HT_Mix)
                                    "iwpriv ra0 set ATETXMCS=" + rate + "\r\n",         //MCS
                                    "iwpriv ra0 set ATETXBW=" + bw + "\r\n",           // Bandwith
                                    "iwpriv ra0 set ATECHANNEL=" + channel + "\r\n",    // Channel
                                    "iwpriv ra0 set ATETXGI=0\r\n",
                                    "iwpriv ra0 set ATETXANT=" + annten + "\r\n",      // Annten
                                    "iwpriv ra0 set ATETXFREQOFFSET=" + FreOffset + "\r\n",
                                    "iwpriv ra0 set ATETXCNT=0\r\n",
                                    "iwpriv ra0 set ATE=TXFRAME\r\n"
                                  };
            try {

                if (telnet2modem.IsConnected) {
                    string _tmpStr = "";
                    for (int i = 0; i < 10; i++) {
                        //telnet2modem.WriteLine(ATEcommand[i]);
                        //telnet2modem.WriteLineAndWaitComplete(ATEcommand[i]);
                        _tmpStr += ATEcommand[i];
                    }
                    telnet2modem.WriteLineAndWaitComplete(_tmpStr);
                    Thread.Sleep(1000);
                    boolResult = true;
                }
            }
            catch {
                MessageBox.Show("ERROR CODE: [Modem_ConfigLAN] \n Loi! cau hinh cho Modem qua cong LAN\n");
                boolResult = false;
            }
            return boolResult;
        }

        /*----------------------------------------------------------*/
        //Hàm gửi lệnh Test Sensitivity.
        public bool TestSensitivity_SendCommand(string Standard_2G_or_5G, string mode, string rate, string bw, string channel, string annten, ref string message) {
            bool boolResult = false;
            List<string> ATEcommands = new List<string>() { string.Format("iwpriv {0} set ATE=ATESTART\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ResetCounter=0\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0"),
                                                            string.Format("iwpriv {0} set ATETXMODE={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0",mode),
                                                            string.Format("iwpriv {0} set ATETXMCS={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0",rate),
                                                            string.Format("iwpriv {0} set ATETXBW={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0",bw),
                                                            string.Format("iwpriv {0} set ATECHANNEL={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0",channel),
                                                            string.Format("iwpriv {0} set ATETXANT={1}\r\n",Standard_2G_or_5G == "2G" ? "ra0":"rai0",annten),
                                                            string.Format("iwpriv {0} set ATE=RXFRAME\r\n", Standard_2G_or_5G == "2G" ? "ra0":"rai0") };

            try {
                if (telnet2modem.IsConnected) {
                    string _tmpStr = "";
                    foreach (var item in ATEcommands) {
                        //telnet2modem.WriteLine(item);
                        //telnet2modem.WriteLineAndWaitComplete(item);
                        //message += item + "\n";
                        _tmpStr += item;
                        message += item;
                    }
                    telnet2modem.WriteLineAndWaitComplete(_tmpStr);
                    Thread.Sleep(1000);
                    boolResult = true;
                }
            }
            catch {
                MessageBox.Show("ERROR CODE: [Modem_ConfigLAN] \n Loi! cau hinh cho Modem qua cong LAN\n");
                boolResult = false;
            }
            return boolResult;
        }

        /*----------------------------------------------------------*/
        //Hàm gửi lệnh đọc PER
        public string TestSensitivity_ReadPER_SendCommand(string Standard_2G_or_5G, ref string _message) {
            string Result = "";
            string commandLine = Standard_2G_or_5G == "2G" ? "iwpriv ra0 stat" : "iwpriv rai0 stat";
            try {
                if (telnet2modem.IsConnected) {
                    telnet2modem.WriteLine(commandLine);
                    Result = telnet2modem.Read();
                    _message = Result;
                    string[] buffer = Result.Split('\n');
                    foreach (var item in buffer) {
                        if (item.Contains("Rx success")) {
                            Result = item.Split('=')[1].Replace("\r", "").Replace("\n", "").Trim();
                            break;
                        }
                    }
                }

            }
            catch {
                MessageBox.Show("ERROR CODE: [Modem_ConfigLAN] \n Loi! cau hinh cho Modem qua cong LAN\n");
                Result = "ERROR";
            }
            return Result;
        }

        //Hàm ghi offset vào thanh ghi DRAM
        public string Read_Register(string Register) {
            try {
                telnet2modem.WriteLine("iwpriv rai0 e2p " + Register);
                //Thread.Sleep(100);
                return telnet2modem.Read().Split('x')[2].Split('#')[0].Substring(0, 4);
            }
            catch {
                return "";
            }
        }

        //Hàm ghi offset vào thanh ghi DRAM
        public void Write_Register(string Register, string value) {
            //MessageBox.Show("iwpriv rai0 e2p " + Register + "=" + value);
            telnet2modem.WriteLine("iwpriv rai0 e2p " + Register + "=" + value);
            telnet2modem.Read();
            //Thread.Sleep(100);
        }

        //Hàm ghi dữ liệu calib lên Flash
        public void Write_into_Flash() {
            telnet2modem.WriteLine("iwpriv rai0 set efuseBufferModeWriteBack=1");
            Thread.Sleep(200);
            telnet2modem.WriteLine("tcapi set WLan11ac_Common WriteBinToFlash 1");
            Thread.Sleep(200);
            telnet2modem.WriteLineAndWaitComplete("tcapi commit WLan11ac");
            //telnet2modem.WriteLine("ifconfig ra0 down");
            //Thread.Sleep(1000);
            telnet2modem.WriteLineAndWaitComplete("ifconfig ra0 down");
            //telnet2modem.WriteLine("ifconfig rai0 down");
            //Thread.Sleep(1000);
            telnet2modem.WriteLineAndWaitComplete("ifconfig rai0 down");
            telnet2modem.WriteLineAndWaitComplete("rmmod mt7615_ap");
            telnet2modem.WriteLineAndWaitComplete("insmod lib/modules/mt7615_ap.ko");
            telnet2modem.WriteLineAndWaitComplete("ifconfig rai0 up");
            telnet2modem.WriteLineAndWaitComplete("ifconfig ra0 up");
            //telnet2modem.WriteLine("ifconfig ra0 up");
            //Thread.Sleep(1000);
        }

        //Hàm kiểm tra dữ liệu được lưu thành công chưa.
        public void Wl_Down_Up() {
            //telnet2modem.WriteLine("ifconfig rai0 down");
            //Thread.Sleep(1000);
            telnet2modem.WriteLineAndWaitComplete("ifconfig rai0 down");
            //telnet2modem.WriteLine("ifconfig rai0 up");
            //Thread.Sleep(1000);
            telnet2modem.WriteLineAndWaitComplete("ifconfig rai0 up");
        }

        //Hàm đọc địa chỉ MAC
        public string getMAC() {
            try {
                telnet2modem.WriteLine("ifconfig br0");
                Thread.Sleep(100);
                string tmpStr = telnet2modem.Read().Replace("\n", "").Replace("\r", "").Trim();
                string[] buffer = tmpStr.Split(new string[] { "HWaddr" }, StringSplitOptions.None);
                tmpStr = buffer[1].Trim();
                return tmpStr.Substring(0, 17);
            }
            catch {
                return "";
            }

        }

    }
}
