using ControlSystemsLibrary.Controls;
using ControlSystemsLibrary.Controls.AdminTabItemContents;
using ControlSystemsLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlSystemsLibrary.Views
{
    
    public partial class Administrator : UserControl, INotifyPropertyChanged
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

        LockInterfaceDelegate LockInterface;
        public Administrator(string CurrentUserName, string CurrentConnectionName, string CurrentCryptConnectionString, LockInterfaceDelegate LockInterface)
        {
            this.CurrentUserName = CurrentUserName;
            this.CurrentConnectionName = CurrentConnectionName;
            this.CurrentCryptConnectionString = CurrentCryptConnectionString;
            this.LockInterface = LockInterface;

            InitializeComponent();
        }

        public ObservableCollection<AdminTabItem> tabItemsCollection = new ObservableCollection<AdminTabItem>();
        public ObservableCollection<AdminTabItem> TabItemsCollection
        {
            get => tabItemsCollection;
            set
            {
                tabItemsCollection = value;
                OnPropertyChanged();
            }
        }

        private void ClickTopLockButton(object sender, RoutedEventArgs e)
        {
            LockInterface();
        }


        private string currentUserName = "";
        public string CurrentUserName
        {
            get => currentUserName;
            set
            {
                currentUserName = value;
                OnPropertyChanged();
            }
        }


        private string currentConnectionName = "";
        public string CurrentConnectionName
        {
            get => currentConnectionName;
            set
            {
                currentConnectionName = value;
                OnPropertyChanged();
            }
        }


        private string currentCryptConnectionString = "";
        public string CurrentCryptConnectionString
        {
            get => currentCryptConnectionString;
            set
            {
                if (Equals(currentCryptConnectionString, value)) return;
                currentCryptConnectionString = value;
                OnPropertyChanged();
            }
        }


        private double adminTabControlActualWidth = 0;
        public double AdminTabControlActualWidth
        {
            get => adminTabControlActualWidth;
            set
            {
                adminTabControlActualWidth = value;
                AlignTabWidth();
                OnPropertyChanged();
            }
        }


        private Thickness tabControlBorderThickness = new Thickness(0);
        public Thickness TabControlBorderThickness
        {
            get => tabControlBorderThickness;
            set
            {
                tabControlBorderThickness = value;
                OnPropertyChanged();
            }
        }

        private void ATI_Closed(object sender, EventArgs e)
        {
            AdminTabItem ATI = sender as AdminTabItem;
            TabItemsCollection.Remove(ATI);
            AlignTabWidth();
        }

        // Метод: Выровняет ширину вкладок
        void AlignTabWidth()
        {
            try
            {
                if (TabItemsCollection.Count == 0)
                {
                    TabControlBorderThickness = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    TabControlBorderThickness = new Thickness(0, 1, 0, 0);
                }


                double ШиринаВкладки = 0;


                if (TabItemsCollection.Count == 1)
                {
                    ШиринаВкладки = AdminTabControlActualWidth;
                }
                else
                {
                    ШиринаВкладки = AdminTabControlActualWidth / TabItemsCollection.Count;
                }
                ШиринаВкладки = Math.Round(ШиринаВкладки);

                int count = 0;

                foreach (AdminTabItem ATI in TabItemsCollection)
                {
                    if (count != TabItemsCollection.Count - 1)
                    {
                        ATI.Width = ШиринаВкладки;
                        ATI.BorderThickness = new Thickness(0, 0, 1, 0);
                    }
                    else
                    {
                        ATI.Width = (AdminTabControlActualWidth - (ШиринаВкладки * count));
                        ATI.BorderThickness = new Thickness(0, 0, 0, 0);
                    }
                    count++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Вы открыли дохуя окон!");
            }
        }

        private void AdminTabControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdminTabControlActualWidth = (sender as TabControl).ActualWidth;
        }

        private void AdminTabControlLoaded(object sender, RoutedEventArgs e)
        {
            AdminTabControlActualWidth = (sender as TabControl).ActualWidth;
            
        }


        private void MenuButton_Click(object sender, EventArgs e)
        {
            AddTabItem((sender as CheckBoxButton).Name);
        }




        void AddTabItem(string ButtonName)
        {
            if (CheckTabItems(GetAdminTabItemName(ButtonName)) == true)
            {
                TabItemsCollection.Add(GetTabItem(ButtonName));
                AdminTabControl.SelectedIndex = AdminTabControl.Items.Count-1;
                AlignTabWidth();
            }
            else
            {
                foreach (AdminTabItem ATI in AdminTabControl.Items)
                {
                    if (GetAdminTabItemName(ButtonName) == ATI.Name)
                        AdminTabControl.SelectedItem = ATI;
                }
            }
        }

        AdminTabItem GetTabItem(string ButtonName)
        {
            AdminTabItem adminTabItem = new AdminTabItem();
            adminTabItem.Name = GetAdminTabItemName(ButtonName);
            adminTabItem.HeaderText = GetAdminTabItemHeaderText(ButtonName);
            adminTabItem.Content = GetTabItemContent(ButtonName);
            adminTabItem.Icon = GetAdminTabItemIcon(ButtonName);
            adminTabItem.IconSize = 17;

            adminTabItem.Foreground = GetColor.Get("Dark-003");
            adminTabItem.Closed += ATI_Closed;
            return adminTabItem;
        }


        string GetAdminTabItemName(string ButtonName)
        {
            switch(ButtonName)
            {
                case "MenuButtonAllDocuments": return "MenuButtonAllDocuments";

                case "MenuButtonZayavkapostavshiku": return "MenuButtonZayavkapostavshiku";
                case "MenuButtonPostuplenie": return "MenuButtonPostuplenie";
                case "MenuButtonVozvratpostavshiku": return "MenuButtonVozvratpostavshiku";
                case "MenuButtonOtchetPostavshiku": return "MenuButtonOtchetPostavshiku";

                case "MenuButtonNomenclatura": return "Номенклатура";

                default: return "";

            }
        }

        string GetAdminTabItemHeaderText(string ButtonName)
        {
            switch (ButtonName)
            {
                case "MenuButtonAllDocuments": return "Журнал документов";

                case "MenuButtonZayavkapostavshiku": return "Заявка поставщику";
                case "MenuButtonPostuplenie": return "Поступление";
                case "MenuButtonVozvratpostavshiku": return "Возврат поставщику";
                case "MenuButtonOtchetPostavshiku": return "Отчет поставщику";

                case "MenuButtonNomenclatura": return "Номенклатура";
                default: return "";

            }
        }

        object GetAdminTabItemIcon(string ButtonName)
        {
            switch (ButtonName)
            {
                case "MenuButtonAllDocuments": return GetIcons.GetIcon("DocumentsJournal");

                case "MenuButtonZayavkapostavshiku": return GetIcons.GetIcon("Заявка поставщику"); 
                case "MenuButtonPostuplenie": return GetIcons.GetIcon("Поступление");
                case "MenuButtonVozvratpostavshiku": return GetIcons.GetIcon("Возврат поставщику");
                case "MenuButtonOtchetPostavshiku": return GetIcons.GetIcon("Отчет поставщику");

                case "MenuButtonNomenclatura": return GetIcons.GetIcon("Номенклатура");
                default: return "";

            }
        }

        UserControl GetTabItemContent(string ButtonName)
        {
            switch (ButtonName)
            {
                case "MenuButtonAllDocuments": return null;

                case "MenuButtonZayavkapostavshiku": return null;
                case "MenuButtonPostuplenie": return null;
                case "MenuButtonVozvratpostavshiku": return null;
                case "MenuButtonOtchetPostavshiku": return null;

                case "MenuButtonNomenclatura": return new NomenclatureATI();
                default: return null;

            }
        }

        bool CheckTabItems(string AdminTabItemName)
        {
            foreach(AdminTabItem ATI in TabItemsCollection)
            {
                if (AdminTabItemName == ATI.Name)
                    return false;
            }
            return true;
        }





















        #region Открывание меню ************************************************************************************

        int LastSelectMenuPage = 1;
        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MenuGrid.Width == 66)
            {
                MenuTC.SelectedIndex = LastSelectMenuPage;
                Storyboard sb = this.FindResource("OpenMenu") as Storyboard;
                sb.Begin(); // Запустить анимацию


            }
        }

        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            if (MenuGrid.Width == 310 && MenuStateCheckBox.IsChecked == false)
            {
                Storyboard sb = this.FindResource("CloseMenu") as Storyboard;
                sb.Begin(); // Запустить анимацию
            }
        }

        private void MenuMouseMove(object sender, MouseEventArgs e)
        {
            Menu_MouseEnter(sender, e);
        }

        private void Menu_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MenuGrid.Width == 66 && e.WidthChanged)
            {
                LastSelectMenuPage = MenuTC.SelectedIndex;
                MenuTC.SelectedItem = null;
            }
        }

        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Menu_MouseLeave(sender, e);
        }

        private void MenuCheckBoxCheck(object sender, RoutedEventArgs e)
        {
            //Storyboard sb = this.FindResource("CheckLine") as Storyboard;
            //sb.Begin(); // Запустить анимацию
        }



        #endregion



    }
}
