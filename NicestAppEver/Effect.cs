using System;
using SkiaSharp;
using Xamarin.Forms;

namespace NicestAppEver
{
    public class Effect : RoutingEffect
    {
        public Effect() : base("XamarinDocs.Effect")
        {
        }

        public event EventHandler<SKPoint> Tapped;
        public event EventHandler DoubleTapped;


        public void Tap(SKPoint point)
        {
            Tapped.Invoke(null, point);
        }

        public void DoubleTap()
        {
            DoubleTapped.Invoke(null, EventArgs.Empty);
        }
    }
}
