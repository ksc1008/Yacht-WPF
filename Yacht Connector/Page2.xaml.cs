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

namespace Yacht_Connector
{
    /// <summary>
    /// Page2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Page2 : Page
    {
        Viewbox[] vbs = new Viewbox[6];
        Viewbox[] vbs2 = new Viewbox[6];
        void loadDice()
        {
            for(int i = 1; i <= 6; i++)
                vbs[i - 1] = loadViewboxFromXaml("/Resources/die" + i.ToString() + ".xaml");
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

        public Page2()
        {
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

        }
    }
}
