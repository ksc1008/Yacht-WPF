using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using HelixToolkit.SharpDX;
using physicsClsssWrapper;

namespace Yacht_Connector
{
    /// <summary>
    /// _3DTestPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class _3DTestPage : Window
    {
        List<float> v = new();
        Random rand = new Random();
        public _3DTestPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (roller.mv.IsAnimationPlaying) {
                return;
            }
            var c = new physicsWrapper();
            v.Clear();
            c.MakeSimulation((uint)rand.Next(int.MaxValue), 5, 60);
            while (c.next())
            {
                for (int i = 0; i < 7; i++)
                    v.Add(c.getAttr(i));
            }
            for (int i = 0; i < 7; i++)
                v.Add(c.getAttr(i));

            long t = Stopwatch.GetTimestamp();
            roller.mv.animators[0].SetAnimation(v, 5, 0);
            roller.mv.animators[1].SetAnimation(v, 5, 1);
            roller.mv.animators[2].SetAnimation(v, 5, 2);
            roller.mv.animators[3].SetAnimation(v, 5, 3);
            roller.mv.animators[4].SetAnimation(v, 5, 4);
            roller.mv.animators[0].StartAnimation(t);
            roller.mv.animators[1].StartAnimation(t);
            roller.mv.animators[2].StartAnimation(t);
            roller.mv.animators[3].StartAnimation(t);
            roller.mv.animators[4].StartAnimation(t);
            roller.mv.StartAnim();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ll.Content = "";
        }
    }
}
