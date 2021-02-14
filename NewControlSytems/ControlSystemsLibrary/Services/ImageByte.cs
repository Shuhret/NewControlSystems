using System.IO;
using System.Windows.Media.Imaging;

namespace ControlSystemsLibrary.Services
{
    class ImageByte
    {
        //---Метод: Преобразует изображение в массив byte и возвращает ----------------
        public static byte[] ImageToByteArray(string path)
        {
            byte[] imageData;
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            {
                imageData = new byte[fs.Length];
                fs.Read(imageData, 0, imageData.Length);
            }
            return imageData;
        }

        //---Метод: Преобразует массив byte в изображение и возвращает----------------
        public static BitmapImage ByteArrayToImage(byte[] data)
        {
            MemoryStream byteStream = new MemoryStream(data);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = byteStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

    }
}
