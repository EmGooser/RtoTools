using RtoTools.Common;
using RtoTools.Common.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.ViewModel.ContractsTab
{
    public class ContractsTabViewModel : ViewModelBase
    {
        public ContractsTabViewModel()
        {
            ContractBuilderList = new List<ContractBuilderViewModel>()
            {
                new ContractBuilderViewModel(), new ContractBuilderViewModel(), new ContractBuilderViewModel(), 
                new ContractBuilderViewModel(), new ContractBuilderViewModel(), new ContractBuilderViewModel(),
                new ContractBuilderViewModel(), new ContractBuilderViewModel(), new ContractBuilderViewModel(), 
                new ContractBuilderViewModel()
            };

            AppController.Instance.RtoServerDataChanged += SingleRtoServerDataChanged;
            AppController.Instance.SelectedFactionChanged += SingleRtoServerDataChanged;

            IsContractBuilderExpanded = true;
        }

        private void SingleRtoServerDataChanged()
        {
            UpdateContracts();
        }

        public bool IsContractBuilderExpanded { get { return GetValue<bool>(); } set { SetValue(value); } }

        public bool IsCurrentContractsExpanded { get { return GetValue<bool>(); } set { SetValue(value); } }

        public bool IsStopHelpingMe { get { return GetValue<bool>(); } set { SetValue(value); } }

        public string TextCurrentContracts { get { return string.Format(Localization.CurrentContracts, CurrentContractList?.Count() ?? 0); } }

        public string TextDeniableAssetWarning { get { return Localization.DeniableAssetWarning; } }

        public string TextSaveFile { get { return Localization.SaveFile; } }

        public string TextLoadFile { get { return Localization.LoadFile; } }

        public List<CurrentContractViewModel> CurrentContractList { get { return GetValue<List<CurrentContractViewModel>>(); } set { SetValue(value); } }

        public string TextContractBuilder { get { return Localization.ContractBuilder; } }

        public List<ContractBuilderViewModel> ContractBuilderList { get { return GetValue<List<ContractBuilderViewModel>>(); } set { SetValue(value); } }

        public override void Execute(object? parameter)
        {
            switch (parameter)
            {
                case "ToggleContractBuilderExpanded":
                    IsContractBuilderExpanded = !IsContractBuilderExpanded;
                    break;
                case "ToggleCurrentContractsExpanded":
                    IsCurrentContractsExpanded = !IsCurrentContractsExpanded;
                    break;
                case "SaveFile":
                    AppController.Instance.SaveContracts(ContractBuilderList);
                    break;
                case "LoadFile":
                    HandleLoadFile();
                    break;
                default:
                    base.Execute(parameter);
                    break;
            }
        }

        private void HandleLoadFile()
        {
            var loadedData = AppController.Instance.LoadContracts();

            if (loadedData != null)
            {
                while (loadedData.Count() < 10)
                {
                    loadedData.Add(new ContractBuilderViewModel());
                }

                ContractBuilderList = loadedData;
            }
        }

        private void UpdateContracts()
        {
            var result = new List<CurrentContractViewModel>();

            if (AppController.Instance.SelectedFaction != null)
            {
                result.AddRange(AppController.Instance.SelectedFaction.Contracts.Select(x => new CurrentContractViewModel(x)));
            }

            CurrentContractList = result;
            RaisePropertyChanged("TextCurrentContracts");
        }

    }
}
