using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToolCalibWifiForGW040H.Function;

namespace ToolCalibWifiForGW040H
{
    /// <summary>
    /// Interaction logic for attenuatorWindow.xaml
    /// </summary>
    public partial class attenuatorWindow : Window
    {
        public attenuatorWindow()
        {
            InitializeComponent();
            this.dgAttenuator.ItemsSource = GlobalData.autoAttenuator;
            GlobalData.autoAttenuator.Clear();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "Auto Calculate Attenuator": {
                        Thread t = new Thread(new ThreadStart(() => { }));
                        t.IsBackground = true;

                        MessageBox.Show("Success.","Calculate Attenuator", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                default: break;
            }
        }
    }
}
