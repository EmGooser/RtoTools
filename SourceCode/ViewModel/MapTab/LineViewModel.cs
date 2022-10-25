using RtoTools.Common;
using RtoTools.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RtoTools.ViewModel.MapTab
{
    public class LineViewModel : ViewModelBase
    {
        public LineViewModel(SystemModel system1, SystemModel system2)
        {
            System1 = system1;
            System2 = system2;
        }

        private SystemModel System1 { get; set; }

        private SystemModel System2 { get; set; }

        public double StartPointX { get { return System1.Cordinates.X; } }

        public double StartPointY { get { return System1.Cordinates.Y; } }

        public double EndPointX { get { return (System2.Cordinates.X - System1.Cordinates.X); } }

        public double EndPointY { get { return (System2.Cordinates.Y - System1.Cordinates.Y); } }


        /*
         * 


            if (AppController.Instance.MapData != null && AppController.Instance.SelectedSystem != null)
            {
                var aSystem = AppController.Instance.SelectedSystem;

                foreach (var connectionSystemId in AppController.Instance.SelectedSystem.ConnectedSystems)
                {
                    var bSystem = AppController.Instance.MapData.Systems.First(x => x.SystemId == connectionSystemId);

                    uxMapCanvas.Children.Insert(0, new Line()
                    {
                        X1 = (aSystem.Cordinates.X + XOffset) * Zoom,
                        Y1 = (aSystem.Cordinates.Y + YOffset) * Zoom,
                        X2 = (bSystem.Cordinates.X + XOffset) * Zoom,
                        Y2 = (bSystem.Cordinates.Y + YOffset) * Zoom,
                        Stroke = new SolidColorBrush(Colors.White),
                        StrokeThickness = 2
                    });
                }
            }

         * 
         * 
         */
    }
}
