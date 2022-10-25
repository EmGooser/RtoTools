using RtoTools.Model.Data;
using RtoTools.Model.Processing;
using RtoTools.ViewModel;
using RtoTools.Common.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using RtoTools.ViewModel.ContractsTab;
using Microsoft.Win32;
using System.IO;
using RtoTools.ViewModel.MapTab;
using System.Windows.Media;
using RtoTools.View;

namespace RtoTools.Common
{
    internal class AppController
    {
        internal static string SelectOptionText { get { return Localization.Localization.SelectOption; } }

        internal static AppController Instance { get; } = new AppController();
        
        internal static void Startup(Dispatcher dispatcher, MainWindowViewModel mainVm)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            if (mainVm == null)
                throw new ArgumentNullException("mainVm");

            if (Instance.ServerProcessing == null)
            {
                Instance.Dispatcher = dispatcher;
                Instance.MainViewModel = mainVm;
                Instance.ServerProcessing = new ServerProcessing();
                Instance.FileIOProcessing = new FileIOProcessing();
            }
            else
            {
                throw new InvalidOperationException("Attempted to call setup twice");
            }
        }

        internal ImageSource BuildSystemsBackgroundImage()
        {
            return SystemsBackgroundImageBuilder.BuildImage(MapData?.Systems, MapData?.Factions);
        }

        private AppController() { }

        internal Dispatcher Dispatcher { get; private set; }

        internal MainWindowViewModel MainViewModel { get; private set; }

        private ServerProcessing ServerProcessing { get; set; }

        private FileIOProcessing FileIOProcessing { get; set; }

        internal MapModel? MapData { get; private set; }

        internal DateTimeOffset? RtoServerAsDate { get; private set; } = null;

        internal FactionModel? SelectedFaction { get; private set; } = null;

        internal SystemModel? SelectedSystem { get; private set; } = null;

        internal SystemLabelType? SelectedSystemLabelType { get; private set; } = null;

        internal List<DebugEventModel> DebugEvents { get; } = new List<DebugEventModel>();

        internal event Action RtoServerDataChanged;

        internal event Action SelectedFactionChanged;

        internal event Action SelectedSystemChanged;

        internal event Action SelectedSystemLabelTypeChanged;

        internal event Action DebugEventsChanged;

        internal void SetSystemLabel(SystemLabelType? value)
        {
            SelectedSystemLabelType = value;
            SelectedSystemLabelTypeChanged?.Invoke();
        }

        internal TimeSpan RtoServerRefreshCooldown
        {
            get
            {
                if (RtoServerAsDate == null)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    // 1 min cooldown
                    var result = RtoServerAsDate.Value.AddMinutes(1) - DateTimeOffset.Now;

                    if (result < TimeSpan.Zero)
                    {
                        return TimeSpan.Zero;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
        }

        internal void RefreshRtoServerData()
        {
            try
            {
                RtoServerAsDate = DateTimeOffset.Now;
                MapData = ServerProcessing.FetchMapData();
                RtoServerDataChanged?.Invoke();
                AddEventLog("RTO Server Data Refreshed");
            }
            catch (Exception ex)
            {
                AddEventLog("Error fatching RTO Server Data", ex);
            }
        }

        internal void SetLanguage(string selectedLanguage)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);
            MainViewModel.RefreshUi();
        }

        internal void SetSelectedFaction(FactionModel faction)
        {
            SelectedFaction = faction;
            SelectedFactionChanged?.Invoke();
        }

        internal void SetSelectedSystem(SystemModel system)
        {
            SelectedSystem = system;
            SelectedSystemChanged?.Invoke();
        }

        internal void SaveContracts(List<ContractBuilderViewModel> contractBuilderList)
        {
            try
            {
                var ofd = new SaveFileDialog();
                ofd.Filter = "Contract JSON|*.json";
                ofd.DefaultExt = "json";
                ofd.Title = "Save Contract File";

                if (ofd.ShowDialog() == true)
                {
                    Instance.FileIOProcessing.SaveContractFile(new FileInfo(ofd.FileName), contractBuilderList);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler("Error saving file", ex);
            }
        }

        internal void SaveDebugLogs()
        {
            try
            {
                var ofd = new SaveFileDialog();
                ofd.Filter = "Debug Log|*.txt";
                ofd.DefaultExt = "txt";
                ofd.Title = "Save Debug Logs";

                if (ofd.ShowDialog() == true)
                {
                    Instance.FileIOProcessing.SaveDebugLogs(new FileInfo(ofd.FileName), DebugEvents);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler("Error saving file", ex);
            }
        }

        internal List<ContractBuilderViewModel> LoadContracts()
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Contract JSON|*.json";
                ofd.DefaultExt = "json";
                ofd.Title = "Load Contract File";

                if (ofd.ShowDialog() == true)
                {
                    return Instance.FileIOProcessing.LoadContractFile(new FileInfo(ofd.FileName));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler("Error loading file", ex);
                return null;
            }
        }

        internal void RaiseAlertDialog(string message)
        {


        }

        internal void ErrorHandler(string message, Exception ex)
        {
            RaiseAlertDialog($"{message}{Environment.NewLine}Check the debug tab for details.");
            AddEventLog($"General Error: {message}", ex);
        }

        internal void AddEventLog(string eventText, Exception? ex = null)
        {
            var debugEvent = new DebugEventModel()
            {
                EventDateTime = DateTimeOffset.UtcNow,
                EventText = eventText,
                Exception = ex
            };

            DebugEvents.Add(debugEvent);
            DebugEventsChanged?.Invoke();
        }
    }
}
