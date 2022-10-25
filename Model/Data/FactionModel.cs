using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RtoTools.Model.Data
{
    public class FactionModel
    {
        /// <summary>
        /// A unique identifier used for processing in this application
        /// </summary>
        public int FactionId { get; set; }

        /// <summary>
        /// The name of the faction that is used for display in this application
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The name that is used to communicate to the RTO server
        /// </summary>
        public string ServerName { get; set; } = string.Empty;

        /// <summary>
        /// The system color that is used on the map for thier control
        /// </summary>
        public Color MapColor { get; set; } = Colors.White;

        /// <summary>
        /// All the contracts that are active for this faction
        /// </summary>
        public List<ContractModel> Contracts { get; set; } = new List<ContractModel>();
    }
}
