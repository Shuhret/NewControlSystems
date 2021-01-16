using ControlSystemsLibrary.Controls;
using ControlSystemsLibrary.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
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

namespace ControlSystemsLibrary.Views
{
    
    public partial class Authorization : UserControl, INotifyPropertyChanged
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

        private ContentControl CurrentUserInterfaceParent;

        // КОНСТРУКТОР ==================================================================================================
        public Authorization(ContentControl CurrentUserInterfaceParent)
        {
            this.CurrentUserInterfaceParent = CurrentUserInterfaceParent;
            InitializeComponent();
            Loaded += Authorization_Loaded;
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
                currentCryptConnectionString = value;
                OnPropertyChanged();
            }
        }

        // Коллекция RadioButton-ов подключений -----------------------------------------------------------------------------
        private ObservableCollection<ConnectionRB> connections;
        public ObservableCollection<ConnectionRB> Connections
        {
            get
            {
                if (connections == null)
                    connections = new ObservableCollection<ConnectionRB>();
                return connections;
            }
        }


        private void Authorization_Loaded(object sender, RoutedEventArgs e)
        {
            StartMethod();
        }







        #region МЕТОДЫ =================================================================================================

        async void StartMethod()
        {
            AuthorizationEnabled = false;
            ShowMessage("Получение имени пользователя...", "Blue-003", true);
            CurrentUserName = await Task.Run(XmlClass.GetCurrentUserName);

            ShowMessage("Получение названия подключения...", "Blue-003", true);
            CurrentConnectionName = await Task.Run(XmlClass.GetSelectedConnectionName);

            ShowMessage("Загрузка зашифрованной строки подключения...", "Blue-003", true);
            CurrentCryptConnectionString = await Task.Run(XmlClass.GetSelectedConnectionString);

            if (CurrentCryptConnectionString != null)
            {
                ShowMessage("Установка соединения...", "Blue-003", true);
                if (await Task.Run(() => OpenCloseConnection(Cryption.Decrypt(CurrentCryptConnectionString))))
                {
                    ShowMessage("Соединение установлено!", "Green-003", false);
                    AuthorizationEnabled = true;
                }
                else
                {
                    ShowMessage("Не удалось установить соединение.", "Red-001", false);
                    AuthorizationEnabled = true;
                }
            }
            else
            {
                AuthorizationEnabled = true;
                CurrentConnectionTextColor = GetColor.Get("Red-001");
                ShowMessage("Создайте подключение.", "Red-001", false);
            }
        }

        // Метод: Пытается открыть и закрыть подключение с создаваемым подключением -----------------------------------------
        bool OpenCloseConnection(string ConnString)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    conn.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Метод: Загружает список подклчений -------------------------------------------------------------------------------
        async void LoadAllConnections()
        {
            ShowMessage("Загрузка списка подключений...", "Blue-003", true);

            Connections.Clear();
            ArrayList array = await Task.Run(XmlClass.ReadAllConnectionsName);
            if (array.Count > 0)
            {
                foreach (string str in array)
                {
                    ConnectionRB connectionRB = new ConnectionRB();
                    connectionRB.GroupName = "ConnGroup";
                    connectionRB.Content = str;
                    connectionRB.Deleted += ConnectionRB_Deleted;
                    connectionRB.Checked += ConnectionRB_Checked;
                    if (str == CurrentConnectionName)
                    {
                        connectionRB.IsChecked = true;
                    }
                    Connections.Add(connectionRB);
                }
            }
            else
            {
                ClickGoToCreateConnectionButton(null, null);
            }

            ShowMessage(false);
        }

        async void ConnectionRB_Checked(object sender, RoutedEventArgs e)
        {
            ShowMessage("Выбор подключения...", "Blue-003", true);

            CurrentConnectionName = (sender as ConnectionRB).Content.ToString();
            CurrentConnectionTextColor = GetColor.Get("Dark-003");


            await Task.Run(() => XmlClass.SetSelectConnection(CurrentConnectionName));

            CurrentCryptConnectionString = await Task.Run(XmlClass.GetSelectedConnectionString);


            ShowMessage("Выбрано подключение: " + '"' + CurrentConnectionName + '"', "Blue-003", false);
        }

        void ConnectionRB_Deleted(object sender, EventArgs e)
        {
            string DelConnName = (sender as ConnectionRB).Content.ToString();

            ShowMessage("Удаление подключения: " + '"' + DelConnName + '"', "Blue-003", true);

            if (XmlClass.DeleteConnection(DelConnName) == true)
            {
                foreach (ConnectionRB CRB in Connections)
                {
                    if (CRB.Content.ToString() == DelConnName)
                    {
                        bool isCheck = (bool)CRB.IsChecked;
                        Connections.Remove(CRB);

                        ShowMessage("Подключение: " + '"' + DelConnName + '"' + " удалено!", "Blue-003", false);

                        if (isCheck && Connections.Count >= 1)// если удален выбранный и есть еще
                        {
                            Connections[Connections.Count - 1].IsChecked = true;
                        }
                        if (Connections.Count == 0)
                        {
                            CurrentConnectionName = null;
                            ShowMessage("Нет созданных подключений.\nСоздайте новое подключение.", "Red-001", false);
                            CurrentConnectionTextColor = GetColor.Get("Red-001");
                        }
                        break;
                    }
                }
            }
        }


        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

















        #region АВТОРИЗАЦИЯ ============================================================================================

        private bool authorizationEnabled = true;
        public bool AuthorizationEnabled
        {
            get => authorizationEnabled;
            set
            {
                authorizationEnabled = value;
                OnPropertyChanged();
            }
        }


        private Visibility authorizationVisibility;
        public Visibility AuthorizationVisibility
        {
            get => authorizationVisibility;
            set
            {
                authorizationVisibility = value;
                OnPropertyChanged();
            }
        }


        private SolidColorBrush currentConnectionTextColor = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF4C566E"));

        public SolidColorBrush CurrentConnectionTextColor
        {
            get => currentConnectionTextColor;
            set
            {
                currentConnectionTextColor = value;
                OnPropertyChanged();
            }
        }


        private void ClickConnectionNameButton(object sender, RoutedEventArgs e)
        {
            AuthorizationVisibility = Visibility.Hidden;
            CreateConnectionVisibility = Visibility.Hidden;

            ConnectionListVisibility = Visibility.Visible;
            LoadAllConnections();
        }


        private void ClickAuthorizationButton(object sender, RoutedEventArgs e)
        {

        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


















        #region СПИСОК ПОДКЛЮЧЕНИЙ =====================================================================================


        private bool connectionListEnabled = true;
        public bool ConnectionListEnabled
        {
            get => connectionListEnabled;
            set
            {
                connectionListEnabled = value;
                OnPropertyChanged();
            }
        }


        private Visibility connectionListVisibility = Visibility.Hidden;
        public Visibility ConnectionListVisibility
        {
            get => connectionListVisibility;
            set
            {
                connectionListVisibility = value;
                OnPropertyChanged();
            }
        }


        private void ClickCheckSelectedConnectionButton(object sender, RoutedEventArgs e)
        {

        }


        private void ClickGoToCreateConnectionButton(object sender, RoutedEventArgs e)
        {
            AuthorizationVisibility = Visibility.Hidden;
            ConnectionListVisibility = Visibility.Hidden;

            CreateConnectionVisibility = Visibility.Visible;
        }


        private void ClickConnectionListCloseButton(object sender, RoutedEventArgs e)
        {
            ConnectionListVisibility = Visibility.Hidden;
            CreateConnectionVisibility = Visibility.Hidden;

            AuthorizationVisibility = Visibility.Visible;
        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



















        #region СОЗДАТЬ ПОДКЛЮЧЕНИЕ ====================================================================================

        private bool createConnectionEnabled = true;
        public bool CreateConnectionEnabled
        {
            get => createConnectionEnabled;
            set
            {
                createConnectionEnabled = value;
                OnPropertyChanged();
            }
        }


        private bool createdMode = false;
        public bool CreatedMode
        {
            get => createdMode;
            set
            {
                createdMode = value;
                if(value == true)
                {
                    CreatedModeConnectionStringVisibility = Visibility.Visible;
                    CreatedModeValuesVisibility = Visibility.Hidden;
                }
                else
                {
                    CreatedModeConnectionStringVisibility = Visibility.Hidden;
                    CreatedModeValuesVisibility = Visibility.Visible;

                }
                OnPropertyChanged();
            }
        }


        private bool checkCreatedConnectionButtonEnabled = false;
        public bool CheckCreatedConnectionButtonEnabled
        {
            get => checkCreatedConnectionButtonEnabled;
            set
            {
                checkCreatedConnectionButtonEnabled = value;
                OnPropertyChanged();
            }
        }


        private bool saveCreatedConnectionButtonEnabled = false;
        public bool SaveCreatedConnectionButtonEnabled
        {
            get => saveCreatedConnectionButtonEnabled;
            set
            {
                saveCreatedConnectionButtonEnabled = value;
                OnPropertyChanged();
            }
        }


        private Visibility createConnectionVisibility = Visibility.Hidden;
        public Visibility CreateConnectionVisibility
        {
            get => createConnectionVisibility;
            set
            {
                createConnectionVisibility = value;
                OnPropertyChanged();
            }
        }


        private Visibility createdModeConnectionStringVisibility = Visibility.Hidden;
        public Visibility CreatedModeConnectionStringVisibility
        {
            get => createdModeConnectionStringVisibility;
            set
            {
                createdModeConnectionStringVisibility = value;
                OnPropertyChanged();
            }
        }


        private Visibility createdModeValuesVisibility = Visibility.Visible;
        public Visibility CreatedModeValuesVisibility
        {
            get => createdModeValuesVisibility;
            set
            {
                createdModeValuesVisibility = value;
                OnPropertyChanged();
            }
        }


        private void ClickCheckCreatedConnectionButton(object sender, RoutedEventArgs e)
        {

        }


        private void ClickSaveCreatedConnectionButton(object sender, RoutedEventArgs e)
        {

        }


        private void ClickPasteButton(object sender, RoutedEventArgs e)
        {

        }


        private void ClickCreatedConnectionCloseButton(object sender, RoutedEventArgs e)
        {
            ConnectionListVisibility = Visibility.Hidden;
            CreateConnectionVisibility = Visibility.Hidden;

            AuthorizationVisibility = Visibility.Visible;
        }


        private void ClickBackToConnectionListButton(object sender, RoutedEventArgs e)
        {
            AuthorizationVisibility = Visibility.Hidden;
            CreateConnectionVisibility = Visibility.Hidden;

            ConnectionListVisibility = Visibility.Visible;
            LoadAllConnections();
        }

        #endregion :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::















        #region ПОКАЗ СООБЩЕНИЯ + АНИМАЦИЯ ЗАГРУЗКИ ====================================================================

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
