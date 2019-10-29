using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Extended;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace NicestAppEver
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [HotReloader.CSharpVisual]
    public partial class MainPage : ContentPage
    {
        private List<SKPoint> _origins;
        private List<PathPoint> _edges;
        private bool _shouldAnimate;
        private SKPath _bezierCurve;

        public MainPage()
        {
            InitializeComponent();
            _origins = new List<SKPoint>();
            _edges = new List<PathPoint>();
        }

        private void Painted(object sender, SKPaintSurfaceEventArgs args)
        {
            var surface = args.Surface;
            var canvas = surface.Canvas;
            var info = args.Info;

            if (!_shouldAnimate)
            {
                _edges.Clear();
            }

            canvas = null;
            var john = new
            {
                canvas
            };

            canvas.Clear();

            var pointsPaint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 8,
                Color = SKColors.Black,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };

            var pathsPaint = new SKPaint
            {
                Color = SKColors.Black.WithAlpha(150),
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                StrokeWidth = 2,
                IsAntialias = true
            };

            for (var i = 0; i < _origins.Count; i++)
            {
                canvas.DrawPoint(_origins[i], pointsPaint);

                if (i < _origins.Count - 1)
                {
                    var path = new SKPath();
                    path.MoveTo(_origins[i]);
                    path.LineTo(_origins[i + 1]);
                    canvas.DrawPath(path, pathsPaint);

                    if (!_shouldAnimate)
                    {
                        _edges.Add(new PathPoint
                        {
                            Path = path,
                            Point = path.Points[0]
                        });

                    }
                }
            }

            if (!_shouldAnimate)
            {
                foreach (var edge in _edges)
                {
                    if (edge != _edges.Last())
                        edge.SubPath = new PathPoint
                        {
                            Point = edge.Path.Points[0],
                            Path = edge.Path
                        };
                }
            }

            if (_shouldAnimate)
            {
                pointsPaint.Color = SKColors.Red;

                foreach (var point in _edges)
                {
                    canvas.DrawPoint(point.Point, pointsPaint);
                }

                for (var i = 0; i < _edges.Count - 1; i++)
                {
                    var unitingPath = new SKPath();

                    unitingPath.MoveTo(_edges[i].Point);
                    unitingPath.LineTo(_edges[i + 1].Point);
                    pathsPaint.Color = SKColors.Blue;

                    _edges[i].SubPath.Path = unitingPath;

                    canvas.DrawPath(unitingPath, pathsPaint);
                }

                var subPaths = _edges.Select(x => x.SubPath).Where(x => x != null);
                var subPoints = subPaths.Select(x => x.Point);

                foreach (var point in subPoints)
                {
                    var paint = pointsPaint;
                    paint.Color = SKColors.Brown;
                    paint.StrokeWidth = 15;

                    _bezierCurve.LineTo(point);

                    canvas.DrawPoint(point, paint);
                }

                var bPaint = pathsPaint;
                bPaint.Color = SKColors.Red;
                canvas.DrawPath(_bezierCurve, bPaint);
            }
        }

        void StackTapped(object sender, SKPoint args)
        {
            _origins.Add(args);
            canvasView.InvalidateSurface();
        }

        void DoubleTapped(object sender, EventArgs e)
        {
            _shouldAnimate = false;
            _origins.Clear();
            _edges.Clear();

            canvasView.InvalidateSurface();
        }

        private List<PathPoint> Populate(List<SKPoint> points)
        {
            var result = new List<PathPoint>();

            for (var i = 0; i < points.Count - 1; i++)
            {
                var path = new SKPath();
                path.MoveTo(points[i]);
                path.LineTo(points[i + 1]);

                var pathPoint = new PathPoint
                {
                    Point = points[i],
                    Path = path
                };

                result.Add(pathPoint);
                result.Add(pathPoint);
            }

            return result;
        }

        void StartAnimating(object sender, EventArgs e)
        {
            _bezierCurve = new SKPath();
            _bezierCurve.MoveTo(_edges.First().Point);

            _shouldAnimate = true;

            var step = 1000;

            foreach (var path in _edges)
            {
                path.Point = path.Path.Points[0];
            }

            Parallel.ForEach(_edges, async pathPoint =>
            {
                var stepLength = pathPoint.Path.LastPoint - pathPoint.Path.Points[0];
                var xOffset = stepLength.X / step;
                var yOffset = stepLength.Y / step;

                for (var i = 0; i < step; i++)
                {
                    pathPoint.Point = new SKPoint(pathPoint.Point.X + xOffset, pathPoint.Point.Y + yOffset);

                    if (pathPoint.SubPath != null)
                    {
                        var stepLeng = pathPoint.SubPath.Path.LastPoint - pathPoint.SubPath.Path.Points[0];
                        var xSub = stepLeng.X / step;
                        var ySub = stepLeng.Y / step;

                        pathPoint.SubPath.Point = new SKPoint(pathPoint.SubPath.Point.X + xSub * 2, pathPoint.SubPath.Point.Y + ySub * 2);
                    }

                    await Task.Delay(4);
                    Device.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
                }
            });
        }
    }

    public class PathPoint
    {
        public SKPath Path { get; set; }
        public SKPoint Point { get; set; }
        public PathPoint SubPath { get; set; }
    }
}
