using RtoTools.Common;
using RtoTools.ViewModel.MapTab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RtoTools.View.MapTab
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
            DataContextChanged += MapViewDataContextChanged;
        }

        private MapTabViewModel? ParsedDataContext { get; set; }

        private void MapViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var doRefresh = false;

            if (e.OldValue is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= DataContextPropertyChanged;
                doRefresh = true;
            }

            if (e.NewValue is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)e.NewValue).PropertyChanged += DataContextPropertyChanged;
                doRefresh = true;                
            }

            ParsedDataContext = e.NewValue as MapTabViewModel;

            if (doRefresh)
            {
                RefreshSystems();
                RefreshLines();
            }
        }

        private void DataContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Systems":
                    SetPrecalcValues();
                    RefreshSystems();
                    RefreshLines();
                    RefreshFocusPoint(new Size(uxMapViewbox.ActualWidth, uxMapViewbox.ActualHeight));
                    break;
                case "Lines":
                    RefreshLines();
                    break;
                case "FocusPoint":
                case "Zoom":
                    RefreshFocusPoint(new Size(uxMapViewbox.ActualWidth, uxMapViewbox.ActualHeight));
                    break;
            }
        }

        private void MapViewboxSizeChanged(object sender, SizeChangedEventArgs e)
        {
            HandleMapResize(e.NewSize);
        }

        private double MinSystemX { get; set; }

        private double MaxSystemX { get; set; }

        private double MinSystemY { get; set; }

        private double MaxSystemY { get; set; }

        private void SetPrecalcValues()
        {
            if (ParsedDataContext != null && ParsedDataContext.Systems != null)
            {
                MinSystemX = ParsedDataContext.Systems.Min(x => x.PointX);
                MaxSystemX = ParsedDataContext.Systems.Max(x => x.PointX);
                MinSystemY = ParsedDataContext.Systems.Min(x => x.PointY);
                MaxSystemY = ParsedDataContext.Systems.Max(x => x.PointY);
            }
            else
            {
                MinSystemX = -10;
                MaxSystemX = 10;
                MinSystemY = -10;
                MaxSystemY = 10;
            }

            uxMapCanvas.Width = (MaxSystemX - MinSystemX);
            uxMapCanvas.Height = (MaxSystemY - MinSystemY);
        }

        private void RefreshSystems()
        {
            var toRemove = new List<Path>();
            foreach (var systemDot in uxMapCanvas.Children)
            {
                if (systemDot is Path)
                {
                    toRemove.Add((Path)systemDot);
                }
            }

            toRemove.ForEach(x => uxMapCanvas.Children.Remove(x));

            if (ParsedDataContext != null && ParsedDataContext.Systems != null)
            {
                var geoGroups = new Dictionary<Color, GeometryGroup>();

                foreach (var systemViewModel in ParsedDataContext.Systems)
                {
                    if (!geoGroups.ContainsKey(systemViewModel.SystemColor))
                    {
                        geoGroups.Add(systemViewModel.SystemColor, new GeometryGroup());
                    }

                    geoGroups[systemViewModel.SystemColor].Children.Add(new EllipseGeometry()
                    {
                        Center = new Point((systemViewModel.PointX - MinSystemX), (systemViewModel.PointY - MinSystemY)),
                        RadiusX = 5,
                        RadiusY = 5
                    });
                }

                foreach (var geoGroup in geoGroups)
                {
                    uxMapCanvas.Children.Add(new Path()
                    {
                        Fill = new SolidColorBrush(geoGroup.Key),
                        Data = geoGroup.Value
                    });
                }

                //foreach (var systemViewModel in ParsedDataContext.Systems)
                //{
                //    var newView = new SystemView();
                //    newView.DataContext = systemViewModel;

                //    Canvas.SetTop(newView, (systemViewModel.PointY - MinSystemY));
                //    Canvas.SetLeft(newView, (systemViewModel.PointX - MinSystemX));
                //    Canvas.SetZIndex(newView, 1);

                //    uxMapCanvas.Children.Add(newView);
                //}

                HandleMapResize(new Size(uxMapViewbox.ActualWidth, uxMapViewbox.ActualHeight));
            }
        }

        private void RefreshFocusPoint(Size uxMapViewboxSize)
        {
            if (ParsedDataContext != null)
            {
                Canvas.SetTop(uxMapCanvas, (ParsedDataContext.FocusPoint.Y * ParsedDataContext.Zoom) + (uxMapViewboxSize.Height / 2) - (uxMapCanvas.Height * ParsedDataContext.Zoom / 2));
                Canvas.SetLeft(uxMapCanvas, (ParsedDataContext.FocusPoint.X * ParsedDataContext.Zoom) + (uxMapViewboxSize.Width / 2) - (uxMapCanvas.Width * ParsedDataContext.Zoom / 2));
            }
        }

        private void RefreshLines()
        {
            var toRemove = new List<LineView>();
            foreach (var line in uxMapCanvas.Children)
            {
                if (line is LineView)
                {
                    toRemove.Add((LineView)line);
                }
            }

            toRemove.ForEach(x => uxMapCanvas.Children.Remove(x));

            if (ParsedDataContext != null && ParsedDataContext.Lines != null)
            {
                foreach (var lineViewModel in ParsedDataContext.Lines)
                {
                    var newView = new LineView();
                    newView.DataContext = lineViewModel;
                    Canvas.SetTop(newView, (lineViewModel.StartPointY - MinSystemY));
                    Canvas.SetLeft(newView, (lineViewModel.StartPointX - MinSystemX));
                    Canvas.SetZIndex(newView, 0);

                    uxMapCanvas.Children.Add(newView);
                }
            }
        }

        private void HandleMapResize(Size newSize)
        {
            RefreshFocusPoint(newSize);
        }

        #region Zoom

        private void MapMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) 
            {
                ParsedDataContext?.ZoomMap(1);
            }
            else if (e.Delta < 0) 
            {
                ParsedDataContext?.ZoomMap(-1);
            }
        }

        #endregion

        #region Move Map Offset

        private Point ArrowStartPosition { get; set; }

        private void MapMouseUp(object sender, MouseButtonEventArgs e)
        {
            uxMoveArrow.Visibility = Visibility.Collapsed;
            var endPosition = e.GetPosition(uxMapViewbox);

            var x = Math.Abs(ArrowStartPosition.X - endPosition.X);
            var y = Math.Abs(ArrowStartPosition.Y - endPosition.Y);

            if (10 > x && 10 > y)
            {
                // select system
                var mapPos = e.GetPosition(uxMapCanvas);
                ParsedDataContext?.SelectClosestSystem(new Point(mapPos.X + MinSystemX, mapPos.Y + MinSystemY));
            }
            else
            {
                // move map
                ParsedDataContext?.MoveMap(ArrowStartPosition, endPosition);
            }

            ReleaseMouseCapture(); 
        }

        private void MapMouseDown(object sender, MouseButtonEventArgs e)
        {            
            ArrowStartPosition = e.GetPosition(uxMapViewbox);
            Canvas.SetTop(uxMoveArrow, ArrowStartPosition.Y);
            Canvas.SetLeft(uxMoveArrow, ArrowStartPosition.X - 9);
            CaptureMouse();
        }

        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var mousePos = e.GetPosition(uxMapViewbox);

                var x = ArrowStartPosition.X - mousePos.X;
                var y = ArrowStartPosition.Y - mousePos.Y;
                var dist = Math.Sqrt(x * x + y * y);
                double degrees;

                degrees = Math.Atan2(y, x) * (180 / Math.PI);

                if (10 > Math.Abs(x) && 10 > Math.Abs(y))
                {
                    uxMoveArrow.Visibility = Visibility.Collapsed;
                }
                else
                {
                    uxMoveArrow.Visibility = Visibility.Visible;
                    uxMoveArrow.Height = Math.Max(dist, 10);
                    //uxMoveArrowScale.ScaleY = ;
                    uxMoveArrowRotate.Angle = degrees - 90;
                }
            }
        }

        #endregion
    }
}
