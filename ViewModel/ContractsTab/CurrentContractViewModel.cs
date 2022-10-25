using RtoTools.Common;
using RtoTools.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.ViewModel.ContractsTab
{
    public class CurrentContractViewModel : ViewModelBase
    {
        private ContractModel Contract { get; set; }

        public CurrentContractViewModel(ContractModel contract)
        {
            Contract = contract;

            SetupContractText();
        }

        public string TextContractDetail { get { return GetValue<string>(); } set { SetValue(value); } }

        public string TextInfo { get { return GetValue<string>(); } set { SetValue(value); } }

        private void SetupContractText()
        {
            var targetFaction = AppController.Instance.MapData?.Factions.FirstOrDefault(x => x.FactionId == Contract.TargetFactionId)?.Name;
            var system = AppController.Instance.MapData?.Systems.FirstOrDefault(x => x.SystemId == Contract.SystemId);
            var systemName = system?.Name;
            var controlAmount = Contract.TargetControl;
            var asFaction = AppController.Instance.MapData?.Factions.FirstOrDefault(x => x.FactionId == Contract.AsFactionId)?.Name;
            var currentControl = "ERROR";

            if (system != null)
            {
                if (Contract.ContractType == Common.Enum.ContractType.Fortify)
                {
                    currentControl = system.Ownership.FirstOrDefault(x => x.Key == AppController.Instance.SelectedFaction?.FactionId).Value.ToString();
                }
                else
                {
                    currentControl = system.Ownership.FirstOrDefault(x => x.Key == Contract.TargetFactionId).Value.ToString();
                }
            }

            switch (Contract.ContractType)
            {
                case Common.Enum.ContractType.Attack:
                    TextContractDetail = $"Attack {targetFaction} on {systemName}";
                    TextInfo = $"Current {targetFaction} control is {currentControl}%";
                    break;
                case Common.Enum.ContractType.Fortify:
                    TextContractDetail = $"Fortify {systemName} to {controlAmount}%";
                    TextInfo = $"Current {targetFaction} control is {currentControl}%";
                    break;
                case Common.Enum.ContractType.FalseFlag:
                    TextContractDetail = $"Attack {targetFaction} on {systemName}.  False Flag as {asFaction}.";
                    TextInfo = $"Current {targetFaction} control is {currentControl}%";
                    break;
                case Common.Enum.ContractType.DeniableAsset:
                    TextContractDetail = $"Attack {targetFaction} on {systemName}.  False Flag as {asFaction} [Deniable Asset].";
                    TextInfo = $"Current {targetFaction} control is {currentControl}%";
                    break;
                default:
                    TextContractDetail = $"ERROR: Unknown ContractType [{Contract.ContractType}]";
                    TextInfo = "ERROR";
                    break;
            }
        }
    }
}
