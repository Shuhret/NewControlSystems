using ControlSystemsLibrary.Services;
using System;
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
    /// Логика взаимодействия для AdditionalUnitsUC.xaml
    /// </summary>
    public partial class AdditionalUnitsUC : UserControl, INotifyPropertyChanged
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

        public AdditionalUnitsUC()
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

        private Guid nomenclatureID;
        public Guid NomenclatureID
        {
            get => nomenclatureID;
            set
            {
                nomenclatureID = value;
                OnPropertyChanged();
            }
        }

        private string baseUnitName;
        public string BaseUnitName
        {
            get => baseUnitName;
            set
            {
                baseUnitName = value;

                if (Quantity > 0)
                    QuantityText = string.Format("{0:0.000}", quantity) + " " + BaseUnitName;
                OnPropertyChanged();
            }
        }


        private double baseUnitWeight;
        public double BaseUnitWeight
        {
            get => baseUnitWeight;
            set
            {
                baseUnitWeight = value;
                OnPropertyChanged();
            }
        }


        private bool readiness;
        public bool Readiness
        {
            get
            {
                //if (readiness == false)
                //{
                //    Storyboard sb = this.FindResource("ReadinessFalse") as Storyboard;
                //    sb.Begin();
                //}
                return readiness;
            }
            set
            {
                readiness = value;
                OnPropertyChanged();
            }
        }


        private string addUnitName;
        public string AddUnitName
        {
            get => addUnitName;
            set
            {
                addUnitName = value;
                CheckReadiness();
                OnPropertyChanged();
            }
        }


        private double quantity;
        public double Quantity
        {
            get => quantity;
            set
            {
                quantity = value;

                if (value > 0)
                {
                    //string.Format("{0:0.000}", 25.657446842);
                    QuantityText = string.Format("{0:0.000}", quantity) + " " + BaseUnitName;
                    Weight = Math.Round((Quantity * BaseUnitWeight), 3); 
                }
                else
                {
                    Weight = 0;
                }


                CheckReadiness();
                OnPropertyChanged();
            }
        }
        private string quantityText;
        public string QuantityText
        {
            get => quantityText;
            set
            {
                quantityText = value;
                OnPropertyChanged();
            }
        }


        private double weight;
        public double Weight
        {
            get => weight;
            set
            {
                weight = value;
                OnPropertyChanged();
                if (value > 0)
                {
                    //string.Format("{0:0.00}", 25.657446842);
                    //WeightText = (Math.Round(weight, 3)).ToString() + " кг";
                    WeightText = string.Format("{0:0.000}", weight) + " кг";
                }
                else
                {
                    WeightText = string.Empty;
                }
                CheckWeight();
                CheckReadiness();
            }
        }
        private string weightText;
        public string WeightText
        {
            get => weightText;
            set
            {
                weightText = value;

                OnPropertyChanged();
            }
        }





        public void ReadinessFalseAnimationBegin()
        {
            Storyboard sb = this.FindResource("ReadinessFalse") as Storyboard;
            sb.Begin();
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox TX = sender as TextBox;
                if (TX.Name == "QuantityTextBox" && Quantity > 0)
                {
                    TX.Text = Quantity.ToString();
                }
                if (TX.Name == "WeightTextBox" && Weight > 0)
                {
                    TX.Text = Weight.ToString();
                }
            }
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox TX = sender as TextBox;
                if (TX.Name == "QuantityTextBox")
                {
                    double value;
                    if (Double.TryParse(TX.Text, out value))
                    {
                        double d = Math.Round(Convert.ToDouble(TX.Text), 3); 
                        if (d > 0)
                        {
                            Quantity = d;
                        }
                        else
                        {
                            Quantity = 0;
                            TX.Text = string.Empty;
                        }
                    }
                    else
                    {
                        Quantity = 0;
                    }
                }
                if (TX.Name == "WeightTextBox")
                {
                    double value;
                    if (Double.TryParse(TX.Text, out value))
                    {
                        double d = Math.Round(Convert.ToDouble(TX.Text), 3);
                        if (d > 0)
                            Weight = d;
                        else
                        {
                            Weight = 0;
                            TX.Text = string.Empty;
                        }
                    }
                    else
                    {
                        Weight = 0;
                    }
                }
            }
        }

        private void DigitText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            double result;
            string str = e.Text;
            bool дробь = false;

            foreach (char ch in ((TextBox)sender).Text)
            {
                if (ch == ',')
                {
                    дробь = true;
                    break;
                }
            }

            if (((TextBox)sender).Text.Length == 0 && (str == "0"))
            {
                ((TextBox)sender).Text = "0,";
                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                ((TextBox)sender).SelectionLength = 0;

                дробь = true;
                e.Handled = true;
            }

            if (((TextBox)sender).Text.Length == 0 && (str == ",") || (str == "."))
            {
                ((TextBox)sender).Text = "0,";

                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                ((TextBox)sender).SelectionLength = 0;

                дробь = true;
                e.Handled = true;
            }
            else if (str == "," && !дробь)
            {
                ((TextBox)sender).Text += ",";
                str = ",";
                дробь = true;
                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                ((TextBox)sender).SelectionLength = 0;

                e.Handled = true;
            }

            if ((str == "." || str == ",") && дробь)
            {
                e.Handled = true;
            }

            if ((!(double.TryParse(str, out result) || str == ".")))
            {
                e.Handled = true;
            }

        }

        private void CheckReadiness()
        {
            if (Weight > 0 && Quantity > 0 && AddUnitName != "" && AddUnitName != null && AddUnitName != string.Empty)
            {
                Readiness = true;
            }
            else
            {
                Readiness = false;
            }
        }

        private void CheckWeight()
        {
            if (Quantity > 0 && Weight > 0)
            {
                if (Weight < (Quantity * BaseUnitWeight))
                {
                    WeightTextBox.Foreground = GetColor.Get("Red-001");
                }
                else
                {
                    WeightTextBox.Foreground = GetColor.Get("Blue-004");
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textBox = sender as TextBox;
                if (e.Key == Key.Enter)
                {
                    switch (textBox.Name)
                    {
                        case "QuantityTextBox":
                            WeightTextBox.Focus();
                            WeightTextBox.SelectionStart = 0;
                            WeightTextBox.SelectionLength = WeightTextBox.Text.Length;
                            break;
                        case "WeightTextBox":
                            {
                                // Все это для снятия фокуса с WeightTextBox
                                FrameworkElement parent = (FrameworkElement)textBox.Parent;
                                while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
                                {
                                    parent = (FrameworkElement)parent.Parent;
                                }
                                DependencyObject scope = FocusManager.GetFocusScope(textBox);
                                FocusManager.SetFocusedElement(scope, parent as IInputElement);
                            }
                            break;
                    }
                }
            }
        }

        bool FirstLoaded = true;
        private void AUUC_Loaded(object sender, RoutedEventArgs e)
        {
            UnitsComboBox.Focus();
            if (FirstLoaded)
            {
                UnitsComboBox.IsDropDownOpen = true;
                FirstLoaded = false;
            }
        }

    }
}
