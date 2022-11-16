using ABI.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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
using System.Windows.Resources;
using System.Windows.Shapes;
using Yacht;

namespace Yacht_Connector
{
    /// <summary>
    /// Page2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Page2 : Page
    {
        Viewbox[] vbs = new Viewbox[6];
        Viewbox[] vbs2 = new Viewbox[6];
        BitmapImage[] dimg = new BitmapImage[6];
        DiceSet ds;
        void loadDice()
        {
            for (int i = 1; i <= 6; i++)
            {
                vbs[i - 1] = loadViewboxFromXaml("/Resources/die" + i.ToString() + ".xaml");
                dimg[i - 1] = loadImage("/Resources/die" + i.ToString() + ".png");
            }
        }

        void loadSpecialDice()
        {
            vbs2[0] = loadViewboxFromXaml("/Resources/d_choice.xaml");
            vbs2[1] = loadViewboxFromXaml("/Resources/d_fok.xaml");
            vbs2[2] = loadViewboxFromXaml("/Resources/d_fullhouse.xaml");
            vbs2[3] = loadViewboxFromXaml("/Resources/d_sstr.xaml");
            vbs2[4] = loadViewboxFromXaml("/Resources/d_lstr.xaml");
            vbs2[5] = loadViewboxFromXaml("/Resources/d_yacht.xaml");
        }

        Viewbox loadViewboxFromXaml(string filename)
        {
            var uri = new System.Uri(filename, UriKind.Relative);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            System.Windows.Markup.XamlReader reader = new System.Windows.Markup.XamlReader();
            return (Viewbox)reader.LoadAsync(info.Stream);
        }
        BitmapImage loadImage(string filename)
        {
            var uri = new BitmapImage(new System.Uri(filename, UriKind.Relative));
            return uri;
        }

        public Page2()
        {
            ds = new DiceSet();
            ScaleTransform scaleTrans = new ScaleTransform();
            scaleTrans.ScaleX = 0.15;
            scaleTrans.ScaleY = 0.15;
            InitializeComponent();
            loadDice();
            loadSpecialDice();
            c1.RenderTransform = scaleTrans;
            c2.RenderTransform = scaleTrans;
            c3.RenderTransform = scaleTrans;
            c4.RenderTransform = scaleTrans;
            c5.RenderTransform = scaleTrans;
            c6.RenderTransform = scaleTrans;
            c11.RenderTransform = scaleTrans;
            c12.RenderTransform = scaleTrans;
            c13.RenderTransform = scaleTrans;
            c14.RenderTransform = scaleTrans;
            c15.RenderTransform = scaleTrans;
            c16.RenderTransform = scaleTrans;
            c1.Children.Add(vbs[0]);
            c2.Children.Add(vbs[1]);
            c3.Children.Add(vbs[2]);
            c4.Children.Add(vbs[3]);
            c5.Children.Add(vbs[4]);
            c6.Children.Add(vbs[5]);
            c11.Children.Add(vbs2[0]);
            c12.Children.Add(vbs2[1]);
            c13.Children.Add(vbs2[2]);
            c14.Children.Add(vbs2[3]);
            c15.Children.Add(vbs2[4]);
            c16.Children.Add(vbs2[5]);

            rd1.MouseEnter += (s, e) => { dg1.Visibility = Visibility.Visible; };
            rd2.MouseEnter += (s, e) => { dg2.Visibility = Visibility.Visible; };
            rd3.MouseEnter += (s, e) => { dg3.Visibility = Visibility.Visible; };
            rd4.MouseEnter += (s, e) => { dg4.Visibility = Visibility.Visible; };
            rd5.MouseEnter += (s, e) => { dg5.Visibility = Visibility.Visible; };

            rd1.MouseLeave += (s,e) => { dg1.Visibility = Visibility.Hidden; };
            rd2.MouseLeave += (s,e) => { dg2.Visibility = Visibility.Hidden; };
            rd3.MouseLeave += (s,e) => { dg3.Visibility = Visibility.Hidden; };
            rd4.MouseLeave += (s,e) => { dg4.Visibility = Visibility.Hidden; };
            rd5.MouseLeave += (s,e) => { dg5.Visibility = Visibility.Hidden; };

        }

        private void RollButton_Click(object sender, RoutedEventArgs e)
        {
            ds.Roll(new bool[] { true, true, true, true, true });
            rd1.Source = dimg[ds.Dices[0]];
            rd2.Source = dimg[ds.Dices[1]];
            rd3.Source = dimg[ds.Dices[2]];
            rd4.Source = dimg[ds.Dices[3]];
            rd5.Source = dimg[ds.Dices[4]];
        }
    }
}
