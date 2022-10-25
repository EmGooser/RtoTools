using Newtonsoft.Json.Linq;
using RtoTools.Common;
using RtoTools.Model.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RtoTools.Model.Processing
{
    internal class ServerProcessing
    {
        private const string TestFactionFileName = "testingFactions.json";
        private const string TestSystemFileName = "testingsystems.json";

        /// <summary>
        /// Requests the latest Map data from the RTO server
        /// </summary>
        /// <returns></returns>
        public MapModel FetchMapData()
        {
            // This will load the testing files until I get this all wired up to the RTO server with Jammie

            var testData = new MapModel();

            try
            {
                var factionIdCapitolCashe = new Dictionary<int, string>();

                if (File.Exists(TestFactionFileName))
                {
                    var rawJson = File.ReadAllText(TestFactionFileName);
                    var jObj = JObject.Parse(rawJson);
                    var id = 1;

                    foreach (var item in jObj)
                    {
                        testData.Factions.Add(new FactionModel()
                        {
                            FactionId = id++,
                            ServerName = item.Key,
                            Name = (string)item.Value["prettyName"],
                            MapColor = (Color)ColorConverter.ConvertFromString((string)item.Value["colour"]),
                            Contracts = new List<ContractModel>()
                        });

                        if (!string.IsNullOrEmpty((string)item.Value["capital"]))
                        {
                            factionIdCapitolCashe.Add(id, (string)item.Value["capital"]);
                        }
                    }
                }

                if (File.Exists(TestSystemFileName))
                {
                    var rawJson = File.ReadAllText(TestSystemFileName);
                    var jObj = JObject.Parse(rawJson);
                    var id = 1;

                    foreach (var item in jObj["systems"].ToList())
                    {
                        var ownerFactionId = testData.Factions.FirstOrDefault(x => x.ServerName == (string)item["owner"])?.FactionId ?? 1;
                        var homeworldFactionId = factionIdCapitolCashe.FirstOrDefault(x => x.Value == (string)item["name"]).Key;

                        testData.Systems.Add(new SystemModel()
                        {
                            SystemId = id++,
                            ServerName = (string)item["name"],
                            Name = (string)item["name"],
                            OwnerFactionId = ownerFactionId,
                            Cordinates = new System.Windows.Point((double)item["posX"] * 2, -(double)item["posY"] * 2),
                            HomeworldFactionId = homeworldFactionId
                        });
                    }
                }


                var range = 50 * 2; // range of connections
                var rangeSq = range * range;

                foreach (var targetSystem in testData.Systems)
                {
                    

                    var minX = targetSystem.Cordinates.X - range;
                    var maxX = targetSystem.Cordinates.X + range;
                    var minY = targetSystem.Cordinates.Y - range;
                    var maxY = targetSystem.Cordinates.Y + range;

                    // making a simple min max check to remove the obvious not in range systems
                    foreach (var connectedSystem in testData.Systems.Where(x => 
                        x.Cordinates.X >= minX && x.Cordinates.X <= maxX &&
                        x.Cordinates.Y >= minY && x.Cordinates.Y <= maxY &&
                        x.SystemId != targetSystem.SystemId))
                    {
                        var xDif = Math.Abs(targetSystem.Cordinates.X - connectedSystem.Cordinates.X);
                        var yDif = Math.Abs(targetSystem.Cordinates.Y - connectedSystem.Cordinates.Y);
                        var actualRangeSq = xDif * xDif + yDif * yDif;

                        if (actualRangeSq <= rangeSq)
                        {
                            targetSystem.ConnectedSystems.Add(connectedSystem.SystemId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppController.Instance.ErrorHandler("Error loading Testing JSON Files", ex);
            }

            /* Start Test Data
             * 
            var testData = new MapModel()
            {
                AsOf = DateTimeOffset.Now,
                Factions = new List<FactionModel>()
                {
                    new FactionModel() { FactionId = 1, ServerName = "TestFaction1", MapColor = Colors.Blue,
                        Contracts = new List<ContractModel>() {
                            new ContractModel() { ContractId = 1, ContractType = Common.Enum.ContractType.Attack, SystemId = 2, TargetFactionId = 2 },
                            new ContractModel() { ContractId = 2, ContractType = Common.Enum.ContractType.Fortify, SystemId = 1, TargetControl = 150 },
                            new ContractModel() { ContractId = 3, ContractType = Common.Enum.ContractType.FalseFlag, SystemId = 7, AsFactionId = 2, TargetFactionId = 3 }
                        }},
                    new FactionModel() { FactionId = 2, ServerName = "TestFaction2", MapColor = Colors.Red },
                    new FactionModel() { FactionId = 3, ServerName = "TestFaction3", MapColor = Colors.Green },
                    new FactionModel() { FactionId = 4, ServerName = "TestFaction4", MapColor = Colors.Yellow },
                    new FactionModel() { FactionId = 5, ServerName = "TestFaction5", MapColor = Colors.Brown }
                },
                Systems = new List<SystemModel>()
                {
                    new SystemModel() { SystemId = 1, ServerName = "TestSystem1", Cordinates = new System.Windows.Point(-60, -40),
                        ConnectedSystems = new List<int>() { 2, 5, 6 }, HomeworldFactionId = 1, OwnerFactionId = 1,
                        Ownership = new Dictionary<int, double>() { { 1, 100 } } },
                    new SystemModel() { SystemId = 2, ServerName = "TestSystem2", Cordinates = new System.Windows.Point(-40, -20),
                        ConnectedSystems = new List<int>() { 1, 4, 6, 7 }, HomeworldFactionId = 2, OwnerFactionId = 2,
                        Ownership = new Dictionary<int, double>() { { 2, 100 } } },
                    new SystemModel() { SystemId = 3, ServerName = "TestSystem3", Cordinates = new System.Windows.Point(-20, 0),
                        ConnectedSystems = new List<int>() { 5, 6, 7 }, HomeworldFactionId = 3, OwnerFactionId = 3,
                        Ownership = new Dictionary<int, double>() { { 3, 100 } } },
                    new SystemModel() { SystemId = 4, ServerName = "TestSystem4", Cordinates = new System.Windows.Point(0, 20),
                        ConnectedSystems = new List<int>() { 2, 5, 6, 7 }, HomeworldFactionId = 4, OwnerFactionId = 4,
                        Ownership = new Dictionary<int, double>() { { 4, 100 } } },
                    new SystemModel() { SystemId = 5, ServerName = "TestSystem5", Cordinates = new System.Windows.Point(20, 0),
                        ConnectedSystems = new List<int>() { 1, 3, 4 }, HomeworldFactionId = 5, OwnerFactionId = 5,
                        Ownership = new Dictionary<int, double>() { { 5, 100 } } },
                    new SystemModel() { SystemId = 6, ServerName = "TestSystem6", Cordinates = new System.Windows.Point(40, -20),
                        ConnectedSystems = new List<int>() { 1, 2, 3, 4 }, OwnerFactionId = 0,
                        Ownership = new Dictionary<int, double>() { } },
                    new SystemModel() { SystemId = 7, ServerName = "TestSystem7", Cordinates = new System.Windows.Point(60, -40),
                        ConnectedSystems = new List<int>() { 2, 3, 4 }, OwnerFactionId = 4,
                        Ownership = new Dictionary<int, double>() { { 1, 25 }, { 2, 25 }, { 3, 25 }, { 4, 25 } } }
                }
            };

            testData.Factions.ForEach(x => x.Name = ConvertToProperCaseFromCammel(x.ServerName));
            testData.Systems.ForEach(x => x.Name = ConvertToProperCaseFromCammel(x.ServerName));
            */

            // end test data


            return testData;
        }

        internal static string ConvertToProperCaseFromCammel(string textValue)
        {
            StringBuilder newText = new StringBuilder(textValue.Length * 2);
            newText.Append(textValue[0]);
            for (int i = 1; i < textValue.Length; i++)
            {
                if (char.IsUpper(textValue[i]) || char.IsNumber(textValue[i]) && textValue[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(textValue[i]);
            }

            return newText.ToString();
        }
    }
}
