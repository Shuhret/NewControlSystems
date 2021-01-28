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

namespace ControlSystemsLibrary.Controls
{
    public partial class LinqButton : Button
    {
        public LinqButton()
        {
            InitializeComponent();
        }
        public Guid ID
        {
            get { return (Guid)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(Guid), typeof(LinqButton), new PropertyMetadata(new Guid("00000000-0000-0000-0000-000000000000")));
    }
}
