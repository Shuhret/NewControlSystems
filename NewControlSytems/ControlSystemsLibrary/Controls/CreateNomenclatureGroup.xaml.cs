using ControlSystemsLibrary.Classes;
using ControlSystemsLibrary.Controls.AdminTabItemContents;
using ControlSystemsLibrary.Services;
using System;
using System.Collections.Generic;
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

namespace ControlSystemsLibrary.Controls
{

    public partial class CreateNomenclatureGroup : UserControl, INotifyPropertyChanged
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
        public string CurrentCryptConnectionString; // Строка подключения (шифровано)
        public Guid CurrentGroupID; // ID группы
        NomenclatureClass EditableNomenclatureGroup;
        CloseCreateNomenclatureGroupDelegate CCNGD;
        ShowCreatedNomenclatureGroupDelegate SCNGD;
        public CreateNomenclatureGroup(string CurrentCryptConnectionString, bool CreateMode, CloseCreateNomenclatureGroupDelegate CCNGD, ShowCreatedNomenclatureGroupDelegate SCNGD, Guid CurrentGroupID)
        {
            this.CurrentCryptConnectionString = CurrentCryptConnectionString;
            this.CreateMode = CreateMode;
            this.CurrentGroupID = CurrentGroupID;
            this.CCNGD = CCNGD;
            this.SCNGD = SCNGD;

            InitializeComponent();
        }
        public CreateNomenclatureGroup(string CurrentCryptConnectionString, bool CreateMode, CloseCreateNomenclatureGroupDelegate CCNGD, ShowCreatedNomenclatureGroupDelegate SCNGD, NomenclatureClass EditableNomenclatureGroup)
        {
            this.CurrentCryptConnectionString = CurrentCryptConnectionString;
            this.CreateMode = CreateMode;
            this.EditableNomenclatureGroup = EditableNomenclatureGroup;
            this.CCNGD = CCNGD;
            this.SCNGD = SCNGD;

            InitializeComponent();
            
        }

        private void CreateNomenclatureGroup_Loaded(object sender, RoutedEventArgs e)
        {
            StartMethod();
            NomenGroupNameTextBox.Focus();
        }

        private string headerText;
        public string HeaderText
        {
            get => headerText;
            set
            {
                headerText = value;
                OnPropertyChanged();
            }
        }


        private string buttonText;
        public string ButtonText
        {
            get => buttonText;
            set
            {
                buttonText = value;
                OnPropertyChanged();
            }
        }


        private bool createButtonEnabled = false;
        public bool CreateButtonEnabled
        {
            get => createButtonEnabled;
            set
            {
                createButtonEnabled = value;
                OnPropertyChanged();
            }
        }


        private Visibility notCloseCheckBoxVisibility;
        public Visibility NotCloseCheckBoxVisibility
        {
            get => notCloseCheckBoxVisibility;
            set
            {
                notCloseCheckBoxVisibility = value;
                OnPropertyChanged();
            }
        }


        private void StartMethod()
        {
            if(CreateMode == true)
            {
                NotCloseCheckBoxVisibility = Visibility.Visible;
                ButtonText = "Создать";
                HeaderText = "Создание новой группы номенклатуры";
            }
            else
            {
                NotCloseCheckBoxVisibility = Visibility.Hidden;
                CreateButtonEnabled = true;
                NomenGroupNameTextBox.Text = EditableNomenclatureGroup.Name;
                NomenGroupNameTextBox.SelectionLength = NomenGroupNameTextBox.Text.Length;
                ButtonText = "Изменить";
                HeaderText = "Изменение группы номенклатуры";
            }
        }

        private void NomenGroupNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if((sender as TextBox).Text != "" && (sender as TextBox).Text != "" && (sender as TextBox).Text != null)
            {
                CreateButtonEnabled = true;
            }
            else
            {
                CreateButtonEnabled = false;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            CCNGD();
        }

        private async void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateButtonEnabled = false;
            if (CreateMode == true)
            {
                NomenclatureClass CreatedNomenclatureGroup = new NomenclatureClass();
                if (await Task.Run(() => CreateNewNomenclatureGroupDB(CreatedNomenclatureGroup)) == true)
                {
                    SCNGD(CreatedNomenclatureGroup);
                    NomenGroupNameTextBox.Clear();
                    NomenGroupNameTextBox.Focus();
                    if (NotCloseCheckBox.IsChecked == false)
                    {
                        CCNGD();
                    }

                }
                CreateButtonEnabled = true;
            }
            else
            {
                if (await Task.Run(() => UpdateNomenclatureGroupDB()) == true)
                {
                    CCNGD();
                    SCNGD(EditableNomenclatureGroup);
                }
            }
        }
        private bool CreateNewNomenclatureGroupDB(NomenclatureClass CreatedNomenclatureGroup)
        {
            bool result = false;

            CreatedNomenclatureGroup.ID = Guid.NewGuid();
            CreatedNomenclatureGroup.GroupID = CurrentGroupID;
            Action action = () =>
            {
                CreatedNomenclatureGroup.Name = NomenGroupNameTextBox.Text;
            };
            Dispatcher.Invoke(action);
            result = DataBaseRequest.CreateNomenclatureGroup(CurrentCryptConnectionString, CreatedNomenclatureGroup);

            return result;
        }
        private bool UpdateNomenclatureGroupDB()
        {
            bool result = false;

            Action action = () =>
            {
                EditableNomenclatureGroup.Name = NomenGroupNameTextBox.Text;
            };
            Dispatcher.Invoke(action);
            result = DataBaseRequest.UpdateNomenclatureGroup(CurrentCryptConnectionString, EditableNomenclatureGroup);

            return result;
        }

        private void CNG_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if(CreateButtonEnabled == true)
                {
                    ButtonCreate_Click(null, null);
                }
            }
            if(e.Key == Key.Escape)
            {
                CCNGD();
            }
        }
    }
}
