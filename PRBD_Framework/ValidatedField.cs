using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRBD_Framework
{
    public class ValidatedField : StackPanel
    {
        public override void EndInit()
        {
            var err = new ErrorMessages { MyTarget = (FrameworkElement)Children[0] };
            Children.Add(err);
            base.EndInit();
        }
    }
}
