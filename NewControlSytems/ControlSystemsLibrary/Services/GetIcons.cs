using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Threading;
using System.Xml;
using System.Windows.Threading;
using System.Windows.Controls;

namespace ControlSystemsLibrary.Services
{
    class GetIcons
    {
        // Метод: Возвращает иконку для вкладки
        public static object GetIcon(string IconName)
        {
            object obj;
            XmlReader xmlReader;
            obj = Application.Current.FindResource(IconName) as object;
            string savedButton = XamlWriter.Save(obj);
            StringReader stringReader = new StringReader(savedButton);
            xmlReader = XmlReader.Create(stringReader);
            return (object)XamlReader.Load(xmlReader);
        }
    }
}
