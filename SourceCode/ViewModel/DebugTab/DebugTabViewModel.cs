using RtoTools.Common;
using RtoTools.Common.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.ViewModel.DebugTab
{
    public class DebugTabViewModel : ViewModelBase
    {
        public DebugTabViewModel()
        {
            AppController.Instance.DebugEventsChanged += AppControllerDebugEventsChanged;
        }

        private void AppControllerDebugEventsChanged()
        {
            RaisePropertyChanged("ErrorCount");
            SetDebugData();
        }

        public string TextSaveLogs { get { return Localization.SaveLogs; } }

        public bool ShowOnlyErrors 
        {
            get { return GetValue<bool>(); } 
            set 
            {
                SetValue(value);
                SetDebugData();
            } 
        }

        public string ErrorCount { get { return AppController.Instance.DebugEvents.Count().ToString(); } }

        public string DebugData { get { return GetValue<string>(); } set { SetValue(value); } }

        public override void Execute(object? parameter)
        {
            switch (parameter)
            {
                case "SaveLogs":
                    AppController.Instance.SaveDebugLogs();
                    break;
            }
        }

        private void SetDebugData()
        {
            var eventsToShow = AppController.Instance.DebugEvents.OrderByDescending(x => x.EventDateTime).Where(x => x.Exception != null || !ShowOnlyErrors).Take(10);
            var eventCount = AppController.Instance.DebugEvents.Count();

            var result = String.Join(Environment.NewLine, eventsToShow);

            if (eventsToShow.Count() != eventCount)
            {
                result += $"{Environment.NewLine}{eventCount - eventsToShow.Count()} other events not shown.  Use the 'Save Logs' button to get a full log report";
            }

            DebugData = result;
        }
    }
}
