using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Xml;

namespace ControlSystemsLibrary.Services
{
    class XmlClass
    {
        private static string path;

        //---Метод: Проверка фала на существование -------------------------------------------------------------------------
        private static bool CheckFileExists()
        {
            try
            {
                return File.Exists(@"C:\Users\" + Environment.UserName + @"\Documents\Control Systems\");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.CheckFileExists()");
                return false;
            }
        }

        //---Метод: Если в директории программы нет файла конфигурации то. создает файл. -----------------------------------
        public static void CreateConfigFile()
        {
            try
            {
                if (!CheckFileExists()) // Если файл не обноружен
                    Directory.CreateDirectory(@"C:\Users\" + Environment.UserName + @"\Documents\Control Systems\");

                path = @"C:\Users\" + Environment.UserName + @"\Documents\Control Systems\Config.xml";

                if (File.Exists(path) == false)
                {
                    XmlDocument document = new XmlDocument(); // Документ

                    XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", "yes");
                    document.AppendChild(declaration);

                    // -----------------------------------------------------------------------------------------------------------
                    XmlNode Root = document.CreateElement("Configuratons"); // Корень
                    document.AppendChild(Root);
                    // -----------------------------------------------------------------------------------------------------------
                    XmlNode Connections = document.CreateElement("Connections");
                    Root.AppendChild(Connections);
                    // -----------------------------------------------------------------------------------------------------------
                    XmlNode CurrentUserName = document.CreateElement("CurrentUserName");
                    Root.AppendChild(CurrentUserName);
                    // -----------------------------------------------------------------------------------------------------------

                    document.Save(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.CreateConfigFile()");
            }
        }

        //---Запись логина текущего пользователя-----------------------------------------------------------------------------
        public static void WriteCurrentUserName(string CurrentUserName)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlNode xnode in RootElement)
                {
                    if (xnode.Name == "CurrentUserName")
                    {
                        xnode.InnerText = CurrentUserName;
                    }
                }
                document.Save(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.WriteCurrentUserName(CurrentUserName)");
            }
        }

        //---Чтение логина текущего пользователя-----------------------------------------------------------------------------
        public static string GetCurrentUserName()
        {
            string CurrentUserName = "";
            CreateConfigFile();
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlElement El in RootElement) // Пробежимся по Элементам Корневого элемента
                {
                    if (El.Name == "CurrentUserName") // Если имя элемента "Книга"
                    {
                        CurrentUserName = El.InnerText;
                    }
                }
                return CurrentUserName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.GetCurrentUserName()");
                return "Anjelina";
            }
        }

        //---Добавление Параметра подключения--------------------------------------------------------------------------------
        public static bool CreateConnectionString(string Name, string ConnectionString)
        {
            bool flag = false;
            try
            {
                CreateConfigFile();
                DeSelectAllConnections();

                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlNode xnode in RootElement)
                {
                    if (xnode.Name == "Connections")
                    {
                        XmlNode Connection = document.CreateElement("Connection");

                        XmlAttribute Selected = document.CreateAttribute("Selected");
                        Selected.Value = Cryption.Encrypt("Y");
                        Connection.Attributes.Append(Selected);

                        XmlAttribute ConnName = document.CreateAttribute("Name");
                        ConnName.Value = Cryption.Encrypt(Name);
                        Connection.Attributes.Append(ConnName);

                        XmlAttribute connectionString = document.CreateAttribute("ConnString");
                        connectionString.Value = Cryption.Encrypt(ConnectionString);
                        Connection.Attributes.Append(connectionString);

                        xnode.AppendChild(Connection);
                    }
                }
                document.Save(path);
                flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AddConnectSetting(string Name, string ConnectionString)");
            }
            return flag;
        }

        //---Метод: Проверяет не создана ли подключение с таким названием----------------------------------------------------
        public static bool CheckConnectionName(string ConnectionName)
        {
            CreateConfigFile();
            bool flag = false;
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlElement El in RootElement) // Пробежимся по Элементам Корневого элемента
                {
                    if (El.Name == "Connections")
                    {
                        foreach (XmlNode childnode in El.ChildNodes)
                        {
                            if (childnode.Name == "Connection")
                            {
                                XmlNode атрибут = childnode.Attributes.GetNamedItem("Name");

                                if (Cryption.Decrypt(атрибут.Value) == ConnectionName)
                                {
                                    flag = true;
                                }
                            }
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.CheckConnectionName(string ConnectionName)");
                return false;
            }
        }

        //---Метод: Отмечает все подключения как НЕвыбранный-----------------------------------------------------------------
        public static void DeSelectAllConnections()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlElement El in RootElement) // Пробежимся по Элементам Корневого элемента
                {
                    if (El.Name == "Connections") // Если имя элемента "Connections"
                        foreach (XmlNode childnode in El.ChildNodes)
                        {
                            if (childnode.Name == "Connection")
                            {
                                childnode.Attributes[0].Value = Cryption.Encrypt("N");
                            }
                        }
                }
                document.Save(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.DeSelectAllConnections()");
            }
        }

        //---Метод: Отмечает подключение как выбранный-----------------------------------------------------------------------
        public static void SetSelectConnection(string ConnectionName)
        {
            try
            {
                DeSelectAllConnections();
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlElement El in RootElement) // Пробежимся по Элементам Корневого элемента
                {
                    if (El.Name == "Connections") // Если имя элемента "Connections"
                    {
                        foreach (XmlNode childnode in El.ChildNodes)
                        {
                            if (childnode.Name == "Connection")
                            {
                                XmlNode атрибут = childnode.Attributes.GetNamedItem("Name");

                                if (Cryption.Decrypt(атрибут.Value) == ConnectionName) // Если "Название" равно 
                                {
                                    childnode.Attributes[0].Value = Cryption.Encrypt("Y");
                                }
                            }
                        }
                    }
                }
                document.Save(path);
            }
            catch (Exception ex)
            {
                //if (MessageBox.Show("Close Application?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                //{
                //    //do no stuff
                //}
                //else
                //{
                //    //do yes stuff
                //}

                MessageBox.Show(ex.Message, "SetSelectConnection(string ConnectionName)");
            }
        }

        //---Метод: Читает и возвращает список названий подключений----------------------------------------------------------
        public static ArrayList ReadAllConnectionsName()
        {
            ArrayList list = new ArrayList();
            try
            {
                CreateConfigFile();



                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlElement Element in RootElement)
                {
                    if (Element.Name == "Connections")
                        foreach (XmlNode xnode in Element)
                        {
                            XmlNode XName = xnode.Attributes.GetNamedItem("Name");
                            list.Add(Cryption.Decrypt(XName.Value));
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.ReadAllConnectionsName()");
            }
            return list;
        }

        //---Метод: Возвращает название "выбранной" строки подключения из Config файла---------------------------------------
        public static string GetSelectedConnectionName()
        {
            CreateConfigFile();

            string ConnName = "";
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlElement El in RootElement) // Пробежимся по Элементам Корневого элемента
                {
                    if (El.Name == "Connections") // Если имя элемента "Книга"
                    {
                        foreach (XmlNode xnode in El)
                        {
                            XmlNode XSelect = xnode.Attributes.GetNamedItem("Selected");
                            if (Cryption.Decrypt(XSelect.Value) == "Y")
                            {
                                XmlNode CS = xnode.Attributes.GetNamedItem("Name");
                                ConnName = Cryption.Decrypt(CS.Value);
                            }
                        }
                    }
                }
            }
            catch
            {
                if (MessageBox.Show("Ошибка чтения файла конфигурации.\nУдалить ошибочный файл и создать новый файл конфигурации?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    File.Delete(@"C:\Users\" + Environment.UserName + @"\Documents\Control Systems\Config.xml");
                    CreateConfigFile();

                    MessageBox.Show("Создан новый файл конфигурации.\nСоздайте новое подключение.", "XmlClass.GetSelectedConnectionName()"); // Вывод сообщения об ошибке
                }
            }
            return ConnName;
        }

        //---Метод: Возвращает строку подключения из Config файла------------------------------------------------------------
        public static string GetSelectedConnectionString()
        {
            string ConnString = null;
            try
            {
                CreateConfigFile();
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlElement El in RootElement) // Пробежимся по Элементам Корневого элемента
                {
                    if (El.Name == "Connections")
                    {
                        foreach (XmlNode xnode in El)
                        {
                            XmlNode XSelect = xnode.Attributes.GetNamedItem("Selected");
                            if (Cryption.Decrypt(XSelect.Value) == "Y")
                            {
                                XmlNode CS = xnode.Attributes.GetNamedItem("ConnString");
                                ConnString = CS.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.GetSelectedConnectionString()");
            }

            return ConnString;
        }

        //---Метод: Удаляет подключение из Config файла----------------------------------------------------------------------
        public static bool DeleteConnection(string ConnectionName)
        {
            bool flag = false;
            try
            {
                //DeSelectAllConnections();
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlElement RootElement = document.DocumentElement;

                foreach (XmlElement El in RootElement) // Пробежимся по Элементам Корневого элемента
                {
                    if (El.Name == "Connections") // Если имя элемента "Connections"
                    {
                        foreach (XmlNode childnode in El.ChildNodes)
                        {
                            if (childnode.Name == "Connection")
                            {
                                XmlNode атрибут = childnode.Attributes.GetNamedItem("Name");

                                if (Cryption.Decrypt(атрибут.Value) == ConnectionName) // Если "Название" равно "Осень"
                                {
                                    El.RemoveChild(childnode);
                                }
                            }
                        }
                    }
                }
                document.Save(path);
                flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XmlClass.DeleteConnection(string ConnectionName)");
            }
            return flag;
        }
    }
}
