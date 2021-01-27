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
            Loaded += NomenclatureATI_Loaded;
            CurrentGroupID = new Guid("00000000-0000-0000-0000-000000000000");
        }



        #region Заполнение таблицы/Навигация =================================================================================================


        #region Поля и свойства 

        ObservableCollection<NomenclatureClass> AllNomenclaturesCollection = new ObservableCollection<NomenclatureClass>();
        ObservableCollection<NomenclatureClass> ShowNomenclaturesCollection = new ObservableCollection<NomenclatureClass>();

        static Guid CurrentGroupID;
        NomenclatureClass DGSelectedItem;

        private string currentGroup; // Временно
        public string CurrentGroup
        {
            get => currentGroup;
            set
            {
                currentGroup = value;
                OnPropertyChanged();
            }
        }

        bool firstLoad = true;

        #endregion


        #region События 
        private void NomenclatureATI_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstLoad)
            {
                LoadAllNomenclatures();
                firstLoad = false;
                PeriodicLoadingAllNomenclatures();
            }

        }

        // Событие: DoubleClick на DataGridRow -----------------------------------------------------------------------------------------------
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenGroup(DataGridNomenclatures);
            e.Handled = true;
        }

        // Событие: PreviewKeyDown для DataGrid номенклатуры ---------------------------------------------------------------------------------
        private void DataGridNomenclatures_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Если нажато: Enter
            if (e.Key == Key.Enter && ((sender as DataGrid).SelectedItem as NomenclatureClass).Name != "")
            {
                OpenGroup(sender);
                e.Handled = true;
            }


            // Если нажато: Ctrl+Down
            if (e.Key == Key.Down && e.KeyboardDevice.Modifiers == ModifierKeys.Control && ((sender as DataGrid).Items.Count > 0)) // Control + Up
            {
                OpenGroup(sender);
                e.Handled = true;
            }

            //// Если нажато: Control+Up
            //if (e.Key == Key.Up && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            //{
            //    CloseGroup();
            //    //e.Handled = true;
            //}
            //if (e.Key == Key.Back)
            //{
            //    CloseGroup();
            //    e.Handled = true;
            //}



            if (e.Key == Key.Home)
            {
                DataGrid DG = sender as DataGrid;
                DG.SelectedIndex = 0;
                if (DG.SelectedItems.Count > 0)
                    DG.CurrentCell = new DataGridCellInfo(DG.Items[0], DG.Columns[3]);
                e.Handled = true;
            }
            if (e.Key == Key.End)
            {
                DataGrid DG = sender as DataGrid;
                DG.SelectedIndex = DG.Items.Count - 1;

                if (DG.SelectedItems.Count > 0)
                    DG.CurrentCell = new DataGridCellInfo(DG.Items[DG.Items.Count - 1], DG.Columns[3]);
                e.Handled = true;
            }
            if (e.Key == Key.F2)
            {
                //ButtonEdit_Click(null, null);
                e.Handled = true;
            }
            if (e.Key == Key.Space)
            {
                //NomenclatureClass NC = (sender as DataGridRow).DataContext as NomenclatureClass;
                foreach (NomenclatureClass DGR in DataGridNomenclatures.SelectedItems)
                {
                    if (DGR.Selected == null || DGR.Selected == false)
                        DGR.Selected = true;
                    else if (DGR.Selected == true)
                    {
                        DGR.Selected = false;
                    }
                }
                e.Handled = true;
            }
            if (e.Key == Key.X && e.KeyboardDevice.Modifiers == ModifierKeys.Control) // Control + X = Вырезать
            {
                CutElements(); // Вырезать
                e.Handled = true;
            }
            if (e.Key == Key.V && e.KeyboardDevice.Modifiers == ModifierKeys.Control) // Control + V = Вставить
            {
                PasteElements(); // Вставить
                e.Handled = true;
            }
        }

        // Событие: PreviewKeyDown для DataGridRow -------------------------------------------------------------------------------------------
        private void DataGridRow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //// Если нажато: Ctrl+Down
            //if (e.Key == Key.Down && e.KeyboardDevice.Modifiers == ModifierKeys.Control) // Control + Up
            //{
            //    NomenclatureClass NC = (sender as DataGridRow).DataContext as NomenclatureClass;
            //    if (NC.GroupNomen == false)
            //    {
            //        CurrentGroupID = NC.ID;
            //        GroupID = CurrentGroupID.ToString();
            //        FillingNomenclaturesList();
            //    }
            //    e.Handled = true;
            //}
            //// Если нажато: Enter
            //if (e.Key == Key.Enter)
            //{
            //    NomenclatureClass NC = (sender as DataGridRow).DataContext as NomenclatureClass;
            //    if (NC.GroupNomen == false)
            //    {
            //        CurrentGroupID = NC.ID;
            //        GroupID = CurrentGroupID.ToString();
            //        FillingNomenclaturesList();
            //    }
            //    e.Handled = true;
            //}
            // Если нажато: Пробел
            if (e.Key == Key.Space)
            {
                NomenclatureClass NC = (sender as DataGridRow).DataContext as NomenclatureClass;
                if (NC.Selected == null || NC.Selected == true)
                    NC.Selected = false;
                else if (NC.Selected == false)
                    NC.Selected = true;
                e.Handled = true;
            }
            // Если нажато: Delete
            if (e.Key == Key.Delete)
            {
                e.Handled = true;
            }

        }

        private void Nom_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // DataGridNomenclatures_PreviewKeyDown(DataGridNomenclatures, e);

            // Если нажато: Control+Up
            if (e.Key == Key.Up && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                CloseGroup();
                e.Handled = true;
            }
            if (e.Key == Key.Back)
            {
                CloseGroup();
                e.Handled = true;
            }



        }

        private void DG_Header_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            CloseGroup();
            //e.Handled = true;
        }

        #endregion


        #region Методы

        // Метод: Периодически обновляет "AllNomenclaturesCollection" из базы данных
        async void PeriodicLoadingAllNomenclatures()
        {
            while (true)
            {
                await Task.Delay(600000);
                ShowMessage(true);
                await Task.Run(() => AllNomenclaturesCollection = DataBaseRequest.GetAllNomenclatures());
                ShowMessage(false);
            }
        }

        // Метод: Загрузка всей номенклатуры и групп из базы данных --------------------------------------------------------------------------
        async void LoadAllNomenclatures()
        {
            ShowMessage(true);
            await Task.Run(() => AllNomenclaturesCollection = DataBaseRequest.GetAllNomenclatures());
            FillingNomenclaturesList();
            DataGridItemSelect(0);
            ShowMessage(false);
        }

        // Метод: Заполняет "ShowNomenclaturesCollection" из "AllNomenclaturesCollection" текушим CurrentGroupID
        void FillingNomenclaturesList()
        {
            ShowNomenclaturesCollection.Clear();
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.GroupID == CurrentGroupID)
                {
                    ShowNomenclaturesCollection.Add(NC);
                }
            }
            DataGridNomenclatures.ItemsSource = ShowNomenclaturesCollection;
        }

        void OpenGroup(object sender)
        {
            UpdateSelected = false;
            if (sender is DataGrid)
            {
                DataGrid DG = sender as DataGrid;
                if (DG.SelectedItem != null)
                {
                    DGSelectedItem = DG.SelectedItem as NomenclatureClass;
                    if (DGSelectedItem.GroupNomen == false)
                    {
                        CurrentGroupID = DGSelectedItem.ID;
                        FillingNomenclaturesList();
                        DataGridItemSelect(0);
                    }
                }
            }

            foreach (NomenclatureClass NC in AllNomenclaturesCollection) // Временно
            {
                if (NC.ID == CurrentGroupID)
                {
                    CurrentGroup = NC.Name;
                    break;
                }
                else
                {
                    CurrentGroup = "Номенклатура";
                }
            }
            UpdateSelected = true;
        }

        void CloseGroup()
        {
            UpdateSelected = false;
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.ID == CurrentGroupID)
                {
                    CurrentGroupID = NC.GroupID;
                    DGSelectedItem = NC;
                    FillingNomenclaturesList();
                    int rowIndex = 0;
                    foreach (NomenclatureClass nc in DataGridNomenclatures.Items)
                    {
                        if (nc == DGSelectedItem)
                            break;
                        rowIndex++;
                    }
                    DataGridItemSelect(rowIndex);
                    break;
                }
            }


            foreach (NomenclatureClass NC in AllNomenclaturesCollection) // Временно
            {
                if (NC.ID == CurrentGroupID)
                {
                    CurrentGroup = NC.Name;
                    break;
                }
                else
                {
                    CurrentGroup = "Номенклатура";
                }
            }
            UpdateSelected = true;
        }

        void DataGridItemSelect(int RowIndex)
        {
            if (DataGridNomenclatures.Items.Count > 0)
            {
                DataGridNomenclatures.SelectedIndex = RowIndex;
                Keyboard.Focus(DataGridNomenclatures);
                DataGridNomenclatures.Focus();
                DataGridNomenclatures.CurrentCell = new DataGridCellInfo(DataGridNomenclatures.Items[RowIndex], DataGridNomenclatures.Columns[3]);
                DataGridNomenclatures.ScrollIntoView(DataGridNomenclatures.SelectedItem);
            }
            else
            {
                Keyboard.Focus(DataGridNomenclatures);
                DataGridNomenclatures.Focus();
            }
        }

        #endregion



        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::











        #region Вырезать/Вставить ============================================================================================================
        // Количество вырезанных элементов ---------------------------------------------------------------------------------------------------
        private int countOfCutElements = 0;
        public int CountOfCutElements
        {
            get => countOfCutElements;
            set
            {
                countOfCutElements = value;
                if (value > 0)
                    PasteButtonVisibility = Visibility.Visible;
                else
                    PasteButtonVisibility = Visibility.Collapsed;
                OnPropertyChanged();
            }
        }

        // Видимость кнопки "Вставить" -------------------------------------------------------------------------------------------------------
        private Visibility pasteButtonVisibility = Visibility.Collapsed;
        public Visibility PasteButtonVisibility
        {
            get => pasteButtonVisibility;
            set
            {
                pasteButtonVisibility = value;
                OnPropertyChanged();
            }
        }


        // Список для основных групп вырезанных элементов ------------------------------------------------------------------------------------

        private BindingList<NomenclatureClass> CutMainGroupList = new BindingList<NomenclatureClass>();

        private void CutElements()
        {
            CleareSelectList();
            bool one = true;
            foreach (NomenclatureClass NC in DataGridNomenclatures.SelectedItems)
            {
                NC.Selected = false;
                EditableElementID = NC.ID;
                if (NC.CutOut == true)
                {
                    if (CheckCutParentGroup(NC) == true)
                    {
                        NC.CutOut = !NC.CutOut;
                        CheckCutNomenclaturesList(NC);
                    }
                    else
                    {
                        if (one)
                        {
                            MessageBox.Show("Группа в которой находится элелемент вырезано!", "Группа вырезано");

                            one = false;
                        }
                    }
                }
                else
                {
                    NC.CutOut = !NC.CutOut;
                    CheckCutNomenclaturesList(NC);
                }
            }

            //SelectLastCreated(EditableElementID);
        }

        private void PasteElements()
        {
            bool flag = false;
            foreach (NomenclatureClass NC in CutMainGroupList)
            {
                if (NC.GroupID != CurrentGroupID)
                {
                    if (NC.ID != CurrentGroupID)
                    {
                        flag = DataBaseRequest.UpdateGroupID(NC.ID, CurrentGroupID);
                        foreach (NomenclatureClass Nlist in AllNomenclaturesCollection)
                        {
                            if (Nlist.ID == NC.ID)
                                Nlist.GroupID = CurrentGroupID;
                        }
                    }
                }
                else
                {
                    flag = true;
                }
            }
            if (flag == true)
            {
                CountOfCutElements = 0;
                FillingNomenclaturesList();

                SelectPastedElements();
                ClearCutList();
            }

        }

        private void SelectPastedElements()
        {
            foreach (NomenclatureClass item in DataGridNomenclatures.Items)
            {
                foreach (NomenclatureClass NC in CutMainGroupList)
                {
                    if (item.ID == NC.ID)
                    {
                        //DataGridNomenclatures.CurrentCell = new DataGridCellInfo(DataGridNomenclatures.Items[0], DataGridNomenclatures.Columns[3]);
                        DataGridNomenclatures.CurrentCell = new DataGridCellInfo(item, DataGridNomenclatures.Columns[3]);
                        DataGridNomenclatures.SelectedItem = item;
                        DataGridNomenclatures.ScrollIntoView(DataGridNomenclatures.SelectedItem);
                    }
                }
            }
        }

        private void ClearCutList()
        {
            CutMainGroupList.Clear();
            foreach (NomenclatureClass Nlist in AllNomenclaturesCollection)
            {
                Nlist.CutOut = false;
            }
            foreach (NomenclatureClass DGlist in DataGridNomenclatures.Items)
            {
                DGlist.CutOut = false;
            }
            CountOfCutElements = 0;
        }


        // Метод: Отмечает вырезанный элемент в списке "AllNomenclaturesList" ----------------------------------------------------------------
        private void CheckCutNomenclaturesList(NomenclatureClass NC)
        {
            foreach (NomenclatureClass list in AllNomenclaturesCollection)
            {
                if (list.ID == NC.ID)
                {
                    list.CutOut = NC.CutOut;

                    GetParentGroup(NC);

                    if (NC.GroupNomen == false)
                        CutChildren(NC);
                }
            }
            CheckCutCount();
        }


        private void GetParentGroup(NomenclatureClass NC)
        {

            bool flag = false;

            if (NC.GroupID != new Guid("00000000-0000-0000-0000-000000000000") && NC.CutOut == true)
            {
                foreach (NomenclatureClass NList in AllNomenclaturesCollection)
                {
                    if (NC.GroupID == NList.ID)
                    {
                        if (NList.CutOut == true)
                        {
                            GetParentGroup(NList);
                        }
                        else
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                flag = true;
            }
            if (flag == true)
            {
                CutMainGroupList.Add(NC);
            }
        }

        // Метод: Считает количество вырезанных и присваивает значение к переменной "CutCount" -----------------------------------------------
        private void CheckCutCount()
        {
            int count = 0;
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.CutOut == true)
                {
                    count++;
                }
            }
            CountOfCutElements = count;
        }

        // Метод: Проверяет вырезано-ли родителская группа вырезаемого элемента --------------------------------------------------------------
        private bool CheckCutParentGroup(NomenclatureClass NC)
        {
            bool ok = true;
            foreach (NomenclatureClass Nlist in AllNomenclaturesCollection)
            {
                if (Nlist.ID == NC.GroupID && Nlist.CutOut == true)
                    ok = false;
            }
            return ok;
        }


        // Метод: Вырезает все Child-ы группы ------------------------------------------------------------------------------------------------
        private void CutChildren(NomenclatureClass NC)
        {
            foreach (NomenclatureClass Nlist in AllNomenclaturesCollection)
            {
                if (Nlist.GroupID == NC.ID)
                {
                    Nlist.CutOut = NC.CutOut;
                    if (Nlist.GroupNomen == false)
                        CutChildren(Nlist);
                }
            }
        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::











        #region Выделение ====================================================================================================================

        // Количество выбранных Номенклатур --------------------------------------------------------------------------------------------------
        private int selectedNomenclaturesCount;
        public int SelectedNomenclaturesCount
        {
            get => selectedNomenclaturesCount;
            set
            {
                selectedNomenclaturesCount = value;
                if (value > 0)
                    ListOfSelectedButtonVisibility = Visibility.Visible;
                else
                    ListOfSelectedButtonVisibility = Visibility.Collapsed;
                OnPropertyChanged();
            }
        }

        // Видимость кнопки "Выбранные" ------------------------------------------------------------------------------------------------------
        private Visibility listOfSelectedButtonVisibility = Visibility.Collapsed;
        public Visibility ListOfSelectedButtonVisibility
        {
            get => listOfSelectedButtonVisibility;
            set
            {
                listOfSelectedButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        // Список для выбранных элементов ----------------------------------------------------------------------------------------------------
        private ObservableCollection<NomenclatureClass> AllSelectedElementsList = new ObservableCollection<NomenclatureClass>();

        // Событие: При изменении списка выбранных элементов ---------------------------------------------------------------------------------
        private void AllSelectedElementsList_ListChanged(object sender, ListChangedEventArgs e)
        {
            SetSelectedCount();
        }

        // Переменная для изменения списка выбранных -----------------------------------------------------------------------------------------
        private bool UpdateSelected = true;

        // Событие: Check и Uncheck на CheckBox Выбора номенклатуры --------------------------------------------------------------------------
        private void Select(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;

            if (checkbox.Name == "MainSelectorCheckBox")
            {
                if (UpdateSelected == true)
                {
                    SelcectAllItemsDataGrid(checkbox);
                }
            }
            else
            {
                if (UpdateSelected == true)
                {
                    NomenclatureClass NC = checkbox.DataContext as NomenclatureClass;
                    DefineSelectedElement(NC);
                    UpdateMainCheckBoxChecked();
                }
            }
            ClearCutList();
        }

        // Метод: Выберает все элементы в DataGrid -------------------------------------------------------------------------------------------
        private void SelcectAllItemsDataGrid(CheckBox checkbox)
        {
            foreach (NomenclatureClass NC in DataGridNomenclatures.Items)
            {
                UpdateSelected = false;
                NC.Selected = checkbox.IsChecked;
                UpdateSelected = true;
                DefineSelectedElement(NC);
            }
        }

        // Метод: Определяет что и как отмечено ----------------------------------------------------------------------------------------------
        private void DefineSelectedElement(NomenclatureClass NC)
        {
            if (NC.GroupNomen == true) // Если номенклатура
            {
                if (NC.Selected == true) // Если CheckBox isChecked = true
                {
                    if (ExistenceCheck(NC.ID) == true)
                    {
                        AddNomenclature(NC); // Вызов метода: Добавить номенклатуру в список
                    }
                }
                if (NC.Selected == false)// Если CheckBox isChecked = false
                {
                    RemoveNomenclature(NC); // Вызов метода: Удалить номенклатуру из списка
                }
            }
            if (NC.GroupNomen == false) // Если группа
            {
                if (NC.Selected == true) // Если CheckBox isChecked = true
                {
                    if (ExistenceCheck(NC.ID) == true)
                    {
                        AddGroup(NC); // Вызов метода: Добавить группу в список
                    }
                }
                if (NC.Selected == false)// Если CheckBox isChecked = false
                {
                    RemoveGroup(NC); // Вызов метода: Удалить группу из списка
                }
            }
        }

        // Метод: Обновляет состояние "IsChecked" главного CheckBox-а ------------------------------------------------------------------------
        private void UpdateMainCheckBoxChecked()
        {
            int CheckedTrueCount = 0;
            int CheckedNullCount = 0;
            foreach (NomenclatureClass NC in DataGridNomenclatures.Items)
            {
                if (NC.Selected == true)
                    CheckedTrueCount++;
                if (NC.Selected == null)
                    CheckedNullCount++;
            }
            UpdateSelected = false;
            if (DataGridNomenclatures.Items.Count > 0 && CheckedTrueCount == DataGridNomenclatures.Items.Count)
            {
                MainSelectorCheckBox.IsChecked = true;
            }
            else if ((CheckedNullCount > 0 || CheckedTrueCount > 0) && CheckedTrueCount != DataGridNomenclatures.Items.Count)
            {
                MainSelectorCheckBox.IsChecked = null;
            }
            else if (DataGridNomenclatures.Items.Count == 0 || CheckedTrueCount == 0)
            {
                MainSelectorCheckBox.IsChecked = false;
            }
            UpdateSelected = true;
        }

        // Метод: Проверяет на наличие или отсуствие в списке "AllSelectedElementsList" ------------------------------------------------------
        private bool ExistenceCheck(Guid ID)
        {
            bool ok = true;
            foreach (NomenclatureClass SelList in AllSelectedElementsList)
            {
                if (ID == SelList.ID)
                {
                    ok = false;
                }
            }
            return ok;
        }

        // Метод: Если номенклатура отмечена Checked -----------------------------------------------------------------------------------------
        private void AddNomenclature(NomenclatureClass NC)
        {
            NC.Selected = true;
            NC.Icon = (Viewbox)GetIcons.GetIcon("Номенклатура");
            AllSelectedElementsList.Add(NC);
            SearchAndCheckParentGroup(NC.GroupID);
        }

        // Метод: Если номенклатура отмечена Unchecked ---------------------------------------------------------------------------------------
        private void RemoveNomenclature(NomenclatureClass NC)
        {
            Guid GroupID;
            foreach (NomenclatureClass SelList in AllSelectedElementsList)
            {
                if (NC.ID == SelList.ID)
                {
                    GroupID = SelList.GroupID;
                    AllSelectedElementsList.Remove(SelList);
                    SearchAndUnCheckParentGroup(GroupID);
                    break;
                }
            }
        }

        // Метод: Если группа отмечена Checked -----------------------------------------------------------------------------------------------
        private void AddGroup(NomenclatureClass NC)
        {
            NC.Icon = (Viewbox)GetIcons.GetIcon("Папка");
            AllSelectedElementsList.Add(NC);
            SearchAndCheckGroupChildren(NC.ID);
            SearchAndCheckParentGroup(NC.ID);
        }

        // Метод: Если группа отмечена Unchecked ---------------------------------------------------------------------------------------------
        private void RemoveGroup(NomenclatureClass NC)
        {
            Guid groupID = NC.GroupID;
            Guid iD = NC.ID;
            AllSelectedElementsList.Remove(NC);

            SearchAndUnCheckGroupChildren(iD);
            SearchAndUnCheckParentGroup(iD);
        }

        // Метод: Находит все child-ы группы и отмечает Checked ------------------------------------------------------------------------------
        private void SearchAndCheckGroupChildren(Guid GroupID)
        {
            UpdateSelected = false;
            foreach (NomenclatureClass ListDB in GetSelectedGroupChildren(GroupID))
            {
                if (ListDB.GroupNomen == true) // Если номенклатура
                {
                    if (ExistenceCheck(ListDB.ID) == true)
                    {
                        ListDB.Selected = true;
                        AddNomenclature(ListDB); // Вызов метода: Добавить номенклатуру в список
                    }
                    else
                    {
                        foreach (NomenclatureClass SelList in AllSelectedElementsList)
                        {
                            if (SelList.ID == ListDB.ID)
                            {
                                SelList.Selected = true;
                            }
                        }
                    }
                }
                else // Если эта группа
                {
                    if (ExistenceCheck(ListDB.ID) == true)
                    {
                        ListDB.Selected = true;
                        AddGroup(ListDB); // Вызов метода: Добавить номенклатуру в список
                    }
                    else
                    {
                        foreach (NomenclatureClass SelList in AllSelectedElementsList)
                        {
                            if (SelList.ID == ListDB.ID)
                            {
                                SelList.Selected = true;
                            }
                        }
                    }
                }

            }
            UpdateSelected = true;

        }

        // Метод: Находит все child-ы группы и отмечает Unchecked ----------------------------------------------------------------------------
        private void SearchAndUnCheckGroupChildren(Guid GroupID)
        {
            UpdateSelected = false;

            foreach (NomenclatureClass SelList in AllSelectedElementsList.ToArray())
            {
                if (SelList.GroupID == GroupID)
                {
                    if (SelList.GroupNomen == true) // Если номенклатура
                    {
                        SelList.Selected = false;
                        RemoveNomenclature(SelList);
                    }
                    else //Если группа
                    {
                        RemoveGroup(SelList);
                    }
                }
            }




            UpdateSelected = true;
        }

        // Метод: Находит все родительские группы отмечает в соответствии (+) ----------------------------------------------------------------
        private void SearchAndCheckParentGroup(Guid GroupID)
        {
            if (GroupID != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                UpdateSelected = false;

                NomenclatureClass Group = GetParentNomenclatureGroup(GroupID);
                int GroupChildrenCountDB = GetNomenclatureGroupChildrenCount(GroupID);
                int GroupChildrenCountSelectList = 0;

                foreach (NomenclatureClass SelList in AllSelectedElementsList)
                {
                    if (SelList.GroupID == GroupID && SelList.Selected == true)
                    {
                        GroupChildrenCountSelectList++;
                    }
                }
                if (GroupChildrenCountSelectList == GroupChildrenCountDB)
                {
                    if (ExistenceCheck(GroupID))
                    {
                        Group.Selected = true;

                        Group.Icon = (Viewbox)GetIcons.GetIcon("Папка");

                        AllSelectedElementsList.Add(Group);
                    }
                    else
                    {
                        foreach (NomenclatureClass SelList in AllSelectedElementsList)
                        {
                            if (SelList.ID == GroupID)
                            {
                                SelList.Selected = true;
                            }
                        }
                    }
                }
                if (GroupChildrenCountSelectList < GroupChildrenCountDB)
                {
                    if (ExistenceCheck(GroupID))
                    {
                        Group.Selected = null;

                        Group.Icon = (Viewbox)GetIcons.GetIcon("Папка");

                        AllSelectedElementsList.Add(Group);
                    }
                    else
                    {
                        foreach (NomenclatureClass SelList in AllSelectedElementsList)
                        {
                            if (SelList.ID == GroupID)
                            {
                                SelList.Selected = null;
                            }
                        }
                    }
                }
                if (Group.GroupID != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    SearchAndCheckParentGroup(Group.GroupID);
                }
                UpdateSelected = true;
            }
        }

        // Метод: Находит все родительские группы отмечает в соответствии (-) ----------------------------------------------------------------
        private void SearchAndUnCheckParentGroup(Guid GroupID)
        {
            UpdateSelected = false;

            NomenclatureClass Group = GetParentNomenclatureGroup(GroupID);
            Guid groupID = Group.GroupID;
            int GroupChildrenCountDB = GetNomenclatureGroupChildrenCount(GroupID);
            int GroupChildrenCountSelectList = 0;
            bool SelectNull = false;

            foreach (NomenclatureClass SelList in AllSelectedElementsList)
            {
                if (SelList.GroupID == GroupID && SelList.Selected == true)
                {
                    GroupChildrenCountSelectList++;
                }
                if (SelList.GroupID == GroupID && SelList.Selected == null)
                {
                    SelectNull = true;
                }
            }
            if (GroupChildrenCountSelectList > 0)
            {
                if (GroupChildrenCountDB == GroupChildrenCountSelectList)
                {
                    foreach (NomenclatureClass SelList in AllSelectedElementsList)
                    {
                        if (SelList.ID == Group.ID)
                        {
                            SelList.Selected = true;
                        }
                    }
                }
                else
                {
                    foreach (NomenclatureClass SelList in AllSelectedElementsList)
                    {
                        if (SelList.ID == Group.ID)
                        {
                            SelList.Selected = null;
                        }
                    }
                }
            }
            else if (SelectNull == true)
            {
                foreach (NomenclatureClass SelList in AllSelectedElementsList)
                {
                    if (SelList.ID == Group.ID)
                    {
                        SelList.Selected = null;
                    }
                }
            }
            else
            {
                foreach (NomenclatureClass SelList in AllSelectedElementsList)
                {
                    if (SelList.ID == Group.ID)
                    {
                        AllSelectedElementsList.Remove(SelList);
                        break;
                    }
                }
            }

            if (groupID != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                SearchAndUnCheckParentGroup(Group.GroupID);
            }

            UpdateSelected = true;
        }

        // Метод: Возвращает родительскую группу элемента ------------------------------------------------------------------------------------
        private NomenclatureClass GetParentNomenclatureGroup(Guid GroupID)
        {
            NomenclatureClass GroupParent = new NomenclatureClass();
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (GroupID == NC.ID)
                {
                    if (NC.GroupNomen == true)
                    {
                        NC.Icon = (Viewbox)GetIcons.GetIcon("Номенклатура");
                    }
                    else
                    {
                        NC.Icon = (Viewbox)GetIcons.GetIcon("Папка");
                    }
                    GroupParent = NC;
                }
            }
            return GroupParent;
        }

        // Метод: Возвращает все Child-ы группы ----------------------------------------------------------------------------------------------
        private BindingList<NomenclatureClass> GetSelectedGroupChildren(Guid GroupID)
        {
            BindingList<NomenclatureClass> list = new BindingList<NomenclatureClass>();
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (GroupID == NC.GroupID)
                {
                    if (NC.GroupNomen == true)
                    {
                        NC.Icon = (Viewbox)GetIcons.GetIcon("Номенклатура");
                    }
                    else
                    {
                        NC.Icon = (Viewbox)GetIcons.GetIcon("Папка");
                    }
                    list.Add(NC);
                }
            }
            return list;
        }

        // Метод: Возвращает количество child-ов ---------------------------------------------------------------------------------------------
        private int GetNomenclatureGroupChildrenCount(Guid GroupID)
        {
            int count = 0;
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (GroupID == NC.GroupID)
                {
                    count++;
                }
            }
            return count;
        }

        // Метод: Отмечает выбранные согласно списку AllSelectedElementsList -----------------------------------------------------------------
        private void DataGridSelectionUpdate()
        {
            if (DataGridNomenclatures.Items.Count > 0)
            {
                UpdateSelected = false;
                foreach (NomenclatureClass DGItem in DataGridNomenclatures.Items)
                {
                    if (AllSelectedElementsList.Count > 0)
                    {
                        foreach (NomenclatureClass SelList in AllSelectedElementsList)
                        {
                            if (SelList.ID == DGItem.ID)
                            {
                                DGItem.Selected = SelList.Selected;
                            }
                        }
                    }
                    else
                    {
                        DGItem.Selected = false;
                    }
                    foreach (NomenclatureClass SelList in AllNomenclaturesCollection)
                    {
                        if (SelList.ID == DGItem.ID)
                        {
                            DGItem.CutOut = SelList.CutOut;
                        }
                    }
                }
                UpdateSelected = true;
                UpdateMainCheckBoxChecked();
            }
        }

        // Метод: Считает количество выбранных номенклатур и присваивает значение к переменной "SelectedCount" -------------------------------
        private void SetSelectedCount()
        {
            int count = 0;
            foreach (NomenclatureClass SelList in AllSelectedElementsList)
            {
                if (SelList.GroupNomen == true)
                {
                    count++;
                }
            }
            SelectedNomenclaturesCount = count;
        }

        // Метод: Открывает окно с выбранными номенклатурами ---------------------------------------------------------------------------------
        private void OpenSelectedNomenclaturesList()
        {
            //ToolsGrid.Visibility = Visibility.Visible;
            //SelectedNomenclatures SelectedNomens = new SelectedNomenclatures(ref AllSelectedElementsList);
            //ToolsContentControl.Content = SelectedNomens;
            //SelectedNomens.ButtonClose.Click += SelectedNomenclaturesButtonClose_Click;
        }


        private void CleareSelectList()
        {
            SelectedNomenclaturesCount = 0;
            AllSelectedElementsList.Clear();
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                NC.Selected = false;
            }
            //DataGridSelectionUpdate();
        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::











        #region Теги =========================================================================================================================
        private void UpdateTagsClick(object sender, RoutedEventArgs e)
        {

        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::











        #region Создание/Редактирование номенклатуры =========================================================================================
        // ID редактиуемого элемента ---------------------------------------------------------------------------------------------------------
        private Guid editableElementID;
        public Guid EditableElementID
        {
            get => editableElementID;
            set
            {
                editableElementID = value;
                OnPropertyChanged();
            }
        }
        private void ButtonCreateNomenclature_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::











        #region Создание/Редактирование группы ===============================================================================================
        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::











        #region Сообщение/Анимация загрузки ==================================================================================================

        LoaderCubes LC = new LoaderCubes();

        private UserControl loaderUC = null;
        public UserControl LoaderUC
        {
            get => loaderUC;
            set
            {
                loaderUC = value;
                OnPropertyChanged();
            }
        }

        private string messageText = "";
        public string MessageText
        {
            get => messageText;
            set
            {
                messageText = value;
                OnPropertyChanged();
            }
        }

        private SolidColorBrush messageTextColor = new SolidColorBrush(Colors.DarkGray);
        public SolidColorBrush MessageTextColor
        {
            get => messageTextColor;
            set
            {
                messageTextColor = value;
                OnPropertyChanged();
            }
        }



        // Метод: Показывает текст сообщения и анимация загрузки (1-перегрузка) ----------------------------------------
        void ShowMessage(string Text, string TextColor, bool ShowLoader)
        {
            MessageText = Text;
            MessageTextColor = GetColor.Get(TextColor);
            if (ShowLoader)
            {
                LoaderUC = LC;
            }
            else
            {
                LoaderUC = null;
            }
        }

        // Метод: Показывает текст сообщения и анимация загрузки (2-перегрузка) ----------------------------------------
        void ShowMessage(bool ShowLoader)
        {
            MessageText = "";
            MessageTextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C566E"));

            if (ShowLoader)
            {
                LoaderUC = LC;
            }
            else
            {
                LoaderUC = null;
            }
        }




        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    }
}
