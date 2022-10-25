using RtoTools.Common;
using RtoTools.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RtoTools.ViewModel.MapTab
{
    public class SystemViewModel : ViewModelBase, IDisposable
    {
        public SystemViewModel(SystemModel system)
        {
            System = system;
            LabelFontSize = 10;

            AppController.Instance.SelectedFactionChanged += AppControllerSelectedFactionChanged;
            AppController.Instance.SelectedSystemChanged += AppControllerSelectedSystemChanged;
            AppController.Instance.SelectedSystemLabelTypeChanged += AppControllerSelectedSystemLabelTypeChanged;
        }

        public void Dispose()
        {
            AppController.Instance.SelectedFactionChanged -= AppControllerSelectedFactionChanged;
            AppController.Instance.SelectedSystemChanged -= AppControllerSelectedSystemChanged;
            AppController.Instance.SelectedSystemLabelTypeChanged -= AppControllerSelectedSystemLabelTypeChanged;
        }

        private void AppControllerSelectedFactionChanged()
        {
            RaisePropertyChanged("SelectionColor");
            RaisePropertyChanged("OwnershipHighlightColor");
            SetSystemLabel();
        }

        private void AppControllerSelectedSystemChanged()
        {
            RaisePropertyChanged("SelectionColor");
            RaisePropertyChanged("OwnershipHighlightColor");
        }

        private void AppControllerSelectedSystemLabelTypeChanged()
        {
            SetSystemLabel();
        }

        internal SystemModel System { get; set; }

        public double PointY { get { return System.Cordinates.Y; } }

        public double PointX { get { return System.Cordinates.X; } }

        public string SystemLabel { get { return GetValue<string>(); } set { SetValue(value); } }

        public double LabelFontSize { get { return GetValue<double>(); } set { SetValue(value); } }

        public Color SystemLabelColor { get { return GetValue<Color>(); } set { SetValue(value); } }

        private void SetSystemLabel()
        {
            switch (AppController.Instance.SelectedSystemLabelType)
            {
                case SystemLabelType.MyControlPercent:
                    SystemLabelColor = System.OwnerFactionId == AppController.Instance.SelectedFaction?.FactionId ? Colors.White : Colors.Red;
                    SystemLabel = $"{CommonLogicHelper.GetSystemControl(System, AppController.Instance.SelectedFaction?.FactionId)}%";
                    break;
                case SystemLabelType.OwnerControlPercent:
                    SystemLabelColor = Colors.White;
                    SystemLabel = $"{CommonLogicHelper.GetSystemControl(System)}%";
                    break;
                case SystemLabelType.SupportProvided:
                    SystemLabelColor = Colors.White;
                    SystemLabel = $"+{CommonLogicHelper.GetSystemSupport(System)}";
                    break;
                case SystemLabelType.MyMissionSupport:
                    var missionSupport = CommonLogicHelper.GetSystemMissionSupport(System, AppController.Instance.SelectedFaction?.FactionId);
                    if (missionSupport == null)
                    {
                        SystemLabelColor = Colors.Gray;
                        SystemLabel = "n/a";
                    }
                    else if (AppController.Instance.SelectedFaction == null || System.OwnerFactionId == AppController.Instance.SelectedFaction.FactionId)
                    {
                        SystemLabelColor = Colors.White;
                        SystemLabel = $"+{missionSupport}";
                    }
                    else
                    {
                        SystemLabelColor = Colors.Red;
                        SystemLabel = missionSupport < 0 ? $"{missionSupport}" : $"+{missionSupport}";
                    }
                    break;
                default:
                    SystemLabelColor = Colors.White;
                    SystemLabel = String.Empty;
                    break;
            }
        }

        public Color SelectionColor
        {
            get
            {
                if (AppController.Instance.SelectedSystem == System)
                {
                    return Colors.White;
                }
                else if (AppController.Instance.SelectedFaction != null)
                {
                    if (AppController.Instance.SelectedFaction.FactionId == System.HomeworldFactionId)
                    {
                        return AppController.Instance.SelectedFaction.MapColor;
                    }
                }

                return Colors.Transparent;
            }
        }

        private readonly Color FullControlHighlight = Color.FromArgb(240, 255, 255, 255);

        private readonly Color OwnedControlHighlight = Color.FromArgb(200, 255, 255, 255);

        private readonly Color SomeControlHighlight = Color.FromArgb(150, 255, 255, 255);

        public Color OwnershipHighlightColor
        {
            get
            {
                if (AppController.Instance.SelectedFaction != null && System.Ownership.ContainsKey(AppController.Instance.SelectedFaction.FactionId))
                {
                    if (System.Ownership[AppController.Instance.SelectedFaction.FactionId] >= 100)
                    {
                        return FullControlHighlight;
                    }
                    else if (AppController.Instance.SelectedFaction.FactionId == System.OwnerFactionId)
                    {
                        return OwnedControlHighlight;
                    }
                    else if (System.Ownership[AppController.Instance.SelectedFaction.FactionId] > 0)
                    {
                        return SomeControlHighlight;
                    }
                }

                return Colors.Transparent;
            }
        }

        public Color SystemColor
        {
            get
            {
                var faction = AppController.Instance.MapData?.Factions.FirstOrDefault(x => x.FactionId == System.OwnerFactionId);
                return faction == null ? Colors.DarkGray : faction.MapColor;
            }
        }

        public override void Execute(object? parameter)
        {
            switch (parameter)
            {
                case "SelectSystem":
                    AppController.Instance.SetSelectedSystem(System);
                    break;
                default:
                    base.Execute(parameter);
                    break;
            }
        }
    }
}
