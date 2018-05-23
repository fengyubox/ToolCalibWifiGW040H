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
    /// Interaction logic for analyzerWindow.xaml
    /// </summary>
    public partial class analyzerWindow : Window {
        public analyzerWindow() {
            InitializeComponent();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }
    }
}
