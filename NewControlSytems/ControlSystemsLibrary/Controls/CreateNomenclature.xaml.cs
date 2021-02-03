using ControlSystemsLibrary.Controls.AdminTabItemContents;
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
    public partial class CreateNomenclature : UserControl
    {
        bool CreateMode;
        CloseCreateNomenclatureDelegate CCND;
        public CreateNomenclature(bool CreateMode, CloseCreateNomenclatureDelegate CCND)
        {
            this.CreateMode = CreateMode;
            this.CCND = CCND;
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            CCND();
        }

        private void CN_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
