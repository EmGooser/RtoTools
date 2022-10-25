using RtoTools.Common;
using RtoTools.Common.Enum;
using RtoTools.Common.Localization;
using RtoTools.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.ViewModel.ContractsTab
{
    public class ContractBuilderViewModel : ViewModelBase
    {
        public ContractBuilderViewModel()
        {
            PropertyChanged += ContractBuilderViewModelPropertyChanged;

            AppController.Instance.RtoServerDataChanged += SingleRtoServerDataChanged;
            AppController.Instance.SelectedFactionChanged += InstanceSelectedFactionChanged;

            SingleRtoServerDataChanged();
            SetInfoAndErrors();
        }

        private void InstanceSelectedFactionChanged()
        {
            SetInfoAndErrors();
        }

        public string TextContractType { get { return Localization.ContractType; } }

        public string TextTargetSystem { get { return Localization.TargetSystem; } }

        public string TextTargetFaction { get { return Localization.TargetFaction; } }

        public string TextTargetControl { get { return Localization.TargetControl; } }

        public string TextAsFaction { get { return Localization.AsFaction; } }

        public string? TextInfo { get { return GetValue<string?>(); } set { SetValue(value); } }

        public string? TextError { get { return GetValue<string?>(); } set { SetValue(value); } }

        public int TargetControl { get { return GetValue<int>(); } set { SetValue(value); } }

        public bool IsAttack { get { return SelectedContractType == ContractType.Attack; } }

        public bool IsFortify { get { return SelectedContractType == ContractType.Fortify; } }

        public bool IsFalseFlag { get { return SelectedContractType == ContractType.FalseFlag; } }

        public bool IsDeniableAsset { get { return SelectedContractType == ContractType.DeniableAsset; } }

        public List<DisplayValueOption<ContractType?>> ContractTypeOptions { get; } = new List<DisplayValueOption<ContractType?>>()
        {
            new DisplayValueOption<ContractType?>(null, "(Select)"),
            new DisplayValueOption<ContractType?>(ContractType.Attack, "Attack"),
            new DisplayValueOption<ContractType?>(ContractType.Fortify, "Fortify"),
            new DisplayValueOption<ContractType?>(ContractType.FalseFlag, "False Flag"),
            new DisplayValueOption<ContractType?>(ContractType.DeniableAsset, "Deniable Asset")
        };

        public ContractType? SelectedContractType { get { return GetValue<ContractType?>(); } set { SetValue(value); } }

        public List<DisplayValueOption<SystemModel>> TargetSystemOptions { get { return GetValue<List<DisplayValueOption<SystemModel>>>(); } set { SetValue(value); } }

        public SystemModel? SelectedTargetSystem { get { return GetValue<SystemModel>(); } set { SetValue(value); } }

        public List<DisplayValueOption<FactionModel>> TargetFactionOptions { get { return GetValue<List<DisplayValueOption<FactionModel>>>(); } set { SetValue(value); } }

        public FactionModel? SelectedTargetFaction { get { return GetValue<FactionModel>(); } set { SetValue(value); } }

        public List<DisplayValueOption<FactionModel>> AsFactionOptions { get { return GetValue<List<DisplayValueOption<FactionModel>>>(); } set { SetValue(value); } }

        public FactionModel? SelectedAsFaction { get { return GetValue<FactionModel>(); } set { SetValue(value); } }
        
        private void ContractBuilderViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedContractType":
                    OnContractTypeChanged();
                    SetInfoAndErrors();
                    break;
                case "SelectedTargetSystem":
                case "SelectedTargetFaction":
                case "SelectedAsFaction":
                case "TargetControl":
                    SetInfoAndErrors();
                    break;
            }
        }

        private void OnContractTypeChanged()
        {
            RaisePropertyChanged("IsAttack");
            RaisePropertyChanged("IsFortify");
            RaisePropertyChanged("IsFalseFlag");
            RaisePropertyChanged("IsDeniableAsset");
        }

        private void SingleRtoServerDataChanged()
        {
            TargetSystemOptions = GetAllSystems();
            TargetFactionOptions = AsFactionOptions = GetAllFactions();
            
            if (TargetSystemOptions.Count(x => x.Value == SelectedTargetSystem) == 0)
            {
                SelectedTargetSystem = null;
            }

            if (TargetFactionOptions.Count(x => x.Value == SelectedTargetFaction) == 0)
            {
                SelectedTargetFaction = null;
            }

            if (AsFactionOptions.Count(x => x.Value == SelectedAsFaction) == 0)
            {
                SelectedAsFaction = null;
            }

            SetInfoAndErrors();
        }

        private List<DisplayValueOption<SystemModel>> GetAllSystems()
        {
            // get the options for selection and the display for them
            var systemOptions = AppController.Instance.MapData?.Systems
                .Select(x => new DisplayValueOption<SystemModel>(x, x.Name))
                .ToList() ?? new List<DisplayValueOption<SystemModel>>();

            // add the "null" option
            systemOptions.Insert(0, new DisplayValueOption<SystemModel>());

            return systemOptions;
        }

        private List<DisplayValueOption<FactionModel>> GetAllFactions()
        {
            // add all the factions
            var factionOptions = AppController.Instance.MapData?.Factions
                .Select(x => new DisplayValueOption<FactionModel>(x, x.Name))
                .ToList() ?? new List<DisplayValueOption<FactionModel>>();

            // add the "null" option
            factionOptions.Insert(0, new DisplayValueOption<FactionModel>());

            return factionOptions;
        }

        private void SetInfoAndErrors()
        {
            if (SelectedContractType != null)
            {
                if (AppController.Instance.MapData == null)
                {
                    TextError = "You must get the map data from the server (top right 'Refresh' button)";

                }
                else if (AppController.Instance.SelectedFaction == null)
                {
                    TextError = "You must select your faction (top left)";

                }
                else if (SelectedTargetSystem == null)
                {
                    TextError = "You must select a target system";
                }
                else
                {
                    switch (SelectedContractType)
                    {
                        case ContractType.Attack:
                            SetInfoForAttack();
                            SetErrorsForAttack();
                            break;
                        case ContractType.Fortify:
                            SetInfoForFority();
                            SetErrorsForFority();
                            break;
                        case ContractType.DeniableAsset:
                        case ContractType.FalseFlag:
                            SetInfoForFalseFlag();
                            SetErrorsForFalseFlag();
                            break;
                    }
                }
            }
        }

        private void SetInfoForAttack()
        {
            var infos = new List<string>();

            if (AppController.Instance.MapData != null && AppController.Instance.SelectedFaction != null && SelectedTargetSystem != null)
            {
                var factionControl = SelectedTargetSystem.Ownership
                        .FirstOrDefault(x => x.Key == AppController.Instance.SelectedFaction.FactionId);

                infos.Add($"{AppController.Instance.SelectedFaction.Name} Control {factionControl.Value}%");

                if (SelectedTargetFaction != null)
                {
                    var targetControl = SelectedTargetSystem.Ownership
                        .FirstOrDefault(x => x.Key == SelectedTargetFaction.FactionId);

                    infos.Add($"{SelectedTargetFaction.Name} Control {targetControl.Value}%");

                    if (targetControl.Value == 0)
                    {
                        infos.Add($"WARNING: [{SelectedTargetFaction.Name}] has no control to attack on [{SelectedTargetSystem.Name}]");
                    }
                }

                var hasNearbySystem = AppController.Instance.MapData.Systems
                    .Where(x => SelectedTargetSystem.ConnectedSystems.Contains(x.SystemId))
                    .Where(x => x.OwnerFactionId == AppController.Instance.SelectedFaction.FactionId)
                    .Any();

                if (!hasNearbySystem)
                {
                    infos.Add($"WARNING: [{AppController.Instance.SelectedFaction.Name}] has no way to attack [{SelectedTargetSystem.Name}]");
                }
            }

            if (infos.Any())
            {
                TextInfo = string.Join(Environment.NewLine, infos);
            }
            else
            {
                TextInfo = null;
            }
        }

        private void SetErrorsForAttack()
        {
            if (SelectedTargetFaction == null)
            {
                TextError = "You must select a target faction";
            }
            else if (SelectedTargetFaction == AppController.Instance.SelectedFaction)
            {
                TextError = "You cant target your own faction";
            }
            else
            {
                TextError = null;
            }    
        }

        private void SetInfoForFority()
        {
            var infos = new List<string>();

            if (AppController.Instance.MapData != null && AppController.Instance.SelectedFaction != null && SelectedTargetSystem != null)
            {
                var factionControl = SelectedTargetSystem.Ownership
                        .FirstOrDefault(x => x.Key == AppController.Instance.SelectedFaction.FactionId);

                infos.Add($"{AppController.Instance.SelectedFaction.Name} Control {factionControl.Value}%");

                if (factionControl.Value < 100)
                {
                    infos.Add($"WARNING: [{AppController.Instance.SelectedFaction.Name}] has [{factionControl.Value}%] control of [{SelectedTargetSystem.Name}]");
                }

                var hasNearbySystem = AppController.Instance.MapData.Systems
                    .Where(x => SelectedTargetSystem.ConnectedSystems.Contains(x.SystemId))
                    .Where(x => x.OwnerFactionId == AppController.Instance.SelectedFaction.FactionId)
                    .Any();

                if (!hasNearbySystem)
                {
                    infos.Add($"WARNING: [{AppController.Instance.SelectedFaction.Name}] has no way to attack [{SelectedTargetSystem.Name}]");
                }
            }

            if (infos.Any())
            {
                TextInfo = string.Join(Environment.NewLine, infos);
            }
            else
            {
                TextInfo = null;
            }
        }

        private void SetErrorsForFority()
        {
            if (TargetControl < 100 || TargetControl > 300)
            {
                TextError = "Target control must be set between 100 and 300";
            }
            else
            {
                TextError = null;
            }
        }

        private void SetInfoForFalseFlag()
        {
            var infos = new List<string>();

            if (AppController.Instance.MapData != null && AppController.Instance.SelectedFaction != null && 
                SelectedTargetSystem != null && SelectedAsFaction != null)
            {
                var factionControl = SelectedTargetSystem.Ownership
                        .FirstOrDefault(x => x.Key == SelectedAsFaction.FactionId);

                infos.Add($"{SelectedAsFaction.Name} Control {factionControl.Value}%");

                if (SelectedTargetFaction != null)
                {
                    var targetControl = SelectedTargetSystem.Ownership
                        .FirstOrDefault(x => x.Key == SelectedTargetFaction.FactionId);

                    infos.Add($"{SelectedTargetFaction.Name} Control {targetControl.Value}%");

                    if (targetControl.Value == 0)
                    {
                        infos.Add($"WARNING: [{SelectedTargetFaction.Name}] has no control to attack on [{SelectedTargetSystem.Name}]");
                    }
                }

                var hasNearbySystem = AppController.Instance.MapData.Systems
                    .Where(x => SelectedTargetSystem.ConnectedSystems.Contains(x.SystemId))
                    .Where(x => x.OwnerFactionId == SelectedAsFaction.FactionId)
                    .Any();

                if (!hasNearbySystem)
                {
                    infos.Add($"WARNING: [{SelectedAsFaction.Name}] has no way to attack [{SelectedTargetSystem.Name}]");
                }
            }

            if (infos.Any())
            {
                TextInfo = string.Join(Environment.NewLine, infos);
            }
            else
            {
                TextInfo = null;
            }
        }

        private void SetErrorsForFalseFlag()
        {
            if (SelectedTargetFaction == null)
            {
                TextError = "You must select a target faction";
            }
            else if (SelectedTargetFaction == AppController.Instance.SelectedFaction)
            {
                TextError = "You cant target your own faction";
            }
            else if (SelectedAsFaction == null)
            {
                TextError = "You must select a target as faction";
            }
            else if (SelectedAsFaction == AppController.Instance.SelectedFaction)
            {
                TextError = "You cant False Flag as your own faction";
            }
            else if (SelectedAsFaction == SelectedTargetFaction)
            {
                TextError = "The cant False Flag a faction targeting itself";
            }
            else
            {
                TextError = null;
            }
        }
    }
}
