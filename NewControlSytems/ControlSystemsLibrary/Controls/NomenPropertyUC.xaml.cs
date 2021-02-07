﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ControlSystemsLibrary.Controls
{
    /// <summary>
    /// Логика взаимодействия для NomenPropertyUC.xaml
    /// </summary>
    public partial class NomenPropertyUC : UserControl, INotifyPropertyChanged
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


        public NomenPropertyUC()
        {
            InitializeComponent();
        }

        private Guid id;
        public Guid ID
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        private bool readiness;
        public bool Readiness
        {
            get
            {
                //CheckReadiness();
                if(readiness == false)
                {
                    Storyboard sb = this.FindResource("ReadinessFalse") as Storyboard;
                    sb.Begin();
                }
                return readiness;
            }
            set
            {
                readiness = value;
                OnPropertyChanged();
            }
        }






        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Parent is StackPanel)
                ((StackPanel)Parent).Children.Remove(this);
        }

        bool FirstLoaded = true;
        private void NPUC_Loaded(object sender, RoutedEventArgs e)
        {
            Readiness = false;
            ID = Guid.NewGuid();
            if (FirstLoaded == true)
            {
                PropertyComboBox.IsDropDownOpen = true;
                FirstLoaded = false;
            }
        }
    }
}