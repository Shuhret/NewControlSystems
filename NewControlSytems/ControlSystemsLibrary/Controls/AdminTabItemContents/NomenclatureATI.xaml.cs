﻿using ControlSystemsLibrary.Classes;
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



        // Методы ----------------------------------------------------------------------------------------------------------------------------


        async void LoadAllNomenclatures()
        {
            AllNomenclaturesCollection.Clear();
            await Task.Run(() => AllNomenclaturesCollection = DataBaseRequest.GetAllNomenclatures(CurrentCryptConnectionString));
            LoadShowNomenclatures();
            DataGridNomenclatures.ItemsSource = ShowNomenclaturesCollection;
            SearchedText = "";
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
            SearchedText = "";
            SelectDataGridRow();
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
                LB.Foreground = GetColor.Get("Dark-001");
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
            CloseCreateNomenclatureGroupDelegate CCNGD = CloseCreateNomenclature;
            ShowCreatedNomenclatureGroupDelegate SCNGD = AddCreatedNomenclatureToCollection;
            CreateNemenclatureUC = new CreateNomenclatureGroup(CurrentCryptConnectionString, true, CCNGD, SCNGD, CurrentGroupID);
        }

        private void Nom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SearchedText = "";
        }
    }
}
