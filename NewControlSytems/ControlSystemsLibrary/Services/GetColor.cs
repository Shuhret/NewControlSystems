using System.Windows;
using System.Windows.Media;


namespace ControlSystemsLibrary.Services
{
    class GetColor
    {
        public static SolidColorBrush Get(string ColorName)
        {
            try
            {
                return new SolidColorBrush((Color)Application.Current.FindResource(ColorName));
            }
            catch
            {
                return new SolidColorBrush(Colors.Red);
            }
        }
    }
}
