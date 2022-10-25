using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.Model.Processing.FileIO
{
    public class v1_contract
    {
        public string asFaction { get; set; }

        public string contractId { get; set; }

        public string contractType { get; set; }

        public string starSystem { get; set; }

        public int targetControl { get; set; }

        public string targetFaction { get; set; }
    }
}
