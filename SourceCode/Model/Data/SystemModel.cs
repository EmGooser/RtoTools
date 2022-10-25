using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RtoTools.Model.Data
{
    public class SystemModel
    {
        /// <summary>
        /// A unique identifier used for processing in this application
        /// </summary>
        public int SystemId { get; set; }

        /// <summary>
        /// The name of the system that is used for display in game and in this application
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The name that is used to communicate to the RTO server
        /// </summary>
        public string ServerName { get; set; } = string.Empty;

        /// <summary>
        /// The X Y position of the system on the map
        /// </summary>
        public Point Cordinates { get; set; }

        /// <summary>
        /// Values are the SystemIds that are connected to this system
        /// </summary>
        public List<int> ConnectedSystems { get; set; } = new List<int>();

        /// <summary>
        /// Key is the Faction Id from the list of factions, Value is the amount of control or fortification the system has as a double
        /// </summary>
        public Dictionary<int, double> Ownership = new Dictionary<int, double>();

        /// <summary>
        /// Faction that owns the system
        /// </summary>
        public int OwnerFactionId { get; set; }

        /// <summary>
        /// If it is a homeworld for a faction
        /// </summary>
        public int? HomeworldFactionId { get; set; }
    }
}
