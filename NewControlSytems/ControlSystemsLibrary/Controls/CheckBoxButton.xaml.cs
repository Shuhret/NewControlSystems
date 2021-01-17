using System;
using System.Collections.Generic;
using System.Linq;
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
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CheckBox", Type = typeof(CheckBox))]
    public partial class CheckBoxButton : UserControl
    {
        public event EventHandler ClickButton;
        public event EventHandler CheckCheckBox;
        public event EventHandler UnCheckCheckBox;
        public CheckBoxButton()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        private void PartButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickButton != null)
                ClickButton(this, e);
        }


        //Событие: Click для Кнопки-а -------------------------------------------------------------------------------------------
        private void PART_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ClickButton != null)
                ClickButton(this, e);
        }


        //Событие: Сhecked для CheckBox-а ---------------------------------------------------------------------------------------
        private void PART_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckCheckBox != null)
                CheckCheckBox(this, e);
        }


        //Событие: Unchecked для CheckBox-а -------------------------------------------------------------------------------------
        private void PART_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (UnCheckCheckBox != null)
                UnCheckCheckBox(this, e);
        }




        public object BcContent
        {
            get { return (object)GetValue(BcContentProperty); }
            set { SetValue(BcContentProperty, value); }
        }

        public static readonly DependencyProperty BcContentProperty = DependencyProperty.Register("BcContent", typeof(object), typeof(CheckBoxButton), new PropertyMetadata(null));





        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckBoxButton), new PropertyMetadata(false));





        public SolidColorBrush StarCheckColor
        {
            get { return (SolidColorBrush)GetValue(StarCheckColorProperty); }
            set { SetValue(StarCheckColorProperty, value); }
        }

        public static readonly DependencyProperty StarCheckColorProperty = DependencyProperty.Register("StarCheckColor", typeof(SolidColorBrush), typeof(CheckBoxButton), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));




        public SolidColorBrush StarUnCheckColor
        {
            get { return (SolidColorBrush)GetValue(StarUnCheckColorProperty); }
            set { SetValue(StarUnCheckColorProperty, value); }
        }

        public static readonly DependencyProperty StarUnCheckColorProperty = DependencyProperty.Register("StarUnCheckColor", typeof(SolidColorBrush), typeof(CheckBoxButton), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

    }
}
