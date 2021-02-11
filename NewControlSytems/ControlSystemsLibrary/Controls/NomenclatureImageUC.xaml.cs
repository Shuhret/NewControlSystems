using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace ControlSystemsLibrary.Controls
{
    [TemplatePart(Name = "PART_DeleteButton", Type = typeof(Button))]
    public partial class NomenclatureImageUC : RadioButton, INotifyPropertyChanged
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

        public event EventHandler Deleted;
        public NomenclatureImageUC()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var delButton = this.GetTemplateChild("PART_DeleteButton") as Button;
            if (delButton != null)
            {
                delButton.Click += DelButton_Click;
            }

        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Deleted != null)
                Deleted(this, e);
        }

        //NomenImage
        private BitmapImage nomenImage;
        public BitmapImage Image
        {
            get => nomenImage;
            set
            {
                nomenImage = value;
                OnPropertyChanged();
            }
        }


        private string imagePath = "";
        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                OnPropertyChanged();
                if(value != "")
                {
                    ShowImage();
                }
            }
        }


        private byte[] imageByte;
        public byte[] ImageByte
        {
            get => imageByte;
            set
            {
                imageByte = value;
                OnPropertyChanged();
            }
        }


        void ShowImage()
        {
            try
            {
                Image = new BitmapImage(new Uri(ImagePath));
                ImageByte = ImageToByteArray(ImagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }





        //---Метод: Преобразует изображение в массив byte и возвращает ----------------
        private byte[] ImageToByteArray(string path)
        {
            byte[] imageData;
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            {
                imageData = new byte[fs.Length];
                fs.Read(imageData, 0, imageData.Length);
            }
            return imageData;
        }

    }
}
