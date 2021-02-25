using ControlSystemsLibrary.Classes;
using ControlSystemsLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace ControlSystemsLibrary.Controls.AdminTabItemContents
{
    public delegate void CloseCreateNomenclatureDelegate();
    public delegate void ShowCreatedNomenclatureDelegate(NomenclatureClass CreatedNomenclature);

    public delegate void CloseCreateNomenclatureGroupDelegate();
    public delegate void ShowCreatedNomenclatureGroupDelegate(NomenclatureClass CreatedNomenclature);

    public delegate void CloseSelectedItemsDelegate();
    public delegate void CheckedSelectedItemParentDelegate(NomenclatureClass Item);
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

        private string CurrentCryptConnectionString;

        // Конструктор
        public NomenclatureATI(string CurrentCryptConnectionString)
        {
            this.CurrentCryptConnectionString = CurrentCryptConnectionString;
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

        int SelectedCollumn = 3;

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

        private string searchedText = "";
        public string SearchedText
        {
            get => searchedText;
            set
            {
                if(searchedText != value)
                {
                    searchedText = value;
                    OnPropertyChanged();
                    if (value != "")
                    {
                        SearchRow();
                    }
                    else
                    {
                        SearchedTextColor = GetColor.Get("Light-005");
                        SearchResult = "Поиск";
                    }
                }
            }
        }

        private SolidColorBrush searchedTextColor = GetColor.Get("Light-005");
        public SolidColorBrush SearchedTextColor
        {
            get => searchedTextColor;
            set
            {
                searchedTextColor = value;
                OnPropertyChanged();
            }
        }

        private string searchResult = "Поиск";
        public string SearchResult
        {
            get => searchResult;
            set
            {
                if (searchResult != value)
                {
                    searchResult = value;
                    OnPropertyChanged();
                }
            }
        }

        bool FirstBoot = true;

        private bool buttonsEnable = false;
        public bool ButtonsEnable
        {
            get => buttonsEnable;
            set
            {
                buttonsEnable = value;
                OnPropertyChanged();
            }
        }

        // События----------------------------------------------------------------------------------------------------------------------------


        private void SearchRow()
        {
            foreach(NomenclatureClass NC in DataGridNomenclatures.Items)
            {
                string name = "";
                string article = "";
                for (int i = 0; i < SearchedText.Length; i++)
                {
                    if (NC.Name.Length - 1 >= i)
                    {
                        name += NC.Name[i];
                    }
                    if (NC.Article.Length - 1 >= i)
                    {
                        article += NC.Article[i];
                    }
                }
                if(name == SearchedText)
                {
                    SearchResult = "Найдено в наименованиях";
                    SearchedTextColor = GetColor.Get("Blue-003");
                    SelectedItem = NC;
                    SelectedCollumn = 3;
                    SelectDataGridRow();
                    break;
                }
                else if (article == SearchedText)
                {
                    SearchResult = "Найдено в артикулах";
                    SearchedTextColor = GetColor.Get("Blue-003");
                    SelectedItem = NC;
                    SelectedCollumn = 4;
                    SelectDataGridRow();
                    break;
                }
                else
                {
                    SearchResult = "Не найдено!";
                    SelectedItem = null;
                    SearchedTextColor = GetColor.Get("Purpure-002");
                }
            }
        }





        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedItem = null;
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
                SelectedItem = null;
                if ((sender as DataGrid).SelectedItem != null)
                {
                    NomenclatureClass NC = (sender as DataGrid).SelectedItem as NomenclatureClass;
                    if (NC.GroupNomen == false)
                    {
                        CurrentGroupID = NC.ID;
                    }
                }
                SearchedText = "";
                e.Handled = true;
            }
            if(e.Key == Key.Up)
            {
                SearchedText = "";
            }
            if (e.Key == Key.Down)
            {
                SearchedText = "";
            }
            if (e.Key == Key.Left)
            {
                SearchedText = "";
            }
            if (e.Key == Key.Right)
            {
                SearchedText = "";
            }
            if(e.Key == Key.Delete)
            {
                SearchedText = "";
                e.Handled = true;
            }
            // Backspace ←
            if (e.Key == Key.Back)
            {
                string str = "";
                for (int i = 0; i < SearchedText.Length - 1; i++)
                {
                    str += SearchedText[i];
                }
                SearchedText = str;
                e.Handled = true;
            }
            // Ctrl + Down ↓
            if (e.Key == Key.Down && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                SelectedItem = null;
                if (DataGridNomenclatures.SelectedItem != null)
                {
                    NomenclatureClass NC = DataGridNomenclatures.SelectedItem as NomenclatureClass;
                    if (NC.GroupNomen == false)
                    {
                        CurrentGroupID = NC.ID;
                    }
                }
                SearchedText = "";
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
                SearchedText = "";
                e.Handled = true;
            }
            // Home 
            if (e.Key == Key.Home)
            {
                NomenclatureClass NC = DataGridNomenclatures.Items[0] as NomenclatureClass;
                SelectedItem = NC;
                SelectDataGridRow();
                SearchedText = "";
                e.Handled = true;
            }
            // End 
            if (e.Key == Key.End)
            {
                NomenclatureClass NC = DataGridNomenclatures.Items[DataGridNomenclatures.Items.Count - 1] as NomenclatureClass;
                SelectedItem = NC;
                SelectDataGridRow();
                SearchedText = "";
                e.Handled = true;
            }
            if (e.Key == Key.N && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                CreateNomenclatureButtonClick(null, null);
                e.Handled = true;
            }
            if (e.Key == Key.X && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                CutSelectedItems();
                e.Handled = true;
            }
            if(e.Key == Key.V && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (CutElementsCoutVisibility == Visibility.Visible)
                {
                    Paste();
                }
                e.Handled = true;
            }
        }

        private void DataGridRow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Если нажато: Пробел
            if (e.Key == Key.Space)
            {
                SelectedToSpace();
                e.Handled = true;
            }

        }

        private void Nom_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            

            // Ctrl + N (Создание новой номенклатуры)
            if (e.Key == Key.N && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                CreateNomenclatureButtonClick(null, null);
                SearchedText = "";
                e.Handled = true;
            }
            // Ctrl + Down ↓
            if (e.Key == Key.Down && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                SelectedItem = null;
                if (DataGridNomenclatures.SelectedItem != null)
                {
                    NomenclatureClass NC = DataGridNomenclatures.SelectedItem as NomenclatureClass;
                    if (NC.GroupNomen == false)
                    {
                        CurrentGroupID = NC.ID;
                    }
                }
                SearchedText = "";
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
                SearchedText = "";
                e.Handled = true;
            }


        }

        private void DataGridNomenclatures_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (DataGridNomenclatures.Items.Count > 0)
            {
                if (e.Text.All(IsGood))
                {
                    SearchedText += e.Text;
                    e.Handled = true;
                }
                else
                {
                    SearchedText = "";
                }
            }
        }

        private void DataGridNomenclatures_Loaded(object sender, RoutedEventArgs e)
        {
            if (FirstBoot == true)
            {
                LoadMainNomenclatures();
                LoadAllNomenclatures();
                FirstBoot = false;
            }
        }


        // Методы ----------------------------------------------------------------------------------------------------------------------------

        private async void LoadMainNomenclatures()
        {
            await Task.Run(() => AllNomenclaturesCollection = DataBaseRequest.GetMainNomenclatures(CurrentCryptConnectionString));
            
            LoadShowNomenclatures();
            DataGridNomenclatures.ItemsSource = ShowNomenclaturesCollection;
            AddLinqButton();
        }

        async void LoadAllNomenclatures()
        {
            AllNomenclaturesCollection.Clear();
            await Task.Run(() => AllNomenclaturesCollection = DataBaseRequest.GetAllNomenclatures(CurrentCryptConnectionString));
            LoadShowNomenclatures();
            ButtonsEnable = true;
            //DataGridNomenclatures.ItemsSource = ShowNomenclaturesCollection;
            //SearchedText = "";
            //AddLinqButton();
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
            SearchedText = "";
            SelectDataGridRow();
            CheckedMainCheckBox();
        }

        void LoadAllNomenclatures(NomenclatureClass CreatedNomenclature)
        {
            AllNomenclaturesCollection.Clear();
            AllNomenclaturesCollection = DataBaseRequest.GetAllNomenclatures(CurrentCryptConnectionString);
            LoadShowNomenclatures(CreatedNomenclature);
        }

        void LoadShowNomenclatures(NomenclatureClass CreatedNomenclature)
        {
            ShowNomenclaturesCollection.Clear();
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.GroupID == CurrentGroupID)
                {
                    ShowNomenclaturesCollection.Add(NC);
                }
                if(NC.ID == CreatedNomenclature.ID)
                {
                    SelectedItem = NC;
                }
            }
            SelectDataGridRow();
        }


        void SelectDataGridRow()
        {
            if (DataGridNomenclatures.Items.Count > 0 && SelectedItem != null)
            {
                DataGridNomenclatures.SelectedItem = SelectedItem;
                DataGridNomenclatures.CurrentCell = new DataGridCellInfo(DataGridNomenclatures.Items[DataGridNomenclatures.SelectedIndex], DataGridNomenclatures.Columns[SelectedCollumn]);
                DataGridNomenclatures.ScrollIntoView(DataGridNomenclatures.SelectedItem);
                DataGridNomenclatures.Columns[3].IsReadOnly = false;
                DataGridNomenclatures.BeginEdit();
                DataGridNomenclatures.Columns[3].IsReadOnly = true;
            }
            if (DataGridNomenclatures.Items.Count > 0 && SelectedItem == null)
            {
                DataGridNomenclatures.SelectedIndex = 0;
                DataGridNomenclatures.CurrentCell = new DataGridCellInfo(DataGridNomenclatures.Items[0], DataGridNomenclatures.Columns[SelectedCollumn]);
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

        bool IsGood(char c)
        {
            if (c >= '0' && c <= '9')
                return true;
            if (c >= 'a' && c <= 'z')
                return true;
            if (c >= 'A' && c <= 'z')
                return true;
            if (c >= 'а' && c <= 'я')
                return true;
            if (c >= 'А' && c <= 'Я')
                return true;
            if (c == ' ' || c == '-' || c == '/' || c == '_' || c == '.' || c == ',' || c == '+' || c == '*')
                return true;
            return false;
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
                LB.Foreground = GetColor.Get("Blue-004");
            else
                LB.Foreground = GetColor.Get("Dark-002");
            LB.Content = ContentText;
            LB.ID = ID;
            LB.Click += LinqButton_Click;
            return LB;
        }





        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        #region Создать номенклатуру =========================================================================================================

        private Visibility createNemenclatureUCVisibility = Visibility.Hidden;
        public Visibility CreateNemenclatureUCVisibility
        {
            get => createNemenclatureUCVisibility;
            set
            {
                createNemenclatureUCVisibility = value;
                OnPropertyChanged();
            }
        }


        private UserControl createNemenclatureUC = null;
        public UserControl CreateNemenclatureUC
        {
            get => createNemenclatureUC;
            set
            {
                if (createNemenclatureUC != value)
                {
                    createNemenclatureUC = value;
                    OnPropertyChanged();
                    if(value == null)
                    {
                        CreateNemenclatureUCVisibility = Visibility.Hidden;
                    }
                    else
                    {
                        CreateNemenclatureUCVisibility = Visibility.Visible;
                    }
                }
            }
        }


        private void CreateNomenclatureButtonClick(object sender, RoutedEventArgs e)
        {
            ClearSelected();
            ClearCutList();

            if (DataGridNomenclatures.SelectedItem != null)
            {
                SelectedItem = DataGridNomenclatures.SelectedItem as NomenclatureClass;
            }
            CloseCreateNomenclatureDelegate CCND = CloseCreateNomenclature;
            ShowCreatedNomenclatureDelegate SCND = AddCreatedNomenclatureToCollection;
            CreateNemenclatureUC = new CreateNomenclature(CurrentCryptConnectionString, true, CCND, SCND, CurrentGroupID);
        }

        private void CloseCreateNomenclature()
        {
            CreateNemenclatureUC = null;
            SelectDataGridRow();
        }

        private void AddCreatedNomenclatureToCollection(NomenclatureClass CreatedNomenclature)
        {
            LoadAllNomenclatures(CreatedNomenclature);
        }


        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


        private void DataGridNomenclatures_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SearchedText = "";
        }

        private void EditNomenclatureButtonClick(object sender, RoutedEventArgs e)
        {
            ClearSelected();
            ClearCutList();
            if (DataGridNomenclatures.SelectedItem != null)
            {
                SelectedItem = DataGridNomenclatures.SelectedItem as NomenclatureClass;
                if (SelectedItem.GroupNomen == true)
                {
                    CloseCreateNomenclatureDelegate CCND = CloseCreateNomenclature;
                    ShowCreatedNomenclatureDelegate SCND = AddCreatedNomenclatureToCollection;
                    CreateNemenclatureUC = new CreateNomenclature(CurrentCryptConnectionString, false, CCND, SCND, SelectedItem);
                }
                else
                {
                    CloseCreateNomenclatureGroupDelegate CCNGD = CloseCreateNomenclature;
                    ShowCreatedNomenclatureGroupDelegate SCNGD = AddCreatedNomenclatureToCollection;
                    CreateNemenclatureUC = new CreateNomenclatureGroup(CurrentCryptConnectionString, false, CCNGD, SCNGD, SelectedItem);
                }
            }
        }

        private void CreateNomenclatureGroupButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSelected();
            ClearCutList();
            if (DataGridNomenclatures.SelectedItem != null)
            {
                SelectedItem = DataGridNomenclatures.SelectedItem as NomenclatureClass;
            }
            CloseCreateNomenclatureGroupDelegate CCNGD = CloseCreateNomenclature;
            ShowCreatedNomenclatureGroupDelegate SCNGD = AddCreatedNomenclatureToCollection;
            CreateNemenclatureUC = new CreateNomenclatureGroup(CurrentCryptConnectionString, true, CCNGD, SCNGD, CurrentGroupID);
        }

        private void Nom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SearchedText = "";
        }






        private void CutButtonClick(object sender, RoutedEventArgs e)
        {
            ClearSelected();
            CutSelectedItems();
        }

        private void CutSelectedItems()
        {
            foreach(NomenclatureClass NC in DataGridNomenclatures.SelectedItems)
            {
                if (NC.CutOut == false)
                {
                    NC.CutOut = true;
                    foreach (NomenclatureClass NCList in AllNomenclaturesCollection)
                    {
                        if (NC.ID == NCList.ID)
                        {
                            NCList.CutOut = true;
                        }
                    }
                    if (NC.GroupNomen == false) // Если это группа
                    {
                        if (CheckChildrenExist(NC) == true)
                        {
                            CutSelectedItemsChildren(NC);
                            SetMainCutElement();
                        }
                        else
                        {
                            NC.MainCutElement = true;
                        }
                    }
                    else
                    {
                        NC.MainCutElement = true;
                    }
                }
                else if(NC.MainCutElement == true)
                {
                    NC.CutOut = false;
                    if(CheckChildrenExist(NC) == true)
                    {
                        CancelCutChildren(NC);
                    }
                }
            }
            CheckCutElementsCout();
        }

        private void CancelCutChildren(NomenclatureClass item)
        {
            foreach (NomenclatureClass NCList in AllNomenclaturesCollection)
            {
                if(NCList.GroupID == item.ID)
                {
                    NCList.CutOut = false;
                    NCList.MainCutElement = false;
                    if(NCList.GroupNomen == false)
                    {
                        CancelCutChildren(NCList);
                    }
                }
            }
        }

        private bool CheckChildrenExist(NomenclatureClass item)
        {
            bool flag = false;
            foreach (NomenclatureClass NCList in AllNomenclaturesCollection)
            {
                if(NCList.GroupID == item.ID)
                {
                    NCList.MainCutElement = false;
                    flag =  true;
                }
            }
            return flag;
        }

        private void CutSelectedItemsChildren(NomenclatureClass item)
        {
            foreach (NomenclatureClass NCList in AllNomenclaturesCollection)
            {
                if(NCList.GroupID == item.ID)
                {
                    NCList.CutOut = true;
                    if(NCList.GroupNomen == false)
                    {
                        CutSelectedItemsChildren(NCList);
                    }
                }
            }
        }

        private void SetMainCutElement()
        {
            foreach (NomenclatureClass NCList in AllNomenclaturesCollection)
            {
                if(NCList.CutOut == true)
                {
                    SetMainCutElementParrent(NCList);
                }
            }
        }

        private void SetMainCutElementParrent(NomenclatureClass item)
        {
            foreach (NomenclatureClass NCList in AllNomenclaturesCollection)
            {
                if(NCList.ID == item.GroupID) // Если это родительская группа
                {
                    if (CheckCutElementParrent(NCList) == true)
                    {
                        SetMainCutElementParrent(NCList);
                    }
                    else if (NCList.CutOut == true)
                    {
                        NCList.MainCutElement = true;
                    }
                }
            }
        }

        private bool CheckCutElementParrent(NomenclatureClass item)
        {
            foreach (NomenclatureClass NCList in AllNomenclaturesCollection)
            {
                if(NCList.ID == item.GroupID && NCList.ID != new Guid("00000000-0000-0000-0000-000000000000") && NCList.CutOut == true)
                {
                    return true;
                }
            }
            return false;
        }

        private int cutElementsCout = 0;
        public int CutElementsCout
        {
            get => cutElementsCout;
            set
            {
                cutElementsCout = value;
                OnPropertyChanged();
                if(value > 0)
                {
                    CutElementsCoutVisibility = Visibility.Visible;
                }
                else
                {
                    CutElementsCoutVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility cutElementsCoutVisibility = Visibility.Collapsed;
        public Visibility CutElementsCoutVisibility
        {
            get => cutElementsCoutVisibility;
            set
            {
                cutElementsCoutVisibility = value;
                OnPropertyChanged();
            }
        }

        private void CheckCutElementsCout()
        {
            int count = 0;
            foreach(NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if(NC.CutOut == true)
                {
                    count++;
                }
            }
            CutElementsCout = count;
        }

        private void ClearCutList_Click(object sender, RoutedEventArgs e)
        {
            ClearCutList();
        }

        private void ClearCutList()
        {
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                NC.CutOut = false;
                NC.MainCutElement = false;
            }
            CheckCutElementsCout();
        }

        private void PasteButtonClick(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void Paste()
        {
            bool flag = false;
            if (CheckCuttingCurrentGroup() == true)
            {
                foreach (NomenclatureClass NC in AllNomenclaturesCollection)
                {
                    if (NC.MainCutElement == true)
                    {
                        if (ChangeNomenclatureGroupIDIntoDataBase(NC) == true)
                        {
                            NC.GroupID = CurrentGroupID;
                            NC.CutOut = false;
                            NC.MainCutElement = false;
                            SelectedItem = NC;
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }
                if (flag == true)
                {
                    LoadShowNomenclatures();
                    ClearCutList();
                }
            }
        }

        private bool CheckCuttingCurrentGroup()
        {
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if(NC.ID == CurrentGroupID)
                {
                    if(NC.CutOut == true)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ChangeNomenclatureGroupIDIntoDataBase(NomenclatureClass PasteElement)
        {
            if (PasteElement.GroupID != CurrentGroupID)
            {
                return DataBaseRequest.ChangeNomenclatureGroupID(CurrentCryptConnectionString, PasteElement.ID, CurrentGroupID);
            }
            else
            {
                return false; 
            }
        }





        // Событие: Клик на Теги (Акция, Фокус и Новинка) ------------------------------------------------------------------------------------
        private void UpdateTagsClick(object sender, RoutedEventArgs e)
        {
            UpdateTagsIntoDataBase(sender);
        }

        private bool UpdateTagsIntoDataBase(object sender)
        {
            bool flag = false;
            if (sender is CheckBox)
            {
                CheckBox ch = sender as CheckBox;
                NomenclatureClass item = ch.DataContext as NomenclatureClass;
                switch (ch.Name)
                {
                    case "TagAksia":
                        {
                            flag =  DataBaseRequest.UpdateTagAksia(CurrentCryptConnectionString, item);
                            break;
                        }
                    case "TagFocus":
                        {
                            flag = DataBaseRequest.UpdateTagFocus(CurrentCryptConnectionString, item);
                            break;
                        }
                    case "TagNew":
                        {
                            flag = DataBaseRequest.UpdateTagNew(CurrentCryptConnectionString, item);
                            break;
                        }
                }
            }
            return flag;
        }








        private int selectedElementsCount = 0;
        public int SelectedElementsCount
        {
            get => selectedElementsCount;
            set
            {
                selectedElementsCount = value;
                OnPropertyChanged();
                if(value > 0)
                {
                    SelectedVisibility = Visibility.Visible;
                }
                else
                {
                    SelectedVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility selectedVisibility = Visibility.Collapsed;
        public Visibility SelectedVisibility
        {
            get => selectedVisibility;
            set
            {
                selectedVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool UpdateSelected = true;

        private void Select(object sender, RoutedEventArgs e)
        {
            if (UpdateSelected == true)
            {
                ClearCutList();
                CheckBox checkbox = sender as CheckBox;
                CheckedSelected(checkbox);
                //SetSelectedElementsCount();
            }
        }

        private void CheckedSelected(CheckBox checkbox)
        {
            if (UpdateSelected == true)
            {
                if (checkbox.Name == "MainSelectorCheckBox")
                {
                    CheckedAllDataGridItems(checkbox.IsChecked);
                }
                else
                {
                    NomenclatureClass NC = checkbox.DataContext as NomenclatureClass;
                    NC.Selected = checkbox.IsChecked;
                    CheckedChildren(NC);
                    CheckedParent(NC);
                }
            }
        }

        private void CheckedAllDataGridItems(bool? Checked)
        {
            foreach(NomenclatureClass DGItems in DataGridNomenclatures.Items)
            {
                foreach(NomenclatureClass NC in AllNomenclaturesCollection)
                {
                    if(DGItems.ID == NC.ID)
                    {
                        DGItems.Selected = Checked;
                        NC.Selected = Checked;
                        if(NC.GroupNomen == false && (Checked == true || Checked == false))
                        {
                            CheckedChildren(NC);
                        }
                    }
                }
            }
        }

        private void CheckedChildren(NomenclatureClass Group)
        {
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if(NC.GroupID == Group.ID)
                {
                    NC.Selected = Group.Selected;
                    if(NC.GroupNomen == false)
                    {
                        CheckedChildren(NC);
                    }
                }
            }
        }

        private void CheckedParent(NomenclatureClass Item)
        {
            UpdateSelected = false;
            if (Item.GroupID != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                foreach (NomenclatureClass NC in AllNomenclaturesCollection)
                {
                    if(NC.ID == Item.GroupID)
                    {
                        if(GetChildrenCount(NC) == GetSelectedChildrenCount(NC))
                        {
                            NC.Selected = true;
                        }
                        if (GetChildrenCount(NC) > GetSelectedChildrenCount(NC))
                        {
                            NC.Selected = null;
                        }
                        if (GetSelectedChildrenCount(NC) == 0)
                        {
                            NC.Selected = false;
                        }
                        if(GetSelectedNullChildrenCount(NC) > 0)
                        {
                            NC.Selected = null;
                        }
                        if(NC.GroupID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            CheckedParent(NC);
                        }

                    }
                }
            }
            CheckedMainCheckBox();
            SetSelectedElementsCount();
            UpdateSelected = true;
        }

        private void CheckedMainCheckBox()
        {
            UpdateSelected = false;
            if (DataGridNomenclatures.Items.Count == GetSelectedDataGridItemsCount())
            {
                MainSelectorCheckBox.IsChecked = true;
            }
            if (DataGridNomenclatures.Items.Count > GetSelectedDataGridItemsCount())
            {
                MainSelectorCheckBox.IsChecked = null;
            }
            if (DataGridNomenclatures.Items.Count == 0)
            {
                MainSelectorCheckBox.IsChecked = false;
            }
            if (GetSelectedDataGridItemsCount() == 0)
            {
                MainSelectorCheckBox.IsChecked = false;
            }
            if(GetSelectedNullDataGridItemsCount() > 0)
            {
                MainSelectorCheckBox.IsChecked = null;
            }
            UpdateSelected = true;
        }


        private int GetSelectedChildrenCount(NomenclatureClass Group)
        {
            int count = 0;
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if(NC.GroupID == Group.ID && NC.Selected == true)
                {
                    count++;
                }
            }
            return count;
        }
        private int GetChildrenCount(NomenclatureClass Group)
        {
            int count = 0;
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.GroupID == Group.ID)
                {
                    count++;
                }
            }
            return count;
        }
        private int GetSelectedNullChildrenCount(NomenclatureClass Group)
        {
            int count = 0;
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.GroupID == Group.ID && NC.Selected == null)
                {
                    count++;
                }
            }
            return count;
        }


        private int GetSelectedDataGridItemsCount()
        {
            int count = 0;
            foreach (NomenclatureClass DGItems in DataGridNomenclatures.Items)
            {
                if(DGItems.Selected == true)
                {
                    count++;
                }
            }
            return count;
        }
        private int GetSelectedNullDataGridItemsCount()
        {
            int count = 0;
            foreach (NomenclatureClass DGItems in DataGridNomenclatures.Items)
            {
                if (DGItems.Selected == null)
                {
                    count++;
                }
            }
            return count;
        }

        private void SetSelectedElementsCount()
        {
            SelectedElementsCount = 0;
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                if (NC.Selected == true && NC.GroupNomen == true)
                {
                    SelectedElementsCount++;
                }
            }
        }
        private void ClearSelected()
        {
            MainSelectorCheckBox.IsChecked = false;
            foreach (NomenclatureClass NC in AllNomenclaturesCollection)
            {
                NC.Selected = false;
            }
            SelectedElementsCount = 0;
        }

        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }


        private void SelectedToSpace()
        {
            if (DataGridNomenclatures.SelectedItems != null)
            {
                foreach (NomenclatureClass NC in DataGridNomenclatures.SelectedItems)
                {
                    if (NC.Selected == null || NC.Selected == true)
                        NC.Selected = false;
                    else if (NC.Selected == false)
                        NC.Selected = true;
                }
            }
        }

        private void SelectedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            CloseSelectedItemsDelegate CSID = CloseSelectedItems;
            CheckedSelectedItemParentDelegate CSIPD = CheckedParent;
            CreateNemenclatureUC = new SelectedNomenclatures(AllNomenclaturesCollection, CSID, CSIPD);
        }

        private void CloseSelectedItems()
        {
            CreateNemenclatureUC = null;
        }
    }
}
