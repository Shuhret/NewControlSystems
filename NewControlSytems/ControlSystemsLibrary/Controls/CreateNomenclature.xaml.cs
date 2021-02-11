using ControlSystemsLibrary.Classes;
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
using System.Linq;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

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

        // Категория Создаваемой/редактируемой номенклатуры
        private string category = "";
        public string Category
        {
            get => category;
            set
            {
                category = value;
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
                CheckAddedUnitsWeight();
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

        private bool addBarcodeButtonEnable = false;
        public bool AddBarcodeButtonEnable
        {
            get => addBarcodeButtonEnable;
            set
            {
                addBarcodeButtonEnable = value;
                OnPropertyChanged();
            }
        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


        #region События ======================================================================================================================

        private void CN_Loaded(object sender, RoutedEventArgs e)
        {
            StartMethod();
        }

        // Событие: Клик кнопки "СОЗДАТЬ"
        private void CreateNomenclatureButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveReadinessFalseAddedUnits();
            RemoveReadinessFalseProperties();
            RemoveReadinessFalseBarcodes();
        }


        private void StartMethod()
        {
            CreateNomenclatureTabControl.SelectedIndex = 0;
            LoadCategories();
            LoadUnits();
            LoadCountry();
            LoadUnitsForBarcodes();
            LoadBarcodeTypes();
            NomenNameTextBox.Focus();
            if (CreateMode == true)
            {
                CreatedModeMethod();
            }
            else
            {
                EditModeMethod();
            }
        }

        private void CreatedModeMethod()
        {
            CreateButtonText = "Создать";
        }

        private void EditModeMethod()
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
                            //string.Format("{0:0.000}", 25.657446842);

                            BaseUnitWeightText = string.Format("{0:0.000}", BaseUnitWeight) + " кг";
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
                switch (box.Name)
                {
                    case "NomenNameTextBox":
                        {
                            CategoriesComboBox.Focus();
                            CategoriesComboBox.IsDropDownOpen = true;
                        }
                        break;
                    case "ArticleTextBox":
                        {
                            BaseUnitComboBox.Focus();
                            BaseUnitComboBox.IsDropDownOpen = true;
                        }
                        break;
                    case "TextBoxBaseWeight":
                        {
                            ComboBoxCountry.Focus();
                            ComboBoxCountry.IsDropDownOpen = true;
                        }
                        break;
                    case "BarcodeTextBox":
                        {
                            if ((sender as TextBox).Text != "" && BarcodeTextBox.Text != null && BarcodeTextBox.Text != string.Empty && AddBarcodeButtonEnable == true)
                            {
                                AddBarcodeButton.Focus();
                            }
                        }
                        break;
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
                CheckBarcodesReadiness();
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
            if (CheckReadinessAddUnits())
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
                AUUC.ReadinessChanged += AUUC_ReadinessChanged;
                LoadAddUnitsToComboBox(ref AUUC);

                AddUnitsStackPanel.Children.Add(AUUC);
                (AddUnitsStackPanel.Parent as ScrollViewer).ScrollToEnd();
            }
        }

        private void AUUC_ReadinessChanged(object sender, EventArgs e)
        {
            CheckBarcodesReadiness();
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
                            AUUC.Readiness = false;
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

        // Метод: Проверяет "Готовность" созданных Доп. Единиц измерения ?????????????????????????????????????????????---------
        private bool CheckReadinessAddUnits()
        {
            if (AddUnitsStackPanel.Children.Count == 0)
            {
                return true;
            }
            int count = 0;
            foreach (AdditionalUnitsUC AUUC in AddUnitsStackPanel.Children)
            {
                if (!AUUC.Readiness)
                {
                    count++;
                    AUUC.ReadinessFalseAnimationBegin();
                }
            }
            if(count > 0)
                return false;
            else
                return true;
        }

        // Метод: Проверяет на наличие такой единицы измерения в Доп. Единицах ?????????????????????????????????????????????????????----
        private bool CheckWithAdditionalUnits(Guid ID, string UnitName)
        {
            int count = 0;
            foreach (AdditionalUnitsUC AUUC in AddUnitsStackPanel.Children)
            {
                AUUC.BaseUnitName = BaseUnitName;
                if ((AUUC.ID != ID) && (AUUC.AddUnitName == UnitName))
                {
                    AUUC.ReadinessFalseAnimationBegin();
                    count++;
                }
            }
            if (count > 0)
                return false;
            else
                return true;
        }













        // Событие: Click (Для Удаления Доп. единицы измерения) ------------------------------------------------------------------------------
        private void AdditionalUnitsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AdditionalUnitsUC parent = (((sender as Button).Parent as Grid).Parent as Border).Parent as AdditionalUnitsUC;
            AddUnitsStackPanel.Children.Remove(parent);
            CheckBarcodesReadiness();
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

        // Двойной клик по StackPanel для добавления Доп. единицы ----------------------------------------------------------------------------
        private void AddUnitsStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                AddAddUnitButton_Click(null, null);
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

        // Метод: Загружает Единицы измерения в ComboBox Базовой единицы из базы данных ------------------------------------------------------
        async void LoadUnitsForBarcodes()
        {
            ArrayList units = new ArrayList();
            await Task.Run(() =>
            {
                units = DataBaseRequest.GetUnits(CurrentCryptConnectionString);
            });

            BarcodeUnitsComboBox.Items.Clear();
            foreach (object obj2 in units)
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Height = 25.0,
                    Foreground = GetColor.Get("Blue-004"),
                    Padding = new Thickness(10.0, 0.0, 0.0, 0.0)
                };
                newItem.Content = obj2.ToString();
                BarcodeUnitsComboBox.Items.Add(newItem);
            }
        }

        // Метод: Загружает Типы штрих-кодов в ComboBox из базы данных -----------------------------------------------------------------------
        async void LoadBarcodeTypes()
        {
            ArrayList barcodeTypes = new ArrayList();
            await Task.Run(() =>
            {
                barcodeTypes = DataBaseRequest.GetBarcodeTypes(CurrentCryptConnectionString);

            });
            BarcodeTypesComboBox.Items.Clear();
            foreach (object obj2 in barcodeTypes)
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Content = obj2.ToString(),
                    Height = 25.0,
                    Padding = new Thickness(10.0, 1.0, 1.0, 1.0),
                    Foreground = GetColor.Get("Blue-004"),
                };
                BarcodeTypesComboBox.Items.Add(newItem);
            }

        }

        // Метод: Загружает Категории в ComboBox из базы данных ------------------------------------------------------
        async void LoadCategories()
        {
            ArrayList units = new ArrayList();

            await Task.Run(() =>
            {
                units = DataBaseRequest.GetAllNomenclatureCategories(CurrentCryptConnectionString);
            });

            CategoriesComboBox.Items.Clear();
            foreach (object obj2 in units)
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Height = 25.0,
                    Foreground = GetColor.Get("Blue-004"),
                    Padding = new Thickness(10.0, 0.0, 0.0, 0.0)
                };
                newItem.Content = obj2.ToString();
                CategoriesComboBox.Items.Add(newItem);
            }
        }



        // Метод: Проверяет указаны-ли важные значения для создания номенклатуры -------------------------------------------------------------
        private void CheckMainValues()
        {
            if (BaseUnitWeightText != "" && NomenclatureName != "" && BaseUnitName != "" && CategoriesComboBox.SelectedItem != null)
            {
                Readiness = true;
            }
            else
            {
                Readiness = false;
            }
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



        // Метод: Проверяет "Готовность" созданных свойств и значений ------------------------------------------------------------------------
        private bool CheckReadinessProperties()
        {
            bool readiness = true;
            if (AddPropertiesStackPanel.Children.Count == 0)
            {
                return readiness;
            }
            foreach (NomenPropertyUC NPUC in AddPropertiesStackPanel.Children)
            {
                if (!NPUC.Readiness)
                {
                    NPUC.ReadinessFalseAnimationBegin();
                    readiness = false;
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

        private void CheckAddedUnitsWeight()
        {
            if(AddUnitsStackPanel.Children.Count > 0)
            {
                foreach(AdditionalUnitsUC AUUC in AddUnitsStackPanel.Children)
                {
                    AUUC.BaseUnitWeight = BaseUnitWeight;
                    AUUC.Quantity = AUUC.Quantity;
                }
            }
        }



        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        // Событие: Click (Для добавления нового свойства) -----------------------------------------------------------------------------------
        private void AddPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckReadinessProperties())
            {
                NomenPropertyUC NPUC = new NomenPropertyUC();
                NPUC.PropertyComboBox.SelectionChanged += PropertyComboBox_SelectionChanged;
                NPUC.ValueComboBox.SelectionChanged += ValueComboBox_SelectionChanged;
                LoadPropertiesInComboBox(ref NPUC.PropertyComboBox);

                NPUC.PropertyComboBox.IsDropDownOpen = true;
                AddPropertiesStackPanel.Children.Add(NPUC);
                (AddPropertiesStackPanel.Parent as ScrollViewer).ScrollToEnd();
            }
        }



        // Метод: Загружает Свойства в ComboBox из базы данных -------------------------------------------------------------------------------
        private void LoadPropertiesInComboBox(ref ComboBox CBX)
        {
            ArrayList allNomenProperties = DataBaseRequest.GetAllNomenProperties(CurrentCryptConnectionString);
            CBX.Items.Clear();
            foreach (object obj2 in allNomenProperties)
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Content = obj2.ToString(),
                    Height = 25.0,
                    Padding = new Thickness(10.0, 1.0, 1.0, 1.0),
                    Foreground = GetColor.Get("Blue-004")
                };
                CBX.Items.Add(newItem);
            }
        }

        // Двойной клик по StackPanel для добавления свойства и значения
        private void AddPropertiesStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                AddPropertyButton_Click(null, null);
            }
        }

        // Событие: SelectionChanged (Для ComboBox Свойство ) --------------------------------------------------------------------------------
        private void PropertyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox propertyComboBox = sender as ComboBox;
                ComboBox valueComboBox = (((propertyComboBox.Parent as Grid).Parent as Border).Parent as NomenPropertyUC).ValueComboBox;
                NomenPropertyUC NPUC = ((propertyComboBox.Parent as Grid).Parent as Border).Parent as NomenPropertyUC;

                LoadValuesInComboBox(ref valueComboBox, (propertyComboBox.SelectedItem as ComboBoxItem).Content.ToString());
                if (CreateMode)
                {
                    SetPropertyesReadiness();
                    valueComboBox.Focus();
                    valueComboBox.IsDropDownOpen = true;
                }
            }
        }

        // Событие: SelectionChanged (Для ComboBox Значение ) --------------------------------------------------------------------------------
        private void ValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox box = sender as ComboBox;
                NomenPropertyUC NPUC = ((box.Parent as Grid).Parent as Border).Parent as NomenPropertyUC;

                if (box.SelectedItem != null)
                {
                    if(!CheckAlreadyCreated(NPUC))
                    {
                        NPUC.Readiness = false;
                        MessageBox.Show("Свойство " + '"' + (NPUC.PropertyComboBox.SelectedItem as ComboBoxItem).Content.ToString() + '"' + " со значением " + '"' + (NPUC.ValueComboBox.SelectedItem as ComboBoxItem).Content.ToString() + '"' + " уже имеется в списке!", "Внимание!");
                        box.SelectedItem = null;
                        box.IsDropDownOpen = true;
                    }
                    else
                    {
                        SetPropertyesReadiness();
                    }
                }
            }
        }

        // Метод: Проверяет на повторение ранее созданных свойств и значений -----------------------------------------------------------------
        private bool CheckAlreadyCreated(NomenPropertyUC CheckedNPUC)
        {
            bool readiness = true;
            string property = (CheckedNPUC.PropertyComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            string value = (CheckedNPUC.ValueComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            foreach (NomenPropertyUC NPUC in AddPropertiesStackPanel.Children)
            {
                if (NPUC.ID != CheckedNPUC.ID && NPUC.Readiness == true)
                {
                    if (NPUC.PropertyComboBox.SelectedItem != null && NPUC.ValueComboBox.SelectedItem != null)
                    {
                        if ((NPUC.PropertyComboBox.SelectedItem as ComboBoxItem).Content.ToString() == property && (NPUC.ValueComboBox.SelectedItem as ComboBoxItem).Content.ToString() == value)
                        {
                            NPUC.ReadinessFalseAnimationBegin();
                            readiness = false;
                        }
                    }
                    else
                    {
                        NPUC.ReadinessFalseAnimationBegin();
                        readiness = false;
                    }
                }
            }
            return readiness;
        }

        private void SetPropertyesReadiness()
        {
            foreach (NomenPropertyUC NPUC in AddPropertiesStackPanel.Children)
            {
                if (NPUC.PropertyComboBox.SelectedItem != null && NPUC.ValueComboBox.SelectedItem != null)
                {
                    NPUC.Readiness = true;
                }
                else
                {
                    NPUC.Readiness = false;
                }
            }
        }


        // Метод: Загружает Значения в ComboBox элемента "Свойства и значения" из базы данных ------------------------------------------------
        private void LoadValuesInComboBox(ref ComboBox CBX, string Property)
        {
            ArrayList allNomenPropertyValues = DataBaseRequest.GetAllNomenPropertyValues(CurrentCryptConnectionString, Property);
            CBX.Items.Clear();
            foreach (object obj2 in allNomenPropertyValues)
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Content = obj2.ToString(),
                    Height = 25.0,
                    Padding = new Thickness(10.0, 1.0, 1.0, 1.0),
                    Foreground = GetColor.Get("Blue-004")
                };
                CBX.Items.Add(newItem);
            }
        }

        private void CN_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                CCND();
            }
        }


        // Метод: Удаляет не готовые дополнительные единицы
        private void RemoveReadinessFalseAddedUnits()
        {
            if (AddUnitsStackPanel.Children.Count > 0)
            {
                var children = AddUnitsStackPanel.Children.OfType<UIElement>().ToList();
                foreach (AdditionalUnitsUC AUUC in children)
                {
                    if (AUUC.Readiness == false)
                    {
                        AddUnitsStackPanel.Children.Remove(AUUC);
                    }
                }
            }
        }

        // Метод: Удаляет не готовые свойства и значения
        private void RemoveReadinessFalseProperties()
        {
            if (AddPropertiesStackPanel.Children.Count > 0)
            {
                var children = AddPropertiesStackPanel.Children.OfType<UIElement>().ToList();
                foreach (NomenPropertyUC NPUC in children)
                {
                    if (NPUC.Readiness == false)
                    {
                        AddPropertiesStackPanel.Children.Remove(NPUC);
                        
                    }
                }
            }
        }

        // Событие: SelectionChanged (Для ComboBox Страна происхождения)
        private void CategoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                Category = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
                CheckMainValues();
                ArticleTextBox.Focus();
            }
        }







        private void AddBarcodeButtonClick(object sender, RoutedEventArgs e)
        {
            AddBarcode();
        }

        private void BarcodeUnitComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckCreatedBarcodeValues();
            if ((sender as ComboBox).SelectedItem != null)
            {
                BarcodeTypesComboBox.Focus();
                BarcodeTypesComboBox.IsDropDownOpen = true;
            }
        }

        private void BarcodeTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckCreatedBarcodeValues();
            if ((sender as ComboBox).SelectedItem != null)
            {
                BarcodeTextBox.Focus();
            }
        }

        private void BarcodeTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckCreatedBarcodeValues();
           
        }

        private void CheckCreatedBarcodeValues()
        {
            if(BarcodeUnitsComboBox.SelectedItem != null && BarcodeTypesComboBox.SelectedItem != null && BarcodeTextBox.Text != "" && BarcodeTextBox.Text != null && BarcodeTextBox.Text != string.Empty)
            {
                AddBarcodeButtonEnable = true;
            }
            else
            {
                AddBarcodeButtonEnable = false;
            }
        }
        private void AddBarcode()
        {
            if (CheckCreatedBarcode() == true)
            {
                BarcodesWrapPanel.Children.Add(GetBarcode());
                BarcodeUnitsComboBox.SelectedItem = null;
                BarcodeTypesComboBox.SelectedItem = null;
                BarcodeTextBox.Text = "";
                AddBarcodeButtonEnable = false;
                CheckBarcodesReadiness();
            }
        }

        
        private bool CheckCreatedBarcode()
        {
            BarcodeUC BUC = new BarcodeUC();
            return BarCodes.CheckCreatedBarcode((BarcodeTypesComboBox.SelectedItem as ComboBoxItem).Content.ToString(), BarcodeTextBox.Text);
        }
        private BarcodeUC GetBarcode()
        {
            BarcodeUC BUC = new BarcodeUC()
            {
                UnitName = (BarcodeUnitsComboBox.SelectedItem as ComboBoxItem).Content.ToString(),
                BarcodeType = (BarcodeTypesComboBox.SelectedItem as ComboBoxItem).Content.ToString(),
                Barcode = BarcodeTextBox.Text,
                BacodeImage = BarCodes.GetBarcodeBitmapSource((BarcodeTypesComboBox.SelectedItem as ComboBoxItem).Content.ToString(), BarcodeTextBox.Text)
            };
            return BUC;
        }

        private void CheckBarcodesReadiness()
        {
            if (BarcodesWrapPanel.Children.Count > 0)
            {
                var children = BarcodesWrapPanel.Children.OfType<UIElement>().ToList();
                foreach (BarcodeUC BUC in children)
                {
                    BUC.Readiness = false;
                    if(BUC.UnitName == BaseUnitName)
                    {
                        BUC.Readiness = true;
                    }
                    else
                    {
                        foreach(AdditionalUnitsUC AUUC in AddUnitsStackPanel.Children)
                        {
                            if (AUUC.Readiness == true)
                            {
                                if (BUC.UnitName == (AUUC.UnitsComboBox.SelectedItem as ComboBoxItem).Content.ToString())
                                {
                                    BUC.Readiness = true;
                                }
                            }
                        }
                    }
                }
            }
        }


        // Метод: Удаляет не готовые штрих-коды
        private void RemoveReadinessFalseBarcodes()
        {
            if (BarcodesWrapPanel.Children.Count > 0)
            {
                var children = BarcodesWrapPanel.Children.OfType<UIElement>().ToList();
                foreach (BarcodeUC BUC in children)
                {
                    if (BUC.Readiness == false)
                    {
                        BarcodesWrapPanel.Children.Remove(BUC);
                    }
                }
            }
        }

        private void AddImageButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files(*.jpg, *.png) | *.jpg; *.png";

                if (openFileDialog.ShowDialog() == true)
                {
                    AddImageControl(openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nПопробуйте загрузить другое изображение.", "Ошибка загрузки изображения");
            }
        }


        // Метод: Добавляет Изображение в WrapPanel из полученной ссылки на файл -------------------------------------------------------------
        private void AddImageControl(string ImagePath)
        {
            NomenclatureImageUC NIUC = new NomenclatureImageUC();
            NIUC.ImagePath = ImagePath;
            NIUC.Deleted += ImageDeleteClick;
            if (CreatedNomenclatureImageWrapPanel.Children.Count == 0)
                NIUC.IsChecked = true;
            CreatedNomenclatureImageWrapPanel.Children.Add(NIUC);
            (CreatedNomenclatureImageWrapPanel.Parent as ScrollViewer).ScrollToEnd();
        }

        private void ImageDeleteClick(object sender, EventArgs e)
        {
            bool selectedDelete = false;
            if (CreatedNomenclatureImageWrapPanel.Children.Count > 0)
            {
                var children = CreatedNomenclatureImageWrapPanel.Children.OfType<UIElement>().ToList();
                foreach (NomenclatureImageUC NIUC in children)
                {

                    if(NIUC.IsChecked == true)
                    {
                        if(CreatedNomenclatureImageWrapPanel.Children.Count > 1)
                        {
                            selectedDelete = true;
                        }
                    }
                    if (NIUC == (sender as NomenclatureImageUC))
                    {
                        CreatedNomenclatureImageWrapPanel.Children.Remove(NIUC);
                        if(selectedDelete == true)
                        {
                            (CreatedNomenclatureImageWrapPanel.Children[0] as NomenclatureImageUC).IsChecked = true;
                        }
                    }
                }
            }
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                AddBarcode();
            }
        }
    }
}
