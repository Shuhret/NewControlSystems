using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ControlSystemsLibrary.Classes
{
    class UserInterfacesClass: INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion

        private string fullUserInterfaceName;
        public string FullUserInterfaceName
        {
            get => fullUserInterfaceName;
            set
            {
                fullUserInterfaceName = value;
                OnPropertyChanged();
            }
        }


        private UserControl userInterfaceControl;
        public UserControl UserInterfaceControl
        {
            get => userInterfaceControl;
            set
            {
                userInterfaceControl = value;
                OnPropertyChanged();
            }
        }
    }
}
