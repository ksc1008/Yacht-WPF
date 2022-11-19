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
        List<float> v = new();
        Random rand = new Random();
        static physicsWrapper c = new physicsWrapper();
        public _3DTestPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (roller.mv.IsAnimationPlaying) {
                return;
            }
            RollDice(5);
        }

        private void RollDice(int diceCount)
        {
            v.Clear();
            c.MakeSimulation((uint)rand.Next(int.MaxValue), diceCount, 60);
            while (c.next())
            {
                for (int i = 0; i < 7; i++)
                    v.Add(c.getAttr(i));
            }
            for (int i = 0; i < 7; i++)
                v.Add(c.getAttr(i));
            long t = Stopwatch.GetTimestamp();
            for (int i = 0; i < diceCount; i++)
            {
                roller.mv.animators[i].SetAnimation(v, diceCount, i,i+1);
                roller.mv.animators[i].StartAnimation(t);
            }
            roller.mv.StartAnim(diceCount);
        }





        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ll.Content = "";
        }
    }
}
