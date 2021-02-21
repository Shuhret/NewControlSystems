using ControlSystemsLibrary.Classes;
using ControlSystemsLibrary.Controls.AdminTabItemContents;
using ControlSystemsLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

    public partial class SelectedNomenclatures : UserControl
    {
        CloseSelectedItemsDelegate CSID;
        CheckedSelectedItemParentDelegate CSIPD;
        ObservableCollection<NomenclatureClass> SelectedNomenclaturesCollection = new ObservableCollection<NomenclatureClass>();
        ObservableCollection<NomenclatureClass> AllNomenclaturesCollection;
        public SelectedNomenclatures(ObservableCollection<NomenclatureClass> AllNomenclaturesCollection, CloseSelectedItemsDelegate CSID, CheckedSelectedItemParentDelegate CSIPD)
        {
            this.AllNomenclaturesCollection = AllNomenclaturesCollection;
            this.CSID = CSID;
            this.CSIPD = CSIPD;
            InitializeComponent();
        }

        private void LoadSelectedItems()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                foreach (NomenclatureClass NC in AllNomenclaturesCollection)
                {
                    if (NC.GroupNomen == true && NC.Selected == true)
                    {
                        NomenclatureClass SN = new NomenclatureClass();
                        SN.ID = NC.ID;
                        SN.GroupID = NC.GroupID;
                        SN.Name = NC.Name;
                        SN.Article = NC.Article;
                        SN.Category = NC.Category;
                        SN.Icon = GetIcons.GetIcon("Номенклатура") as Viewbox;
                        SelectedNomenclaturesCollection.Add(SN);
                    }
                }
                DataGridSelectedItems.ItemsSource = SelectedNomenclaturesCollection;
            }));
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            CSID();
        }


        private async void DataGridSelectedItems_Loaded(object sender, RoutedEventArgs e)
        {
            //await Task.Run(() => XmlClass.AddConnectSetting(CurrentConnectionName, ConnectionStringBuilder.ConnectionString));
            await Task.Run(() => LoadSelectedItems());
        }

        private void DataGridRow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (NomenclatureClass NC in DataGridSelectedItems.SelectedItems)
                {
                    foreach (NomenclatureClass NCList in AllNomenclaturesCollection)
                    {
                        if (NC.ID == NCList.ID)
                        {
                            NCList.Selected = false;
                            CSIPD(NCList);
                        }
                    }
                }
            }
        }
    }
}
