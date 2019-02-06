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

namespace SPPIDPlantGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel oMV = null;

        public MainWindow()
        {
            InitializeComponent();
            oMV = new MainViewModel();

            this.DataContext = oMV;
        }

        private void BtnGetGroups_Click(object sender, RoutedEventArgs e)
        {
            oMV.ConnectToDB();

            this.dgvPG.ItemsSource = oMV.Children;

            //this.tvPlantGroups.ItemsSource = null;
            this.tvPlantGroups.ItemsSource = oMV.Plantsytems;

        }

        private void TvPlantGroups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender == null) return;

            PlantGroup selectedPG = ((sender as TreeView).SelectedItem) as PlantGroup;
            
            if (selectedPG == null) return;
            
            List<PlantGroup> childs = oMV.FindChildren(selectedPG);
            selectedPG.Children.AddRange(childs);

            if(selectedPG.Children.Count > 0)
            {
                selectedPG.IsExpanded = true;

                foreach (PlantGroup pg in selectedPG.Children)
                {
                    pg.IsExpanded = true;
                }
            }

            //oMV.FindChildren(selectedPG);

            //this.dgvPG.ItemsSource = oMV.Children;
            //this.dgvPG.Items.Refresh();

            //this.tvPlantGroups.ItemsSource = null;
            this.tvPlantGroups.ItemsSource = oMV.Plantsytems;
            this.tvPlantGroups.Items.Refresh();
            //this.tvPlantGroups.ItemsSource = oMV.Plantsytems;
        }
    }
}
