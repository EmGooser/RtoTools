using RtoTools.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RtoTools.ViewModel.MapTab
{
    public class MapTabViewModel : ViewModelBase
    {
        public MapTabViewModel()
        {
            Zoom = 1;

            AppController.Instance.RtoServerDataChanged += InstanceRtoServerDataChanged;
            AppController.Instance.SelectedFactionChanged += InstanceSelectedFactionChanged;
            AppController.Instance.SelectedSystemChanged += InstanceSelectedSystemChanged;
        }

        private void InstanceRtoServerDataChanged()
        {
            if (AppController.Instance.MapData != null)
            {
                if (Systems != null)
                {
                    Systems.ForEach(x => x.Dispose());
                }

                Systems = AppController.Instance.MapData.Systems.Select(x => new SystemViewModel(x)).ToList();
                var minX = Systems.Min(x => x.PointX);
                var maxX = Systems.Max(x => x.PointX);
                var minY = Systems.Min(x => x.PointY);
                var maxY = Systems.Max(x => x.PointY);

                FocusPointBounds = new Rect(minX, minY, maxX - minX, maxY - minY);

                SystemsBackground = AppController.Instance.BuildSystemsBackgroundImage();

            }
            else
            {
                Systems = new List<SystemViewModel>();
                FocusPointBounds = new Rect(-10, -10, 20, 20);
            }

            InstanceSelectedSystemChanged();
        }

        private void InstanceSelectedSystemChanged()
        {
            if (AppController.Instance.SelectedSystem != null)
            {
                SelectionTop = AppController.Instance.SelectedSystem.Cordinates.Y - FocusPointBounds.Y;
                SelectionLeft = AppController.Instance.SelectedSystem.Cordinates.X - FocusPointBounds.X;
                ShowSelection = true;

                var faction = AppController.Instance.MapData?.Factions
                    .FirstOrDefault(x => x.FactionId == AppController.Instance.SelectedSystem.OwnerFactionId);

                if (faction != null)
                {
                    SelectionOwnershipColor = faction.MapColor;
                }
                else
                {
                    SelectionOwnershipColor = Colors.White;
                }

                Lines = AppController.Instance.SelectedSystem.ConnectedSystems
                    .Select(conSystemId =>
                    {
                        var conSystem = AppController.Instance.MapData?.Systems.FirstOrDefault(x => x.SystemId == conSystemId);

                        if (conSystem != null)
                        {
                            return new LineViewModel(AppController.Instance.SelectedSystem, conSystem);
                        }
                        else
                        {
                            return null;
                        }
                    }).Where(x => x != null).Cast<LineViewModel>().ToList();
            }
            else
            {
                Lines = new List<LineViewModel>();
                ShowSelection = false;
            }
        }

        private void InstanceSelectedFactionChanged()
        {

        }

        public ImageSource SystemsBackground { get { return GetValue<ImageSource>(); } set { SetValue(value); } }

        public SystemDetailViewModel SystemDetail { get; } = new SystemDetailViewModel();

        public double SelectionTop { get { return GetValue<double>(); } set { SetValue(value); } }

        public double SelectionLeft { get { return GetValue<double>(); } set { SetValue(value); } }

        public Color SelectionOwnershipColor { get { return GetValue<Color>(); } set { SetValue(value); } }

        public bool ShowSelection { get { return GetValue<bool>(); } set { SetValue(value); } }

        public double Zoom 
        {
            get { return GetValue<double>(); } 
            set
            {
                if (Systems != null)
                {
                    Systems.ForEach(s => s.LabelFontSize = 10 - (value * 0.7));
                }
                SetValue(value); 
            } 
        }

        public Point FocusPoint { get { return GetValue<Point>(); } set { SetValue(value); } }

        public List<SystemViewModel> Systems { get { return GetValue<List<SystemViewModel>>(); } set { SetValue(value); } }

        public List<LineViewModel> Lines { get { return GetValue<List<LineViewModel>>(); } set { SetValue(value); } }

        private Rect FocusPointBounds { get; set; }

        internal void MoveMap(Point prevMouse, Point nextMouse)
        {
            var newX = FocusPoint.X + ((nextMouse.X - prevMouse.X) / Zoom);
            var newY = FocusPoint.Y + ((nextMouse.Y - prevMouse.Y) / Zoom);

            newX = Math.Min(Math.Max(newX, FocusPointBounds.X), FocusPointBounds.X + FocusPointBounds.Width);
            newY = Math.Min(Math.Max(newY, FocusPointBounds.Y), FocusPointBounds.Y + FocusPointBounds.Height);

            FocusPoint = new Point(newX, newY);
        }

        internal void ZoomMap(int changeDirection)
        {
            var newZoom = Zoom;

            if (changeDirection < 0)
            {
                newZoom = Zoom * 0.9;
            }
            else if (changeDirection > 0)
            {
                newZoom = Zoom * 1.1;
            }
            
            Zoom = Math.Max(0.1, Math.Min(10, newZoom));
        }

        internal void SelectClosestSystem(Point position)
        {
            var systemsInRange = Systems.Where(x =>
                x.PointX > position.X - 10 && x.PointX < position.X + 10 &&
                x.PointY > position.Y - 10 && x.PointY < position.Y + 10);

            var system = systemsInRange.FirstOrDefault();
            if (system != null)
            {
                AppController.Instance.SetSelectedSystem(system.System);
            }
        }
    }
}


