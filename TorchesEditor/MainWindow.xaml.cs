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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;

namespace TorchesEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        char[,] map;

        public MainWindow()
        {
            InitializeComponent();

            map = new char[8,16];
            for(int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    map[y, x] = ',';
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int x = Grid.GetColumn((UIElement)sender);
            int y = 7 - Grid.GetRow((UIElement)sender);

            map[y, x] = map[y, x] == ',' ? '#' : ',';
            ((Button)sender).Content = map[y, x];
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string output = $"{check_Right.IsChecked.ToString()} {check_Up.IsChecked.ToString()} {check_Left.IsChecked.ToString()} {check_Down.IsChecked.ToString()}\n";

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    output += $"{map[y, x]} {(map[y, x] == '#' ? "True" : "False")}\n";
                }
            }

            Clipboard.SetText(output);
        }
    }
}
