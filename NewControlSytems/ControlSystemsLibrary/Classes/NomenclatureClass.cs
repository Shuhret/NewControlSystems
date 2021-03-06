﻿using ControlSystemsLibrary.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml;

namespace ControlSystemsLibrary.Classes
{
    public class NomenclatureClass : INotifyPropertyChanged
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



        // ID Номенклатуры
        private Guid iD;
        public Guid ID
        {
            get => iD;
            set
            {
                iD = value;
                OnPropertyChanged();
            }
        }


        // ID Группы
        private Guid groupID;
        public Guid GroupID
        {
            get => groupID;
            set
            {
                groupID = value;
                OnPropertyChanged();
            }
        }


        // Группа или Номенклатура
        private bool groupNomen;
        public bool GroupNomen
        {
            get => groupNomen;
            set
            {
                groupNomen = value;
                SetIcons();
                OnPropertyChanged();
            }
        }


        // Иконка Группа или Номенклатура
        public Viewbox icon;
        public Viewbox Icon
        {
            get => icon;
            set
            {
                icon = value;
                OnPropertyChanged();
            }
        }


        // Выбрано Номенклатура
        private bool? selected = false;
        public bool? Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }


        // Артикул Номенклатуры
        private string article;
        public string Article
        {
            get => article;
            set
            {
                article = value;
                OnPropertyChanged();
            }
        }


        // Название Номенклатуры
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }


        // Название Базовой Единицы Измерения
        private string baseUnit;
        public string BaseUnit
        {
            get => baseUnit;
            set
            {
                baseUnit = value;
                OnPropertyChanged();
            }
        }


        // Вес Базовой Единицы 
        private double weightBaseUnit;
        public double WeightBaseUnit
        {
            get => weightBaseUnit;
            set
            {
                weightBaseUnit = value;
                if (value > 0)
                {
                    if (value < 1)
                        WeightBaseUnitShow = (weightBaseUnit * 1000).ToString() + " гр";
                    else
                        WeightBaseUnitShow = value.ToString() + " кг";
                }
            }
        }


        // Вес Базовой Единицы для показа в DataGrid
        private string weightBaseUnitShow;
        public string WeightBaseUnitShow
        {
            get => weightBaseUnitShow;
            set
            {
                weightBaseUnitShow = value;
                OnPropertyChanged();
            }
        }


        // Штрих-код
        private string barcode;
        public string Barcode
        {
            get => barcode;
            set
            {
                barcode = value;
                OnPropertyChanged();
            }
        }


        // Тип Штрих-кода        
        private string barcodeType;
        public string BarcodeType
        {
            get => barcodeType;
            set
            {
                barcodeType = value;
                OnPropertyChanged();
            }
        }


        // Страна Происхождения
        private string countryOfOrigin;
        public string CountryOfOrigin
        {
            get => countryOfOrigin;
            set
            {
                countryOfOrigin = value;
                OnPropertyChanged();
            }
        }


        // Описание
        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }


        // Метка: "Акция"
        private bool aksia;
        public bool Aksia
        {
            get => aksia;
            set
            {
                aksia = value;
                OnPropertyChanged();
            }
        }


        // Метка: "Фокус"
        private bool focus;
        public bool Focus
        {
            get => focus;
            set
            {
                focus = value;
                OnPropertyChanged();
            }
        }


        // Метка: "Новинка"
        private bool _new;
        public bool New
        {
            get => _new;
            set
            {
                _new = value;
                OnPropertyChanged();
            }
        }


        // Видимость меток
        private Visibility tagVisibility;
        public Visibility TagVisibility
        {
            get => tagVisibility;
            set
            {
                tagVisibility = value;
                OnPropertyChanged();
            }
        }


        // Вырезано
        private bool cutOut = false;
        public bool CutOut
        {
            get => cutOut;
            set
            {
                cutOut = value;
                if (value == true)
                    CutIconVisibility = Visibility.Visible;
                else
                    CutIconVisibility = Visibility.Collapsed;
                OnPropertyChanged();
            }
        }


        // Видимость вырезано
        private Visibility cutIconVisibility = Visibility.Collapsed;
        public Visibility CutIconVisibility
        {
            get => cutIconVisibility;
            set
            {
                cutIconVisibility = value;
                OnPropertyChanged();
            }
        }



        void SetIcons()
        {
            if (GroupNomen == true)
            {
                TagVisibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke((Action)delegate 
                {
                    Icon = GetIcons.GetIcon("Номенклатура") as Viewbox;
                });
            }
            else
            {
                TagVisibility = Visibility.Collapsed;
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Icon = GetIcons.GetIcon("Папка") as Viewbox;
                });
            }
        }
    }
}
