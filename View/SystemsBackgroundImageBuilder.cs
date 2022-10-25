using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RtoTools.View
{
    internal static class SystemsBackgroundImageBuilder
    {
        public static ImageSource BuildImage(List<Model.Data.SystemModel>? systems, List<Model.Data.FactionModel>? factions)
        {
            var canvas = new Canvas();
            var multiplier = 1;

            if (systems != null && factions != null)
            {
                var minX = systems.Min(x => x.Cordinates.X) * multiplier;
                var maxX = systems.Max(x => x.Cordinates.X) * multiplier;
                var minY = systems.Min(x => x.Cordinates.Y) * multiplier;
                var maxY = systems.Max(x => x.Cordinates.Y) * multiplier;

                canvas.Width = (maxX - minX) + 20;
                canvas.Height = (maxY - minY) + 20;

                foreach (var system in systems ?? new List<Model.Data.SystemModel>())
                {
                    var factionOwner = factions?.FirstOrDefault(x => x.FactionId == system.OwnerFactionId);

                    EllipseGeometry dot = new EllipseGeometry();
                    dot.Center = new System.Windows.Point(5, 5);
                    dot.RadiusX = 5;
                    dot.RadiusY = 5;

                    var newPath = new System.Windows.Shapes.Path()
                    {
                        Fill = new SolidColorBrush(factionOwner?.MapColor ?? Colors.Pink),
                        Width = 10,
                        Height = 10,
                        Data = dot,
                    };

                    Canvas.SetTop(canvas, (system.Cordinates.Y * multiplier - minY) + 5);
                    Canvas.SetLeft(canvas, (system.Cordinates.X * multiplier - minX) + 5);

                    canvas.Children.Add(newPath);
                }
            }
            else
            {
                canvas.Height = 100;
                canvas.Width = 100;
            }

            var rtb = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(canvas);
            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            using (var stream = new MemoryStream())
            {
                png.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.DecodePixelWidth = 30;
                bi.StreamSource = stream;
                bi.EndInit();

                return bi;
            }
        }
    }
}
