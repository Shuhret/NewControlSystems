using System;
using System.Windows;
using System.Windows.Controls;

namespace ControlSystemsLibrary.Controls
{
    [TemplatePart(Name = "PART_DeleteButton", Type = typeof(Button))]
    public partial class ConnectionRB : RadioButton
    {
        public event EventHandler Deleted;
        public ConnectionRB()
        {
            InitializeComponent();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var deleteButton = this.GetTemplateChild("PART_DeleteButton") as Button;
            if (deleteButton != null)
            {
                deleteButton.Click += DeleteButton_Click;
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Deleted != null)
                Deleted(this, e);
        }
    }
}
