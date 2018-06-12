using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToolCalibWifiForGW040H {
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window {
        public About() {
            InitializeComponent();
            listHist.Add(new history() { ID = "1", VERSION = "1.0.0.0", CONTENT = "- Phát hành lần đầu", DATE = "14/04/2018", CHANGETYPE = "Tạo mới", PERSON = "Hồ Đức Anh" });
            listHist.Add(new history() { ID = "2", VERSION = "1.0.0.1", CONTENT = "- Tích hợp tool test anten sau đóng vỏ", DATE = "29/05/2018", CHANGETYPE = "Chỉnh sửa", PERSON = "Hồ Đức Anh" });
            listHist.Add(new history() { ID = "3", VERSION = "1.0.0.2", CONTENT = "- Cho phép test anten ở băng tần 5G", DATE = "08/06/2018", CHANGETYPE = "Chỉnh sửa", PERSON = "Hồ Đức Anh" });
            this.GridAbout.ItemsSource = listHist;
        }

        List<history> listHist = new List<history>();

        private class history {
            public string ID { get; set; }
            public string VERSION { get; set; }
            public string CONTENT { get; set; }
            public string DATE { get; set; }
            public string CHANGETYPE { get; set; }
            public string PERSON { get; set; }
        }

    }
}
