using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.Model.Data
{
    public class DebugEventModel
    {
        public DateTimeOffset EventDateTime { get; set; }

        public string EventText { get; set; } = string.Empty;

        public Exception? Exception { get; set; }

        public override string ToString()
        {
            if (Exception == null)
            {
                return $"{EventDateTime:g}: {EventText}";
            }
            else
            {
                return $"{EventDateTime:g}: {EventText}{Environment.NewLine}{Exception}{Environment.NewLine}";
            }
        }
    }
}
