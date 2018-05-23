using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolCalibWifiForGW040H.Function {

    class FunctionSupport {
        public static string PowerOffsetValue(string offset) {
            Int32 i_offset;
            string StrOffset = "";
            i_offset = Int32.Parse("A", System.Globalization.NumberStyles.Float);
            i_offset = i_offset / 2;
            //StrOffset = i_offset.ToString();
            int num = Int32.Parse(i_offset.ToString(), System.Globalization.NumberStyles.HexNumber);
            StrOffset = num.ToString("X2").Trim();

            return StrOffset;

        }

        public static string FrequencyOffsetValue(string offset) {
            Int32 i_Freqoffset;
            i_Freqoffset = Int32.Parse(offset);     //offset = Hz
            i_Freqoffset = i_Freqoffset / 2000;  //2KHz = 1 Step.
            return i_Freqoffset.ToString();
        }

        //Hàm chỉ chuyển đổi số DEC sang số nhị phân, không thực hiện thao tác gì khác.
        public static string DECtoBin(int a) {
            int i;
            int du;
            string str = "";//Lưu số dư của phép chia cho 2

            do {
                du = a % 2;//Lấy số dư
                a = a / 2;//Lấy phần nguyên
                str += du.ToString();//Lưu số dư
            }
            while (a > 0);
            string temp = "";
            for (i = str.Length - 1; i >= 0; i--) {
                temp += str[i];
            }
            return temp;
        }

        //Hàm chuyển từ dạng Nhị Phân sang Hexa (Step9 -> Step10)
        public static string BintoHexOnly(string temp) {
            string Tach1 = temp[0].ToString() + temp[1].ToString() + temp[2].ToString() + temp[3].ToString();
            string Tach2 = temp[4].ToString() + temp[5].ToString() + temp[6].ToString() + temp[7].ToString();

            string Tach1_Hexa = "";
            string Tach2_Hexa = "";
            switch (Int32.Parse(Tach1)) {
                case 0000:
                    Tach1_Hexa = "0"; break;
                case 0001:
                    Tach1_Hexa = "1"; break;
                case 0010:
                    Tach1_Hexa = "2"; break;
                case 0011:
                    Tach1_Hexa = "3"; break;
                case 0100:
                    Tach1_Hexa = "4"; break;
                case 0101:
                    Tach1_Hexa = "5"; break;
                case 0110:
                    Tach1_Hexa = "6"; break;
                case 0111:
                    Tach1_Hexa = "7"; break;
                case 1000:
                    Tach1_Hexa = "8"; break;
                case 1001:
                    Tach1_Hexa = "9"; break;
                case 1010:
                    Tach1_Hexa = "A"; break;
                case 1011:
                    Tach1_Hexa = "B"; break;
                case 1100:
                    Tach1_Hexa = "C"; break;
                case 1101:
                    Tach1_Hexa = "D"; break;
                case 1110:
                    Tach1_Hexa = "E"; break;
                case 1111:
                    Tach1_Hexa = "F"; break;
            }

            switch (Int32.Parse(Tach2)) {
                case 0000:
                    Tach2_Hexa = "0"; break;
                case 0001:
                    Tach2_Hexa = "1"; break;
                case 0010:
                    Tach2_Hexa = "2"; break;
                case 0011:
                    Tach2_Hexa = "3"; break;
                case 0100:
                    Tach2_Hexa = "4"; break;
                case 0101:
                    Tach2_Hexa = "5"; break;
                case 0110:
                    Tach2_Hexa = "6"; break;
                case 0111:
                    Tach2_Hexa = "7"; break;
                case 1000:
                    Tach2_Hexa = "8"; break;
                case 1001:
                    Tach2_Hexa = "9"; break;
                case 1010:
                    Tach2_Hexa = "A"; break;
                case 1011:
                    Tach2_Hexa = "B"; break;
                case 1100:
                    Tach2_Hexa = "C"; break;
                case 1101:
                    Tach2_Hexa = "D"; break;
                case 1110:
                    Tach2_Hexa = "E"; break;
                case 1111:
                    Tach2_Hexa = "F"; break;
            }
            string ketqua = Tach1_Hexa + Tach2_Hexa;
            return ketqua;
        }

        //Hàm chuyển đổi hệ thập phân sang nhị phân, chèn 2 bit 11 vào 2 bit cao nhất, rồi lại chuyển sang Hexa
        public static string DECtoHex(int a) {
            int i;
            int du;
            string str = "";//Lưu số dư của phép chia cho 2

            do {
                du = a % 2;//Lấy số dư
                a = a / 2;//Lấy phần nguyên
                str += du.ToString();//Lưu số dư
            }
            while (a > 0);
            string temp = "";
            for (i = str.Length - 1; i >= 0; i--) {
                temp += str[i];
            }
            //return temp;


            //string Ghepthem = Binary(n);
            switch (temp.Length) {
                case 2:
                    temp = "110000" + temp; break;
                case 3:
                    temp = "11000" + temp; break;
                case 4:
                    temp = "1100" + temp; break;
                case 5:
                    temp = "110" + temp; break;
                case 6:
                    temp = "11" + temp; break;
            }
            string Tach1 = temp[0].ToString() + temp[1].ToString() + temp[2].ToString() + temp[3].ToString();
            string Tach2 = temp[4].ToString() + temp[5].ToString() + temp[6].ToString() + temp[7].ToString();


            string Tach1_Hexa = "";
            string Tach2_Hexa = "";
            switch (Int32.Parse(Tach1)) {
                case 0000:
                    Tach1_Hexa = "0"; break;
                case 0001:
                    Tach1_Hexa = "1"; break;
                case 0010:
                    Tach1_Hexa = "2"; break;
                case 0011:
                    Tach1_Hexa = "3"; break;
                case 0100:
                    Tach1_Hexa = "4"; break;
                case 0101:
                    Tach1_Hexa = "5"; break;
                case 0110:
                    Tach1_Hexa = "6"; break;
                case 0111:
                    Tach1_Hexa = "7"; break;
                case 1000:
                    Tach1_Hexa = "8"; break;
                case 1001:
                    Tach1_Hexa = "9"; break;
                case 1010:
                    Tach1_Hexa = "A"; break;
                case 1011:
                    Tach1_Hexa = "B"; break;
                case 1100:
                    Tach1_Hexa = "C"; break;
                case 1101:
                    Tach1_Hexa = "D"; break;
                case 1110:
                    Tach1_Hexa = "E"; break;
                case 1111:
                    Tach1_Hexa = "F"; break;
            }

            switch (Int32.Parse(Tach2)) {
                case 0000:
                    Tach2_Hexa = "0"; break;
                case 0001:
                    Tach2_Hexa = "1"; break;
                case 0010:
                    Tach2_Hexa = "2"; break;
                case 0011:
                    Tach2_Hexa = "3"; break;
                case 0100:
                    Tach2_Hexa = "4"; break;
                case 0101:
                    Tach2_Hexa = "5"; break;
                case 0110:
                    Tach2_Hexa = "6"; break;
                case 0111:
                    Tach2_Hexa = "7"; break;
                case 1000:
                    Tach2_Hexa = "8"; break;
                case 1001:
                    Tach2_Hexa = "9"; break;
                case 1010:
                    Tach2_Hexa = "A"; break;
                case 1011:
                    Tach2_Hexa = "B"; break;
                case 1100:
                    Tach2_Hexa = "C"; break;
                case 1101:
                    Tach2_Hexa = "D"; break;
                case 1110:
                    Tach2_Hexa = "E"; break;
                case 1111:
                    Tach2_Hexa = "F"; break;
            }
            string ketqua = Tach1_Hexa + Tach2_Hexa;
            return ketqua;
        }

        // Hàm cho Step 3
        // Hàm cộng thanh ghi F4:bit[6:0] với thanh ghi F6:bit[5:0] để tính offset tần số.
        // Dạng input: F4 = 0x00B7, F6 = 0x888F
        public static string Plus_F4_With_F6(string ChuoiF4, string ChuoiF6) {
            string F4 = "";
            string F6 = "";
            F4 = ChuoiF4.Substring(4, 2);
            string F4_1 = F4.Substring(0, 1);
            string F4_2 = F4.Substring(1, 1);

            F6 = ChuoiF6.Substring(4, 2);
            string F6_1 = F6.Substring(0, 1);
            string F6_2 = F6.Substring(1, 1);

            string F4toBin1 = "";
            switch (F4_1) {
                case "0": F4toBin1 = "0000"; break;
                case "1": F4toBin1 = "0001"; break;
                case "2": F4toBin1 = "0010"; break;
                case "3": F4toBin1 = "0011"; break;
                case "4": F4toBin1 = "0100"; break;
                case "5": F4toBin1 = "0101"; break;
                case "6": F4toBin1 = "0110"; break;
                case "7": F4toBin1 = "0111"; break;
                case "8": F4toBin1 = "1000"; break;
                case "9": F4toBin1 = "1001"; break;
                case "A": F4toBin1 = "1010"; break;
                case "B": F4toBin1 = "1011"; break;
                case "C": F4toBin1 = "1100"; break;
                case "D": F4toBin1 = "1101"; break;
                case "E": F4toBin1 = "1110"; break;
                case "F": F4toBin1 = "1111"; break;
            }

            string F4toBin2 = "";
            switch (F4_2) {
                case "0": F4toBin2 = "0000"; break;
                case "1": F4toBin2 = "0001"; break;
                case "2": F4toBin2 = "0010"; break;
                case "3": F4toBin2 = "0011"; break;
                case "4": F4toBin2 = "0100"; break;
                case "5": F4toBin2 = "0101"; break;
                case "6": F4toBin2 = "0110"; break;
                case "7": F4toBin2 = "0111"; break;
                case "8": F4toBin2 = "1000"; break;
                case "9": F4toBin2 = "1001"; break;
                case "A": F4toBin2 = "1010"; break;
                case "B": F4toBin2 = "1011"; break;
                case "C": F4toBin2 = "1100"; break;
                case "D": F4toBin2 = "1101"; break;
                case "E": F4toBin2 = "1110"; break;
                case "F": F4toBin2 = "1111"; break;
            }
            string F4toBin = F4toBin1 + F4toBin2;
            //////////////////////////
            string F6toBin1 = "";
            switch (F6_1) {
                case "0": F6toBin1 = "0000"; break;
                case "1": F6toBin1 = "0001"; break;
                case "2": F6toBin1 = "0010"; break;
                case "3": F6toBin1 = "0011"; break;
                case "4": F6toBin1 = "0100"; break;
                case "5": F6toBin1 = "0101"; break;
                case "6": F6toBin1 = "0110"; break;
                case "7": F6toBin1 = "0111"; break;
                case "8": F6toBin1 = "1000"; break;
                case "9": F6toBin1 = "1001"; break;
                case "A": F6toBin1 = "1010"; break;
                case "B": F6toBin1 = "1011"; break;
                case "C": F6toBin1 = "1100"; break;
                case "D": F6toBin1 = "1101"; break;
                case "E": F6toBin1 = "1110"; break;
                case "F": F6toBin1 = "1111"; break;
            }

            string F6toBin2 = "";
            switch (F6_2) {
                case "0": F6toBin2 = "0000"; break;
                case "1": F6toBin2 = "0001"; break;
                case "2": F6toBin2 = "0010"; break;
                case "3": F6toBin2 = "0011"; break;
                case "4": F6toBin2 = "0100"; break;
                case "5": F6toBin2 = "0101"; break;
                case "6": F6toBin2 = "0110"; break;
                case "7": F6toBin2 = "0111"; break;
                case "8": F6toBin2 = "1000"; break;
                case "9": F6toBin2 = "1001"; break;
                case "A": F6toBin2 = "1010"; break;
                case "B": F6toBin2 = "1011"; break;
                case "C": F6toBin2 = "1100"; break;
                case "D": F6toBin2 = "1101"; break;
                case "E": F6toBin2 = "1110"; break;
                case "F": F6toBin2 = "1111"; break;
            }
            string F6toBin = F6toBin1 + F6toBin2;

            string F4Split = "";
            F4Split = F4toBin.Substring(1, 7);

            string F6Split = "";
            F6Split = F6toBin.Substring(2, 6);

            string F6toBin_vitri6 = F6toBin.Substring(1, 1);
            if (Int32.Parse(F6toBin_vitri6) == 0) {
                // Cong 2 so nhi phan F4[6:0] + F6[5:0]
                long F4SplittoDecimal = Convert.ToInt64(F4Split, 2);
                long F6SplittoDecimal = Convert.ToInt64(F6Split, 2);
                long KetquacongThapphan = F4SplittoDecimal + F6SplittoDecimal;
                return KetquacongThapphan.ToString();
            }
            else {
                // Tru 2 so nhi phan F4[6:0] - F6[5:0]
                long F4SplittoDecimal = Convert.ToInt64(F4Split, 2);
                long F6SplittoDecimal = Convert.ToInt64(F6Split, 2);
                long KetquatruThapphan = F4SplittoDecimal - F6SplittoDecimal;
                return KetquatruThapphan.ToString();
            }
        }

        //Hàm cộng thanh ghi 58,59. Đầu vào là giá trị thanh 58,59 ví dụ C0, 20
        public static string Plus_F58_With_F59(string ChuoiF58, string ChuoiF59) {
            //string F58 = "";
            //string F59 = "";

            string F58_1 = ChuoiF58.Substring(0, 1);
            string F58_2 = ChuoiF58.Substring(1, 1);


            string F59_1 = ChuoiF59.Substring(0, 1);
            string F59_2 = ChuoiF59.Substring(1, 1);

            string F58toBin1 = "";
            switch (F58_1) {
                case "0": F58toBin1 = "0000"; break;
                case "1": F58toBin1 = "0001"; break;
                case "2": F58toBin1 = "0010"; break;
                case "3": F58toBin1 = "0011"; break;
                case "4": F58toBin1 = "0100"; break;
                case "5": F58toBin1 = "0101"; break;
                case "6": F58toBin1 = "0110"; break;
                case "7": F58toBin1 = "0111"; break;
                case "8": F58toBin1 = "1000"; break;
                case "9": F58toBin1 = "1001"; break;
                case "A": F58toBin1 = "1010"; break;
                case "B": F58toBin1 = "1011"; break;
                case "C": F58toBin1 = "1100"; break;
                case "D": F58toBin1 = "1101"; break;
                case "E": F58toBin1 = "1110"; break;
                case "F": F58toBin1 = "1111"; break;
            }

            string F58toBin2 = "";
            switch (F58_2) {
                case "0": F58toBin2 = "0000"; break;
                case "1": F58toBin2 = "0001"; break;
                case "2": F58toBin2 = "0010"; break;
                case "3": F58toBin2 = "0011"; break;
                case "4": F58toBin2 = "0100"; break;
                case "5": F58toBin2 = "0101"; break;
                case "6": F58toBin2 = "0110"; break;
                case "7": F58toBin2 = "0111"; break;
                case "8": F58toBin2 = "1000"; break;
                case "9": F58toBin2 = "1001"; break;
                case "A": F58toBin2 = "1010"; break;
                case "B": F58toBin2 = "1011"; break;
                case "C": F58toBin2 = "1100"; break;
                case "D": F58toBin2 = "1101"; break;
                case "E": F58toBin2 = "1110"; break;
                case "F": F58toBin2 = "1111"; break;
            }
            string F58toBin = F58toBin1 + F58toBin2;
            //////////////////////////
            string F59toBin1 = "";
            switch (F59_1) {
                case "0": F59toBin1 = "0000"; break;
                case "1": F59toBin1 = "0001"; break;
                case "2": F59toBin1 = "0010"; break;
                case "3": F59toBin1 = "0011"; break;
                case "4": F59toBin1 = "0100"; break;
                case "5": F59toBin1 = "0101"; break;
                case "6": F59toBin1 = "0110"; break;
                case "7": F59toBin1 = "0111"; break;
                case "8": F59toBin1 = "1000"; break;
                case "9": F59toBin1 = "1001"; break;
                case "A": F59toBin1 = "1010"; break;
                case "B": F59toBin1 = "1011"; break;
                case "C": F59toBin1 = "1100"; break;
                case "D": F59toBin1 = "1101"; break;
                case "E": F59toBin1 = "1110"; break;
                case "F": F59toBin1 = "1111"; break;
            }

            string F59toBin2 = "";
            switch (F59_2) {
                case "0": F59toBin2 = "0000"; break;
                case "1": F59toBin2 = "0001"; break;
                case "2": F59toBin2 = "0010"; break;
                case "3": F59toBin2 = "0011"; break;
                case "4": F59toBin2 = "0100"; break;
                case "5": F59toBin2 = "0101"; break;
                case "6": F59toBin2 = "0110"; break;
                case "7": F59toBin2 = "0111"; break;
                case "8": F59toBin2 = "1000"; break;
                case "9": F59toBin2 = "1001"; break;
                case "A": F59toBin2 = "1010"; break;
                case "B": F59toBin2 = "1011"; break;
                case "C": F59toBin2 = "1100"; break;
                case "D": F59toBin2 = "1101"; break;
                case "E": F59toBin2 = "1110"; break;
                case "F": F59toBin2 = "1111"; break;
            }
            string F59toBin = F59toBin1 + F59toBin2;

            string F58Split = "";
            F58Split = F58toBin.Substring(1, 7);

            string F59Split = "";
            F59Split = F59toBin.Substring(2, 6);

            string F59toBin_vitri6 = F59toBin.Substring(1, 1);
            if (Int32.Parse(F59toBin_vitri6) == 1) {
                // Cong 2 so nhi phan F58[6:0] + F59[5:0]
                long F58SplittoDecimal = Convert.ToInt64(F58Split, 2);
                long F59SplittoDecimal = Convert.ToInt64(F59Split, 2);
                long KetquacongThapphan = F58SplittoDecimal + F59SplittoDecimal;
                return KetquacongThapphan.ToString();
            }
            else {
                // Tru 2 so nhi phan F58[6:0] - F59[5:0]
                long F58SplittoDecimal = Convert.ToInt64(F58Split, 2);
                long F59SplittoDecimal = Convert.ToInt64(F59Split, 2);
                long KetquatruThapphan = F58SplittoDecimal - F59SplittoDecimal;
                return KetquatruThapphan.ToString();
            }
        }

        //Hàm đổi từ Hexa sang Binary. Đầu vào dạng A5 hoặc C3.....
        public static string HextoBin(string ChuoiF4) {
            //string F4 = "";
            //F4 = ChuoiF4.Substring(4, 2);
            string F4_1 = ChuoiF4.Substring(0, 1);
            string F4_2 = ChuoiF4.Substring(1, 1);

            string F4toBin1 = "";
            switch (F4_1) {
                case "0": F4toBin1 = "0000"; break;
                case "1": F4toBin1 = "0001"; break;
                case "2": F4toBin1 = "0010"; break;
                case "3": F4toBin1 = "0011"; break;
                case "4": F4toBin1 = "0100"; break;
                case "5": F4toBin1 = "0101"; break;
                case "6": F4toBin1 = "0110"; break;
                case "7": F4toBin1 = "0111"; break;
                case "8": F4toBin1 = "1000"; break;
                case "9": F4toBin1 = "1001"; break;
                case "A": F4toBin1 = "1010"; break;
                case "B": F4toBin1 = "1011"; break;
                case "C": F4toBin1 = "1100"; break;
                case "D": F4toBin1 = "1101"; break;
                case "E": F4toBin1 = "1110"; break;
                case "F": F4toBin1 = "1111"; break;
            }

            string F4toBin2 = "";
            switch (F4_2) {
                case "0": F4toBin2 = "0000"; break;
                case "1": F4toBin2 = "0001"; break;
                case "2": F4toBin2 = "0010"; break;
                case "3": F4toBin2 = "0011"; break;
                case "4": F4toBin2 = "0100"; break;
                case "5": F4toBin2 = "0101"; break;
                case "6": F4toBin2 = "0110"; break;
                case "7": F4toBin2 = "0111"; break;
                case "8": F4toBin2 = "1000"; break;
                case "9": F4toBin2 = "1001"; break;
                case "A": F4toBin2 = "1010"; break;
                case "B": F4toBin2 = "1011"; break;
                case "C": F4toBin2 = "1100"; break;
                case "D": F4toBin2 = "1101"; break;
                case "E": F4toBin2 = "1110"; break;
                case "F": F4toBin2 = "1111"; break;
            }
            string F4toBin = F4toBin1 + F4toBin2;
            return F4toBin;
        }

        //Hàm đổi từ Hexa sang Decimal để thực hiện Step 7.Đầu vào dạng 0x00AC..
        public static string HextoDec(string ChuoiF4) {
            //string F4 = "";
            //F4 = ChuoiF4.Substring(4, 2);
            string F4_1 = ChuoiF4.Substring(0, 1);
            string F4_2 = ChuoiF4.Substring(1, 1);

            string F4toBin1 = "";
            switch (F4_1) {
                case "0": F4toBin1 = "0000"; break;
                case "1": F4toBin1 = "0001"; break;
                case "2": F4toBin1 = "0010"; break;
                case "3": F4toBin1 = "0011"; break;
                case "4": F4toBin1 = "0100"; break;
                case "5": F4toBin1 = "0101"; break;
                case "6": F4toBin1 = "0110"; break;
                case "7": F4toBin1 = "0111"; break;
                case "8": F4toBin1 = "1000"; break;
                case "9": F4toBin1 = "1001"; break;
                case "A": F4toBin1 = "1010"; break;
                case "B": F4toBin1 = "1011"; break;
                case "C": F4toBin1 = "1100"; break;
                case "D": F4toBin1 = "1101"; break;
                case "E": F4toBin1 = "1110"; break;
                case "F": F4toBin1 = "1111"; break;
            }

            string F4toBin2 = "";
            switch (F4_2) {
                case "0": F4toBin2 = "0000"; break;
                case "1": F4toBin2 = "0001"; break;
                case "2": F4toBin2 = "0010"; break;
                case "3": F4toBin2 = "0011"; break;
                case "4": F4toBin2 = "0100"; break;
                case "5": F4toBin2 = "0101"; break;
                case "6": F4toBin2 = "0110"; break;
                case "7": F4toBin2 = "0111"; break;
                case "8": F4toBin2 = "1000"; break;
                case "9": F4toBin2 = "1001"; break;
                case "A": F4toBin2 = "1010"; break;
                case "B": F4toBin2 = "1011"; break;
                case "C": F4toBin2 = "1100"; break;
                case "D": F4toBin2 = "1101"; break;
                case "E": F4toBin2 = "1110"; break;
                case "F": F4toBin2 = "1111"; break;
            }
            string F4toBin = F4toBin1 + F4toBin2;
            long F4toDec = Convert.ToInt64(F4toBin, 2);
            return F4toDec.ToString();
        }

        //Hàm cho Step 9: Kiểm tra F6>0 hay F6<0. Nếu F6> 0 thì chuyển về dạng F6 = 10xxxxxx, Nếu F6<0 thì chuyển về dạng F6 = 11xxxxxx. Input F6 dạng thập phân
        public static string KiemtraF6(string dauhieu, int F6) {
            string F6toBin = DECtoBin(F6);

            if (dauhieu == "Can Tang") {
                switch (F6toBin.Length) {
                    case 6: F6toBin = "10" + F6toBin; break;
                    case 5: F6toBin = "100" + F6toBin; break;
                    case 4: F6toBin = "1000" + F6toBin; break;
                    case 3: F6toBin = "10000" + F6toBin; break;
                    case 2: F6toBin = "100000" + F6toBin; break;
                    case 1: F6toBin = "1000000" + F6toBin; break;
                }
                return F6toBin;
            }
            else {
                int F6_trituyetdoi = Math.Abs(Convert.ToInt32(F6));
                string F6_trituyetdoi_toBin = DECtoBin(F6_trituyetdoi);
                switch (F6_trituyetdoi_toBin.Length) {
                    case 6: F6_trituyetdoi_toBin = "11" + F6_trituyetdoi_toBin; break;
                    case 5: F6_trituyetdoi_toBin = "110" + F6_trituyetdoi_toBin; break;
                    case 4: F6_trituyetdoi_toBin = "1100" + F6_trituyetdoi_toBin; break;
                    case 3: F6_trituyetdoi_toBin = "11000" + F6_trituyetdoi_toBin; break;
                    case 2: F6_trituyetdoi_toBin = "110000" + F6_trituyetdoi_toBin; break;
                    case 1: F6_trituyetdoi_toBin = "1100000" + F6_trituyetdoi_toBin; break;
                }
                return F6_trituyetdoi_toBin;
            }
        }

        //Hàm kiểm tra thanh ghi 0x59. Nếu 0x59 > 0 thì đưa về dạng 11xxxxxx, 0x59 < 0 thì đưa về dạng 10xxxxxx. Input dạng số thập phân
        public static string KiemtraF59(int F59) {
            string F59toBin = DECtoBin(F59);

            if (F59 < 0) {
                switch (F59toBin.Length) {
                    case 6: F59toBin = "10" + F59toBin; break;
                    case 5: F59toBin = "100" + F59toBin; break;
                    case 4: F59toBin = "1000" + F59toBin; break;
                    case 3: F59toBin = "10000" + F59toBin; break;
                    case 2: F59toBin = "100000" + F59toBin; break;
                    case 1: F59toBin = "1000000" + F59toBin; break;
                }
                return F59toBin;
            }
            else {
                int F59_trituyetdoi = Math.Abs(Convert.ToInt32(F59));
                string F59_trituyetdoi_toBin = DECtoBin(F59_trituyetdoi);
                switch (F59_trituyetdoi_toBin.Length) {
                    case 6: F59_trituyetdoi_toBin = "11" + F59_trituyetdoi_toBin; break;
                    case 5: F59_trituyetdoi_toBin = "110" + F59_trituyetdoi_toBin; break;
                    case 4: F59_trituyetdoi_toBin = "1100" + F59_trituyetdoi_toBin; break;
                    case 3: F59_trituyetdoi_toBin = "11000" + F59_trituyetdoi_toBin; break;
                    case 2: F59_trituyetdoi_toBin = "110000" + F59_trituyetdoi_toBin; break;
                    case 1: F59_trituyetdoi_toBin = "1100000" + F59_trituyetdoi_toBin; break;
                }
                return F59_trituyetdoi_toBin;
            }
        }

        //Hàm để tăng giá trị thanh ghi F6
        public static string Increase_F6_by_Offset(string chuoiF6, Decimal offset) //F6=10xxxxxx
        {
            string Increase_F6 = "";


            return Increase_F6;
        }

        // Hàm để giảm giá trị thanh ghi F6
        public static string Decrease_F6_by_Offset(string chuoiF6, Decimal offset) //F6=11xxxxxx
        {
            string Decrease_F6 = "";


            return Decrease_F6;
        }

        public static string Get_WifiStandard_By_Mode(string mode, string bandwidth) {
            switch (mode) {
                case "0": return "802.11b";
                case "1": return "802.11g";
                case "3": {
                        return bandwidth == "0" ? "802.11nHT20" : "802.11nHT40";
                    }
                case "4": {
                        string data = "";
                        switch (bandwidth) {
                            case "0": { data = "802.11acHT20"; break; }
                            case "1": { data = "802.11acHT40"; break; }
                            case "2": { data = "802.11acHT80"; break; }
                            case "3": { data = "802.11acHT160"; break; }
                        }
                        return data;
                    }
                default: return "";
            }
        }

        public static bool Compare_TXMeasure_With_Standard(string _stMax, string _stMin, decimal _value) {
            decimal dstMax = _stMax.Trim() == "-" ? decimal.MinValue : decimal.Parse(_stMax.Replace("%", "").Replace("dBm", ""));
            decimal dstMin = _stMin.Trim() == "-" ? decimal.MinValue : decimal.Parse(_stMin.Replace("%", "").Replace("dBm", ""));
            if (_value >= dstMin && _value <= dstMax) return true;
            else return false;
        }

        public static double RoundDecimal(double value) {
            double temp = Math.Round(value, 2);
            double num = (int)temp;
            double extra = (double)(temp - num);
            if (extra <= 0.25) return num;
            else if (0.25 < extra && extra <= 0.75) return num + 0.5;
            else return num + 1;
        }
    }
}
