using RtoTools.Common;
using RtoTools.Common.Enum;
using RtoTools.Model.Data;
using RtoTools.Model.Processing.FileIO;
using RtoTools.ViewModel.ContractsTab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace RtoTools.Model.Processing
{
    internal class FileIOProcessing
    {
        internal void SaveContractFile(FileInfo file, List<ContractBuilderViewModel> contractBuilderList)
        {
            // throwing all this into anon objects as this data structure is only used here and it will be set the clipboard
            var contracts = new List<v1_contract>();
            var id = 1;

            // this should already be validated by the caller by this point but just in case have all the null checks in place
            foreach (var contractVm in contractBuilderList)
            {
                switch (contractVm.SelectedContractType)
                {
                    case ContractType.Attack:
                        contracts.Add(new v1_contract()
                        {
                            asFaction = string.Empty,
                            contractId = $"Contract_Request_{id++}",
                            contractType = "Attack",
                            starSystem = contractVm.SelectedTargetSystem?.ServerName ?? string.Empty,
                            targetControl = 0,
                            targetFaction = contractVm.SelectedTargetFaction?.ServerName ?? string.Empty
                        });
                        break;
                    case ContractType.Fortify:
                        contracts.Add(new v1_contract()
                        {
                            asFaction = string.Empty,
                            contractId = $"Contract_Request_{id++}",
                            contractType = "Fortify",
                            starSystem = contractVm.SelectedTargetSystem?.ServerName ?? string.Empty,
                            targetControl = contractVm.TargetControl,
                            targetFaction = string.Empty
                        });
                        break;
                    case ContractType.FalseFlag:
                        contracts.Add(new v1_contract()
                        {
                            asFaction = contractVm.SelectedAsFaction?.ServerName ?? string.Empty,
                            contractId = $"Contract_Request_{id++}",
                            contractType = "FalseFlag",
                            starSystem = contractVm.SelectedTargetSystem?.ServerName ?? string.Empty,
                            targetControl = 0,
                            targetFaction = contractVm.SelectedTargetFaction?.ServerName ?? string.Empty
                        });
                        break;
                    case ContractType.DeniableAsset:
                        contracts.Add(new v1_contract()
                        {
                            asFaction = contractVm.SelectedAsFaction?.ServerName ?? string.Empty,
                            contractId = $"Contract_Request_{id++}",
                            contractType = "DeniableAsset",
                            starSystem = contractVm.SelectedTargetSystem?.ServerName ?? string.Empty,
                            targetControl = 0,
                            targetFaction = contractVm.SelectedTargetFaction?.ServerName ?? string.Empty
                        });
                        break;
                }                
            }

            // Serialize and copy to the clipboard
            var jsonObject = new v1_root()
            {
                version = 1,
                contracts = contracts
            };

            string jsonString = JsonSerializer.Serialize(jsonObject);

            using (var fs = file.Open(FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(jsonString);
            }
        }

        internal List<ContractBuilderViewModel> LoadContractFile(FileInfo file)
        {
            var jsonString = string.Empty;

            using (var fs = file.OpenRead())
            using (var sr = new StreamReader(fs))
            {
                jsonString = sr.ReadToEnd();
            }

            var parsedJson = JsonSerializer.Deserialize<v1_root>(jsonString);

            var result = new List<ContractBuilderViewModel>();

            foreach (var contract in parsedJson?.contracts ?? new List<v1_contract>())
            {
                result.Add(new ContractBuilderViewModel()
                {
                    SelectedContractType = ContractTypeLookup(contract.contractType),
                    SelectedAsFaction = FactionLookup(contract.asFaction),
                    SelectedTargetSystem = SystemLookup(contract.starSystem),
                    SelectedTargetFaction = FactionLookup(contract.targetFaction),
                    TargetControl = contract.targetControl
                });
            }

            return result;
        }

        internal void SaveDebugLogs(FileInfo file, List<DebugEventModel> debugEvents)
        {
            var sBuilder = new StringBuilder();

            debugEvents.ForEach(x => sBuilder.AppendLine(x.ToString()));

            using (var fs = file.Open(FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(sBuilder);
            }
        }

        private ContractType ContractTypeLookup(string contractType)
        {
            var result = ContractType.Attack;
            Enum.TryParse<ContractType>(contractType, out result);
            return result;
        }

        private FactionModel? FactionLookup(string faction)
        {
            return AppController.Instance.MapData?.Factions.FirstOrDefault(x => x.ServerName == faction);
        }

        private SystemModel? SystemLookup(string system)
        {
            return AppController.Instance.MapData?.Systems.FirstOrDefault(x => x.ServerName == system);
        }
    }
}