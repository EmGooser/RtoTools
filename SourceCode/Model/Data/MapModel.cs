using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.Model.Data
{
    public class MapModel
    {
        /// <summary>
        /// When this data was gathered from the server
        /// </summary>
        public DateTimeOffset? AsOf { get; set; }

        /// <summary>
        /// All the systems that are on the map
        /// </summary>
        public List<SystemModel> Systems { get; set; } = new List<SystemModel>();

        /// <summary>
        /// All the factions that are on the map
        /// </summary>
        public List<FactionModel> Factions { get; set; } = new List<FactionModel>();
    }
}
