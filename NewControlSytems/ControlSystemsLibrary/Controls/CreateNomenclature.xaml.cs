﻿using ControlSystemsLibrary.Classes;
using ControlSystemsLibrary.Controls.AdminTabItemContents;
using ControlSystemsLibrary.Services;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlSystemsLibrary.Controls
{
    public partial class CreateNomenclature : UserControl, INotifyPropertyChanged
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


        bool CreateMode; // Режим (Создать/Изменить)
        CloseCreateNomenclatureDelegate CCND; // Делегат
        public string CurrentCryptConnectionString; // Строка подключения (шифровано)

        // Конструктор (1-перегрузка (Создание))
        public CreateNomenclature(string CurrentCryptConnectionString, bool CreateMode, CloseCreateNomenclatureDelegate CCND, Guid CurrentGroupID)
        {
            this.CurrentCryptConnectionString = CurrentCryptConnectionString;
            this.CreateMode = CreateMode;
            this.CCND = CCND;
            this.CurrentGroupID = CurrentGroupID;

            InitializeComponent();
        }
        // Конструктор (2-перегрузка (Редактирование))
        public CreateNomenclature(string CurrentCryptConnectionString, bool CreateMode, CloseCreateNomenclatureDelegate CCND, NomenclatureClass EditableNomenclature )
        {
            this.CurrentCryptConnectionString = CurrentCryptConnectionString;
            this.CreateMode = CreateMode;
            this.CCND = CCND;
            this.EditableNomenclature = EditableNomenclature;

            InitializeComponent();
        }

        #region Поля и свойства ==============================================================================================================

        // Редактируемая номенлатура
        NomenclatureClass EditableNomenclature { get; set; }

        // ID Создаваемой/редактируемой номенклатуры
        private Guid id;
        public Guid ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        // ID группы Создаваемой/редактируемой номенклатуры
        private Guid groupID;
        public Guid CurrentGroupID
        {
            get
            {
                return groupID;
            }
            set
            {
                groupID = value;
                OnPropertyChanged();
            }
        }

        // Артикул Создаваемой/редактируемой номенклатуры
        private string article = "";
        public string Article
        {
            get => article;
            set
            {
                article = value;
                OnPropertyChanged();
            }
        }

        // Название Создаваемой/редактируемой номенклатуры
        private string nomenclatureName = "";
        public string NomenclatureName
        {
            get => nomenclatureName;
            set
            {
                nomenclatureName = value;
                OnPropertyChanged();
            }
        }

        // Базовая единица Создаваемой/редактируемой номенклатуры
        private string baseUnitName = "";
        public string BaseUnitName
        {
            get => baseUnitName;
            set
            {
                baseUnitName = value;
                OnPropertyChanged();
            }
        }

        // Вес базовой единицы Создаваемой/редактируемой номенклатуры
        private double baseUnitWeight = 0;
        public double BaseUnitWeight
        {
            get
            {
                return baseUnitWeight;
            }
            set
            {
                baseUnitWeight = value;
                OnPropertyChanged();
            }
        }

        // Вес базовой единицы Создаваемой/редактируемой номенклатуры для отображения в TextBox-е 
        private string baseUnitWeightText = "";
        public string BaseUnitWeightText
        {
            get
            {
                return baseUnitWeightText;
            }
            set
            {
                baseUnitWeightText = value;
                OnPropertyChanged();
            }
        }

        // Страна происхождения Создаваемой/редактируемой номенклатуры
        private string countryOfOrigin;
        public string CountryOfOrigin
        {
            get
            {
                return countryOfOrigin;
            }
            set
            {
                countryOfOrigin = value;
                OnPropertyChanged();
            }
        }

        // Описание Создаваемой/редактируемой номенклатуры
        private string description = "";
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }








        // Enabled кнопки "Создать/Принять изменения"
        private bool readiness;
        public bool Readiness
        {
            get
            {
                return readiness;
            }
            set
            {
                readiness = value;
                OnPropertyChanged();
            }
        }

        // Текст кнопки "Создать/Принять изменения"
        private string createButtonText;
        public string CreateButtonText
        {
            get
            {
                return createButtonText;
            }
            set
            {
                createButtonText = value;
                OnPropertyChanged();
            }
        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


        #region События ======================================================================================================================

        private void CN_Loaded(object sender, RoutedEventArgs e)
        {
            StartMethod();
        }

        private void StartMethod()
        {
            CreateNomenclatureTabControl.SelectedIndex = 0;
            LoadUnits();
            LoadCountry();

            if (CreateMode == true)
            {
                CreatedModeMethod();
            }
            else
            {
                EditModeMethod();
            }
        }

        async void CreatedModeMethod()
        {
            ArticleTextBox.Focus();
            CreateButtonText = "Создать";

        }
        async void EditModeMethod()
        {
            CreateButtonText = "Принять изменения";
        }


        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            CCND();
        }


        // Событие: GotFocus (Для отображения веса в формате "10.5кг" по потере элементом фокуса) --------------------------------------------
        private void TextBoxBaseWeight_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox box = sender as TextBox;
                if ((box.Name == "TextBoxBaseWeight") && (BaseUnitWeight > 0.0))
                {
                    box.Text = BaseUnitWeight.ToString();
                }
            }
        }

        // Событие: GotFocus (Для отображения веса в формате "10.5" по получении элементом фокуса) -------------------------------------------
        private void TextBoxBaseWeight_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox box = sender as TextBox;
                if (box.Name == "TextBoxBaseWeight")
                {
                    double num;
                    if (double.TryParse(box.Text, out num))
                    {
                        double num2 = Math.Round(Convert.ToDouble(box.Text), 3);

                        if (num2 > 0.0)
                        {
                            BaseUnitWeight = num2;
                            BaseUnitWeightText = BaseUnitWeight.ToString() + " кг";
                        }
                        else
                        {
                            BaseUnitWeight = 0.0;
                            box.Text = string.Empty;
                        }
                    }
                    else
                    {
                        BaseUnitWeight = 0.0;
                    }
                }
            }
        }

        // Событие: PreviewTextInput (Для введенияв TextBox только цифр) ---------------------------------------------------------------------
        private void DigitText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            double num;
            string text = e.Text;
            bool flag = false;
            foreach (char ch in ((TextBox)sender).Text)
            {
                if (ch == ',')
                {
                    flag = true;
                    break;
                }
            }
            if ((((TextBox)sender).Text.Length == 0) && (text == "0"))
            {
                ((TextBox)sender).Text = "0,";
                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                ((TextBox)sender).SelectionLength = 0;
                flag = true;
                e.Handled = true;
            }
            if (((((TextBox)sender).Text.Length == 0) && (text == ",")) || (text == "."))
            {
                ((TextBox)sender).Text = "0,";
                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                ((TextBox)sender).SelectionLength = 0;
                flag = true;
                e.Handled = true;
            }
            else if ((text == ",") && !flag)
            {
                TextBox box1 = (TextBox)sender;
                box1.Text = box1.Text + ",";
                text = ",";
                flag = true;
                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                ((TextBox)sender).SelectionLength = 0;
                e.Handled = true;
            }
            if (((text == ".") || (text == ",")) & flag)
            {
                e.Handled = true;
            }
            if (!double.TryParse(text, out num) && !(text == "."))
            {
                e.Handled = true;
            }
        }

        // Событие: KeyDown (Для перехода на следующий элемент по нажатию Enter) -------------------------------------------------------------
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender is TextBox) && (e.Key == Key.Enter))
            {
                TextBox box = sender as TextBox;
                string name = box.Name;
                if (!(name == "ArticleTextBox"))
                {
                    if (name == "NomenNameTextBox")
                    {
                        BaseUnitComboBox.Focus();
                        BaseUnitComboBox.IsDropDownOpen = true;
                    }
                    else if (name == "TextBoxBaseWeight")
                    {
                        ComboBoxCountry.Focus();
                        ComboBoxCountry.IsDropDownOpen = true;
                    }
                }
                else
                {
                    NomenNameTextBox.Focus();
                    NomenNameTextBox.SelectionStart = 0;
                    NomenNameTextBox.SelectionLength = NomenNameTextBox.Text.Length;
                }
            }
        }

        // Событие: TextChanged (Для проверки заполнения основных важных значений для создания номенклатуры) ---------------------------------
        private void TextBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckMainValues();
        }

        // Событие: SelectionChanged (Для ComboBox Базовая единица измерения) ----------------------------------------------------------------
        private void BaseUnitNameComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                string unitName = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();

                if (CheckWithAdditionalUnits(unitName))
                {
                    BaseUnitName = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();

                    TextBoxBaseWeight.Focus();
                    TextBoxBaseWeight.SelectionStart = 0;
                    TextBoxBaseWeight.SelectionLength = TextBoxBaseWeight.Text.Length;
                }
                else
                {
                    string message = "Единица " + '"' + unitName + '"' + " указано в списке дополнительных единиц.\nУдалить единицу " + '"' + unitName + '"' + " из списка дополнительных единиц?";

                    if (MessageBox.Show(message, "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        // Если нажато "Нет"
                        foreach (ComboBoxItem item in (sender as ComboBox).Items)
                        {
                            if (item.Content.ToString() == BaseUnitName)
                            {
                                (sender as ComboBox).SelectedItem = item;
                            }
                        }
                    }
                    else
                    {
                        // Если нажато "Да"
                        BaseUnitName = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
                        RemoveAddedUnit(unitName);
                    }
                }
            }
        }

        // Событие: SelectionChanged (Для ComboBox Страна происхождения) ---------------------------------------------------------------------
        private void ComboBoxCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                CountryOfOrigin = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
                DescriptionTextBox.Focus();
            }
        }





        // Событие: Click (Для добавления новой Доп. Единицы измерения) ----------------------------------------------------------------------
        private void AddAddUnitButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.CheckReadinessAddUnits())
            {
                AdditionalUnitsUC AUUC = new AdditionalUnitsUC
                {
                    ID = Guid.NewGuid(),
                    NomenclatureID = ID,
                    BaseUnitName = BaseUnitName,
                    BaseUnitWeight = BaseUnitWeight
                };
                AUUC.UnitsComboBox.SelectionChanged += AdditionalUnitsComboBox_SelectionChanged;
                AUUC.DeleteButton.Click += AdditionalUnitsDeleteButton_Click;

                LoadAddUnitsToComboBox(ref AUUC);

                AddUnitsStackPanel.Children.Add(AUUC);
                (AddUnitsStackPanel.Parent as ScrollViewer).ScrollToEnd();
            }
        }

        // Событие: SelectionChanged (Для ComboBox Доп. единица измерения) -------------------------------------------------------------------
        private void AdditionalUnitsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox box = sender as ComboBox;
                AdditionalUnitsUC AUUC = ((box.Parent as Grid).Parent as Border).Parent as AdditionalUnitsUC;
                if (box.SelectedItem != null)
                {
                    string SelectedUnitName = (box.SelectedItem as ComboBoxItem).Content.ToString();
                    if (CompareToBaseUnit(SelectedUnitName))
                    {
                        if (CheckWithAdditionalUnits(AUUC.ID, SelectedUnitName))
                        {
                            AUUC.AddUnitName = SelectedUnitName;
                            if (CreateMode)
                            {
                                AUUC.QuantityTextBox.Focus();
                                AUUC.QuantityTextBox.SelectionStart = 0;
                                AUUC.QuantityTextBox.SelectionLength = AUUC.QuantityTextBox.Text.Length;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Единица \"" + SelectedUnitName + "\" уже имеется в списке.", "Исправьте!");
                            box.SelectedItem = null;
                            box.IsDropDownOpen = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Единица \"" + SelectedUnitName + "\" установлена как базовая единица.", "Исправьте!");
                        AUUC.Readiness = false;
                        box.SelectedItem = null;
                        box.IsDropDownOpen = true;
                    }
                }
            }
        }

        // Событие: Click (Для Удаления Доп. единицы измерения) ------------------------------------------------------------------------------
        private void AdditionalUnitsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AdditionalUnitsUC parent = (((sender as Button).Parent as Grid).Parent as Border).Parent as AdditionalUnitsUC;
            AddUnitsStackPanel.Children.Remove(parent);
            //CheckBarcodesAfterChangeUnits();
        }

        // Метод: Загружает Единицы измерения в ComboBox элемента "Доп. Един." из базы данных ------------------------------------------------
        private void LoadAddUnitsToComboBox(ref AdditionalUnitsUC AUUC)
        {
            foreach (string str in DataBaseRequest.GetUnits(CurrentCryptConnectionString))
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Content = str,
                    Height = 25.0,
                    Padding = new Thickness(10.0, 1.0, 1.0, 1.0),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B5BA6"))
                };
                AUUC.UnitsComboBox.Items.Add(newItem);
            }
        }


        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


        #region Методы =======================================================================================================================

        // Метод: Загружает Единицы измерения в ComboBox Базовой единицы из базы данных ------------------------------------------------------
        async void LoadUnits()
        {
            ArrayList units = new ArrayList();

            await Task.Run(() => 
            { 
                units = DataBaseRequest.GetUnits(CurrentCryptConnectionString);
            });

            BaseUnitComboBox.Items.Clear();
            foreach (object obj2 in units)
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Height = 25.0,
                    Foreground = GetColor.Get("Blue-004"),
                    Padding = new Thickness(10.0, 0.0, 0.0, 0.0)
                };
                newItem.Content = obj2.ToString();
                BaseUnitComboBox.Items.Add(newItem);
            }
        }

        // Метод: Загружает в CommboBox Страны из базы данных --------------------------------------------------------------------------------
        async void LoadCountry()
        {
            ArrayList country = new ArrayList();

            await Task.Run(() =>
            {
                country = DataBaseRequest.GetCountry(CurrentCryptConnectionString);
            });

            ComboBoxCountry.Items.Clear();
            foreach (object obj2 in country)
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Height = 25.0,
                    Foreground = GetColor.Get("Blue-004"),
                    Padding = new Thickness(10.0, 0.0, 0.0, 0.0)
                };
                newItem.Content = obj2.ToString();
                ComboBoxCountry.Items.Add(newItem);
            }
        }


        // Метод: Проверяет указаны-ли важные значения для создания номенклатуры -------------------------------------------------------------
        private void CheckMainValues()
        {
            if (BaseUnitWeightText != "" && NomenclatureName != "" && BaseUnitName != "")
            {
                Readiness = true;
            }
            else
            {
                Readiness = false;
            }
        }








        // Метод: Проверяет "Готовность" созданных Доп. Единиц измерения ---------------------------------------------------------------------
        private bool CheckReadinessAddUnits()
        {
            bool flag = true;
            if (AddUnitsStackPanel.Children.Count == 0)
            {
                return true;
            }
            foreach (AdditionalUnitsUC AUUC in AddUnitsStackPanel.Children)
            {
                if (!AUUC.Readiness)
                {
                    flag = false;
                }
            }
            return flag;
        }

        // Метод: Сверяет с базовой единицей -------------------------------------------------------------------------------------------------
        private bool CompareToBaseUnit(string UnitName)
        {
            if (BaseUnitName == UnitName)
            {
                return false;
            }
            return true;
        }

        // Метод: Проверяет на наличие такой единицы измерения в Доп. Единицах (2-перегрузка) ------------------------------------------------
        private bool CheckWithAdditionalUnits(Guid ID, string UnitName)
        {
            bool readiness = true;
            foreach (AdditionalUnitsUC AUUC in AddUnitsStackPanel.Children)
            {
                AUUC.BaseUnitName = BaseUnitName;
                if ((AUUC.ID != ID) && (AUUC.AddUnitName == UnitName))
                {
                    AUUC.Readiness = false;
                    readiness = AUUC.Readiness;
                    AUUC.Readiness = true;
                }
            }
            return readiness;
        }

        // Метод: Проверяет на наличие такой единицы измерения в Доп. Единицах (1-перегрузка) ------------------------------------------------
        private bool CheckWithAdditionalUnits(string UnitName)
        {
            bool flag = true;
            foreach (AdditionalUnitsUC AUUC in AddUnitsStackPanel.Children)
            {
                AUUC.BaseUnitName = UnitName;
                if (AUUC.AddUnitName == UnitName)
                {
                    AUUC.Readiness = false;
                    flag = AUUC.Readiness;
                    AUUC.Readiness = true;
                }
            }
            return flag;
        }

        // Метод: Удаляет элемент Доп. единицы из списка, по названию ------------------------------------------------------------------------
        private void RemoveAddedUnit(string UnitName)
        {
            foreach (AdditionalUnitsUC suc in AddUnitsStackPanel.Children)
            {
                if (suc.AddUnitName == UnitName)
                {
                    AddUnitsStackPanel.Children.Remove(suc);
                    break;
                }
            }
        }
        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


    }
}
