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
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Bd", Type = typeof(Border))]

    public partial class AdminTabItem : TabItem
    {
        public event EventHandler Closed;
        public AdminTabItem()
        {
            InitializeComponent();
        }
        #region Свойства
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(AdminTabItem), new PropertyMetadata(string.Empty));


        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(AdminTabItem), new PropertyMetadata(null));


        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(AdminTabItem), new PropertyMetadata((double)20));

        #endregion


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var button = this.GetTemplateChild("PART_CloseButton") as Button; // Найти в шаблоне объект с названием "PART_CloseButton" и привести в тип Button
            if (button != null)
            {
                button.Click += ClickCloseButton;
            }


            var bd = this.GetTemplateChild("PART_Bd") as Border; // Найти в шаблоне объект с названием "PART_Bd" и привести в тип Border
            if (bd != null)
            {
                bd.MouseRightButtonUp += Bd_MouseRightButtonUp;
            }

        }



        private void Bd_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Closed != null) Closed(this, e);
        }

        private void ClickCloseButton(object sender, RoutedEventArgs e)
        {
            if (Closed != null) Closed(this, e);
        }

    }
}
