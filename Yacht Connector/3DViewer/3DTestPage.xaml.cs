using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using physicsClsssWrapper;
using SharpDX;

namespace Yacht_Connector
{
    /// <summary>
    /// _3DTestPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class _3DTestPage : Window
    {
        public _3DTestPage()
        {
            InitializeComponent();
        }

        private void b_Click(object sender, RoutedEventArgs e)
        {
            roller.Roll();
        }
    }
}
