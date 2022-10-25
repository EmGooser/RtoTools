using RtoTools.ViewModel.ContractsTab;
using RtoTools.ViewModel.DebugTab;
using RtoTools.ViewModel.MapTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public RibbonViewModel Ribbon { get; } = new RibbonViewModel();

        public ContractsTabViewModel ContractsTab { get; } = new ContractsTabViewModel();

        public MapTabViewModel MapTab { get; } = new MapTabViewModel();

        public DebugTabViewModel DebugTab { get; } = new DebugTabViewModel();

        internal void RefreshUi()
        {
            RaisePropertyChanged("Ribbon");
            RaisePropertyChanged("ContractsTab");
        }
    }
}
