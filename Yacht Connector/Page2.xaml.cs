
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
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
        Label? highlighted = null;

        QuarticEase ef = new QuarticEase();

        Color ScoreNormal = Color.FromArgb(255, 255, 255, 255);
        Color ScoreSelected = Color.FromArgb(255, 0xff, 0xe0, 0x82);

        Storyboard initStoryboard;
        int remainingRoll = 3;

        Viewbox[] vbs = new Viewbox[6];
        Viewbox[] vbs2 = new Viewbox[6];

        List<Label> P1UpperScores = new();
        List<Label> P2UpperScores = new();

        List<Label> P1LowerScores = new();
        List<Label> P2LowerScores = new();

        List<Label> P1Totals = new();
        List<Label> P2Totals = new();

        DiceSet ds;
        ScoreBoard P1scoreBoard;
        ScoreBoard P2scoreBoard;

        int isScoreboardValid = 1;

        void ResetHighlighted()
        {
            if (highlighted != null)
            {
                if (((SolidColorBrush)(highlighted.Background)).Color != ScoreNormal)
                {
                    ColorAnimation ca = new ColorAnimation();
                    ca.From = ((SolidColorBrush)highlighted.Background).Color;
                    ca.To = ScoreNormal;
                    ca.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                    ca.EasingFunction = ef;
                    highlighted.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca, HandoffBehavior.SnapshotAndReplace);
                }

                highlighted = null;
            }
        }

        void UpdateScore(bool isP1)
        {
            List<Label> labels1;
            List<Label> labels2;
            List<Label> labels3;
            ScoreBoard sc;
            if (isP1)
            {
                labels1 = P1UpperScores;
                labels2 = P1LowerScores;
                labels3 = P1Totals;
                sc = P1scoreBoard;
            }
            else
            {
                labels1 = P2UpperScores;
                labels2 = P2LowerScores;
                labels3 = P2Totals;
                sc = P2scoreBoard;
            }

            for(int i = 0; i < 6; i++)
            {
                if(!sc.GetChecked((DiceSet.category)i))
                    labels1[i].Content = ds.GetPoint((DiceSet.category)i);
                if (!sc.GetChecked((DiceSet.category)(i+6)))
                    labels2[i].Content = ds.GetPoint((DiceSet.category)(i+6));
            }
        }

        void ClearScore(bool isP1)
        {
            List<Label> labels1;
            List<Label> labels2;
            List<Label> labels3;
            if (isP1)
            {
                labels1 = P1UpperScores;
                labels2 = P1LowerScores;
                labels3 = P1Totals;
            }
            else
            {
                labels1 = P2UpperScores;
                labels2 = P2LowerScores;
                labels3 = P2Totals;
            }
        }

        void loadDice()
        {
            for (int i = 1; i <= 6; i++)
            {
                vbs[i - 1] = loadViewboxFromXaml("/Resources/die" + i.ToString() + ".xaml");
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
            ef.EasingMode = EasingMode.EaseOut;
            ds = new DiceSet();
            P1scoreBoard = new ScoreBoard();
            P2scoreBoard = new ScoreBoard();
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

            P1LowerScores.Add(P1lower1);
            P1LowerScores.Add(P1lower2);
            P1LowerScores.Add(P1lower3);
            P1LowerScores.Add(P1lower4);
            P1LowerScores.Add(P1lower5);
            P1LowerScores.Add(P1lower6);
            P1UpperScores.Add(P1upper1);
            P1UpperScores.Add(P1upper2);
            P1UpperScores.Add(P1upper3);
            P1UpperScores.Add(P1upper4);
            P1UpperScores.Add(P1upper5);
            P1UpperScores.Add(P1upper6);
                         
            P2LowerScores.Add(P2lower1);
            P2LowerScores.Add(P2lower2);
            P2LowerScores.Add(P2lower3);
            P2LowerScores.Add(P2lower4);
            P2LowerScores.Add(P2lower5);
            P2LowerScores.Add(P2lower6);
                      
            P2UpperScores.Add(P2upper1);
            P2UpperScores.Add(P2upper2);
            P2UpperScores.Add(P2upper3);
            P2UpperScores.Add(P2upper4);
            P2UpperScores.Add(P2upper5);
            P2UpperScores.Add(P2upper6);

            foreach(var l in P1LowerScores)
                l.Background = ((SolidColorBrush)l.Background).Clone();
            foreach (var l in P1UpperScores)
                l.Background = ((SolidColorBrush)l.Background).Clone();
            foreach (var l in P2LowerScores)
                l.Background = ((SolidColorBrush)l.Background).Clone();
            foreach (var l in P2UpperScores)
                l.Background = ((SolidColorBrush)l.Background).Clone();

            TranslateTransform trt = new TranslateTransform(0,0);
            ScoreboardGrid.RenderTransform = trt;
            MakeStoryboardFadeout();

            dr.OnFirstRender += () => { InitiateScene(); };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dr.ReadyToRoll())
            {
                if (--remainingRoll == 0)
                    dr.Roll(true);
                else
                    dr.Roll(false);
            }
        }

        private void dr_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void InitiateScene()
        {
            initStoryboard.Begin(this);
            dr.Visibility = Visibility.Visible;
        }

        void MakeStoryboardFadeout()
        {
            initStoryboard = new Storyboard();
            DoubleAnimation[] anims = { new DoubleAnimation(), new DoubleAnimation() };
            ThicknessAnimation dd = new();

            dd.From = new Thickness(-500, 0, 500, 0);
            dd.To = new Thickness(-20, 0, 20, 0);
            dd.EasingFunction = ef;
            dd.Duration = new Duration(TimeSpan.FromSeconds(1));
            dd.BeginTime = TimeSpan.FromSeconds(1.5);

            anims[0].From = 0;
            anims[0].To = 1;
            anims[0].EasingFunction = ef;
            anims[0].Duration = new Duration(TimeSpan.FromSeconds(2));
            anims[0].BeginTime = TimeSpan.FromSeconds(1);

            Storyboard.SetTarget(anims[0], dr.view1);
            Storyboard.SetTarget(dd, ScoreboardGrid);
            Storyboard.SetTargetProperty(anims[0], new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(dd, new PropertyPath(MarginProperty));

            initStoryboard.Children.Add(anims[0]);
            initStoryboard.Children.Add(dd);

            initStoryboard.Completed += (o, e) =>
            {
                Rollbtn.Visibility = Visibility.Visible;
            };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dr.OnDiceRoll += (arr) =>
            {
                ds.SetDice(arr);
                UpdateScore(true);
            };

            dr.OnButtonActivateChange += (value) =>
            {
                if (value)
                    Rollbtn.Visibility = Visibility.Visible;
                else
                    Rollbtn.Visibility = Visibility.Hidden;
            };

            Debug.WriteLine("Added callback");
            //InitiateScene();
        }

        bool CheckMouseOverValid(Label l, int position)
        {
            int t;
            switch (position)
            {
                case 0:
                    if (isScoreboardValid != 1)
                        return false;
                    t = P1UpperScores.IndexOf(l);
                    if (P1scoreBoard.GetChecked((DiceSet.category)t))
                        return false;
                    break;
                case 1:
                    if (isScoreboardValid != 2)
                        return false;
                    t = P2UpperScores.IndexOf(l);
                    if (P2scoreBoard.GetChecked((DiceSet.category)t))
                        return false;
                    break;
                case 2:
                    if (isScoreboardValid != 1)
                        return false;
                    t = P1LowerScores.IndexOf(l);
                    if (P1scoreBoard.GetChecked((DiceSet.category)(t+6)))
                        return false;
                    break;
                case 3:
                    if (isScoreboardValid != 2)
                        return false;
                    t = P2LowerScores.IndexOf(l);
                    if (P2scoreBoard.GetChecked((DiceSet.category)(t+6)))
                        return false;
                    break;
            }
            return true;
        }

        (int,int) CheckMouseClickValid(Label l)
        {
            int t;
            t = P1UpperScores.IndexOf(l);
            if (t != -1) {
                if (P1scoreBoard.GetChecked((DiceSet.category)t))
                    return (-1,-1);
                if (isScoreboardValid != 1)
                    return (-1, -1);
                return (1, t);
            }
            t = P2UpperScores.IndexOf(l);
            if (t != -1)
            {
                if (P2scoreBoard.GetChecked((DiceSet.category)t))
                    return (-1, -1);
                if (isScoreboardValid != 2)
                    return (-1, -1);
                return (1, t);
            }

            t = P1LowerScores.IndexOf(l);
            if (t != -1)
            {
                if (P1scoreBoard.GetChecked((DiceSet.category)(t+6)))
                    return (-1, -1);
                if (isScoreboardValid != 1)
                    return (-1, -1);
                return (1, t+6);
            }

            t = P2LowerScores.IndexOf(l);
            if (t != -1)
            {
                if (P2scoreBoard.GetChecked((DiceSet.category)(t+6)))
                    return (-1, -1);
                if (isScoreboardValid != 2)
                    return (-1, -1);
                return (2, t+6);
            }
            return (-1, -1);
        }

        private void HighlightLabel(Label l)
        {
            highlighted = l;
            ColorAnimation ca = new ColorAnimation();
            ca.From = ((SolidColorBrush)l.Background).Color;
            ca.To = ScoreSelected;
            ca.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            ca.EasingFunction = ef;
            l.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
        }

        private void P2UpperMouseEnter(object sender, MouseEventArgs e)
        {
            ResetHighlighted();
            if (CheckMouseOverValid((Label)sender, 1))
            {
                HighlightLabel((Label)sender);
            }
        }

        private void P1UpperMouseEnter(object sender, MouseEventArgs e)
        {
            ResetHighlighted();
            if (CheckMouseOverValid((Label)sender, 0))
            {
                HighlightLabel((Label)sender);
            }
        }


        private void P2LowerMouseEnter(object sender, MouseEventArgs e)
        {
            ResetHighlighted();
            if (CheckMouseOverValid((Label)sender, 3))
            {
                HighlightLabel((Label)sender);
            }
        }

        private void P1LowerMouseEnter(object sender, MouseEventArgs e)
        {
            ResetHighlighted();
            if (CheckMouseOverValid((Label)sender, 2))
            {
                HighlightLabel((Label)sender);
            }
        }

        private void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            if (((SolidColorBrush)((Label)sender).Background).Color == ScoreNormal)
            {
                return;
            }

            ColorAnimation ca = new ColorAnimation();
            ca.From = ((SolidColorBrush)((Label)sender).Background).Color;
            ca.To = ScoreNormal;
            ca.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            ca.EasingFunction = ef;
            ((Label)sender).Background.BeginAnimation(SolidColorBrush.ColorProperty, ca,HandoffBehavior.SnapshotAndReplace);
        }

        private void ScoreClick(object sender, MouseButtonEventArgs e)
        {
            var check = CheckMouseClickValid((Label)sender);
            if (check.Item1 == -1)
                return;
            ScoreBoard sc;
            if (check.Item1 == 1)
                sc = P1scoreBoard;
            else
                sc = P2scoreBoard;

            sc.SetScore((DiceSet.category)check.Item2, ds.GetPoint((DiceSet.category)check.Item2));
            ((Label)sender).FontSize = 28;
            ((Label)sender).FontWeight = FontWeights.Bold;
            ((SolidColorBrush)((Label)sender).Foreground).Color = Color.FromArgb(255, 0, 0, 0);
            ColorAnimation ca = new ColorAnimation();
            ca.From = ((SolidColorBrush)((Label)sender).Background).Color;
            ca.To = ScoreNormal;
            ca.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            ca.EasingFunction = ef;
            ((Label)sender).Background.BeginAnimation(SolidColorBrush.ColorProperty, ca, HandoffBehavior.SnapshotAndReplace);
        }
    }
}
