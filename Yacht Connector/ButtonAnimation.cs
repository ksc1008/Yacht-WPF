using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Yacht_Connector
{
    internal class ButtonAnimation
    {
        public DoubleAnimation animY { get; private set; }
        public DoubleAnimation animTrans { get; private set; }

        public double duration { get; private set; } = 1.5;
        public double yDelta { get; private set; } = 20;

        Control _button;

        TranslateTransform trans = new();

        public ButtonAnimation(Control button, double duration = 1.5, double delay = 0, double height = 20) 
        {
            _button = button;
            animY = new DoubleAnimation();
            animTrans = new DoubleAnimation();

            QuarticEase ef = new QuarticEase();
            ef.EasingMode = EasingMode.EaseOut;

            button.RenderTransform = trans;
            animY.BeginTime = TimeSpan.FromSeconds(delay);
            animY.From = -height;
            animY.To = 0;
            animY.EasingFunction = ef;
            animY.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(duration));

            animTrans.BeginTime = TimeSpan.FromSeconds(delay);
            animTrans.From = 0;
            animTrans.To = 1;
            animTrans.EasingFunction = ef;
            animTrans.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(duration));
        }

        public void Begin()
        {
            if(_button == null)
            {
                throw new InvalidOperationException("Button is not selected");
            }

            trans.BeginAnimation(TranslateTransform.YProperty, animY);
            _button.BeginAnimation(Button.OpacityProperty, animTrans);
        }
    }
}
