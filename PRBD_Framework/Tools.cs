using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRBD_Framework
{
    public static class Tools
    {
        public static void RefreshFromModel<T>(this ObservableCollection<T> list, IEnumerable<T> model)
        {
            list.Clear();
            foreach (var o in model)
                list.Add(o);
        }
    }
}
