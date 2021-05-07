using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRBD_Framework
{
    public class WindowBase : Window, INotifyPropertyChanged, INotifyDataErrorInfo, IErrorManager
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected ErrorManager errors;

        public WindowBase()
        {
            errors = new ErrorManager(ErrorsChanged);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary. (origin: Prism)
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary. (origin: Prism)
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <param name="onChanged">Action that is called after the property value has been changed.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }

        public void AddError(string propertyName, string error)
        {
            errors.AddError(propertyName, error);
        }

        public void SetError(string propertyName, string error)
        {
            errors.SetError(propertyName, error);
        }

        public void SetErrors(string propertyName, IEnumerable errors) {
            this.errors.SetErrors(propertyName, errors);
        }

        public void ClearErrors(string propertyName)
        {
            errors.ClearErrors(propertyName);
        }

        public void ClearErrors()
        {
            errors.ClearErrors();
        }

        public void RaiseErrors()
        {
            errors.RaiseErrors();
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            errors.RaiseErrorsChanged(propertyName);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return errors.GetErrors(propertyName);
        }

        public Dictionary<string, ICollection<string>> GetErrors() {
            return errors.GetErrors();
        }

        public void SetErrors(Dictionary<string, ICollection<string>> errors)
        {
            this.errors.SetErrors(errors);
        }

        public virtual bool Validate()
        {
            return errors.Validate();
        }

        public bool HasErrors
        {
            get
            {
                return errors.HasErrors;
            }
        }
    }
}
