using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ControlSystemsLibrary.Controls
{
    /// <summary>
    /// Логика взаимодействия для LoaderCubes.xaml
    /// </summary>
    public partial class LoaderCubes : UserControl
    {
        public LoaderCubes()
        {
            InitializeComponent();
            Thread thread = new Thread(new ThreadStart(RunAnimation));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
        void RunAnimation()
        {
            Action ac = () =>
            {
                Storyboard sb = this.FindResource("loop") as Storyboard; // AAA-Название анимации
                sb.Begin(); // Запустить анимацию
            };
            Dispatcher.Invoke(ac);
        }
    }
}
