using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRBD_Framework {
    public interface IErrorManager {
        void AddError(string propertyName, string error);
        void SetError(string propertyName, string error);
        void SetErrors(string propertyName, IEnumerable errors);
        void ClearErrors(string propertyName);
        void ClearErrors();
        void RaiseErrors();
        void RaiseErrorsChanged(string propertyName);
        IEnumerable GetErrors(string propertyName);
        Dictionary<string, ICollection<string>> GetErrors();
        void SetErrors(Dictionary<string, ICollection<string>> errors);
        bool HasErrors { get; }
        bool Validate();
    }

    public class ErrorManager : INotifyDataErrorInfo, IErrorManager {
        protected readonly Dictionary<string, ICollection<string>> validationErrors = new Dictionary<string, ICollection<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public ErrorManager(EventHandler<DataErrorsChangedEventArgs> errorsChanged = null) {
            this.ErrorsChanged = errorsChanged;
        }

        public void AddError(string propertyName, string error) {
            if (!validationErrors.ContainsKey(propertyName)) {
                validationErrors[propertyName] = new List<string>();
            }

            validationErrors[propertyName].Add(error);
        }

        public void SetError(string propertyName, string error) {
            if (!validationErrors.ContainsKey(propertyName)) {
                validationErrors[propertyName] = new List<string>();
            }

            validationErrors[propertyName].Clear();
            validationErrors[propertyName].Add(error);
        }

        public void SetErrors(string propertyName, IEnumerable errors) {
            validationErrors[propertyName] = new List<string>();
            if (errors != null) {
                foreach (var s in errors) {
                    validationErrors[propertyName].Add(s.ToString());
                }
            }
        }

        public void ClearErrors(string propertyName) {
            validationErrors[propertyName].Clear();
        }

        public void ClearErrors() {
            foreach (var key in validationErrors.Keys) {
                validationErrors[key].Clear();
            }
        }

        public void RaiseErrors() {
            foreach (var key in validationErrors.Keys) {
                RaiseErrorsChanged(key);
            }
        }

        public void RaiseErrorsChanged(string propertyName) {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName) {
            if (string.IsNullOrEmpty(propertyName) || !validationErrors.ContainsKey(propertyName)) {
                return null;
            }

            return validationErrors[propertyName];
        }

        public void SetErrors(Dictionary<string, ICollection<string>> errors) {
            ClearErrors();
            foreach (var key in errors.Keys) {
                if (!validationErrors.ContainsKey(key)) {
                    validationErrors[key] = new List<string>();
                }

                foreach (var s in errors[key]) {
                    validationErrors[key].Add(s);
                }
            }
        }

        public bool HasErrors {
            get {
                foreach (var key in validationErrors.Keys) {
                    if (validationErrors[key].Count > 0) {
                        return true;
                    }
                }

                return false;
            }
        }

        public Dictionary<string, ICollection<string>> GetErrors() {
            return validationErrors;
        }

        public virtual bool Validate() { return true; }
    }
}
