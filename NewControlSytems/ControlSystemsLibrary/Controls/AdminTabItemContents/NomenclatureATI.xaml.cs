using ControlSystemsLibrary.Classes;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlSystemsLibrary.Controls.AdminTabItemContents
{

    public partial class NomenclatureATI : UserControl, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged =======================================================================================================

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        // Конструктор
        public NomenclatureATI()
        {
            InitializeComponent();
        }


        #region Заполнение таблицы и навигация ===============================================================================================

        // Поля и свойства -------------------------------------------------------------------------------------------------------------------

        ObservableCollection<NomenclatureClass> AllNomenclaturesCollection = new ObservableCollection<NomenclatureClass>();

        ObservableCollection<NomenclatureClass> ShowNomenclaturesCollection = new ObservableCollection<NomenclatureClass>();

        private NomenclatureClass selectedItem;
        public NomenclatureClass SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        private Guid currentGroupID = new Guid("00000000-0000-0000-0000-000000000000");
        public Guid CurrentGroupID
        {
            get => currentGroupID;
            set
            {
                if (currentGroupID != value)
                {
                    currentGroupID = value;
                    LoadShowNomenclatures();
                    SelectedItem = null;
                    AddLinqButton();
                    OnPropertyChanged();
                }
            }
        }


        // События----------------------------------------------------------------------------------------------------------------------------
        bool FirstBoot = true;
         
        private void DataGridNomenclatures_Loaded(object sender, RoutedEventArgs e)
        {
            if (FirstBoot == true)
            {
                LoadAllNomenclatures();
                FirstBoot = false;
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            NomenclatureClass NC = row.Item as NomenclatureClass;
            if (NC.GroupNomen == false)
            {
                CurrentGroupID = NC.ID;
            }
        }

        private void DG_Header_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach(NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if(NC.ID == CurrentGroupID)
                {
                    SelectedItem = NC;
                    CurrentGroupID = NC.GroupID;
                    break;
                }
            }
        }

        private void DataGridNomenclatures_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Enter
            if (e.Key == Key.Enter)
            {
                if ((sender as DataGrid).SelectedItem != null)
                {
                    NomenclatureClass NC = (sender as DataGrid).SelectedItem as NomenclatureClass;
                    if (NC.GroupNomen == false)
                    {
                        CurrentGroupID = NC.ID;
                    }
                }
                e.Handled = true;
            }
        }

        private void Nom_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + Down ↓
            if (e.Key == Key.Down && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (DataGridNomenclatures.SelectedItem != null)
                {
                    NomenclatureClass NC = DataGridNomenclatures.SelectedItem as NomenclatureClass;
                    if (NC.GroupNomen == false)
                    {
                        CurrentGroupID = NC.ID;
                    }
                }
                e.Handled = true;
            }

            // Ctrl + Up ↑
            if (e.Key == Key.Up && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                foreach (NomenclatureClass NC in AllNomenclaturesCollection)
                {
                    if (NC.ID == CurrentGroupID)
                    {
                        SelectedItem = NC;
                        CurrentGroupID = NC.GroupID;
                        break;
                    }
                }
                e.Handled = true;
            }

            // Backspace ←
            if (e.Key == Key.Back)
            {
                foreach (NomenclatureClass NC in AllNomenclaturesCollection)
                {
                    if (NC.ID == CurrentGroupID)
                    {
                        SelectedItem = NC;
                        CurrentGroupID = NC.GroupID;
                        break;
                    }
                }
                e.Handled = true;
            }

            // Home 
            if (e.Key == Key.Home)
            {
                NomenclatureClass NC = DataGridNomenclatures.Items[0] as NomenclatureClass;
                SelectedItem = NC;
                SelectDataGridRow();

                e.Handled = true;
            }
            
            // End 
            if (e.Key == Key.End)
            {
                NomenclatureClass NC = DataGridNomenclatures.Items[DataGridNomenclatures.Items.Count-1] as NomenclatureClass;
                SelectedItem = NC;
                SelectDataGridRow();

                e.Handled = true;
            }
        }


        // Методы ----------------------------------------------------------------------------------------------------------------------------
        
        async void LoadAllNomenclatures()
        {
            await Task.Run(() => AllNomenclaturesCollection = DataBaseRequest.GetAllNomenclatures());
            LoadShowNomenclatures();
            DataGridNomenclatures.ItemsSource = ShowNomenclaturesCollection;
            AddLinqButton();
        }

        void LoadShowNomenclatures()
        {
            ShowNomenclaturesCollection.Clear();
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.GroupID == CurrentGroupID)
                {
                    ShowNomenclaturesCollection.Add(NC);
                }
            }
            
            SelectDataGridRow();
        }

        void SelectDataGridRow()
        {
            if (DataGridNomenclatures.Items.Count > 0 && SelectedItem != null)
            {
                DataGridNomenclatures.SelectedItem = SelectedItem;
                DataGridNomenclatures.CurrentCell = new DataGridCellInfo(DataGridNomenclatures.Items[DataGridNomenclatures.SelectedIndex], DataGridNomenclatures.Columns[3]);
                DataGridNomenclatures.ScrollIntoView(DataGridNomenclatures.SelectedItem);
                DataGridNomenclatures.Columns[3].IsReadOnly = false;
                DataGridNomenclatures.BeginEdit();
                DataGridNomenclatures.Columns[3].IsReadOnly = true;
            }
            if (DataGridNomenclatures.Items.Count > 0 && SelectedItem == null)
            {
                DataGridNomenclatures.SelectedIndex = 0;
                DataGridNomenclatures.CurrentCell = new DataGridCellInfo(DataGridNomenclatures.Items[0], DataGridNomenclatures.Columns[3]);
                DataGridNomenclatures.ScrollIntoView(DataGridNomenclatures.SelectedItem);
                DataGridNomenclatures.Columns[3].IsReadOnly = false;
                DataGridNomenclatures.BeginEdit();
                DataGridNomenclatures.Columns[3].IsReadOnly = true;
            }
            if (DataGridNomenclatures.Items.Count <= 0)
            {
                DataGridNomenclatures.Focus();
            }
        }


        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



        #region LinqButtons ==================================================================================================================

        List<LinqButton> LinqButtonsList = new List<LinqButton>();

        private void LinqButton_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedItem((sender as LinqButton).ID);
            CurrentGroupID = (sender as LinqButton).ID;
        }

        void SetSelectedItem(Guid ID)
        {
            if (LinqButtonsList.Count > 1)
            {
                for (int i = LinqButtonsList.Count - 1; i > 0; i--)
                {
                    if (LinqButtonsList[i].ID == ID)
                    {
                        foreach(NomenclatureClass NC in  AllNomenclaturesCollection)
                        {
                            if(NC.ID == LinqButtonsList[i - 1].ID)
                            {
                                SelectedItem = NC;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                SelectedItem = null;
            }
        }

        void AddLinqButton()
        {
            LinqButtonsList.Clear();
            LinqButtonsStackPanel.Children.Clear();

            AddCurrentGroupLinqButtonToList();

            for (int i = LinqButtonsList.Count - 1; i >= 0; i--)
            {
                LinqButtonsStackPanel.Children.Add(LinqButtonsList[i]);
            }
        }

        void AddCurrentGroupLinqButtonToList()
        {
            if (CurrentGroupID == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                LinqButtonsList.Add(GetLinqButton("Номенклатура", new Guid("00000000-0000-0000-0000-000000000000")));
            }
            else
            {
                foreach (NomenclatureClass NC in AllNomenclaturesCollection)
                {
                    if (NC.ID == CurrentGroupID)
                    {
                        LinqButtonsList.Add(GetLinqButton(NC.Name, NC.ID));
                        AddParrentGroupLinqButtonToList(NC.GroupID);
                    }
                }
                LinqButtonsList.Add(GetLinqButton("Номенклатура", new Guid("00000000-0000-0000-0000-000000000000")));
            }
        }

        void AddParrentGroupLinqButtonToList(Guid GroupID)
        {
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.ID == GroupID)
                {
                    LinqButtonsList.Add(GetLinqButton(NC.Name, NC.ID));
                    AddParrentGroupLinqButtonToList(NC.GroupID);
                }
            }
        }

        LinqButton GetLinqButton(string ContentText, Guid ID)
        {
            LinqButton LB = new LinqButton();
            if (ID == new Guid("00000000-0000-0000-0000-000000000000"))
                LB.Foreground = GetColor.Get("Blue-003");
            else
                LB.Foreground = GetColor.Get("Dark-002");
            LB.Background = GetColor.Get("Light-002");
            LB.Content = ContentText;
            LB.ID = ID;
            LB.Click += LinqButton_Click;
            return LB;
        }


        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    }
}
