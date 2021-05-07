using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace PRBD_Framework {
    public class EntityBase<T> : ErrorManager, INotifyPropertyChanged where T : DbContext {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static DbContext GetDbContextFromEntity(object entity) {
            var object_context = GetObjectContextFromEntity(entity);

            if (object_context == null || object_context.TransactionHandler == null) {
                return null;
            }

            return object_context.TransactionHandler.DbContext;
        }

        private static ObjectContext GetObjectContextFromEntity(object entity) {
            var field = entity.GetType().GetField("_entityWrapper");

            if (field == null) {
                return null;
            }

            var wrapper = field.GetValue(entity);
            if (wrapper == null) {
                return null;
            }

            var property = wrapper.GetType().GetProperty("Context");
            if (property == null) {
                return null;
            }

            var context = (ObjectContext)property.GetValue(wrapper, null);

            return context;
        }

        private T _model = null;

        public bool Attached { get { return Model != null; } }
        public bool Detached { get { return Model == null; } }

        public T Model {
            get {
                if (_model == null) {
                    _model = (T)GetDbContextFromEntity(this);
                }
                return _model;
            }
        }

        public EntityState State {
            get {
                return Model.Entry(this).State;
            }
        }

        public bool IsAdded { get => State == EntityState.Added; }
        public bool IsModified { get => State == EntityState.Modified; }
        public bool IsDeleted { get => State == EntityState.Deleted; }
        public bool IsUnchanged { get => State == EntityState.Unchanged; }

        public void Reload() {
            if (Attached) {
                Model.Entry(this).Reload();
            }
        }
    }
}