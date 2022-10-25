using RtoTools.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RtoTools.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, ICommand
    {
        public event EventHandler CanExecuteChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public static event Action<Exception> UnhandledException;

        private Dictionary<string, object> Values { get; } = new Dictionary<string, object>();

        protected T GetValue<T>([CallerMemberName] string propertyName = "")
        {
            if (Values.ContainsKey(propertyName))
            {
                return (T)Values[propertyName];
            }
            else
            {
                return default(T);
            }
        }

        protected void SetValue(object value, [CallerMemberName] string propertyName = "")
        {
            if (Values.ContainsKey(propertyName))
            {
                Values[propertyName] = value;
            }
            else
            {
                Values.Add(propertyName, value);
            }

            RaisePropertyChanged(propertyName);
        }

        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public virtual void Execute(object? parameter)
        {
            throw new NotImplementedException();
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (AppController.Instance?.Dispatcher?.CheckAccess() != false)
            {
                try
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                catch (Exception ex)
                {
                    UnhandledException?.Invoke(ex);
                }
            }
            else
            {
                AppController.Instance?.Dispatcher?.Invoke(() => RaisePropertyChanged(propertyName));
            }
        }

        protected virtual void RaiseCanExecuteChanged()
        {
            if (AppController.Instance?.Dispatcher?.CheckAccess() != false)
            {
                try
                {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    UnhandledException?.Invoke(ex);
                }
            }
            else
            {
                AppController.Instance?.Dispatcher?.Invoke(() => RaiseCanExecuteChanged());
            }
        }
    }
}
