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

namespace Yacht_Connector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ButtonAnimation Ba;
        ButtonAnimation Ba2;
        static public MainWindow? Instance = null;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            fr.Content = new Page1();
            Ba = new ButtonAnimation(StartButton);
            Ba2 = new ButtonAnimation(HowToPlayButton);
            Ba.Begin();
            Ba2.Begin();
            fr.IsEnabled = false;
        }

        private void HowToPlay_Click(object sender, RoutedEventArgs e)
        {
            ara.Visibility = Visibility.Visible;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            fr.Visibility = Visibility.Visible;
            fr.IsEnabled = true;
        }
    }
}
