using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.Model.Processing.FileIO
{
    public class v1_root
    {
        public int version { get; set; }

        public List<v1_contract> contracts { get; set; }
    }
}
