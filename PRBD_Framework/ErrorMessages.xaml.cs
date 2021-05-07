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
    public partial class ErrorMessages : UserControl
    {
        public ErrorMessages()
        {
            InitializeComponent();
        }

        public FrameworkElement MyTarget
        {
            get { return (FrameworkElement)GetValue(MyTargetProperty); }
            set { SetValue(MyTargetProperty, value); }
        }

        public static readonly DependencyProperty MyTargetProperty =
            DependencyProperty.Register("MyTarget", typeof(FrameworkElement),
              typeof(ErrorMessages), new PropertyMetadata(default(FrameworkElement)));
    }
}
