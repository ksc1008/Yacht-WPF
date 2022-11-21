using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.Wpf;
using physicsClsssWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Yacht_Connector
{

    public partial class DiceRoller : UserControl
    {
        int floatNum = 5;
        List<Label> FloatClicks = new();
        List<Label> KeepClicks = new();
        List<float> v = new();
        Random rand = new Random();
        static physicsWrapper c = new physicsWrapper();
        private MainViewModel mv;

        public MainViewModel.ReturnDiceDelegate OnDiceRoll { get => mv.DiceRollCallback; set => mv.DiceRollCallback = value; }
        public MainViewModel.GeneralCallbackDelegate OnFirstRender { get => mv.FirstRenderCallback; set => mv.FirstRenderCallback = value; }
        public MainViewModel.ButtonDelegate OnButtonActivateChange { get => mv.ButtonStateCallback; set => mv.ButtonStateCallback = value; }

        public DiceRoller()
        {
            mv = new(this);
            DataContext = mv;
            InitializeComponent();
            FloatClicks.Add(l1);
            FloatClicks.Add(l2);
            FloatClicks.Add(l3);
            FloatClicks.Add(l4);
            FloatClicks.Add(l5);

            KeepClicks.Add(kl1);
            KeepClicks.Add(kl2);
            KeepClicks.Add(kl3);
            KeepClicks.Add(kl4);
            KeepClicks.Add(kl5);
        }

        void RearrangeLabels(int num)
        {
            double width = Gr.ActualWidth;
            double span = 190 + (5 - num) * 45;

            double t;
            int i;
            for (i = 0; i < num; i++)
            {
                t = i * span - (float)(num - 1) / 2 * span;
                FloatClicks[i].Visibility = Visibility.Visible;
                FloatClicks[i].Margin = new Thickness(width / 2 + t / 2 - FloatClicks[i].ActualWidth / 2, 0, width / 2 - t / 2 - FloatClicks[i].ActualWidth / 2, 0);
            }

            for (; i < 5; i++)
            {
                FloatClicks[i].Visibility = Visibility.Hidden;
            }
        }

        public void HideDice()
        {
            mv.HideDice();
        }

        public bool ReadyToRoll()
        {
            return mv.DiceRollReady();
        }

        public bool Roll(bool isLast)
        {
            Debug.Assert(mv.DiceRollReady(),"Not ready to roll dice.");
            if (mv.CurrentState==MainViewModel.State.Cleared)
            {
                floatNum = 5;
                var dresult = new List<int>();
                for (int i = 0; i < 5; i++)
                    dresult.Add(rand.Next(6)+1);
                RollDice(5, dresult,false);
            }
            else
            {
                var dresult = new List<int>();
                for (int i = 0; i < floatNum; i++)
                    dresult.Add(rand.Next(6) + 1);
                RollDice(floatNum, dresult,isLast);
            }

            return true;
        }

        private void RollDice(int diceCount, List<int> diceResult,bool isLast)
        {
            if (diceCount == 0 || diceCount > 5)
                return;
            RearrangeLabels(diceCount);
            v.Clear();
            c.MakeSimulation((uint)rand.Next(int.MaxValue), diceCount, 60);
            while (c.next())
            {
                for (int i = 0; i < 7; i++)
                    v.Add(c.getAttr(i));
            }
            for (int i = 0; i < 7; i++)
                v.Add(c.getAttr(i));

            mv.SetDiceResult(diceResult);
            mv.AnimateDiceRoll(v,isLast);
        }

        private void FloatClick(object sender, MouseButtonEventArgs e)
        {
            var index = FloatClicks.IndexOf((Label)sender);
            Debug.WriteLine("Label Index : " + index);
            if (mv.ClickFloating(index))
            {
                floatNum--;
                RearrangeLabels(floatNum);
            }
        }


        private void KeepClick(object sender, MouseButtonEventArgs e)
        {
            var index = KeepClicks.IndexOf((Label)sender);
            Debug.WriteLine("Label Index : " + index);
            if (mv.ClickKeep(index))
            {
                floatNum++;
                RearrangeLabels(floatNum);
            }
        }
    }
}
