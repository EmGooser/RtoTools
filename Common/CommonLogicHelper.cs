using RtoTools.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.Common
{
    internal static class CommonLogicHelper
    {
        public static int GetSystemSupport(SystemModel? systemModel)
        {
            var ownerships = systemModel?.Ownership.Where(o => o.Key == systemModel.OwnerFactionId);

            if (ownerships?.Any() == true)
            {
                var ownership = ownerships.First().Value;

                if (ownership == 300)
                {
                    return 5;
                }
                else if (ownership > 260)
                {
                    return 4;
                }
                else if (ownership > 190)
                {
                    return 3;
                }
                else if (ownership > 120)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 0;
            }
        }

        internal static double GetSystemControl(SystemModel systemModel, int? factionId = null)
        {
            var ownerships = systemModel.Ownership.Where(o => o.Key == (factionId ?? systemModel.OwnerFactionId));

            if (ownerships.Any())
            {
                return ownerships.First().Value;
            }
            else
            {
                return 0;
            }
        }

        internal static int? GetSystemMissionSupport(SystemModel systemModel, int? factionId)
        {
            if (AppController.Instance.MapData == null)
                return 0;

            var supportSystems = AppController.Instance.MapData.Systems
                .Where(x => systemModel.SystemId == x.SystemId || systemModel.ConnectedSystems.Contains(x.SystemId));

            if (factionId.HasValue)
            {
                if (supportSystems.Any(x => x.OwnerFactionId == factionId))
                {
                    var myFaction = supportSystems.Where(x => x.OwnerFactionId == factionId).Select(x => GetSystemSupport(x)).Sum();
                    var ownerFaction = supportSystems.Where(x => x.OwnerFactionId == systemModel.OwnerFactionId).Select(x => GetSystemSupport(x)).Sum();

                    if (systemModel.OwnerFactionId == factionId)
                    {
                        return myFaction;
                    }
                    else
                    {
                        return myFaction - ownerFaction;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return supportSystems.Where(x => x.OwnerFactionId == systemModel.OwnerFactionId)
                    .Select(x => GetSystemSupport(x))
                    .Sum();
            }
        }
    }
}
