using RtoTools.Common;
using RtoTools.Common.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.ViewModel.MapTab
{
    public class SystemDetailViewModel : ViewModelBase
    {
        public SystemDetailViewModel()
        {
            AppController.Instance.SelectedSystemChanged += AppControllerSelectedSystemChanged;
        }

        private void AppControllerSelectedSystemChanged()
        {
            RaisePropertyChanged("ShowDetails");
            RaisePropertyChanged("SystemName");
            RaisePropertyChanged("CurrentOwner");
            RaisePropertyChanged("OwnerControl");
            RaisePropertyChanged("ProvidedSupport");
            RaisePropertyChanged("OtherFactionControlItems");            
        }

        public List<DisplayValueOption<SystemLabelType?>> SystemLabelOptions { get; } =
            new List<DisplayValueOption<SystemLabelType?>>()
            {
                new DisplayValueOption<SystemLabelType?>(null, "(None)"),
                new DisplayValueOption<SystemLabelType?>(SystemLabelType.OwnerControlPercent, "Owner Control %"),
                new DisplayValueOption<SystemLabelType?>(SystemLabelType.MyControlPercent, "Your Control %"),
                new DisplayValueOption<SystemLabelType?>(SystemLabelType.SupportProvided, "Owner Support"),
                new DisplayValueOption<SystemLabelType?>(SystemLabelType.MyMissionSupport, "Your Mission Support")
            };

        public string TextSystemName { get { return Localization.SystemName; } }

        public string TextCurrentOwner { get { return Localization.CurrentOwner; } }

        public string TextOwnerControl { get { return Localization.OwnerControl; } }

        public string TextProvidedSupport { get { return Localization.ProvidedSupport; } }

        public string TextOtherFactionControl { get { return Localization.OtherFactionControl; } }

        public SystemLabelType? SelectedSystemLabel 
        {
            get 
            {
                return GetValue<SystemLabelType?>(); 
            }
            set
            {
                SetValue(value);
                AppController.Instance.SetSystemLabel(value);
            }
        }

        public bool ShowDetails
        {
            get { return AppController.Instance.SelectedSystem != null; }
        }

        public string SystemName 
        {
            get { return AppController.Instance.SelectedSystem?.Name ?? String.Empty; } 
        }

        public string CurrentOwner 
        {
            get 
            {
                var faction = AppController.Instance.MapData?.Factions
                    .FirstOrDefault(x => x.FactionId == AppController.Instance.SelectedSystem?.OwnerFactionId);

                return faction?.Name ?? String.Empty; 
            } 
        }

        public string OwnerControl 
        {
            get 
            {
                var ownerships = AppController.Instance.SelectedSystem?.Ownership
                    .Where(o => o.Key == AppController.Instance.SelectedSystem?.OwnerFactionId);

                if (ownerships?.Any() == true)
                {
                    return $"{ownerships.First().Value}%";
                }
                else
                {
                    return "0%";
                }
            }
        }

        public string ProvidedSupport 
        {
            get 
            {
                var support = CommonLogicHelper.GetSystemSupport(AppController.Instance.SelectedSystem);
                return $"+{support}";
            } 
        }

        public List<string> OtherFactionControlItems
        {
            get
            {
                var ownerships = AppController.Instance.SelectedSystem?.Ownership
                    .Where(o => o.Key != AppController.Instance.SelectedSystem?.OwnerFactionId);

                if (ownerships?.Any() == true)
                {
                    return ownerships.OrderByDescending(x => x.Value).Select(x =>
                    {
                        var faction = AppController.Instance.MapData?.Factions.FirstOrDefault(faction => faction.FactionId == x.Key);
                        return $"{x.Value}% {faction?.Name ?? "UNKNOWN"}";
                    }).ToList();
                }
                else
                {
                    return new List<string>() { "none" };
                }
            }
        }
    }
}
