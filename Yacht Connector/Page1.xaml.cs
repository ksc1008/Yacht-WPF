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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yacht_Connector
{
    /// <summary>
    /// Page1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Page1 : Page
    {
        Storyboard storyboardOnCreate;
        Storyboard storyboardFadeout;

        bool buttonEnabled = true;


        public Page1()
        {
            InitializeComponent();
            MakeButtonAnim();
            MakeStoryboardFadeout(1.5);
            card1.Opacity = 0;
            card2.Opacity = 0;
        }


        void MakeStoryboardFadeout(double duration)
        {
            var dur = new Duration(TimeSpan.FromSeconds(duration-0.5));

            QuarticEase ef = new QuarticEase();
            ef.EasingMode = EasingMode.EaseOut;
            storyboardFadeout = new Storyboard();
            DoubleAnimation[] anims = { new DoubleAnimation(), new DoubleAnimation(), new DoubleAnimation() };
            foreach (var anim in anims)
            {
                anim.From = 1;
                anim.To = 0;
                anim.EasingFunction = ef;
                anim.Duration = dur;
                storyboardFadeout.Children.Add(anim);
            }
            Storyboard.SetTarget(anims[0], card1);
            Storyboard.SetTarget(anims[1], card2);
            Storyboard.SetTarget(anims[2], titleLabel);
            Storyboard.SetTargetProperty(anims[0], new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(anims[1], new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(anims[2], new PropertyPath(OpacityProperty));

            var pa = new DoubleAnimation();
            pa.Duration = dur;
            pa.From = 600;
            pa.To = 600 + 680;
            pa.EasingFunction = ef;
            Storyboard.SetTarget(pa, MainWindow.Instance);
            Storyboard.SetTargetProperty(pa, new PropertyPath(MainWindow.WidthProperty));
            storyboardFadeout.Children.Add(pa);
            storyboardFadeout.Completed += (o, e) =>
            {
                MainWindow.Instance.fr.Content = new Page2();
                NavigationService.RemoveBackEntry();

            };

        }
        void MakeButtonAnim()
        {
            storyboardOnCreate = new Storyboard();
            QuarticEase ef = new QuarticEase();
            ef.EasingMode = EasingMode.EaseOut;
            var ca1 = new ButtonAnimation(card1, 1.5, 0.3);
            var ca2 = new ButtonAnimation(card2, 1.5, 0.6);
            var da = new DoubleAnimation();


            da.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            da.From = 0;
            da.To = 1;
            da.EasingFunction = ef;

            Storyboard.SetTarget(ca1.animY, card1);
            Storyboard.SetTarget(ca1.animTrans, card1);
            Storyboard.SetTarget(ca2.animY, card2);
            Storyboard.SetTarget(ca2.animTrans, card2);
            Storyboard.SetTarget(da, mainGrid);

            Storyboard.SetTargetProperty(ca1.animY, new PropertyPath(TranslateTransform.YProperty));
            Storyboard.SetTargetProperty(ca2.animY, new PropertyPath(TranslateTransform.YProperty));
            Storyboard.SetTargetProperty(ca1.animTrans, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(ca2.animTrans, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));

            storyboardOnCreate.Children.Add(ca1.animY);
            storyboardOnCreate.Children.Add(ca1.animTrans);
            storyboardOnCreate.Children.Add(ca2.animY);
            storyboardOnCreate.Children.Add(ca2.animTrans);
            storyboardOnCreate.Children.Add(da);
        }

        private void Grid_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as Grid).IsEnabled)
            {
                storyboardOnCreate.Begin(this);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!buttonEnabled)
                return;
            storyboardFadeout.Begin();
            buttonEnabled = false;
        }
    }
}
