using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRBD_Framework {
    public partial class ApplicationBase : Application {
        protected static Messenger messenger = new Messenger();
        protected static Dictionary<object, List<Tuple<Enum, Guid>>> ids = new Dictionary<object, List<Tuple<Enum, Guid>>>();

        public static void Register(object owner, Enum message, Action callback) {
            var id = messenger.Register(message, callback);
            if (!ids.ContainsKey(owner))
                ids[owner] = new List<Tuple<Enum, Guid>>();
            ids[owner].Add(new Tuple<Enum, Guid>(message, id));
        }

        public static void Register<T>(object owner, Enum message, Action<T> callback) {
            var id = messenger.Register<T>(message, callback);
            if (!ids.ContainsKey(owner))
                ids[owner] = new List<Tuple<Enum, Guid>>();
            ids[owner].Add(new Tuple<Enum, Guid>(message, id));
        }

        public static void NotifyColleagues(Enum message, object parameter) {
            messenger.NotifyColleagues(message, parameter);
        }

        public static void NotifyColleagues(Enum message) {
            messenger.NotifyColleagues(message);
        }

        public static void UnRegister(object owner) {
            if (ids.ContainsKey(owner)) {
                foreach(var tuple in ids[owner]) {
                    messenger.UnRegister(tuple.Item1, tuple.Item2);
                }
            }
        }
    }
}
