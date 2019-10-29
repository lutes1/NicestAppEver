using System;
using System.Linq;
using SkiaSharp;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(NicestAppEver.iOS.Effect), "Effect")]
namespace NicestAppEver.iOS
{
    public class Effect : PlatformEffect
    {
        private nfloat _scale;
        private UIView _view;
        private NicestAppEver.Effect _effect;

        protected override void OnAttached()
        {
            _scale = UIScreen.MainScreen.Scale;
            _view = Control ?? Container;
            _effect = Element.Effects.First() as NicestAppEver.Effect;
            _view.AddGestureRecognizer(new UIKit.UITapGestureRecognizer(Tapped) { NumberOfTapsRequired = 1});
            _view.AddGestureRecognizer(new UIKit.UITapGestureRecognizer(DoubeTapped) { NumberOfTapsRequired = 2 });
        }

        private void DoubeTapped()
        {
            _effect.DoubleTap();
        }

        private void Tapped(UITapGestureRecognizer gestureRecognizer)
        {
            var point = new SKPoint((float)gestureRecognizer.LocationInView(_view).X * (float)_scale, (float)gestureRecognizer.LocationInView(_view).Y * (float)_scale);
            _effect.Tap(point);
        }



        protected override void OnDetached()
        {
        }
    }
}
