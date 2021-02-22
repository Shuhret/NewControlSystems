using ControlSystemsLibrary.Classes;
using ControlSystemsLibrary.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ControlSystemsLibrary.Services
{
    class DataBaseRequest
    {


        //---Метод: Проверка авторизации (ХП)---------------------------------------------------------------------------
        public static bool CheckAuthorization(string Connectionstring, string login, string password, ref string message)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("CheckAuthorization", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@Login", Value = login };
                    command.Parameters.Add(Param1);

                    SqlParameter Param2 = new SqlParameter { ParameterName = "@Password", Value = password };
                    command.Parameters.Add(Param2);

                    int result = Convert.ToInt32(command.ExecuteScalar());

                    if (result == 1)
                        ok = true;
                    else
                    {
                        ok = false;
                        message = "Хуёвый логин или пароль.";
                    }

                }
                catch (Exception ex)
                {
                    message = ex.Message.ToString();
                }
            }
            return ok;
        }

        //---Метод: Получение названия интерфейса для пользователя (ХП)-------------------------------------------------
        public static string GetUserInterfaceName(string Connectionstring, string Login)
        {
            string UserInterfaceName = "";
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("GetUserInterface", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@Login", Value = Login };
                    command.Parameters.Add(Param1);


                    UserInterfaceName = command.ExecuteScalar().ToString();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            return UserInterfaceName;
        }

        public static ObservableCollection<NomenclatureClass> GetAllNomenclatures(string Connectionstring)
        {
            ObservableCollection<NomenclatureClass> list = new ObservableCollection<NomenclatureClass>();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("GetAllNomenclatures", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        NomenclatureClass NC = new NomenclatureClass();
                        NC.ID = (Guid)reading.GetValue(0);
                        NC.GroupID = (Guid)reading.GetValue(1);
                        NC.GroupNomen = (bool)reading.GetValue(2);
                        NC.Name = reading.GetValue(3).ToString();

                        if (NC.GroupNomen == true)
                        {
                            NC.Category = reading.GetValue(4).ToString();
                            NC.Article = reading.GetValue(5).ToString();
                            NC.BaseUnit = reading.GetValue(6).ToString();
                            NC.WeightBaseUnit = Convert.ToDouble(reading.GetValue(7));
                            NC.CountryOfOrigin = reading.GetValue(8).ToString();
                            NC.Description = reading.GetValue(9).ToString();
                            NC.Aksia = (bool)reading.GetValue(10);
                            NC.Focus = (bool)reading.GetValue(11);
                            NC.New = (bool)reading.GetValue(12);
                        }
                        list.Add(NC);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка в методе: DataBaseRequest.GetAllNomenclatures" + "\n" + ex.Message, "Хуёво!");
                }
            }
            return list;
        }


        //---Метод: Возвращает из базы данных Единицы измерения----------------------------------------------------------
        public static ArrayList GetUnitsName(string Connectionstring)
        {
            ArrayList list = new ArrayList();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = connect.CreateCommand(); // Создание команды
                    command.CommandText = "SELECT [Unit] FROM [dbo].[Units] ORDER BY [Unit]"; // Текст команды
                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        list.Add(reading.GetValue(0).ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Пиздец кароче нахуй блядь!");
                }
            }
            return list;
        }

        //---Метод: Возвращает из базы данных ID Единицы измерения ----------------------------------------------------------
        public static Guid GetUnitID(string Connectionstring, string UnitName)
        {
            Guid UnitID = new Guid();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = connect.CreateCommand(); // Создание команды
                    command.CommandText = "SELECT [ID] FROM [dbo].[Units] WHERE [Unit] = '" + UnitName+"'"; // Текст команды
                    UnitID = (Guid)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            return UnitID;
        }

        //---Метод: Возвращает из базы данных ID тип штрих кода ----------------------------------------------------------
        public static Guid GetBarcodeTypeID(string Connectionstring, string BarcodeTypeName)
        {
            Guid BarcodTypeID = new Guid();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = connect.CreateCommand(); // Создание команды
                    command.CommandText = "SELECT [ID] FROM [dbo].[BarсodeTypes] WHERE [TypeName]  = '" + BarcodeTypeName+"'"; // Текст команды
                    return (Guid)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            return BarcodTypeID;
        }

        //---Метод: Возвращает из базы данных ID тип штрих кода ----------------------------------------------------------
        public static Guid GetNomenclaturePropertyValueID(string Connectionstring, string PropertyName, string ValuesName)
        {
            Guid ValueID = new Guid();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = connect.CreateCommand(); // Создание команды
                    command.CommandText = "SELECT [ID] FROM [dbo].[NomenPropertyValues] WHERE [ValueName] = '"+ ValuesName + "' AND [PropertyID] = (SELECT [ID] FROM [dbo].[NomenProperties] WHERE [PropertyName] = '" + PropertyName + "')"; // Текст команды
                    ValueID = (Guid)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            return ValueID;
        }

        //---Метод: Возвращает из базы данных Страны происхождения-------------------------------------------------------
        public static ArrayList GetCountry(string Connectionstring)
        {
            ArrayList list = new ArrayList();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = connect.CreateCommand(); // Создание команды
                    command.CommandText = "SELECT [CountryName] FROM [dbo].[Country] ORDER By [CountryName]"; // Текст команды
                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        list.Add(reading.GetValue(0).ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Пиздец кароче нахуй блядь!");
                }
            }
            return list;
        }

        //---Метод: Загружает все свойства номенклатуры (ХП)-------------------------------------------------------------
        public static ArrayList GetAllNomenProperties(string Connectionstring)
        {
            ArrayList list = new ArrayList();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("GetAllNomenProperties", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        list.Add(reading.GetValue(0).ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "GetAllNomenProperties");
                }
            }
            return list;
        }

        //---Метод: Загружает все значения свойства номенклатуры (ХП)----------------------------------------------------
        public static ArrayList GetAllNomenPropertyValues(string Connectionstring, string PropertyName)
        {
            ArrayList list = new ArrayList();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("GetPropertyValues", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@PropertyName", Value = PropertyName }; //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    command.ExecuteNonQuery();

                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        list.Add(reading.GetValue(0).ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "GetAllNomenPropertyValues");
                }
            }
            return list;
        }

        //---Метод: Возвращает из базы данных Типы Штрих кодов-----------------------------------------------------------
        public static ArrayList GetBarcodeTypes(string Connectionstring)
        {
            ArrayList list = new ArrayList();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = connect.CreateCommand(); // Создание команды
                    command.CommandText = "SELECT [TypeName] FROM [dbo].[BarсodeTypes] ORDER BY [TypeName]"; // Текст команды
                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        list.Add(reading.GetValue(0).ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Пиздец кароче нахуй блядь!");
                }
            }
            return list;
        }

        //---Метод: Возвращает из базы данных Категории номенклатуры ----------------------------------------------------
        public static ArrayList GetAllNomenclatureCategories(string Connectionstring)
        {
            ArrayList list = new ArrayList();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                //try
                //{
                    connect.Open();
                    SqlCommand command = connect.CreateCommand(); // Создание команды
                    command.CommandText = "SELECT [Category] FROM [dbo].[NomenclatureCategories] ORDER BY [Category]"; // Текст команды
                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        list.Add(reading.GetValue(0).ToString());
                    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, "Пиздец кароче нахуй блядь!");
                //}
            }
            return list;
        }

        //---Метод: Создание новой номенклатуры (Основные данные + штрих-код базовой единицы) (ХП)---------------------------------------------------------------------
        public static bool CreateNewNomenclature(string Connectionstring, NomenclatureClass Nomen, DataTable AdditionalUnitsDataTable, DataTable PropertyValueDataTable, DataTable ImagesDataTable, DataTable BarcodesDataTable)
        {
            string message = "";
            bool ok = false;

            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("CreateNewNomenclature", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramID = command.Parameters.AddWithValue("@ID", Nomen.ID); // Передаваемый параметр "ID номенклатуры"
                    paramID.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter paramGroupID = command.Parameters.AddWithValue("@GroupID", Nomen.GroupID); // Передаваемый параметр "ID группы номенклатуры"
                    paramGroupID.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter paramAricle = command.Parameters.AddWithValue("@Aricle", Nomen.Article);  // Передаваемый параметр "Артикул"
                    paramAricle.SqlDbType = SqlDbType.NVarChar;
                    paramAricle.Size = 50;

                    SqlParameter paramNomenclatureName = command.Parameters.AddWithValue("@NomenclatureName", Nomen.Name); // Передаваемый параметр "Наименование"
                    paramNomenclatureName.SqlDbType = SqlDbType.NVarChar;
                    paramNomenclatureName.Size = 100;

                    SqlParameter paramBaseUnitName = command.Parameters.AddWithValue("@BaseUnitName", Nomen.BaseUnit); // Передаваемый параметр "Базовая единица"
                    paramBaseUnitName.SqlDbType = SqlDbType.NVarChar;
                    paramBaseUnitName.Size = 50;

                    SqlParameter paramCountryOfOriginName = command.Parameters.AddWithValue("@CountryOfOriginName", Nomen.CountryOfOrigin); // Передаваемый параметр "Страна происхождения"
                    paramCountryOfOriginName.SqlDbType = SqlDbType.NVarChar;
                    paramCountryOfOriginName.Size = 50;

                    SqlParameter paramCategory = command.Parameters.AddWithValue("@Category", Nomen.Category); // Передаваемый параметр "Категория"
                    paramCategory.SqlDbType = SqlDbType.NVarChar;
                    paramCategory.Size = 50;

                    SqlParameter paramBaseUnitWeight = command.Parameters.AddWithValue("@BaseUnitWeight", Nomen.WeightBaseUnit); // Передаваемый параметр "Вес базовой единицы"
                    paramBaseUnitWeight.SqlDbType = SqlDbType.Decimal;
                    paramBaseUnitWeight.Precision = 18;
                    paramBaseUnitWeight.Scale = 3;

                    SqlParameter paramDescription = command.Parameters.AddWithValue("@Description", Nomen.Description); // Передаваемый параметр "Описание"
                    paramDescription.SqlDbType = SqlDbType.NVarChar;
                    paramDescription.Size = 500;


                    SqlParameter ParamAdditionalUnits = new SqlParameter { ParameterName = "@AdditionalUnits", Value = AdditionalUnitsDataTable }; // Передаваемый табличный параметр (Дополнительные единицы)
                    ParamAdditionalUnits.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(ParamAdditionalUnits);

                    SqlParameter ParamPropertyValues = new SqlParameter { ParameterName = "@PropertyValues", Value = PropertyValueDataTable }; // Передаваемый табличный параметр (Свойства и значения)
                    ParamPropertyValues.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(ParamPropertyValues);

                    SqlParameter ParamImages = new SqlParameter { ParameterName = "@Images", Value = ImagesDataTable }; // Передаваемый табличный параметр (Изображения)
                    ParamImages.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(ParamImages);

                    SqlParameter ParamBarcodes = new SqlParameter { ParameterName = "@Barcodes", Value = BarcodesDataTable }; // Передаваемый табличный параметр (Штрих-коды)
                    ParamBarcodes.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(ParamBarcodes);


                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр (Результат 1 или 0)
                    command.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output; // Выходной параметр (Сообщение об ошибке)

                    command.ExecuteNonQuery(); // Выполнение запроса

                    ok = (bool)command.Parameters["@Result"].Value; 
                    message = command.Parameters["@Message"].Value.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message+"\nSQL Server: "+message, "DataBaseRequest.CreateNewNomenclature");
                }
            return ok;
            }
        }

        public static List<AdditionalUnits> GetEditableNomenclatureAddedUnits(string Connectionstring, Guid EditableNomenclatureID)
        {
            List<AdditionalUnits> list = new List<AdditionalUnits>();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("GetEditableNomenclatureAddedUnits", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@ID", Value = EditableNomenclatureID }; //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        AdditionalUnits AUC = new AdditionalUnits();
                        AUC.ID = new Guid(reading.GetValue(0).ToString());
                        AUC.UnitName = reading.GetValue(1).ToString();
                        AUC.Quantity = Double.Parse(reading.GetValue(2).ToString());
                        AUC.UnitWeight = Double.Parse(reading.GetValue(3).ToString());

                        list.Add(AUC);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка в методе: DataBaseRequest.GetEditableNomenclatureAddedUnits" + "\n" + ex.Message, "Хуёво!");
                }
            }
            return list;
        }

        public static List<NomenclaturePropertyValues> GetEditableNomenclaturePropertiesAndValues(string Connectionstring, Guid EditableNomenclatureID)
        {
            List<NomenclaturePropertyValues> list = new List<NomenclaturePropertyValues>();

            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("GetEditableNomenclaturePropertiesAndValues", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@NomenclatureID", Value = EditableNomenclatureID }; //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        NomenclaturePropertyValues NPV = new NomenclaturePropertyValues();
                        NPV.ID = (Guid)reading.GetValue(0);
                        NPV.PropertyName = reading.GetValue(1).ToString();
                        NPV.ValueName = reading.GetValue(2).ToString();

                        list.Add(NPV);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка в методе: DataBaseRequest.GetEditableNomenclaturePropertiesAndValues" + "\n" + ex.Message, "Хуёво!");
                }
            }
            return list;
        }

        // Метод: Возвращает Штрих-код базовой единицы
        public static List<NomenclatureBarcodes> GetEditableNomenclatureBarcode(string Connectionstring, Guid NomenclatureID)
        {
            List<NomenclatureBarcodes> list = new List<NomenclatureBarcodes>();

            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("GetEditableNomenclatureBarcodes", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@NomenclatureID", Value = NomenclatureID }; //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlDataReader reading = command.ExecuteReader();

                    while (reading.Read())
                    {
                        NomenclatureBarcodes NB = new NomenclatureBarcodes();
                        NB.ID = (Guid)reading.GetValue(0);
                        NB.UnitName = reading.GetValue(1).ToString();
                        NB.BarcodeType = reading.GetValue(2).ToString();
                        NB.Barcode = reading.GetValue(3).ToString();
                        list.Add(NB);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка в методе: DataBaseRequest.GetEditableNomenclatureBarcode" + "\n" + ex.Message, "Хуёво!");
                }
            }
            return list;
        }

        public static List<NomenclatureImageClass> GetEditableNomenclatureImages(string Connectionstring, Guid NomenclatureID)
        {
            List<NomenclatureImageClass> list = new List<NomenclatureImageClass>();
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                connect.Open();
                SqlCommand command = new SqlCommand("GetEditableNomenclatureImages", connect);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter Param0 = new SqlParameter { ParameterName = "@NomenclatureID", Value = NomenclatureID }; //---Передаваемый параметр
                command.Parameters.Add(Param0);
                SqlDataReader reading = command.ExecuteReader();

                while (reading.Read())
                {
                    NomenclatureImageClass NI = new NomenclatureImageClass();
                    NI.ID = (Guid)reading.GetValue(0);
                    NI.ImageArray = (byte[])reading.GetValue(1);
                    NI.MainImage = (bool)reading.GetValue(2);
                    list.Add(NI);
                }
            }
            return list;
        }

        public static bool EditNomenclature(string Connectionstring, NomenclatureClass Nomen, DataTable AdditionalUnitsDataTable, DataTable PropertyValueDataTable, DataTable ImagesDataTable, DataTable BarcodesDataTable)
        {
            string message = "";
            bool ok = false;

            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("EditNomenclature", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramID = command.Parameters.AddWithValue("@ID", Nomen.ID); // Передаваемый параметр "ID номенклатуры"
                    paramID.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter paramGroupID = command.Parameters.AddWithValue("@GroupID", Nomen.GroupID); // Передаваемый параметр "ID группы номенклатуры"
                    paramGroupID.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter paramAricle = command.Parameters.AddWithValue("@Aricle", Nomen.Article);  // Передаваемый параметр "Артикул"
                    paramAricle.SqlDbType = SqlDbType.NVarChar;
                    paramAricle.Size = 50;

                    SqlParameter paramNomenclatureName = command.Parameters.AddWithValue("@NomenclatureName", Nomen.Name); // Передаваемый параметр "Наименование"
                    paramNomenclatureName.SqlDbType = SqlDbType.NVarChar;
                    paramNomenclatureName.Size = 100;

                    SqlParameter paramBaseUnitName = command.Parameters.AddWithValue("@BaseUnitName", Nomen.BaseUnit); // Передаваемый параметр "Базовая единица"
                    paramBaseUnitName.SqlDbType = SqlDbType.NVarChar;
                    paramBaseUnitName.Size = 50;

                    SqlParameter paramCountryOfOriginName = command.Parameters.AddWithValue("@CountryOfOriginName", Nomen.CountryOfOrigin); // Передаваемый параметр "Страна происхождения"
                    paramCountryOfOriginName.SqlDbType = SqlDbType.NVarChar;
                    paramCountryOfOriginName.Size = 50;

                    SqlParameter paramCategory = command.Parameters.AddWithValue("@Category", Nomen.Category); // Передаваемый параметр "Категория"
                    paramCategory.SqlDbType = SqlDbType.NVarChar;
                    paramCategory.Size = 50;

                    SqlParameter paramBaseUnitWeight = command.Parameters.AddWithValue("@BaseUnitWeight", Nomen.WeightBaseUnit); // Передаваемый параметр "Вес базовой единицы"
                    paramBaseUnitWeight.SqlDbType = SqlDbType.Decimal;
                    paramBaseUnitWeight.Precision = 18;
                    paramBaseUnitWeight.Scale = 3;

                    SqlParameter paramDescription = command.Parameters.AddWithValue("@Description", Nomen.Description); // Передаваемый параметр "Описание"
                    paramDescription.SqlDbType = SqlDbType.NVarChar;
                    paramDescription.Size = 500;


                    SqlParameter ParamAdditionalUnits = new SqlParameter { ParameterName = "@AdditionalUnits", Value = AdditionalUnitsDataTable }; // Передаваемый табличный параметр (Дополнительные единицы)
                    ParamAdditionalUnits.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(ParamAdditionalUnits);

                    SqlParameter ParamPropertyValues = new SqlParameter { ParameterName = "@PropertyValues", Value = PropertyValueDataTable }; // Передаваемый табличный параметр (Свойства и значения)
                    ParamPropertyValues.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(ParamPropertyValues);

                    SqlParameter ParamImages = new SqlParameter { ParameterName = "@Images", Value = ImagesDataTable }; // Передаваемый табличный параметр (Изображения)
                    ParamImages.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(ParamImages);

                    SqlParameter ParamBarcodes = new SqlParameter { ParameterName = "@Barcodes", Value = BarcodesDataTable }; // Передаваемый табличный параметр (Штрих-коды)
                    ParamBarcodes.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(ParamBarcodes);


                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр (Результат 1 или 0)
                    command.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output; // Выходной параметр (Сообщение об ошибке)

                    command.ExecuteNonQuery(); // Выполнение запроса

                    ok = (bool)command.Parameters["@Result"].Value;
                    message = command.Parameters["@Message"].Value.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\nSQL Server: " + message, "DataBaseRequest.CreateNewNomenclature");
                }
                return ok;
            }
        }




        //---Метод: Создание новой группы номенклатуры (ХП)---------------------------------------------------------------------
        public static bool CreateNomenclatureGroup(string Connectionstring, NomenclatureClass NC)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("CreateNomenclatureGroup", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@ID", Value = NC.ID };// Новый ID //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@GroupID", Value = NC.GroupID }; //---Передаваемый параметр
                    command.Parameters.Add(Param1);

                    SqlParameter Param2 = new SqlParameter { ParameterName = "@GroupName", Value = NC.Name }; //---Передаваемый параметр
                    command.Parameters.Add(Param2);

                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр

                    command.ExecuteNonQuery();

                    ok = (bool)command.Parameters["@Result"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.CreateNomenclatureGroup");
                }
                return ok;
            }
        }

        //---Метод: Изменение группы номенклатуры (ХП)---------------------------------------------------------------------
        public static bool UpdateNomenclatureGroup(string Connectionstring, NomenclatureClass NC)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("UpdateNomenclatureGroup", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@ID", Value = NC.ID };// Новый ID //---Передаваемый параметр
                    command.Parameters.Add(Param0);
                    SqlParameter Param2 = new SqlParameter { ParameterName = "@GroupName", Value = NC.Name }; //---Передаваемый параметр
                    command.Parameters.Add(Param2);

                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр

                    command.ExecuteNonQuery();

                    ok = (bool)command.Parameters["@Result"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.UpdateNomenclatureGroup");
                }
                return ok;
            }
        }


        //---Метод: Вырезать/Вставить (ХП)---------------------------------------------------------------------
        public static bool ChangeNomenclatureGroupID(string Connectionstring, Guid ID, Guid NewGroupID)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("ChangeNomenclatureGroupID", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@ID", Value = ID };
                    command.Parameters.Add(Param0);

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@GroupID", Value = NewGroupID };
                    command.Parameters.Add(Param1);

                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    ok = (bool)command.Parameters["@Result"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.UpdateGroupID");
                }
                return ok;
            }
        }




        //---Метод: Создание новой номенклатуры (ХП)---------------------------------------------------------------------
        public static bool UpdateTagAksia(string Connectionstring, NomenclatureClass Nomen)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("UpdateTagAksia", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@NomenID", Value = Nomen.ID };// Новый ID //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@Checked", Value = Nomen.Aksia }; //---Передаваемый параметр
                    command.Parameters.Add(Param1);


                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр

                    command.ExecuteNonQuery();

                    ok = (bool)command.Parameters["@Result"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.UpdateTagAksia");
                }
                return ok;
            }
        }

        //---Метод: Создание новой номенклатуры (ХП)---------------------------------------------------------------------
        public static bool UpdateTagFocus(string Connectionstring, NomenclatureClass Nomen)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("UpdateTagFocus", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@NomenID", Value = Nomen.ID };// Новый ID //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@Checked", Value = Nomen.Focus }; //---Передаваемый параметр
                    command.Parameters.Add(Param1);


                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр

                    command.ExecuteNonQuery();

                    ok = (bool)command.Parameters["@Result"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.UpdateTagAksia");
                }
                return ok;
            }
        }

        //---Метод: Создание новой номенклатуры (ХП)---------------------------------------------------------------------
        public static bool UpdateTagNew(string Connectionstring, NomenclatureClass Nomen)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("UpdateTagNew", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@NomenID", Value = Nomen.ID };// Новый ID //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@Checked", Value = Nomen.New }; //---Передаваемый параметр
                    command.Parameters.Add(Param1);


                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр

                    command.ExecuteNonQuery();

                    ok = (bool)command.Parameters["@Result"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.UpdateTagAksia");
                }
                return ok;
            }
        }


        public static bool UniquenessCheck(string Connectionstring, string Article)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Cryption.Decrypt(Connectionstring)))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("UniquenessCheck", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@Article", Value = Article };
                    command.Parameters.Add(Param1);

                    int result = (int)command.ExecuteScalar();

                    if (result > 0)
                        ok = false;
                    else
                    {
                        ok = true;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            return ok;
        }












































        //---Метод: Создает Самую базовую группу для номенклатуры если такаго еще нет -----------------------------------
        public static void CheckAndCreateMainNomenGroup(string Connectionstring)
        {
            using (SqlConnection connect = new SqlConnection(Connectionstring))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("CheckAndCreateMainNomenGroup", connect);
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.GetMainNomenGroupID");
                }
            }
        }











        //---Метод: Создание новой номенклатуры (ХП)---------------------------------------------------------------------
        public static bool CreateNomenPropValue(string Connectionstring, Guid NomenklatureID, string ValueName)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Connectionstring))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("CreateNomenPropretyValue", connect);
                    command.CommandType = CommandType.StoredProcedure;


                    SqlParameter Param0 = new SqlParameter { ParameterName = "@NomenID", Value = NomenklatureID.ToString() };// Новый ID //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@PropertyValueName", Value = ValueName }; //---Передаваемый параметр
                    command.Parameters.Add(Param1);

                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр

                    command.ExecuteNonQuery();

                    ok = (bool)command.Parameters["@Result"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.CreateNomenPropValue");
                }
                return ok;
            }
        }

        //---Метод: Создание дополнительной единицы измерения номенклатуры (ХП)------------------------------------------
        public static bool CreateAdditionalUnits(string Connectionstring, Guid NomenklatureID, string UnitName, double RecountAmount, double Weight)
        {
            bool ok = false;
            using (SqlConnection connect = new SqlConnection(Connectionstring))
            {
                try
                {
                    connect.Open();
                    SqlCommand command = new SqlCommand("CreateAdditionalUnits", connect);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter Param0 = new SqlParameter { ParameterName = "@NomenclatureID", Value = NomenklatureID.ToString() };// Новый ID //---Передаваемый параметр
                    command.Parameters.Add(Param0);

                    SqlParameter Param1 = new SqlParameter { ParameterName = "@UnitName", Value = UnitName }; //---Передаваемый параметр
                    command.Parameters.Add(Param1);

                    SqlParameter Param2 = new SqlParameter { ParameterName = "@RecountAmount", Value = RecountAmount }; //---Передаваемый параметр
                    command.Parameters.Add(Param2);

                    SqlParameter Param3 = new SqlParameter { ParameterName = "@Weight", Value = Weight }; //---Передаваемый параметр
                    command.Parameters.Add(Param3);

                    command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output; // Выходной параметр

                    command.ExecuteNonQuery();

                    ok = (bool)command.Parameters["@Result"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DataBaseRequest.CreateDopEdin");
                }
                return ok;
            }
        }



        private static Viewbox GetNomenTag(string TagName, bool TagValue)
        {
            Viewbox vb = new Viewbox() { Width = 18, Height = 18 };
            if (TagName == "TagNew" && TagValue == true)
                vb.Child = GetIcons.GetIcon("Новинка") as UIElement;
            if (TagName == "TagNew" && TagValue == false)
                vb.Child = GetIcons.GetIcon("НовинкаНет") as UIElement;

            if (TagName == "TagFocus" && TagValue == true)
                vb.Child = GetIcons.GetIcon("Фокус") as UIElement;
            if (TagName == "TagFocus" && TagValue == false)
                vb.Child = GetIcons.GetIcon("ФокусНет") as UIElement;

            if (TagName == "TagAksia" && TagValue == true)
                vb.Child = GetIcons.GetIcon("Акция") as UIElement;
            if (TagName == "TagAksia" && TagValue == false)
                vb.Child = GetIcons.GetIcon("АкцияНет") as UIElement;
            return vb;
        }

    }

}
