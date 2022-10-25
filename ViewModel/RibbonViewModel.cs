using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RtoTools.Common;
using RtoTools.Common.Localization;
using RtoTools.Model.Data;

namespace RtoTools.ViewModel
{
    public class RibbonViewModel : ViewModelBase
    {
        public RibbonViewModel()
        {
            AppController.Instance.RtoServerDataChanged += AppControlHelper_RtoServerDataChanged;

            SelectedLanguage = "en";

            PropertyChanged += RibbonViewModelPropertyChanged;
        }

        private void RibbonViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedLanguage":
                    AppController.Instance.SetLanguage(SelectedLanguage);
                    break;
                case "SelectedFaction":
                    AppController.Instance.SetSelectedFaction(SelectedFaction);
                    break;
            }
        }

        public string TextLanguage { get { return Localization.Language; } }

        public string TextYourFaction { get { return Localization.YourFaction; } }

        public string TextLastUpdated 
        {
            get 
            {
                return string.Format(Localization.LastUpdated, AppController.Instance.RtoServerAsDate?.ToString("yyyy-MM-dd hh:mm ") ?? "(no data)");
            }
        }

        public string TextRefreshData
        {
            get
            {
                var rtoServerAsDate = AppController.Instance.RtoServerRefreshCooldown;

                if (rtoServerAsDate != TimeSpan.Zero)
                {
                    return string.Format(Localization.RefreshDataCooldown, Math.Round(rtoServerAsDate.TotalSeconds));
                }
                else
                {
                    return Localization.RefreshDataReady;
                }
            }
        }

        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "RefreshData":
                    AppController.Instance.RefreshRtoServerData();
                    break;
                default:
                    base.Execute(parameter);
                    break;
            }
        }

        public override bool CanExecute(object parameter)
        {
            switch (parameter)
            {
                case "RefreshData":
                    return AppController.Instance.RtoServerRefreshCooldown == TimeSpan.Zero;
                default:
                    return base.CanExecute(parameter);
            }
        }

        private void AppControlHelper_RtoServerDataChanged()
        {
            RaisePropertyChanged("TextLastUpdated");
            UpdateRefreshButtonCooldown();

            var factionOptions = new List<DisplayValueOption<FactionModel>>();
            factionOptions.Insert(0, new DisplayValueOption<FactionModel>());

            if (AppController.Instance.MapData != null)
            {
                factionOptions.AddRange(AppController.Instance.MapData.Factions.Select(x => new DisplayValueOption<FactionModel>(x, x.Name)));
            }

            FactionOptions = factionOptions;
            SelectedFaction = null;
        }

        private void UpdateRefreshButtonCooldown()
        {
            RaisePropertyChanged("TextRefreshData");
            RaiseCanExecuteChanged();

            if (AppController.Instance.RtoServerRefreshCooldown != TimeSpan.Zero)
            {
                Task.Run(async delegate
                {
                    // get the UI to refresh in 500ms if refresh is not ready
                    await Task.Delay(500);
                    UpdateRefreshButtonCooldown();
                });
            }
        }

        public Dictionary<string, string> LanguageOptions { get; } = new Dictionary<string, string>()
        {
            { "en", "English" },
            { "cki", "Klingon" }
        };

        public string SelectedLanguage { get { return GetValue<string>(); } set { SetValue(value); } }

        public List<DisplayValueOption<FactionModel>> FactionOptions { get { return GetValue<List<DisplayValueOption<FactionModel>>>(); } set { SetValue(value); } }

        public FactionModel SelectedFaction { get { return GetValue<FactionModel>(); } set { SetValue(value); } }
    }
}
