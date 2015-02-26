using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Alta.Class
{
    public static class UIElementExtensions
    {
        public static void setLeft(this UIElement e, double left)
        {
            Canvas.SetLeft(e, left);
        }
        public static void setTop(this UIElement e, double top)
        {
            Canvas.SetTop(e, top);
        }

        public static double getLeft(this UIElement e)
        {
            return Canvas.GetLeft(e);
        }
        public static double getTop(this UIElement e)
        {
            return Canvas.GetTop(e);
        }
        public static void setZIndex(this UIElement e, int zindex)
        {
            Canvas.SetZIndex(e,zindex);
        }
        public static int getZIndex(this UIElement e)
        {
            return Canvas.GetZIndex(e);
        }

        public static void Animation_Opacity_View_Frame(this UIElement E, bool isview = true, Action CompleteAction = null, double minisecond=500)
        {
            Visibility visibility = E.Visibility;
            if (visibility != Visibility.Visible)
            {
                E.Visibility = Visibility.Visible;
            }
            DoubleAnimation da = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(minisecond), EasingFunction = new PowerEase() { Power = 4, EasingMode = EasingMode.EaseInOut } };
            if (isview)
            {
                da.From = 0;
                da.To = 1;
            }
            else
            {
                da.From = 1;
                da.To = 0;
            }
            da.Completed += (o, e) =>
            {
                if (!isview)
                    E.Visibility = Visibility.Hidden;
                if (CompleteAction != null)
                {
                    CompleteAction();
                }
            };
            E.BeginAnimation(UIElement.OpacityProperty, da);
        }
        public static void Animation_Translate_Frame(this UIElement E, double fromX, double fromY, double toX, double toY, double minisecond=500, Action CompleteAction= null)
        {
            if (double.IsNaN(fromX))
            {
                fromX = E.getLeft();
            }
            if (double.IsNaN(fromY))
            {
                fromY = E.getTop();
            }
            DoubleAnimation da = new DoubleAnimation(fromX, toX, TimeSpan.FromMilliseconds(minisecond)) { EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 3 } };
            DoubleAnimation db = new DoubleAnimation(fromY, toY, TimeSpan.FromMilliseconds(minisecond)) { EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 3 } };
            da.Completed += (o, e) =>
            {
                if (CompleteAction != null)
                {
                    CompleteAction();
                }
            };
            E.BeginAnimation(Canvas.LeftProperty, da);
            E.BeginAnimation(Canvas.TopProperty, db);
        }
        public static void Animation_Translate_Frame(this UIElement E, double toX, double toY, double minisecond = 500, bool AutoReverse=false)
        {
            BounceEase BounceOrientation = new BounceEase();
            BounceOrientation.Bounces = 1;
            BounceOrientation.Bounciness = 1;
           // BounceOrientation.EasingMode = EasingMode.EaseInOut;
            if (!double.IsNaN(toX))
            {
                double fromX = E.getLeft();
                DoubleAnimation da = new DoubleAnimation(fromX, toX+fromX, TimeSpan.FromMilliseconds(minisecond)) { EasingFunction = BounceOrientation };
                da.AutoReverse = AutoReverse;
                E.BeginAnimation(Canvas.LeftProperty, da);
            }
            if (!double.IsNaN(toY))
            {
                double fromY = E.getTop();
                DoubleAnimation db = new DoubleAnimation(fromY, fromY + toY, TimeSpan.FromMilliseconds(minisecond)) { EasingFunction = BounceOrientation };
                db.AutoReverse = AutoReverse;
                E.BeginAnimation(Canvas.TopProperty, db);
            }
        }
    }
}
