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
using System.Collections.Specialized;
using ControlSystemsLibrary.Classes;

namespace ControlSystemsLibrary.Views
{
    public delegate void LockInterfaceDelegate();
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

        ContentControl CurrentUserInterfaceParent;
        ObservableCollection<UserInterfacesClass> UserInterfacesCollection = new ObservableCollection<UserInterfacesClass>();
        LockInterfaceDelegate LockInterface;

        private string CurrentCryptConnectionString = "";
        // КОНСТРУКТОР ==================================================================================================
        public Authorization(ContentControl CurrentUserInterfaceParent)
        {
            this.CurrentUserInterfaceParent = CurrentUserInterfaceParent;

            InitializeComponent();

            LockInterface = LockInterfaceMethod;
            Loaded += Authorization_Loaded;
        }

        void LockInterfaceMethod()
        {
            CurrentUserInterfaceParent.Content = this;
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

        private bool authorizationButtonEnabled = false;
        public bool AuthorizationButtonEnabled
        {
            get => authorizationButtonEnabled;
            set
            {
                authorizationButtonEnabled = value;
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
            Connections.CollectionChanged += Connections_CollectionChanged;
        }
        private void Connections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Connections.Count == 0)
                CheckSelectedConnectionButtonEnabled = false;
            else
                CheckSelectedConnectionButtonEnabled = true;
        }







        #region МЕТОДЫ =================================================================================================

        // Метод: Загружает список подклчений --------------------------------------------------------------------------
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
            SetFocus();
        }

        void SetFocus()
        {
            if(UserNameTextBox.Text == "" || UserNameTextBox.Text == null)
            {
                UserNameTextBox.Focus();
            }
            else if(AuthorizationPasswordBox.Password == "" || AuthorizationPasswordBox.Password == null)
            {
                AuthorizationPasswordBox.Focus();
            }
        }

        // Метод: Пытается открыть и закрыть подключение с создаваемым подключением ------------------------------------
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

        // Метод: Метод загружает все созданные подключения ------------------------------------------------------------
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

        // Метод: Проверяет подключение с выбранным из списка ----------------------------------------------------------
        async void CheckSelectedConnection()
        {
            ConnectionListEnabled = false;
            ShowMessage("Установка соединения...", "Blue-003", true);

            if (await Task.Run(() => OpenCloseConnection(Cryption.Decrypt(CurrentCryptConnectionString))) == true)
            {
                ShowMessage("Соединение установлено!", "Green-003", false);
                ConnectionListEnabled = true;
            }
            else
            {
                ShowMessage("Не удалось устновить соединение.", "Red-001", false);
                ConnectionListEnabled = true;
            }
        }


        #endregion ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::






        #region СОБЫТИЯ =================================================================================================

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
                            CurrentConnectionName = "";
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
                if (authorizationVisibility != value)
                {
                    authorizationVisibility = value;
                    AuthorizationButtonEnabled = LoginTextChanged();
                    if(value == Visibility.Visible)
                    {
                        SetFocus();
                    }
                    OnPropertyChanged();
                }
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
            UserAuthorization(AuthorizationPasswordBox);
        }

        async void UserAuthorization(PasswordBox passwordBox)
        {
            AuthorizationEnabled = false;
            ShowMessage("Проверка логина и пароля...", "Blue-003", true);
            string message = "";
            if (await Task.Run(() => DataBaseRequest.CheckAuthorization(CurrentCryptConnectionString, CurrentUserName, passwordBox.Password, ref message) == true))
            {
                string UserInterfaceName = DataBaseRequest.GetUserInterfaceName(CurrentCryptConnectionString, CurrentUserName);
                string UserInterfaceFullName = GenerateUserInterfaceFullName(UserInterfaceName);

                if (CheckUserInterfacesCollection(UserInterfaceFullName) == false)
                {
                    UserControl UC = GetNewUserInterface(UserInterfaceName);
                    UserInterfacesClass UIC = new UserInterfacesClass();
                    UIC.FullUserInterfaceName = UserInterfaceFullName;
                    UIC.UserInterfaceControl = UC;
                    UserInterfacesCollection.Add(UIC);
                    CurrentUserInterfaceParent.Content = UIC.UserInterfaceControl;

                    ShowMessage("Сохранение имени пользователя", "Blue-003", true);
                    await Task.Run(() => XmlClass.WriteCurrentUserName(CurrentUserName));
                }
                else
                {
                    CurrentUserInterfaceParent.Content = GetUserInterfaceFromCollection(UserInterfaceFullName).UserInterfaceControl;
                }

                ShowMessage(false);
                AuthorizationEnabled = true;
                AuthorizationPasswordBox.Clear();
            }
            else
            {
                AuthorizationEnabled = true;
                AuthorizationButtonEnabled = false;
                AuthorizationPasswordBox.Focus();
                AuthorizationPasswordBox.SelectAll();
                ShowMessage(message, "Red-001", false);
            }
        }

        string GenerateUserInterfaceFullName(string UserInterfaceControlName)
        {
            return Strings.RemoveCharacters(CurrentUserName) + Strings.RemoveCharacters(UserInterfaceControlName) + Strings.RemoveCharacters(CurrentConnectionName);
        }

        bool CheckUserInterfacesCollection(string UserInterfaceFullName)
        {
            foreach (UserInterfacesClass UIC in UserInterfacesCollection)
            {
                if (UIC.FullUserInterfaceName == UserInterfaceFullName)
                {
                    return true;
                }
            }
            return false;
        }


        UserControl GetNewUserInterface(string UserInterfaceName)
        {
            switch (UserInterfaceName)
            {
                case "Администратор": return new Administrator(CurrentUserName, CurrentConnectionName, CurrentCryptConnectionString, LockInterface);

                default: return new UserInterfaceSelection();
            }
        }

        UserInterfacesClass GetUserInterfaceFromCollection(string UserInterfaceFullName)
        {
            foreach (UserInterfacesClass UIC in UserInterfacesCollection)
            {
                if (UIC.FullUserInterfaceName == UserInterfaceFullName)
                {
                    return UIC;
                }
            }
            return null;
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


        private bool checkSelectedConnectionButtonEnabled = false;
        public bool CheckSelectedConnectionButtonEnabled
        {
            get => checkSelectedConnectionButtonEnabled;
            set
            {
                checkSelectedConnectionButtonEnabled = value;
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
            CheckSelectedConnection();
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
                CheckCreatedValues();
                if (value == true)
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

                if (value == Visibility.Visible)
                    ClearCreatedValues();

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




        

        private string createdConnectionName = "";
        public string CreatedConnectionName
        {
            get => createdConnectionName;
            set
            {
                createdConnectionName = value;
                CheckCreatedValues();
                OnPropertyChanged();
            }
        }


        private string createdConnectionServer = "";
        public string CreatedConnectionServer
        {
            get => createdConnectionServer;
            set
            {
                createdConnectionServer = value;
                CheckCreatedValues();
                OnPropertyChanged();
            }
        }


        private string createdConnectionDataBase = "";
        public string CreatedConnectionDataBase
        {
            get => createdConnectionDataBase;
            set
            {
                createdConnectionDataBase = value;
                CheckCreatedValues();
                OnPropertyChanged();
            }
        }


        private string createdConnectionUserID = "";
        public string CreatedConnectionUserID
        {
            get => createdConnectionUserID;
            set
            {
                createdConnectionUserID = value;
                CheckCreatedValues();
                OnPropertyChanged();
            }
        }


        private string createdConnectionPassword = "";
        public string CreatedConnectionPassword
        {
            get => createdConnectionPassword;
            set
            {
                createdConnectionPassword = value;
                CheckCreatedValues();
                OnPropertyChanged();
            }
        }


        private string createdConnectionString = "";
        public string CreatedConnectionString
        {
            get => createdConnectionString;
            set
            {
                createdConnectionString = value;
                CheckCreatedValues();
                OnPropertyChanged();
            }
        }


















        private void ClickCheckCreatedConnectionButton(object sender, RoutedEventArgs e)
        {
            CheckCreatedConnection();
        }

        private void ClickSaveCreatedConnectionButton(object sender, RoutedEventArgs e)
        {
            SaveConnection();
        }

        private void ClickPasteButton(object sender, RoutedEventArgs e)
        {
            CreatedConnectionString = Clipboard.GetText();
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



        // Метод: Проверяет заполненость значений для проверки соединения ----------------------------------------------
        bool CheckCreatedValues()
        {
            bool flag = false;
            if (CreatedMode == true)
            {
                if (CreatedConnectionName != "" && CreatedConnectionString != "")
                    flag = true;
                else
                    flag = false;
            }
            else
            {
                if (CreatedConnectionName != "" && CreatedConnectionServer != "" && CreatedConnectionDataBase != "" && CreatedConnectionUserID != "" && CreatedConnectionPassword != "")
                    flag = true;
                else
                    flag = false;
            }
            MessageText = "";
            CheckCreatedConnectionButtonEnabled = flag;
            return flag;
        }


        // Метод: Очищает все значения создаваеого подключения ---------------------------------------------------------
        void ClearCreatedValues()
        {
            CreatedConnectionName = "";
            CreatedConnectionServer = "";
            CreatedConnectionDataBase = "";
            CreatedConnectionUserID = "";
            CreatedConnectionPassword = "";
            CreatedConnectionString = "";

            CheckCreatedValues();
        }

        // Метод: Проверяет соединение создаваемым подключением --------------------------------------------------------
        async void CheckCreatedConnection()
        {
            ShowMessage("Проверка названия подключения", "Blue-003", true);
            if (await Task.Run(() => XmlClass.CheckConnectionName(createdConnectionName)) == false)
            {
                CreateConnectionEnabled = false;
                CheckCreatedConnectionButtonEnabled = false;

                ShowMessage("Установка соединения...", "Blue-003", true);

                if (await Task.Run(() => OpenCloseConnection(ConnectionBuilding())))
                {
                    ShowMessage("Соединение установлено!\nСохраните подключение.", "Green-003", false);
                    SaveCreatedConnectionButtonEnabled = true;
                }
                else
                {
                    ShowMessage("Не удалось установить соединение.", "Red-001", false);
                    SaveCreatedConnectionButtonEnabled = false;
                }

                CreateConnectionEnabled = true;
            }
            else
            {
                ShowMessage("Подключение с названием " + '"' + createdConnectionName + '"' + " уже создано. Измените название.", "Red-001", false);
            }
        }

        string ConnectionBuilding()
        {
            SqlConnectionStringBuilder ConnectionStringBuilder = new SqlConnectionStringBuilder();
            if (CreatedMode == true)
            {
                try
                {
                    ConnectionStringBuilder.ConnectionString = CreatedConnectionString;
                    ConnectionStringBuilder.Pooling = true;
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Red-001", false);
                }
            }
            else
            {
                try
                {
                    ConnectionStringBuilder.DataSource = CreatedConnectionServer;
                    ConnectionStringBuilder.InitialCatalog = CreatedConnectionDataBase;
                    ConnectionStringBuilder.UserID = CreatedConnectionUserID;
                    ConnectionStringBuilder.Password = CreatedConnectionPassword;
                    ConnectionStringBuilder.Pooling = true;
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Red-001", false);
                }
            }
            return ConnectionStringBuilder.ConnectionString;
        }

        async void SaveConnection()
        {
            CreateConnectionEnabled = false;
            ShowMessage("Сохранение подключения...", "Blue-003", true);

            CurrentConnectionName = CreatedConnectionName;
            if (await Task.Run(() => XmlClass.CreateConnectionString(CurrentConnectionName, ConnectionBuilding())) == true)
            {
                ClearCreatedValues();
                ShowMessage("Подключение " + '"' + CurrentConnectionName + '"' + " сохранено!", "Green-003", false);
                CurrentConnectionTextColor = GetColor.Get("Dark-003");
            }
            else
            {
                ShowMessage("Что-то пошло не так", "Green-003", false);
            }

            SaveCreatedConnectionButtonEnabled = false;
            CreateConnectionEnabled = true;
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
                if (loaderUC != value)
                {
                    loaderUC = value;
                    OnPropertyChanged();
                }
            }
        }

        private string messageText = "";
        public string MessageText
        {
            get => messageText;
            set
            {
                if (messageText != value)
                {
                    messageText = value;
                    OnPropertyChanged();
                }
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

        private void Login_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                if (CurrentConnectionName != "")
                {
                    if (sender is TextBox)
                    {
                        if (UserNameTextBox.Text != "" && UserNameTextBox.Text != null)
                        {
                            if (AuthorizationPasswordBox.Password != "" && AuthorizationPasswordBox.Password != null)
                            {
                                if (AuthorizationButtonEnabled == true)
                                {
                                    UserAuthorization(AuthorizationPasswordBox);
                                }
                            }
                            else
                            {
                                AuthorizationPasswordBox.Focus();
                                AuthorizationPasswordBox.SelectAll();
                            }
                        }
                    }

                    if (sender is PasswordBox)
                    {
                        if (AuthorizationPasswordBox.Password != "" && AuthorizationPasswordBox.Password != null)
                        {
                            if (UserNameTextBox.Text != "" && UserNameTextBox.Text != null)
                            {
                                UserAuthorization(AuthorizationPasswordBox);
                            }
                            else
                            {
                                UserNameTextBox.Focus();
                                UserNameTextBox.SelectAll();
                            }
                        }
                    }

                    e.Handled = true;
                }
            }
        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(CurrentConnectionName != "")
                ShowMessage(false);
            AuthorizationButtonEnabled = LoginTextChanged();
        }

        private void AuthorizationPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentConnectionName != "")
                ShowMessage(false);
            AuthorizationButtonEnabled = LoginTextChanged();
        }

        bool LoginTextChanged()
        {
            bool ok = true;
            if ((UserNameTextBox.Text == "" || UserNameTextBox.Text == string.Empty) || (AuthorizationPasswordBox.Password == "" || AuthorizationPasswordBox.Password == string.Empty) || CurrentConnectionName == "")
            {
                ok = false;
            }
            return ok;
        }



        // UserNameTextBox
        // AuthorizationPasswordBox

    }
}
