using RtoTools.Common.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.ViewModel
{
    public class DisplayValueOption<T>
    {
        public DisplayValueOption()
        {
            Display = Localization.SelectOption;
            Value = default(T);
        }
        
        public DisplayValueOption(T? value, string display)
        {
            Display = display;
            Value = value;
        }

        public string Display { get; private set; }

        public T? Value { get; private set; }
    }
}
